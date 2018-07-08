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
    }
}
