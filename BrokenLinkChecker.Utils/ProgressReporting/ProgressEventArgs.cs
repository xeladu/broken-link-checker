using System;

namespace BrokenLinkChecker.Utils.ProgressReporting;

public class ProgressEventArgs : EventArgs
{
    public string Message { get; set; }

    public ProgressEventArgs(string message) => Message = message;
}
