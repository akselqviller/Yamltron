/*
 * Copyright (c) 2017 Aksel Qviller
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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

                var newNodes = direction == BakeDirection.ToBase64 
                        ?
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
