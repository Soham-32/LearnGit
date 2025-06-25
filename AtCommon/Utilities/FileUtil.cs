using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace AtCommon.Utilities
{
    public class FileUtil
    {
        public string GetBasePath()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            return path.EndsWith(@"\") ? path : @$"{path}\";
        }

        public string GetDownloadPath()
        {
            var pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(pathUser, "Downloads");
        }

        public void DeleteFilesInDownloadFolder(string fileName)
        {
            var splitFile = new FileName(fileName);

            new DirectoryInfo(GetDownloadPath()).GetFiles()
                .Where(f => f.Name.StartsWith(splitFile.Name) &&
                f.Extension.Equals($".{splitFile.Extension}", StringComparison.InvariantCultureIgnoreCase)).ToList()
                .ForEach(m => m.Delete());
        }

        public bool IsFileDownloaded(string fileName, int timeout = 60)
        {
            var splitFile = new FileName(fileName);

            for (var i = 0; i < timeout; i++)
            {
                if (new DirectoryInfo(GetDownloadPath()).GetFiles($"{splitFile.Name}*").Any(
                    f => f.Extension.ToLower().Equals($".{splitFile.Extension}".ToLower())))
                    return true;

                //if file not found wait for 1s.
                Thread.Sleep(1000);
            }

            return false;
        }

        public string WaitUntilFileDownloaded(string fileName, int timeOut = 120)
        {
            var splitFile = new FileName(fileName);

            var dir = new DirectoryInfo(GetDownloadPath());
            for (var i = 0; i < timeOut; i++)
            {
                var matches = dir.GetFiles($"{splitFile.Name}*").Where(
                    f => f.Extension.ToUpper().Equals($".{splitFile.Extension}".ToUpper())).ToList();
                if (matches.Any())
                {
                    return matches.First().FullName;
                }

                Thread.Sleep(1000);

            }
            throw new Exception($"The file <{fileName}> was not downloaded. Searched for {timeOut} seconds.");
        }

        public void DeleteAllFilesOlderThan(int days, string directoryPath, string fileExtension = "")
        {
            foreach (var file in new DirectoryInfo(directoryPath).GetFiles("*" + fileExtension))
            {
                if (file.CreationTime.AddDays(days).CompareTo(DateTime.Now) >= 0) continue;
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public List<string> TextFileToList(string filePath)
        {
            var text = File.ReadAllText(filePath);

            return text.SplitLines();
        }

        public static string GetTestConfigurationIdFromJson(string jsonFile)
        {
            // get the contents of the file
            var text = File.ReadAllText(jsonFile);
            // convert the json to object
            dynamic json = JsonConvert.DeserializeObject(text);

            return json?.TestCases[0].TestConfigurationId;
        }

        public struct FileName
        {
            public FileName(string name)
            {
                Name = name.Substring(0, name.LastIndexOf('.'));
                Extension = name.Substring(name.LastIndexOf('.') + 1);
            }

            public string Name { get; set; }
            public string Extension { get; set; }

            public override string ToString()
            {
                return $"{Name}.{Extension}";
            }

        }

        public string GetPdfData(string fileName)
        {
            var pdfPath = WaitUntilFileDownloaded(fileName);
            Thread.Sleep(1000);

            var sb = new StringBuilder();
            var reader = new PdfReader(pdfPath);

            for (var pageNo = 1; pageNo <= reader.NumberOfPages; pageNo++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(reader, pageNo, strategy);
                text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
                sb.Append(text);
            }
            reader.Close();
            return sb.ToString();
        }

    }
}
