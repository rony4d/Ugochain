﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UgoChain.Features
{
    public class Block
    {
        private const double GenesisTime = 1531044120; //unix time
        public string TimeStamp { get; set; }
        public string LastHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }
        public Block()
        {

        }

        public Block(string timeStamp,string lastHash,string hash,string data)
        {
            TimeStamp = timeStamp;
            LastHash = lastHash;
            Hash = hash;
            Data = data;
        }

        public override string ToString()
        {
            return $"Block - " +
                $"TimeStamp: {TimeStamp} \n" +
                $"Last Hash: {LastHash} \n" +
                $"Hash: {Hash} \n" +
                $"Data: {Data} \n";
        }

        public static Block GenesisBlock()
        {
            return new Block(GenesisTime.ToString(),"xxxxxx","gen0SHA-94-01-25","");
        }

        public static Block MineBlock(Block previousBlock,string data)
        {
            string timeStamp = DateTime.Now.ToString();
            string lastHash = previousBlock.Hash;
            string hash = GetHash(timeStamp,lastHash,data);

            return new Block(timeStamp, lastHash, hash, data);
        }
        
        public static string GetHash(string timeStamp,string lastHash,string data)
        {
            SHA256 sHA256 = SHA256.Create();
            Byte [] hashBytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(timeStamp + lastHash + data));
            string hash = Convert.ToBase64String(hashBytes);
            return hash;
        }
    }
}