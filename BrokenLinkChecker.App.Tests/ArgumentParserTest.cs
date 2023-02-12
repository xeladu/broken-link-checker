using BrokenLinkChecker.App.ArgumentParsing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrokenLinkChecker.App.Tests;

[TestClass]
public class ArgumentParserTest
{
    [TestMethod]
    [DataRow(new[] { "test", "--no-follow-internal-links", "--output", "C:\\test1", "--verbose" }, false, "test", false, "C:\\test1", true, false)]
    [DataRow(new[] { "test", "--output", "C:\\test2", "--verbose" }, false, "test", true, "C:\\test2", true, false)]
    [DataRow(new[] { "test", "--output", "C:\\test3", "--verbose" }, false, "test", true, "C:\\test3", true, false)]
    [DataRow(new[] { "test", "--no-follow-internal-links", "--output", "C:\\test4", "--verbose" }, false, "test", false, "C:\\test4", true, false)]
    [DataRow(new[] { "test", "--output", "C:\\test5", "--verbose" }, false, "test", true, "C:\\test5", true, false)]
    [DataRow(new[] { "test", "--no-follow-internal-links", "C:\\test6", "--output" }, true, "test", false, "C:\\test6", false, false)]
    [DataRow(new[] { "test" }, false, "test", true, "", false, false)]
    [DataRow(new[] { "test", "-v", "--output", "C:\\test7" }, false, "test", true, "C:\\test7", true, false)]
    [DataRow(new[] { "--no-follow-internal-links", "--output", "C:\\test8", "--json" }, true, "test", true, "C:\\test8", false, true)]
    [DataRow(new[] { "test", "-v", "--json" }, true, "test", true, "C:\\test9", true, true)]
    [DataRow(new string[0], true, "test", true, "", false, false)]
    [DataRow(new[] { "test", "abc" }, true, "test", false, "", false, false)]
    [DataRow(new[] { "test", "--abc" }, true, "test", false, "", false, false)]
    public void Test(
        string[] input,
        bool shouldThrowException,
        string expectedBaseUrlValue,
        bool expectedFollowInternalLinksValue,
        string expectedOutputValue,
        bool expectedVerboseValue,
        bool expectedJsonValue)
    {
        try
        {
            var settings = ArgumentParser.Parse(input);
            Assert.AreEqual(expectedBaseUrlValue, settings.BaseUrl);
            Assert.AreEqual(expectedFollowInternalLinksValue, settings.FollowInternalLinks);
            Assert.AreEqual(expectedOutputValue, settings.OutputPath);
            Assert.AreEqual(expectedVerboseValue, settings.DetailedLogMessages);
            Assert.AreEqual(expectedJsonValue, settings.Json);

            Assert.IsFalse(shouldThrowException, "Unexpected ArgumentParseException was thrown!");
        }
        catch (ArgumentParseException)
        {
            Assert.IsTrue(shouldThrowException, "Expected ArgumentParseException was not thrown!");
        }
    }
}