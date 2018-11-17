using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");

            var exitFlg = false;
            while (!exitFlg)
            {
                Console.Write("Agent> ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "exit": exitFlg = true; break;
                    case "upload": Upload();  break;
                    default: break;
                }
            }
        }
        static void Upload()
        {
            var config = File.ReadAllText(Environment.CurrentDirectory + "\\config\\" + "config.json");
            var configToken = JObject.Parse(config);

            var host = configToken["FtpServerIp"].ToString();
            var port = configToken["FtpServerPort"].ToString();
            var username = configToken["User"].ToString();
            var password = configToken["Password"].ToString();
            var workingdirectory = configToken["DestinationPath"].ToString();
            var uploadfile = configToken["SourceFiles"][0].ToString();

            Common.Upload(host, port, username, password, workingdirectory, uploadfile);
        }
    }
}
