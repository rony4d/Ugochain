using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.PeerTwo.Features
{
    public interface IBlockchain
    {
        List<IBlock> Chain { get; set; }

        IBlock AddBlock(string data);

        bool IsChainValid(IBlockchain blockchain);

        (bool, string) ReplaceChain(IBlockchain blockchain);
    }
}
