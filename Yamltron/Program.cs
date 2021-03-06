﻿/*
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

namespace Yamltron
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!(args.Length == 2 || args.Length == 3) ||
                !(args[0] == "-s" || args[0] == "-c"))
            {
                Console.WriteLine(
@"Expected input:
-s|-c <INPUT FILE> [<OUTPUT FILE>]

-s : Convert secrets from clear text to Base64 for secrets
-c : Convert secrets from Base64 to clear text

If no output file is specified, the file is converted in-place.");

                return;
            }

            string inputFilename = args[1];
            string outputFilename = args.Length == 3 ? args[2] : inputFilename;

            if (!File.Exists(inputFilename))
            {
                Console.WriteLine("Input file {0} not found.", inputFilename);

                return;
            }
                        
            if (!string.IsNullOrEmpty(outputFilename) && 
                inputFilename != outputFilename &&
                File.Exists(outputFilename))
            {
                Console.WriteLine("The file {1} already exists. Overwrite? (y/n)", outputFilename);

                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Console.WriteLine("Aborting");
                    return;
                }
            }

            BakeDirection direction = args[0] == "-c" ? BakeDirection.ToClearText : BakeDirection.ToBase64;

            using (var output = new StringWriter())
            {
                using (var input = File.OpenText(inputFilename))   
                {
                    new YamlBaker().BakeYaml(input, output, direction);
                }

                string result = output.ToString().Trim();

                File.WriteAllText(outputFilename, result);

                Console.WriteLine("Results written to {0}", outputFilename);
            }
        }
    }
}
