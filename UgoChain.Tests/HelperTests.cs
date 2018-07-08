using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests
{
    public class HelperTests
    {
        private ITestOutputHelper _testOutputHelper;

        public HelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

        }

        [Fact]
        public void ConvertToUnixTimeStamp()
        {
            DateTime genesisTime = new DateTime(2018, 07, 8, 11, 2, 0, DateTimeKind.Local);
            double genesisUnixTime = Helper.ConvertToUnixTimeStamp(genesisTime);
            _testOutputHelper.WriteLine(genesisUnixTime.ToString());
        }

        [Fact]
        public void ConvertToLocalDateTime()
        {
            DateTime genesisLocalTime = Helper.ConvertLocalTime(1531044120);
            _testOutputHelper.WriteLine(genesisLocalTime.ToString());
        }
    }
}
