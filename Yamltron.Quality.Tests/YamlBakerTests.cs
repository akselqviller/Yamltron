using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yamltron.Quality.Tests
{
    [TestClass]
    public class YamlBakerTests
    {
        [TestMethod]
        public void BakeToBase64()
        {
            using (var input = File.OpenText(@"Yamls\KubernetesSecrets_ClearText.yaml"))
            {
                using (var output = new StringWriter())
                {
                    new YamlBaker().BakeYaml(input, output, BakeDirection.ToBase64);

                    string result = output.ToString().Trim();
                    string expected = File.ReadAllText(@"Yamls\KubernetesSecrets_B64Text.yaml");

                    Assert.AreEqual(expected, result);
                }
            }
        }

        [TestMethod]
        public void BakeToClearText()
        {
            using (var input = File.OpenText(@"Yamls\KubernetesSecrets_B64Text.yaml"))
            {
                using (var output = new StringWriter())
                {
                    new YamlBaker().BakeYaml(input, output, BakeDirection.ToClearText);

                    string result = output.ToString().Trim();
                    string expected = File.ReadAllText(@"Yamls\KubernetesSecrets_ClearText.yaml");

                    Assert.AreEqual(expected, result);
                }
            }
        }
    }
}
