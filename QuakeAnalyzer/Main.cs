using System;
using System.IO;
using System.Text;
using P2PQuake.JMAInformation.Quake;
using P2PQuake.JMAInformation.Quake.Convert;
using Newtonsoft.Json;

namespace QuakeAnalyzer
{
    class MainClass
    {
        public static void Main (string[] args)
        { 
            if (args.Length < 1)
            {
                Console.Error.WriteLine("usage: QuakeAnalyzer <input.html> [<output.json>]");
                return;
            }

            string inputFileName  = args[0];
            string outputFileName = (args.Length >= 2) ? args[1] : null;

            Console.Error.WriteLine ("Convert from " + inputFileName + " to " + outputFileName + ".");

            // ファイル読み込み
            StreamReader reader  = new StreamReader(inputFileName, Encoding.UTF8);
            string       content = reader.ReadToEnd ();
            reader.Close ();

            Core core = JMAQuakeConverter.fromString(content);
            string json = JsonConvert.SerializeObject(core, Formatting.Indented);

            if (outputFileName != null)
            {
                // ファイル書き込み
                StreamWriter writer = new StreamWriter(outputFileName, false, new UTF8Encoding(false));
                writer.Write(json);
                writer.Close();
            }
            else
            {
                Console.WriteLine(json);
            }

            Console.Error.WriteLine ("Finish at " + DateTime.Now.ToString());
        }
    }
}
