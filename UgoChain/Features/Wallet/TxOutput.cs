namespace UgoChain.Features.Wallet
{
    /// <summary>
    /// Outgoing transaction
    /// 1. Amount being sent out
    /// 2. Address:- The address will be for the recipient and for the sender, as the sender
    ///    is supposed to send the remainder back to his/her address
    /// </summary>
    public class TxOutput
    {
        public decimal Amount { get; set; }
        public string Address { get; set; }
    }
}