﻿using System;
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
    }
}