using System;
using System.ComponentModel.DataAnnotations;
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
        var ex = await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            await GoogleDrive.Delete(DefaultInput(), DefaultOptions(), default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = await GoogleDrive.Delete(DefaultInput(), options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await GoogleDrive.Delete(DefaultInput(), options, default));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input DefaultInput() => new() { ServiceAccountKeyJSON = string.Empty, FileQuery = null };

    private static Options DefaultOptions() => new();
}
