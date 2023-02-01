using System;

namespace BrokenLinkChecker.App.ProgressReporting;

public class ProgressEventArgs : EventArgs
{
    public string Message { get; set; }

    public ProgressEventArgs(string message) => Message = message;
}
