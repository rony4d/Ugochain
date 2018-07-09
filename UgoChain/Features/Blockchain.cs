using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UgoChain.Features
{
    public class Blockchain
    {
        public List<Block> Chain { get; set; } = new List<Block>();

        public Blockchain()
        {
            Chain.Add(Block.GenesisBlock());
        }

        /// <summary>
        /// Mine the new block and add the block data to it
        /// Then add the new block to the blockchain
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Mined block</returns>
        public Block AddBlock(string data)
        {
            Block freshBlock = Block.MineBlock(Chain.LastOrDefault(), data);
            Chain.Add(freshBlock);
            return freshBlock;
        }

        /// <summary>
        /// 1. Checks if the first block is the genesis block
        /// 2. Checks if the current block's last hash property is equal to the last block's hash property
        /// 3. Checks if data manipulation has occcured by rebuilding the current block's hash with the Blocks HashFunction
        /// and checking if the two hashes are equal
        /// </summary>
        /// <param name="blockchain"></param>
        /// <returns></returns>
        public bool isChainValid(Blockchain blockchain)
        {
            if (!blockchain.Chain.FirstOrDefault().Equals(Block.GenesisBlock()))
                return false;

            for (int i = 1; i < blockchain.Chain.Count; i++)
            {
                Block currentBlock = blockchain.Chain[i];
                Block lastBlock = blockchain.Chain[i - 1];
                if (currentBlock.LastHash != lastBlock.Hash || currentBlock.Hash != Block.BlockHash(currentBlock))
                    return false;
                
            }

            return true;
        }
    }
}
