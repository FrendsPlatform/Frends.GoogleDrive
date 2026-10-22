using System;
using System.Threading.Tasks;
using Frends.GoogleDrive.Download.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Download.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new()
    {
        ServiceAccountKeyJSON = string.Empty,
        DestinationDirectory = "destination",
        FileQuery = "name = 'test.txt'",
    };

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = new Options { ThrowErrorOnFailure = true, CreateDestinationFolder = false };
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Download(InvalidInput(), options, default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false, CreateDestinationFolder = false };
        var result = await GoogleDrive.Download(InvalidInput(), options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ThrowErrorOnFailure = true, ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await GoogleDrive.Download(InvalidInput(), options, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }
}
