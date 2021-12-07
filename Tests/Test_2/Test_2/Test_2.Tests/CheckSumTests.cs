using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test_2.Tests
{
    [TestFixture]
    public class Tests
    {
        private const string path = "..\\..\\..\\..\\TestData";

        [TestCaseSource(nameof(CheckSums))]
        public void CheckSumShouldCalculateHashCorrectly(Func<string, byte[]> checkSum)
        {
            var firstSum = checkSum(path);
            var secondSum = checkSum(path);

            Assert.AreEqual(firstSum, secondSum);
        }

        [Test]
        public void SinglethreadedAndMultithreadedCheckSumsAreSame()
        {
            var firstSum = CheckSum.GetCheckSumSinglethreaded(path);
            var secondSum = CheckSum.GetCheckSumMiltithreaded(path);

            Assert.AreEqual(firstSum, secondSum);
        }

        private static IEnumerable<Func<string, byte[]>> CheckSums()
        {
            yield return CheckSum.GetCheckSumSinglethreaded;
            yield return CheckSum.GetCheckSumMiltithreaded;
        }
    }
}