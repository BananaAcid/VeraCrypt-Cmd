using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeraCrypt_Cmd
{
    class Program
    {
        enum OutMode
        {
            TEXT,
            JSON,
            CSV
        }
        static OutMode outModeStrean = OutMode.TEXT;


        static void Main(string[] args)
        {
            if (Environment.GetCommandLineArgs().Contains<string>("/json"))
                outModeStrean = OutMode.JSON;
            if (Environment.GetCommandLineArgs().Contains<string>("/csv"))
                outModeStrean = OutMode.CSV;



            switch (outModeStrean)
            {
                case OutMode.TEXT:
                    {
                        Console.WriteLine("{0} v.{1}",
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                        );

                        Console.WriteLine("Find VeraCrypt mounts and drive letters.\nVeraCrypt must be installed. Use /x for extended info, /json for json output, /csv for comma seperated values\n");

                        break;
                    }
                case OutMode.JSON:
                    {
                        Console.WriteLine("{{\n\t\"version\": \"{0}\",\n\t\"mounts\": [",
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                        );
                        break;
                    }
                case OutMode.CSV:
                    { break; }
            }

            if (Environment.GetCommandLineArgs().Contains<string>("/x"))
                GetMounts().Wait();
            else
                GetMountsSimple().Wait();

            if (outModeStrean == OutMode.TEXT)
                Console.WriteLine("\n");
            else if (outModeStrean == OutMode.JSON)
                Console.WriteLine("\t]\n}");
        }



        private static async Task GetMountsSimple()
        {
            var found = false;
            var a = await Utils.VcGetMounts.getMounted();


            try
            {
                // header: CSV needs it, even if there is nothing found
                if (outModeStrean == OutMode.CSV)
                    Console.WriteLine("letter, volumeName");

                foreach (var item in a)
                {
                    // header
                    if (!found) // on first iteration
                    {
                        if (outModeStrean == OutMode.TEXT)
                            Console.WriteLine("Letter  Mount source (volumeName)");
                    }

                    lineOut(new Dictionary<string, object> {
                        { "letter", item.Key},
                        { "volumeName", item.Value.volumeName}
                    }, found);
                    //Console.WriteLine(item.Key + "\t" + item.Value.volumeName);

                    found = true;
                }
            }
            catch (Exception ex)
            {
                lineOut(new Dictionary<string, object> {
                    { "error", ex.Message}
                }, found);
                //Console.WriteLine(ex.Message);
            }

            if (!found && outModeStrean == OutMode.TEXT)
                Console.WriteLine("No mounts found.");
        }



        private static async Task GetMounts()
        {
            var found = false;
            var a = await Utils.VcGetMounts.getMounted();

            // fixed order
            var keys = new string[] { "letter", "truecryptMode", "diskLength", "volumeLabel", "volumeName" };

            try
            {
                // header: CSV needs it, even if there is nothing found
                if (outModeStrean == OutMode.CSV)
                    Console.Write("{0}\n", String.Join(", ", keys));

                foreach (var item in a)
                {
                    // header
                    if (!found) // on first iteration
                    {
                        if (outModeStrean == OutMode.TEXT)
                            Console.Write("{0}\n", String.Join("  ", keys));
                    }

                    var row = new Dictionary<string, object>();

                    // data
                    foreach (var pKey in keys)
                    {
                        var prop = item.Value.GetType().GetProperty(pKey);
                        try
                        {
                            var x = prop.GetValue(item.Value);

                            row.Add(pKey, x);
                            // Console.Write("{0}\t", x.ToString().Equals("") ? "?" : x);
                        }
                        catch
                        {
                            //Console.Write("?\t");
                            row.Add(pKey, null);
                        }
                    }

                    lineOut(row, found);
                    //Console.Write("\n");

                    found = true;
                }
            }
            catch (Exception ex)
            {
                lineOut(new Dictionary<string, object> {
                    { "error", ex.Message}
                }, found);
                //Console.WriteLine(ex.Message);
            }

            if (!found && outModeStrean == OutMode.TEXT)
                Console.WriteLine("No mounts found.");
        }




        /// <summary>
        /// Own way of a stream writer, we do not cache up lines to generate a JSON file (line by line)
        /// </summary>
        /// <param name="row">key value pairs of drive properties</param>
        /// <param name="isPreceeded">for JSON: if there was a block outputted before (JSON: no trailing comma allowed)</param>
        private static void lineOut(Dictionary<string, object> row, bool isPreceeded = false)
        {
            switch (outModeStrean)
            {
                case OutMode.JSON:
                    {
                        var rows = row.Select(x =>
                        {
                            if (x.Value is System.String || x.Value is System.Char)
                                return String.Format("\t\t\t\"{0}\": \"{1}\"", x.Key, x.Value.ToString().Replace(@"\", @"\\"));
                            else if (x.Value.Equals(null))
                                return String.Format("\t\t\t\"{0}\": {1}", x.Key, "null");
                            else if (x.Value is System.Boolean)
                                return String.Format("\t\t\t\"{0}\": {1}", x.Key, (bool)x.Value ? "true" : "false");
                            else
                                return String.Format("\t\t\t\"{0}\": {1}", x.Key, (ulong)x.Value);
                        });

                        Console.Write("\t\t{0}\n\t\t{{\n{1}\n\t\t}}\n", isPreceeded ? "," : "", String.Join(",\n", rows));

                        break;
                    }
                case OutMode.TEXT:
                    {
                        var rows = row.Select(x =>
                        {
                            // min field width is header 
                            var val = x.Value.ToString().Equals("") ? "?" : x.Value;
                            var diff = x.Key.Length - val.ToString().Length;
                            val = diff > 0 ? val + new String(' ', diff) : val;

                            return String.Format("{0}", val);
                        });
                        Console.Write("{0}\n", String.Join("  ", rows));

                        break;
                    }
                case OutMode.CSV:
                    {
                        var rows = row.Select(x =>
                        {
                            return Csv.Escape( x.Value.ToString() );
                        });
                        Console.Write("{0}\n", String.Join(", ", rows));
                        break;
                    }
                default:
                    {
                        Console.Write("unknown output type");
                        return;
                    }
            }
        }

    }


    /// <summary>
    /// Class to escape CSV fields
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/769713/1644202"/>
    public static class Csv
    {
        public static string Escape(string s)
        {
            if (s.Contains(QUOTE))
                s = s.Replace(QUOTE, ESCAPED_QUOTE);

            if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                s = QUOTE + s + QUOTE;

            return s;
        }

        public static string Unescape(string s)
        {
            if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(ESCAPED_QUOTE))
                    s = s.Replace(ESCAPED_QUOTE, QUOTE);
            }

            return s;
        }


        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };
    }

}
