using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraCrypt_Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Find VeraCrypt mounts and drive letter. use /x for extended info.");
            
            if (Environment.GetCommandLineArgs().Contains<string>("/x"))
                GetMounts().Wait();
            else
                GetMountsSimple().Wait();
        }



        private static async Task GetMountsSimple()
        {
            var found = false;
            var a = await Utils.VcGetMounts.getMounted();


            try
            {
                foreach (var item in a)
                {
                    if (!found) // on first iteration
                        Console.WriteLine("letter\tMount source");

                    Console.WriteLine(item.Key + "\t" + item.Value.volumeName);

                    found = true;
                }
            }
            catch
            { }

            if (!found)
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
                foreach (var item in a)
                {
                    if (!found) // on first iteration
                    {
                        foreach (var prop in keys)
                            Console.Write("{0}\t", prop);

                        Console.Write("\n");
                    }

                    foreach (var pKey in keys)
                    {
                        var prop = item.Value.GetType().GetProperty(pKey);
                        try
                        {
                            var x = prop.GetValue(item.Value);
                            Console.Write("{0}\t", x.ToString().Equals("") ? "?" : x);
                        }
                        catch
                        {
                            Console.Write("?\t");
                        }
                    }

                    Console.Write("\n");
                    found = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!found)
                Console.WriteLine("No mounts found.");
        }

     }
}
