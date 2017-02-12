using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ScheduleRipper
{
    static class HttpRequestHandler
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static string filePath = path + "/classes.xml";

        public static void StartApp(string cookie)
        {
            try
            {
                var request = CreateRequest(cookie);

                if (request == null) return;

                Console.WriteLine("Sending request...");
                var response = (HttpWebResponse)request.GetResponse();
                ChangeMessageColor("Response received! details below:");
                Console.Write("Response status:");
                ResponseMessage(response.StatusCode);
                
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                File.WriteAllText(Environment.CurrentDirectory + "/out.txt", responseString);

                Ripper();
            }
            catch (Exception e)
            {
                ChangeMessageColor(e.ToString(), ConsoleColor.Red);
            }
        }

        private static HttpWebRequest CreateRequest(string cookie)
        {
            try
            {
                Console.WriteLine("Creating request...");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.rocwb.nl/portalapps/roosters/RC/THW350/frames/navbar.htm");

                request.KeepAlive = true;
                request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
                request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Referer = "https://portal.rocwb.nl/CookieAuth.dll?GetLogon?curl=Z2FportalappsZ2FroostersZ2FRCZ2FTHW350Z2FframesZ2Fnavbar.htm&reason=0&formdir=51";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "nl-NL,nl;q=0.8,en-US;q=0.6,en;q=0.4");
                request.Headers.Set(HttpRequestHeader.Cookie, cookie);

                ChangeMessageColor("Request succesfully created!");

                return request;
            }
            catch (Exception ex)
            {
                ChangeMessageColor("An error occured! \n" + ex.ToString(), ConsoleColor.Red);
                return null;
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
            }
            catch (Exception ex)
            {
                ChangeMessageColor("An error occured! \n" + ex.ToString(), ConsoleColor.Red);
            }
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
