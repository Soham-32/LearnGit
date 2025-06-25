using System;
using System.IO;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Utilities
{
    public class Logger : ILogger
    {
        public Logger(TestContext testContext)
        {
            var fileUtil = new FileUtil();
            Context = testContext;
            LogPath = $@"{fileUtil.GetBasePath()}\Resources\Logs\[LOG]_{testContext.TestName}_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log";
            StepCount = 1;
        }

        public Logger(string logPath)
        {
            LogPath = logPath;
            StepCount = 1;
        }

        public TestContext Context { get; set; }
        public string LogPath { get; set; }
        public int StepCount { get; set; }

        public void Step(string pageName, string message)
        {
            var fileStream = File.AppendText(LogPath);
            fileStream.WriteLine($"{DateTime.Now} | Step {StepCount} | {pageName} | {message}");
            fileStream.Close();
            StepCount++;
        }

        public void Info(string message)
        {
            var fileStream = File.AppendText(LogPath);
            fileStream.WriteLine($"{DateTime.Now} | INFO | {message}");
            fileStream.Close();
        }

        public void Warning(string message)
        {
            var fileStream = File.AppendText(LogPath);
            fileStream.WriteLine($"{DateTime.Now} | WARNING | {message}");
            fileStream.Close();
        }

        public void Error(Exception e)
        {
            var fileStream = File.AppendText(LogPath);

            fileStream.WriteLine($"{DateTime.Now} | ERROR | {e.Message}");
            fileStream.WriteLine(e.ToLogString(Environment.StackTrace));
            if (e.InnerException != null)
            {
                fileStream.WriteLine($"Inner Exception: {e.InnerException.Message}");
            }
            fileStream.Close();
        }
    }
}
