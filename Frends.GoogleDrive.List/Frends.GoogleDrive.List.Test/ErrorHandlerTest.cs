using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Frends.GoogleDrive.List.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.List.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            await GoogleDrive.List(InvalidInput(), new Options(), default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await GoogleDrive.List(InvalidInput(), options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await GoogleDrive.List(InvalidInput(), options, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input InvalidInput() => new() { ServiceAccountKeyJSON = string.Empty };
}
