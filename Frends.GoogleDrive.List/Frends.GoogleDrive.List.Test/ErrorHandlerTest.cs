using Frends.GoogleDrive.List.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.GoogleDrive.List.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new() { ServiceAccountKeyJSON = string.Empty };

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await GoogleDrive.List(InvalidInput(), new Options(), default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = await GoogleDrive.List(InvalidInput(), new Options { ThrowErrorOnFailure = false }, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await GoogleDrive.List(InvalidInput(), new Options { ErrorMessageOnFailure = CustomErrorMessage }, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }
}
