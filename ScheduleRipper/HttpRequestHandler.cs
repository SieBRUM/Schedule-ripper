using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using System.Text;

namespace ScheduleRipper
{
    static class HttpRequestHandler
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static string filePath = path + "/classes.xml";
        private static WebBrowser browser = new WebBrowser() { ScriptErrorsSuppressed = true };

        public static void StartApp()
        {
            ChangeMessageColor("Initializing application...", ConsoleColor.Red);
            browser.Navigate(@"https://portal.rocwb.nl/portalapps/roosters/RC/THW350/frames/navbar.htm");
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Page_LoadComplete);
        }

        private static void Page_LoadComplete(object sender, EventArgs e)
        {
            var element = browser.Document.GetElementById("username");
            if (element != null)
            {
                ChangeMessageColor("Not logged in, logging in....");
                browser.Document.GetElementById("username").Focus();
                browser.Document.GetElementById("username").InnerText = "sk153265";
                browser.Document.GetElementById("password").Focus();
                browser.Document.GetElementById("password").InnerText = "26-06-1998";
                browser.Document.GetElementById("SubmitCreds").Focus();
                browser.Document.GetElementById("SubmitCreds").InvokeMember("click");
            }
            else
            {
                ChangeMessageColor("logged in!");
                File.WriteAllText(Environment.CurrentDirectory + "/out.txt", browser.Document.Body.Parent.OuterHtml, Encoding.GetEncoding(browser.Document.Encoding));
                Ripper();
            }
        }

        private static void Ripper()
        {
            List<string> klassen = new List<string>();

            string data = File.ReadAllText(Environment.CurrentDirectory + "/out.txt");

            Regex rgx = new Regex(@"var\sclasses\s=\s\[(.*?)\]");
            Match matches = rgx.Match(data);

            string[] split = matches.Groups[1].Value.Replace("\"", "").Split(',');

            for (int i = 0; i < split.Length; i++)
			{
			    klassen.Add(split[i]);
			}

            ListToXml(klassen);
        }

        private static void ListToXml(List<string> list)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            Console.WriteLine("Would you like to see all the output? y/whatever");
            string r = Console.ReadLine().ToLower();

            List<string> klassen = list;

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (XmlWriter w = XmlWriter.Create(fs, settings))
                {
                    w.WriteStartDocument();
                    w.WriteStartElement("klassen");

                    for (int i = 0; i < klassen.Count; i++)
                    {
                        var code = String.Format("{0:00000}", (i + 1));
                        if (r == "y")
                            ChangeMessageColor("saving " + klassen[i] + "    with url: " + code);

                        w.WriteStartElement("klas");
                        w.WriteAttributeString("ID", code);
                        w.WriteAttributeString("klas", klassen[i]);
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }

                if (File.Exists(Environment.CurrentDirectory + "/out.txt"))
                {
                    File.Delete(Environment.CurrentDirectory + "/out.txt");
                }
            }
            catch (Exception ex)
            {
                ChangeMessageColor("An error occured! \n" + ex.ToString(), ConsoleColor.Red);
            }

            ChangeMessageColor("Done ripping. Get rekt IT-Works", ConsoleColor.Red);
            Console.ReadKey();
            Application.Exit();
        }

        public static void ChangeMessageColor(String message, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void ResponseMessage(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Accepted)
            {
                ChangeMessageColor(statusCode.ToString());
            }
            else
            {
                ChangeMessageColor(statusCode.ToString(), ConsoleColor.Red);
            }
        }
    }
}
