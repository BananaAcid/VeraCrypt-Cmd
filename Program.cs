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
            Console.WriteLine("Find VeraCrypt mounts and drive letter.");
            GetMounts().Wait();
        }


        private static async Task GetMounts()
        {
            var found = false;
            var a = await Utils.VcGetMounts.getMounted();


            try
            {
                foreach (var item in a)
                {
                    if (!found) // on first iteration
                        Console.WriteLine("Letter\tMount source");

                    Console.WriteLine(item.Key + "\t" + item.Value);

                    found = true;
                }
            }
            catch
            { }

            if (!found)
                Console.WriteLine("No mounts found.");
        }
    }
}
