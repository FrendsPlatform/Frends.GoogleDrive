using Frends.GoogleDrive.Upload.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.GoogleDrive.Upload.Tests;
#pragma warning disable CS8604 // Possible null reference argument.

[TestClass]
public class UnitTests
{
    private readonly string? _googleDrive_CredBase64_Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? _googleDrive_CredBase64_Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly string? _driveFolder = Environment.GetEnvironmentVariable("GoogleDrive_FolderId");
    private Input _input = new();
    private Options _options = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));

        _input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileMask = default,
            SourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles"),
            TargetFolderId = _driveFolder,
        };

        _options = new()
        {
            ThrowErrorIfNoMatch = true,
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task UploadTest_ALL_Success()
    {
        _input.FileMask = "*";
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
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
        _input.ServiceAccountKeyJSON = "filepath";
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_JPG_Success()
    {
        _input.FileMask = "*.jpg";
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_PDF_Success()
    {
        _input.FileMask = "*.pdf";
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
    }

    [TestMethod]
    public async Task UploadTest_NoFileMask_Success()
    {
        _input.FileMask = null;
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_InvalidSource_Throw()
    {
        _input.SourceDirectory = "NoFilesHere";
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Upload.GoogleDrive.Upload(_input, _options, default));
        Assert.AreEqual("Source path not found. (Parameter 'sourceDirectory Value:NoFilesHere')", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_MissingJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await Upload.GoogleDrive.Upload(_input, _options, default));
        Assert.AreEqual("Service account key missing. (Parameter 'serviceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_InvalidJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await Upload.GoogleDrive.Upload(_input, _options, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_FileNotFound_Throw()
    {
        _input.FileMask = "NoFile";
        var result = await Assert.ThrowsExceptionAsync<Exception>(async () => await Upload.GoogleDrive.Upload(_input, _options, default));
        Assert.AreEqual("No files match the filemask 'NoFile' within supplied path.", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_FileNotFound_ThrowErrorIfNoMatch_false()
    {
        _input.FileMask = "NoFile";
        _options.ThrowErrorIfNoMatch = false;
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_Throw()
    {
        _input.TargetFolderId = "123";
        var result = await Assert.ThrowsExceptionAsync<Exception>(async () => await Upload.GoogleDrive.Upload(_input, _options, default));
        Assert.AreEqual("Error uploading file: test-pdf-document.pdf. Error: The service drive has thrown an exception. HttpStatusCode is NotFound. File not found: 123.", result.Message);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_ThrowErrorOnFailure_false()
    {
        _input.TargetFolderId = "123";
        _options.ThrowErrorOnFailure = false;
        var result = await Upload.GoogleDrive.Upload(_input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(2, result.ErrorMessage.Count);
    }
}
