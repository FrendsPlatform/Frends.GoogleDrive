using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frends.GoogleDrive.Download.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Download.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? googleDriveCredBase64Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? googleDriveCredBase64Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly string destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles");
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(googleDriveCredBase64Part1 + googleDriveCredBase64Part2));

        input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            DestinationDirectory = destinationFolder,
            FileQuery = null,
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
            CreateDestinationFolder = true,
        };
    }

    [TestMethod]
    [Ignore("Local testing only, requires shared drive setup.")]
    public async Task DownloadFromSharedDriveTest()
    {
        var input = new Input
        {
            ServiceAccountKeyJSON = string.Empty,
            FileQuery = string.Empty,
            DestinationDirectory = destinationFolder,
            IncludeSharedDrives = true,
        };

        var options = new Options
        {
            CreateDestinationFolder = true,
            ThrowErrorOnFailure = true,
        };

        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("my-file.pdf")), "Testitiedosto.txt not found");
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (Directory.Exists(destinationFolder))
            Directory.Delete(destinationFolder, true);
    }

    [TestMethod]
    public async Task DownloadTest_ALL_Success()
    {
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Count > 0);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [Ignore("Can't test this in github")]
    [TestMethod]
    public async Task DownloadTest_JSONasFile()
    {
        input.ServiceAccountKeyJSON = "filepath";
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_JPG_Success()
    {
        input.FileQuery = $"name = 'test-photograph.jpg'";
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Count > 0);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_FileDoesntExists_Success()
    {
        input.FileQuery = $"name = 'no file'";
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
    }

    [TestMethod]
    public async Task DownloadTest_InvalidDestination_Throw()
    {
        input.DestinationDirectory = "no folder";
        options.CreateDestinationFolder = false;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Download(input, options, default));
        Assert.AreEqual("Destination directory doesn't exist. (Parameter 'DestinationDirectory Value:no folder')", result.Message);
    }

    [TestMethod]
    public async Task DownloadTest_MissingJSON_Throw()
    {
        input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Download(input, options, default));
        Assert.AreEqual("Service account key missing. (Parameter 'serviceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task DownloadTest_InvalidJSON_Throw()
    {
        input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.Download(input, options, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }
}