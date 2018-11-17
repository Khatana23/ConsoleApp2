﻿using Renci.SshNet;
using System;
using System.IO;

namespace ConsoleApp2
{
    public class Common
    {
        public static void Upload(string host, string port, string username, string password, string workingdirectory, string uploadfile)
        {
            try
            {
                Console.WriteLine("Creating client and connecting");
                using (var client = new SftpClient(host, int.Parse(port), username, password))
                {
                    client.Connect();
                    Console.WriteLine("Connected to {0}", host);

                    client.ChangeDirectory(workingdirectory);
                    Console.WriteLine("Changed directory to {0}", workingdirectory);

                    var listDirectory = client.ListDirectory(workingdirectory);
                    Console.WriteLine("Listing directory:");
                    foreach (var fi in listDirectory)
                    {
                        Console.WriteLine(" - " + fi.Name);
                    }
                    Console.WriteLine("Press to continue...");
                    Console.ReadLine();
                    using (var fileStream = new FileStream(uploadfile, FileMode.Open))
                    {
                        Console.WriteLine("Uploading {0} ({1:N0} bytes)", uploadfile, fileStream.Length);
                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, Path.GetFileName(uploadfile));
                        Console.WriteLine("Uploaded successfully.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
