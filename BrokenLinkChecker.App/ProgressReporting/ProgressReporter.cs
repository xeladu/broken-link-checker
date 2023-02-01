using System;

namespace BrokenLinkChecker.App.ProgressReporting;

public abstract class ProgressReporter
{
    public event EventHandler<ProgressEventArgs> OnReportProgress = delegate { };

    public event EventHandler<ProgressEventArgs> OnReportProgressVerbose = delegate { };

    /// <summary>
    /// Publishes a message to all listeners. Also calls ReportProgressVerbose!
    /// </summary>
    /// <param name="message">The message to publish</param>
    protected void ReportProgress(string message)
    {
        OnReportProgress?.Invoke(this, new ProgressEventArgs(message));
        OnReportProgressVerbose?.Invoke(this, new ProgressEventArgs(message));
    }

    /// <summary>
    /// Publishes a message to all listeners. Also calls ReportProgressVerbose!
    /// </summary>
    /// <param name="message">The message to publish</param>
    protected void ReportProgressVerbose(string message)
    {
        OnReportProgressVerbose?.Invoke(this, new ProgressEventArgs(message));
    }
}
