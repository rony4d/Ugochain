using System;
using UgoChain.Features;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Blockchain.Tests
{
    public class BlockTests
    {
        private ITestOutputHelper _testOutputHelper;

        private Block _geneis { get; set; }
        public BlockTests(ITestOutputHelper testOutputHelper)
        {
            _geneis = Block.GenesisBlock();

            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void CreateBlock()
        {
            Block block = new Block(DateTime.Now.ToString(), "sdjvoisdhvoisbdvionvd", "78484biiuwgeiug", "Ugo's blockchain will be solve problems",0,Block.DIFFICULTY);
            Assert.NotNull(block);
            _testOutputHelper.WriteLine(block.ToString());
        }

        [Fact]
        public void CreateGenesisBlock()
        {
            Block genesis = Block.GenesisBlock();
            Assert.NotNull((Block)genesis);
            Assert.Empty(genesis.Data);
            _testOutputHelper.WriteLine(genesis.ToString());
        }

        /// <summary>
        /// Mine a new block using genesis block as previous block
        /// </summary>
        [Fact]
        public void MineBlock()
        {
            Block newBlock = Block.MineBlock(_geneis, "New block mined");
            Assert.NotNull((Block)newBlock);
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
            string secondBlockHash = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block",0,Block.DIFFICULTY);
            string secondBlockHashSame = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block",0,Block.DIFFICULTY);

            Assert.Equal(secondBlockHash, secondBlockHashSame);

            string secondBlockHashChanged = Block.GetHash(timeStamp.ToString(), _geneis.Hash, "second block changed",0,Block.DIFFICULTY);
            Assert.NotEqual(secondBlockHash, secondBlockHashChanged);
        }
        /// <summary>
        /// Should generate a hash that matches the difficulty
        /// </summary>
        [Fact]
        public void ShouldGenerateAHashThatMatchesDifficulty()
        {
            Block newBlock = Block.MineBlock(_geneis, "New block mined");

            Assert.Equal(newBlock.Hash.Substring(0, newBlock.Difficulty), "0".PadRight(newBlock.Difficulty, '0'));

            _testOutputHelper.WriteLine(newBlock.ToString());

        }

        /// <summary>
        /// Lowers difficulty when difference between the previous block timestamp and current block time stamp 
        /// is greater than the mine rate
        /// </summary>
        [Fact]
        public void ShouldLowerDifficultyForSlowlyMinedBlock()
        {
            //Fake a current timestamp that is very large, it should be greater than
            //the previous block, in this case the genesis block
            DateTime currentBlockDateFake = DateTime.Now.AddDays(3);
            DateTime fakeGenesisTime = DateTime.Now; // let genesis time be less than current block time
            _geneis.TimeStamp = Helper.ConvertToUnixTimeStamp(fakeGenesisTime).ToString();
            string currentBlockTimestampFake = Helper.ConvertToUnixTimeStamp(currentBlockDateFake).ToString();
            int diffculty = Block.AdjustDifficulty(_geneis, currentBlockTimestampFake);
            Assert.Equal(_geneis.Difficulty - 1, diffculty); 
        }

        /// <summary>
        /// Raises difficulty when difference between the previous block timestamp and current block time stamp 
        /// is less than the mine rate
        /// </summary>
        [Fact]
        public void ShouldRaiseDifficultyForQuicklyMinedBlock()
        {
            //Fake a current timestamp that is very large
            DateTime currentBlockDateFake = DateTime.Now;
            DateTime fakeGenesisTime = DateTime.Now.AddDays(0.2); // let genesis time be greater than current block time
            _geneis.TimeStamp = Helper.ConvertToUnixTimeStamp(fakeGenesisTime).ToString();

            int diffculty = Block.AdjustDifficulty(_geneis, Helper.ConvertToUnixTimeStamp(currentBlockDateFake).ToString());
            Assert.Equal(_geneis.Difficulty + 1, diffculty);
        }

       
    }
}
