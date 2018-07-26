namespace UgoChain.Features.Wallet
{
    public class TxInput
    {
        public string TimeStamp { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public byte [] Signature { get; set; }
    }
}