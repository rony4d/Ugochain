using System;
using UgoChain.Features;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Blockchain.Tests
{
    public class BlockTests
    {
        private ITestOutputHelper _testOutputHelper;
        private Block _geneis { get; set; } = Block.GenesisBlock();
        public BlockTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void CreateBlock()
        {
            Block block = new Block(DateTime.Now.ToString(), "sdjvoisdhvoisbdvionvd", "78484biiuwgeiug", "Ugo's blockchain will be solve problems");
            Assert.NotNull(block);
            _testOutputHelper.WriteLine(block.ToString());
        }

        [Fact]
        public void CreateGenesisBlock()
        {
            Block genesis = Block.GenesisBlock();
            Assert.NotNull(genesis);
            Assert.Empty(genesis.Data);
            _testOutputHelper.WriteLine(genesis.ToString());
        }

        [Fact]
        public void MineBlock()
        {
            
            Block newBlock = Block.MineBlock(_geneis, "New block mined");
            Assert.NotNull(newBlock);
            Assert.Equal(newBlock.LastHash, _geneis.Hash);

            _testOutputHelper.WriteLine(newBlock.ToString());
        }

        /// <summary>
        /// Should ensure that the new block always produces the same hash with old block
        /// Test will be run against genesis block and a hardcoded second block
        /// </summary>
        [Fact]
        public void ShouldEnsureConsistentHash()
        {
            double timeStamp = 1531044121;
            string secondBlockHash = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block");
            string secondBlockHashSame = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block");

            Assert.Equal(secondBlockHash, secondBlockHashSame);

            string secondBlockHashChanged = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block changed");
            Assert.NotEqual(secondBlockHash, secondBlockHashChanged);
        }


    }
}
