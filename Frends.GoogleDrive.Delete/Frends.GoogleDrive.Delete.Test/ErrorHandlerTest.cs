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
        await Assert.ThrowsExceptionAsync<System.ComponentModel.DataAnnotations.ValidationException>(
            async () => await GoogleDrive.Delete(DefaultInput(), new Options(), default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await GoogleDrive.Delete(DefaultInput(), options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(
            async () => await GoogleDrive.Delete(DefaultInput(), options, default));
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }

    private static Input DefaultInput() => new() { ServiceAccountKeyJSON = null, FileQuery = null };
}
