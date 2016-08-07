using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using P2PQuake.JMAInformation.Quake.Convert;

namespace P2PQuake.TestJMAInformation.Quake.Analyze
{
    [TestFixture]
    public class MonoDateTest
    {
        [TestCase("平成1年2月3日4時5分", "1989/02/03 04:05:00")]
        [TestCase("平成24年12月31日23時59分", "2012/12/31 23:59:00")]
        [TestCase("2012年12月31日23時59分", "2012/12/31 23:59:00")]
        public void ValidDate(string target, string expected)
        {
            DateTime parsedTime = MonoDate.Parse(target);
            string parsedTimeStr = parsedTime.ToString("yyyy/MM/dd HH:mm:ss");
            Assert.AreEqual(expected, parsedTimeStr);
        }

        [Test]
        public void UnknownDate()
        {
            string target   = "不明年号1年12月31日4時5分";
            string expected = "12/31 04:05:00";

            DateTime now = DateTime.Now;
            if (now.Month == 12 && now.Day == 31)
            {
                expected = now.Year.ToString() + "/" + expected;
            }
            else
            {
                expected = (now.Year - 1).ToString() + "/" + expected;
            }

            ValidDate(target, expected);
        }
    }
}
