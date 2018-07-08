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
    }
}
