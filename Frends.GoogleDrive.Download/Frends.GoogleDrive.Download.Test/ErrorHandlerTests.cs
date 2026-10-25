using System;
using Frends.GoogleDrive.Download.Definitions;
using Frends.GoogleDrive.Download.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Download.Tests;

[TestClass]
public class ErrorHandlerTests
{
    [TestMethod]
    public void Handle_WhenNotThrowing_ReturnsFailureWithOriginalMessage()
    {
        var exception = new InvalidOperationException("Download failed.");
        var options = new Options { ThrowErrorOnFailure = false };

        var result = exception.Handle(options);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Download failed.", result.Error.Message);
        Assert.AreSame(exception, result.Error.AdditionalInfo);
    }

    [TestMethod]
    public void Handle_WhenNotThrowingWithCustomMessage_ReturnsCombinedMessage()
    {
        var exception = new InvalidOperationException("Download failed.");
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Task failed",
        };

        var result = exception.Handle(options);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Task failed: Download failed.", result.Error.Message);
        Assert.AreSame(exception, result.Error.AdditionalInfo);
    }

    [TestMethod]
    public void Handle_WhenThrowingWithoutCustomMessage_ThrowsOriginalException()
    {
        var exception = new InvalidOperationException("Download failed.");
        var options = new Options { ThrowErrorOnFailure = true };

        var thrown = Assert.ThrowsException<InvalidOperationException>(() => exception.Handle(options));

        Assert.AreSame(exception, thrown);
    }

    [TestMethod]
    public void Handle_WhenThrowingWithCustomMessage_ThrowsWrappedException()
    {
        var exception = new InvalidOperationException("Download failed.");
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Task failed",
        };

        var thrown = Assert.ThrowsException<Exception>(() => exception.Handle(options));

        Assert.AreEqual("Task failed", thrown.Message);
        Assert.AreSame(exception, thrown.InnerException);
    }

    [TestMethod]
    public void Handle_WhenCancellationIsRethrown_PreservesCancellationException()
    {
        var exception = new OperationCanceledException();
        var options = new Options { ThrowErrorOnFailure = false };

        var thrown = Assert.ThrowsException<OperationCanceledException>(() => exception.Handle(options));

        Assert.AreSame(exception, thrown);
    }

    [TestMethod]
    public void Handle_WhenCancellationIsNotRethrown_ReturnsFailure()
    {
        var exception = new OperationCanceledException("Canceled.");
        var options = new Options { ThrowErrorOnFailure = false };

        var result = exception.Handle(options, throwCanceled: false);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Canceled.", result.Error.Message);
    }
}
