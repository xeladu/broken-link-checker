using System;

namespace BrokenLinkChecker.App.ArgumentParsing;

public class ArgumentParseException : Exception
{
    public ArgumentParseException() : base() { }

    public ArgumentParseException(string message) : base(message) { }

    public ArgumentParseException(string message, Exception innerException) : base(message, innerException) { }
}
