using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using P2PQuake.JMAInformation.Tsunami;
using P2PQuake.JMAInformation.Tsunami.Convert;

namespace TestJMAInformation.Tsunami
{
    [TestFixture]
    class SmokeTest
    {
        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"../../TestData/Tsunami");
            string[] files = Directory.GetFiles(directory, "*.htm*");

            foreach (string path in files)
            {
                string expectedFileName = Path.GetFileNameWithoutExtension(path) + ".json";
                string expectedPath = Path.Combine(directory, expectedFileName);
                yield return new TestCaseData(path, expectedPath);
            }
        }

        [Test, TestCaseSource("GenerateTestCases")]
        public void JMATsunamiConverterTest(string sourcePath, string expectedPath)
        {
            StreamReader reader = new StreamReader(sourcePath, Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();

            Core core = JMATsunamiConverter.fromString(content);
            string json = JsonConvert.SerializeObject(core, Formatting.Indented).Replace("\r", "");

            if (!File.Exists(expectedPath))
            {
                StreamWriter writer = new StreamWriter(expectedPath, false, Encoding.UTF8);
                writer.Write(json);
                writer.Close();

                Assert.Ignore();
                return;
            }

            StreamReader expectedReader = new StreamReader(expectedPath, Encoding.UTF8);
            string expectedJson = expectedReader.ReadToEnd().Replace("\r", "");
            expectedReader.Close();

            Assert.AreEqual(expectedJson, json);
        }
    }
}
