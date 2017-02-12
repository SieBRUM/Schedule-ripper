using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ScheduleRipper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            HttpRequestHandler.ChangeMessageColor("------------Booting Application------------", ConsoleColor.Cyan);
            HttpRequestHandler.ChangeMessageColor("Version 1.0", ConsoleColor.Cyan);
            HttpRequestHandler.ChangeMessageColor("Made by Siebren Kraak", ConsoleColor.Cyan);
            Console.WriteLine();

            Console.WriteLine("Press a key to start ripping...");
            Console.ReadKey();
            Console.Clear();

            Console.Write("Please put cookie on clipboard. Press enter to continue...");
            Console.ReadLine();
            string a = Clipboard.GetText().Replace(@"\", "").Replace("\"\"", "\"");
            HttpRequestHandler.StartApp(a);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}
