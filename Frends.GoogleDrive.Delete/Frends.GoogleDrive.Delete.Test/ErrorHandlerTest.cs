using System;
using System.Threading;
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
        var ex = await Assert.ThrowsExceptionAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () =>
            await GoogleDrive.Delete(DefaultInput(), DefaultOptions(), CancellationToken.None));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = await GoogleDrive.Delete(DefaultInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await GoogleDrive.Delete(DefaultInput(), options, CancellationToken.None));
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input DefaultInput() => new() { ServiceAccountKeyJSON = string.Empty };

    private static Options DefaultOptions() => new();
}
