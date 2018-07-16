﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using UgoChain.Features;
using System.Linq;
using Xunit.Abstractions;

namespace UgoChain.Tests
{
    public class BlockchainTest
    {
        private ITestOutputHelper _testOutputHelper;

        private Features.Blockchain _blockchain { get; set; } = new Features.Blockchain();
        private Features.Blockchain _blockchain2 { get; set; } = new Features.Blockchain();

        public BlockchainTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        /// <summary>
        /// Blockchain should start with genesis block
        /// </summary>
        [Fact]
        public void ShouldStartWithGenesisBlock()
        {
            Assert.Equal(Block.GenesisBlock(), _blockchain.Chain.FirstOrDefault());           
        }

        /// <summary>
        /// Adds a new block and checks if the block is the last block
        /// By comparing the last blockdata and the data used to mine the last block
        /// </summary>
        [Fact]
        public void ShouldAddNewBlock()
        {
            string data = "Test Data";
            Block freshBlock =_blockchain.AddBlock(data);
            Assert.Equal(_blockchain.Chain.LastOrDefault().Data, data);

        }
        /// <summary>
        /// Validates a valid chain
        /// 1. Add a new block to the _blockchain2 instance and test its 
        /// validity with the  _blockchain instance
        /// </summary>
        [Fact]
        public void ShoudValidateValidChain()
        {
            //Test 1
            _blockchain2.AddBlock("New block on second blockchain");

            Assert.True(_blockchain.IsChainValid(_blockchain2));

        }

        /// <summary>
        /// Invalids a chain with a corrupt genesis block
        /// </summary>
        [Fact]
        public void ShouldInvalidateChainWithCorruptGenesisBlock()
        {
            _blockchain2.Chain[0].Data = "I am changing the genesis data. Haha!!";
            Assert.False(_blockchain.IsChainValid(_blockchain2));
        }

        /// <summary>
        /// Invalidates a corrupt chain that is not the genesis block
        /// </summary>
        [Fact]
        public void InvalidatesCorruptChain()
        {
            _blockchain2.AddBlock("Good data :)");
            _blockchain2.Chain[1].Data = "Corrupting the good data :(";

            Assert.False(_blockchain.IsChainValid(_blockchain2));
        }

        /// <summary>
        /// Replaces the existing chain with a valid new chain.
        /// The two chains must now be equal
        /// </summary>
        [Fact]
        public void ShouldReplaceExisitingChainWithValidNewChain()
        {
            _blockchain2.AddBlock("Second block");
            (bool,string) replaceResponse =_blockchain.ReplaceChain(_blockchain2);
            Assert.True(_blockchain.Equals(_blockchain2));
            Assert.True(replaceResponse.Item1);
            _testOutputHelper.WriteLine(replaceResponse.Item2);
        }

        /// <summary>
        /// Should not replace exisiting chain with a chain with less blocks
        /// or exactly the same number of blocks
        /// </summary>
        [Fact]
        public void ShouldNotReplaceChainWithShorterLengthChain()
        {
            _blockchain.AddBlock("Second block on the first blockchain");
            (bool, string) replaceResponse = _blockchain.ReplaceChain(_blockchain2);
            Assert.False(_blockchain.Equals(_blockchain2));
            _testOutputHelper.WriteLine(replaceResponse.Item2);

        }
    }
}