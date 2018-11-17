using ExcelDataReader;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    /// <summary>
    /// All common methods
    /// </summary>
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

        public static bool SaveExcelFileAsCsv(string excelFilePath, string destinationCsvFilePath)
        {

            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader = null;
                if (excelFilePath.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (excelFilePath.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                if (reader == null)
                    return false;

                var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                });

                var csvContent = string.Empty;
                int row_no = 0;
                while (row_no < ds.Tables[0].Rows.Count)
                {
                    var arr = new List<string>();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        arr.Add(string.Format("\"{0}\"", ds.Tables[0].Rows[row_no][i].ToString().Replace("\"","\"\"")));
                    }
                    row_no++;
                    csvContent += string.Join(",", arr) + "\n";
                }
                StreamWriter csv = new StreamWriter(destinationCsvFilePath, false, System.Text.Encoding.UTF8);
                csv.Write(csvContent);
                csv.Close();
                return true;
            }
        }
    }
}
