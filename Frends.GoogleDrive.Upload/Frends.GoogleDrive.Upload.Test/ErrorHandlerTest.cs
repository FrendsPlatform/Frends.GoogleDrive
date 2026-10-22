using System;
using Frends.GoogleDrive.Upload.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Frends.GoogleDrive.Upload.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";
    private readonly Input _input = new() { ServiceAccountKeyJSON = string.Empty, SourceDirectory = "NoFilesHere" };

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = new Options { ThrowErrorOnFailure = true };
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Upload(_input, options, default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await GoogleDrive.Upload(_input, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ThrowErrorOnFailure = true, ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await GoogleDrive.Upload(_input, options, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }
}
