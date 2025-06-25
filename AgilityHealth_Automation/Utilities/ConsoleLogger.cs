using System;
namespace AgilityHealth_Automation.Utilities
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
            StepCount = 1;
        }

        public int StepCount { get; set; }
        
        public void Info(string message)
        {
            Console.WriteLine($"{DateTime.Now} | INFO | {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"{DateTime.Now} | WARNING | {message}");
        }

        public void Error(Exception e)
        {
            Console.WriteLine($"{DateTime.Now} | ERROR | {e.Message}");
            if (e.InnerException != null)
                Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
        }

        public void Step(string pageName, string message)
        {
            Console.WriteLine($"{DateTime.Now} | Step {StepCount} | {pageName} | {message}");
            StepCount++;
        }
    }
}