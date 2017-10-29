using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Yamltron
{
    public class YamlBaker
    {
        public void BakeYaml(
            Stream inputStream,
            Stream outputStream,
            BakeDirection direction)
        {
            BakeYaml(
                new StreamReader(inputStream),
                new StreamWriter(outputStream),
                direction);
        }

        public void BakeYaml(
            TextReader inputText,
            TextWriter outputText,
            BakeDirection direction)
        {
            var yam = new YamlStream();
            yam.Load(inputText);

            var secretsDocuments = yam.Documents
                .Select(d => d.RootNode)
                .OfType<YamlMappingNode>()
                .Where(n => (n.Children["kind"] as YamlScalarNode).Value == "Secret");

            foreach (var doc in secretsDocuments)
            {
                var dataNode = (YamlMappingNode)doc.Children["data"];

                var newNodes = direction == BakeDirection.ToBase64 ?
                    dataNode.Select(n => new KeyValuePair<YamlNode, YamlNode>(
                        n.Key, 
                        new YamlScalarNode(Convert.ToBase64String(Encoding.UTF8.GetBytes(((YamlScalarNode)n.Value).Value)))))
                        :
                    dataNode.Select(n =>new KeyValuePair<YamlNode, YamlNode>(
                        n.Key,
                        new YamlScalarNode(Encoding.UTF8.GetString(Convert.FromBase64String(((YamlScalarNode)n.Value).Value))))); // Feels like Lisp

                doc.Children["data"] = new YamlMappingNode(newNodes);
            }

            yam.Save(outputText);
        }
    }
}
