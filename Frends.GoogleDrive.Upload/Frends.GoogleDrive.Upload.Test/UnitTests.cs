using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frends.GoogleDrive.Upload.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Upload.Tests;
#pragma warning disable CS8604 // Possible null reference argument.

[TestClass]
public class UnitTests
{
    private readonly string? googleDriveCredBase64Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? googleDriveCredBase64Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly string? driveFolder = Environment.GetEnvironmentVariable("GoogleDrive_FolderId");
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(googleDriveCredBase64Part1 + googleDriveCredBase64Part2));

        input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileMask = default,
            SourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles"),
            TargetFolderId = driveFolder,
        };

        options = new()
        {
            ThrowErrorIfNoMatch = true,
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task UploadTest_ALL_Success()
    {
        input.FileMask = "*";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [Ignore("Can't test this in github")]
    [TestMethod]
    public async Task UploadTest_JSONasFile()
    {
        input.ServiceAccountKeyJSON = "filepath";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_JPG_Success()
    {
        input.FileMask = "*.jpg";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_PDF_Success()
    {
        input.FileMask = "*.pdf";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
    }

    [TestMethod]
    public async Task UploadTest_NoFileMask_Success()
    {
        input.FileMask = null;
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_InvalidSource_Throw()
    {
        input.SourceDirectory = "NoFilesHere";
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Upload.GoogleDrive.Upload(input, options, default));
        Assert.AreEqual("Source path not found. (Parameter 'sourceDirectory Value:NoFilesHere')", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_MissingJSON_Throw()
    {
        input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Upload.GoogleDrive.Upload(input, options, default));
        Assert.AreEqual("Service account key missing. (Parameter 'serviceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_InvalidJSON_Throw()
    {
        input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await Upload.GoogleDrive.Upload(input, options, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_FileNotFound_Throw()
    {
        input.FileMask = "NoFile";
        var result = await Assert.ThrowsExceptionAsync<Exception>(async () => await Upload.GoogleDrive.Upload(input, options, default));
        Assert.AreEqual("No files match the filemask 'NoFile' within supplied path.", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_FileNotFound_ThrowErrorIfNoMatch_false()
    {
        input.FileMask = "NoFile";
        options.ThrowErrorIfNoMatch = false;
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_Throw()
    {
        input.TargetFolderId = "123";
        var result = await Assert.ThrowsExceptionAsync<Exception>(async () => await Upload.GoogleDrive.Upload(input, options, default));
        Assert.AreEqual("Error uploading file: test-pdf-document.pdf. Error: The service drive has thrown an exception. HttpStatusCode is NotFound. File not found: 123.", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_ThrowErrorOnFailure_false()
    {
        input.TargetFolderId = "123";
        options.ThrowErrorOnFailure = false;
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(2, result.ErrorMessage.Count);
    }
}
