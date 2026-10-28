using System;
using System.Threading.Tasks;
using Frends.GoogleDrive.Delete.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Delete.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await GoogleDrive.Delete(InvalidInput(), new Options(), default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = await GoogleDrive.Delete(InvalidInput(), new Options { ThrowErrorOnFailure = false }, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await GoogleDrive.Delete(InvalidInput(), new Options { ErrorMessageOnFailure = CustomErrorMessage }, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input InvalidInput() => new() { ServiceAccountKeyJSON = string.Empty, FileQuery = null };
}
