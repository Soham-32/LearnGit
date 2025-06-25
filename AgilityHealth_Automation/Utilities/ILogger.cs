using System;

namespace AgilityHealth_Automation.Utilities
{
    public interface ILogger
    {

        int StepCount { get; set; }

        void Info(string message);
        void Warning(string message);
        void Error(Exception e);
        void Step(string pageName, string message);
    }
}