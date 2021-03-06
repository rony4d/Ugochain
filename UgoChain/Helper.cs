﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UgoChain
{
    public static class Helper
    {
        public static double ConvertToUnixTimeStamp(DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            double unixTimeStamp = (dateTime.ToUniversalTime() - epoch).TotalSeconds;
            return unixTimeStamp;
        }
        public static DateTime ConvertLocalTime(double unixDateTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(unixDateTime);
            DateTime localDateTime = new DateTime(timeSpan.Ticks).ToLocalTime();
            return localDateTime;
        }

        public static double GetMilliSecondsFrom(double days)
        {
            return days * 24 * 60 * 60 * 1000;
        }
    }

    public enum PeersEnum
    {
        Main = 1,
        PeerOne = 2,
        PeerTwo = 3
    }

    public enum PeerColorsEnum
    {
        Main = ConsoleColor.White,
        PeerOne = ConsoleColor.Green,
        PeerTwo = ConsoleColor.Yellow
    }

   

}
