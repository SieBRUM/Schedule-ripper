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
            HttpRequestHandler.ChangeMessageColor("Version 2.0", ConsoleColor.Cyan);
            HttpRequestHandler.ChangeMessageColor("Made by Siebren Kraak", ConsoleColor.Cyan);
            Console.WriteLine();

            Console.WriteLine("Press a key to start ripping...");
            Console.ReadKey();
            Console.Clear();
            HttpRequestHandler.StartApp();
            Application.Run();
        }
    }
}
