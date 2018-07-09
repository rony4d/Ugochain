using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using UgoChain.Features;
using System.Linq;

namespace UgoChain.Tests
{
    public class BlockchainTest
    {
        private Features.Blockchain _blockchain { get; set; } = new Features.Blockchain();
        private Features.Blockchain _blockchain2 { get; set; } = new Features.Blockchain();
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

            Assert.True(_blockchain.isChainValid(_blockchain2));

        }

        /// <summary>
        /// Invalids a chain with a corrupt genesis block
        /// </summary>
        [Fact]
        public void ShouldInvalidateChainWithCorruptGenesisBlock()
        {
            _blockchain2.Chain[0].Data = "I am changing the genesis data. Haha!!";
            Assert.False(_blockchain.isChainValid(_blockchain2));
        }

        /// <summary>
        /// Invalidates a corrupt chain that is not the genesis block
        /// </summary>
        [Fact]
        public void InvalidatesCorruptChain()
        {
            _blockchain2.AddBlock("Good data :)");
            _blockchain2.Chain[1].Data = "Corrupting the good data :(";

            Assert.False(_blockchain.isChainValid(_blockchain2));
        }
    }
}
