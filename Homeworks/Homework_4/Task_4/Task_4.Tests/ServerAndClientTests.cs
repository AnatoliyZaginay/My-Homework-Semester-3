using NUnit.Framework;
using MyFTPServer;
using MyFTPClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Task_4.Tests
{
    [TestFixture]
    public class ServerAndClientTests
    {
        private MyServer server;

        private const string path = "..\\..\\..\\..\\TestData";

        [SetUp]
        public void Setup()
        {
            server = new(IPAddress.Parse("127.0.0.1"), 8888);
            server.Run();
        }

        [TearDown]
        public void Teardown()
        {
            server.Stop();
            File.Delete(path + "\\TestDirectory\\result.txt");
        }

        [Test]
        public async Task ListTest()
        {
            var client = new MyClient("127.0.0.1", 8888);

            var result = await client.List(path);
            Assert.AreEqual(2, result.size);

            var expected = new[]
            {
                (path + "\\test.txt", false),
                (path + "\\TestDirectory", true)
            };

            for (int i = 0; i < result.list.Count; ++i)
            {
                Assert.AreEqual(expected[i], result.list[i]);
            }
        }

        [Test]
        public void ListShouldThrowsExceptionIfDirectoryNotExists()
        {
            var client = new MyClient("127.0.0.1", 8888);

            Assert.Throws<AggregateException>(() => client.List(path + "\\ThisDirectoryNotExists").Wait());
        }

        [Test]
        public async Task GetTest()
        {
            var client = new MyClient("127.0.0.1", 8888);

            var result = await client.Get(path + "\\TestDirectory\\result.txt", path + "\\TestDirectory\\test.txt");

            Assert.IsTrue(File.Exists(path + "\\TestDirectory\\result.txt"));

            var resultFileContent = File.ReadAllBytes(path + "\\TestDirectory\\result.txt");
            var expectedFileContent = File.ReadAllBytes(path + "\\TestDirectory\\test.txt");

            Assert.AreEqual(expectedFileContent.Length, result);
            Assert.AreEqual(expectedFileContent, resultFileContent);
        }

        [Test]
        public void GetShouldThrowsExceptionIfFileNotExists()
        {
            var client = new MyClient("127.0.0.1", 8888);

            Assert.Throws<AggregateException>(() => client.Get(path + "\\TestDirectory\\result.txt", path + "\\TestDirectory\\thisFileNotExists.txt").Wait());
        }
    }
}