using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UgoChain.Features.Wallet;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Wallets.Tests
{
    public class TransactionTests
    {
        ITestOutputHelper _testOutputHelper;
        Transaction _transaction;
        Wallet _wallet;
        string recipientAddress = "r3c1p13entPu8liCK3y";
        string nextRecipient = "n3x7 a44r355";
        decimal nextAmount = 40;
        decimal amountToSend = 50;

        public TransactionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _wallet = new Wallet();
            _transaction = new Transaction();
            _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);

        }

        /// <summary>
        /// Should output the amount subtracted from the wallet balance which
        /// is the amount in the changeBack TxOutput object
        /// </summary>
        [Fact]
        public void ShouldOutputTransactionBalance()
        {

            //get TxOutput where the address is the public key of the wallet( which is address of the sender)
            TxOutput changeBackOutput = _transaction.TxOutputs.Where(p => p.Address == _wallet.PublicKey.Key).FirstOrDefault();

            Assert.Equal(_wallet.Balance - amountToSend, changeBackOutput.Amount);
        }

        /// <summary>
        /// Should output the amount the recipient is going to receive from the TxOutput object
        /// </summary>
        [Fact]
        public void ShouldOutputRecipientTransactionAmount()
        {
            decimal amountToSend = 50;
            //get TxOutput where the address is the recipient address/public key
            TxOutput recipientOutput = _transaction.TxOutputs.Where(p => p.Address == recipientAddress).FirstOrDefault();

            Assert.Equal(amountToSend, recipientOutput.Amount);
        }

        /// <summary>
        /// Should try and send an amount greater than the wallet balance
        /// </summary>
        [Fact]
        public void ShouldExceedWalletBalance()
        {
            decimal largeAmount = 40000;
            _transaction = new Transaction();
            (bool, string) response = _transaction.CreateTransaction(_wallet, recipientAddress, largeAmount);

            Assert.True(response.Item1 == false, response.Item2);
            //_testOutputHelper.WriteLine(response.Item2);
        }

        /// <summary>
        /// Check the TxInput exists and that the value of the amount is equal to the balance
        /// </summary>
        [Fact]
        public void ShouldCheckTxInputExisitsAndHasCorrectAmount()
        {
            (bool, string) txResponse = _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);

            Assert.Equal(_transaction.Input.Amount, _wallet.Balance);
        }

        /// <summary>
        /// Should verify valid transaction
        /// </summary>
        [Fact]
        public void ShouldVerifyTransaction()
        {
            (bool, string) txResponse = _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);
            bool isVerified = _transaction.VerifyTransaction();

            Assert.True(isVerified);
        }

        /// <summary>
        /// Should invalidate corrupt transaction
        /// </summary>
        [Fact]
        public void ShouldInvalidatCorruptTransaction()
        {
            (bool, string) txResponse = _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);
            _transaction.TxOutputs[0].Amount = 890000; // corrupting transaction by changing data
            bool isVerified = _transaction.VerifyTransaction();

            Assert.False(isVerified);
        }

        /*
         * Transaction Update Tests
         * 1. ShouldSubtractAmountFromSendersChangeBackTxOutput
         * 2. ShouldOutputAmountForNextRecipient
         */
        /// <summary>
        /// It should subtract the next amount to send from the senders changeback TxOut
        /// The amount from the changeback TxOut should be equal to the wallet balance minus inital amount to send 
        /// minus next amount to send
        /// </summary>
        [Fact]
        public void ShouldSubtractAmountFromSendersChangeBackTxOutput()
        {
            
            (bool, string) txResponse = _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);


            txResponse = _transaction.UpdateTransaction(_wallet, nextRecipient, nextAmount);

            decimal changeBackAmount = _transaction.TxOutputs.Where(p => p.Address == _wallet.PublicKey.Key).FirstOrDefault().Amount;

            Assert.Equal(_wallet.Balance - amountToSend - nextAmount, changeBackAmount);
        }

        /// <summary>
        /// Should show that the output amount for the next recipient from the 
        /// newly added transaction is equal to the next recipient amount
        /// </summary>
        [Fact]
        public void ShouldOutputAmountForNextRecipient()
        {
            (bool, string) txResponse = _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);


            txResponse = _transaction.UpdateTransaction(_wallet, nextRecipient, nextAmount);

            decimal newRecipientAmount = _transaction.TxOutputs.Where(p => p.Address == nextRecipient).FirstOrDefault().Amount;
            Assert.Equal(nextAmount, newRecipientAmount);
        }
    }
}
