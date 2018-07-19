using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.Features
{
    public interface IBlockchain
    {
        List<Block> Chain { get; set; }

        Block AddBlock(string data);

        bool IsChainValid(Blockchain blockchain);

        (bool, string) ReplaceChain(Blockchain blockchain);
    }
}
