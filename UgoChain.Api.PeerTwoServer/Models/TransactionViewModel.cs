using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UgoChain.Api.PeerTwoServer.Models
{
    public class TransactionViewModel
    {
        public string RecipientAddress { get; set; }
        public decimal AmountToSend { get; set; }
    }
}
