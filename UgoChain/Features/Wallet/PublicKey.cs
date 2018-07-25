using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.Features.Wallet
{
    public class PublicKey
    {
        public string Key { get; set; }

        public override string ToString()
        {
            return $"{Key}";
        }
    }
}
