using BrokenLinkChecker.App.ArgumentParsing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrokenLinkChecker.App.Tests;

[TestClass]
public class ArgumentParserTest
{
    [TestMethod]
    [DataRow(new[] { "test", "--follow-internal-links", "true", "--output", "C:\\test1", "--verbose" }, false, "test", true, "C:\\test1", true)]
    [DataRow(new[] { "test", "--follow-internal-links", "TRUE", "--output", "C:\\test2", "--verbose" }, false, "test", true, "C:\\test2", true)]
    [DataRow(new[] { "test", "--follow-internal-links", "TrUe", "--output", "C:\\test3", "--verbose" }, false, "test", true, "C:\\test3", true)]
    [DataRow(new[] { "test", "--follow-internal-links", "false", "--output", "C:\\test4", "--verbose" }, false, "test", false, "C:\\test4", true)]
    [DataRow(new[] { "test", "true", "--follow-internal-links", "--output", "C:\\test5", "--verbose" }, true, "test", true, "C:\\test5", true)]
    [DataRow(new[] { "test", "--follow-internal-links", "true", "C:\\test6", "--output" }, true, "test", true, "C:\\test6", false)]
    [DataRow(new[] { "test", "--follow-internal-links" }, true, "test", true, "", false)]
    [DataRow(new[] { "test", "--follow-internal-links", "true" }, false, "test", true, "", false)]
    [DataRow(new[] { "test" }, false, "test", true, "", false)]
    [DataRow(new[] { "test", "-v", "--output", "C:\\test7" }, false, "test", true, "C:\\test7", true)]
    [DataRow(new[] { "--follow-internal-links", "true", "--output", "C:\\test8" }, true, "test", true, "C:\\test8", false)]
    [DataRow(new string[0], true, "test", true, "", false)]
    [DataRow(new[] { "test", "abc" }, true, "test", false, "", false)]
    [DataRow(new[] { "test", "--abc" }, true, "test", false, "", false)]
    public void Test(
        string[] input,
        bool shouldThrowException,
        string expectedBaseUrlValue,
        bool expectedFollowInternalLinksValue,
        string expectedOutputValue,
        bool expectedVerboseValue)
    {
        var parser = new ArgumentParser();

        try
        {
            var settings = parser.Parse(input);
            Assert.AreEqual(expectedBaseUrlValue, settings.BaseUrl);
            Assert.AreEqual(expectedFollowInternalLinksValue, settings.FollowInternalLinks);
            Assert.AreEqual(expectedOutputValue, settings.OutputPath);
            Assert.AreEqual(expectedVerboseValue, settings.DetailedLogMessages);

            Assert.IsFalse(shouldThrowException, "Unexpected ArgumentParseException was thrown!");
        }
        catch (ArgumentParseException)
        {
            Assert.IsTrue(shouldThrowException, "Expected ArgumentParseException was not thrown!");
        }
    }
}