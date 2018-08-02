using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UgoChain.PeerTwo.Features
{
    public class Block : IBlock
    {
        public const int DIFFICULTY = 2;

        public const int MINE_RATE = 3000; //3000 milliseconds

        private const double GenesisTime = 1531044120; //unix time

        public string TimeStamp { get; set; }
        public string LastHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }

        public long Nonce { get; set; }
        public int Difficulty { get; set; }
        public Block()
        {

        }

        public Block(string timeStamp, string lastHash, string hash, string data, long nonce, int difficulty)
        {
            TimeStamp = timeStamp;
            LastHash = lastHash;
            Hash = hash;
            Data = data;
            Nonce = nonce;
            Difficulty = difficulty;
        }

        public override string ToString()
        {
            return $"Block - " +
                $"TimeStamp: {TimeStamp} \n" +
                $"Last Hash: {LastHash} \n" +
                $"Hash: {Hash} \n" +
                $"Data: {Data} \n" +
                $"Nonce: {Nonce} \n" +
                $"Difficulty: {Difficulty}";
        }

        public static Block GenesisBlock()
        {
            // mine the genesis block with the diffculty of the system
            return new Block(GenesisTime.ToString(), "xxxxxx", "gen0SHA-94-01-25", "", 0, DIFFICULTY);
        }

        public static Block MineBlock(Block previousBlock, string data)
        {
            string timeStamp;
            string lastHash = previousBlock.Hash;
            string hash;
            long nonce = 0;
            int difficulty = previousBlock.Difficulty;
            do
            {
                nonce++;
                timeStamp = Helper.ConvertToUnixTimeStamp(DateTime.Now).ToString();
                difficulty = AdjustDifficulty(previousBlock, timeStamp);
                hash = GetHash(timeStamp, lastHash, data, nonce, difficulty);

            } while (hash.Substring(0, difficulty) != ("0".PadRight(difficulty, '0')));

            return new Block(timeStamp, lastHash, hash, data, nonce, difficulty);
        }

        public static int AdjustDifficulty(Block previousBlock, string currentBlockTimeStampUnix)
        {
            int lastDifficulty = previousBlock.Difficulty;

            double minerTimeDifference = (Helper.ConvertLocalTime(double.Parse(currentBlockTimeStampUnix)) - Helper.ConvertLocalTime(double.Parse(previousBlock.TimeStamp))).TotalDays; ;
            double minerTimeDifferenceInMilliseconds = Helper.GetMilliSecondsFrom(minerTimeDifference);
            int currentDifficulty = minerTimeDifferenceInMilliseconds > MINE_RATE ? lastDifficulty - 1 : lastDifficulty + 1;
            return currentDifficulty;
        }

        public static string GetHash(string timeStamp, string lastHash, string data, long nonce, long difficulty)
        {
            string dataToHash = timeStamp + lastHash + data + nonce + difficulty;
            string hash = Convert.ToBase64String(ChainUtility.Hash(dataToHash));
            return hash;
        }

        public static string BlockHash(Block block)
        {
            return GetHash(block.TimeStamp, block.LastHash, block.Data, block.Nonce, block.Difficulty);
        }
        public override bool Equals(object obj)
        {
            var block = obj as Block;
            if (block == null)
            {
                return false;
            }
            return (TimeStamp == block.TimeStamp
                && LastHash == block.LastHash
                && Hash == block.Hash
                && Data == block.Data);
        }

        public override int GetHashCode()
        {
            //use default hash function for now
            return base.GetHashCode();
        }
    }
}
