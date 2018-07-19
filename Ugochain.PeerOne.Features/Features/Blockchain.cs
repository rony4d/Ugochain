using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UgoChain.PeerOne.Features
{
    public class Blockchain : IBlockchain
    {
        public List<Block> Chain { get; set; }

        public Blockchain()
        {
            Chain = new List<Block>();
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
        public bool IsChainValid(Blockchain blockchain)
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

        /// <summary>
        /// Replace the exisiting blockchain if:
        /// 1. The new chain is longer than the existing blockchain
        /// 2. The new chain is valid
        /// </summary>
        /// <param name="newBlockChain"></param>
        /// <returns>Status and the message</returns>
        public (bool, string) ReplaceChain(Blockchain newBlockChain)
        {
            if (Chain.Count > newBlockChain.Chain.Count)
            {
                return (false, "Exising chain is longer than the new chain");
            }
            else if (Chain.Count == newBlockChain.Chain.Count)
            {
                return (false, "Exising chain is equal to the new chain");

            }
            else if (!IsChainValid(newBlockChain))
            {
                return (false, "The new chain is not valid");
            }
            Chain = newBlockChain.Chain;
            return (true, "New chain has replaced exisiting chain");
        }


        public override bool Equals(object obj)
        {
            Blockchain blockchain = obj as Blockchain;
            if (blockchain == null)
                return false;
            if (Chain.Count != blockchain.Chain.Count)
                return false;
            if (blockchain.Chain == null)
                return false;

            for (int i = 0; i < blockchain.Chain.Count; i++)
            {
                if (!Chain[i].Equals(blockchain.Chain[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
