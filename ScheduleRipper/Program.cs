using System;
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
            HttpRequestHandler.StartApp();
            Application.Run();
        }
    }
}
