using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotenv.net;
using Frends.GoogleDrive.Upload.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Upload.Tests;
#pragma warning disable CS8604 // Possible null reference argument.

[TestClass]
public class UnitTests
{
    private readonly string? googleDriveCredBase64Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1") ?? string.Empty;
    private readonly string? googleDriveCredBase64Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2") ?? string.Empty;
    private readonly string? driveFolder = Environment.GetEnvironmentVariable("GoogleDrive_FolderId");
    private string testSourceDirectory = string.Empty;
    private string testCredentialsDirectory = string.Empty;
    private Input input = new();
    private Options options = new();

    static UnitTests()
    {
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
    }

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(googleDriveCredBase64Part1 + googleDriveCredBase64Part2));
        testSourceDirectory = Path.Combine(Path.GetTempPath(), $"Frends.GoogleDrive.Upload.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(testSourceDirectory);
        File.WriteAllBytes(Path.Combine(testSourceDirectory, "test-pdf-document.pdf"), Array.Empty<byte>());
        File.WriteAllBytes(Path.Combine(testSourceDirectory, "test-photograph.jpg"), Array.Empty<byte>());

        input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileMask = default,
            SourceDirectory = testSourceDirectory,
            TargetFolderId = driveFolder,
            IncludeSharedDrives = true,
        };

        options = new()
        {
            ThrowErrorIfNoMatch = true,
            ThrowErrorOnFailure = true,
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(testSourceDirectory))
            Directory.Delete(testSourceDirectory, recursive: true);

        if (Directory.Exists(testCredentialsDirectory))
            Directory.Delete(testCredentialsDirectory, recursive: true);
    }

    [TestMethod]
    public async Task UploadTest_ALL_Success()
    {
        input.FileMask = "*";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.IsNull(result.Error);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_JSONasFile()
    {
        testCredentialsDirectory = Path.Combine(Path.GetTempPath(), $"Frends.GoogleDrive.Upload.Credentials.{Guid.NewGuid():N}");
        Directory.CreateDirectory(testCredentialsDirectory);
        var credentialsPath = Path.Combine(testCredentialsDirectory, "service-account.json");
        await File.WriteAllTextAsync(credentialsPath, input.ServiceAccountKeyJSON);
        input.ServiceAccountKeyJSON = credentialsPath;

        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.IsNull(result.Error);
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
        Assert.IsNull(result.Error);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task UploadTest_PDF_Success()
    {
        input.FileMask = "*.pdf";
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.IsNull(result.Error);
        Assert.IsTrue(result.Data.Any(x => x.UploadedFileName.Equals("test-pdf-document.pdf")));
    }

    [TestMethod]
    public async Task UploadTest_NoFileMask_Success()
    {
        input.FileMask = null;
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.IsNull(result.Error);
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
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_Throw()
    {
        input.TargetFolderId = "123";
        input.FileMask = "*.pdf";
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
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task UploadTest_NoSuchFolder_ContinueOnFailure_true()
    {
        input.TargetFolderId = "123";
        options.ContinueOnFailure = true;
        var result = await Upload.GoogleDrive.Upload(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(2, result.Error.FailedFiles.Length);
    }
}
