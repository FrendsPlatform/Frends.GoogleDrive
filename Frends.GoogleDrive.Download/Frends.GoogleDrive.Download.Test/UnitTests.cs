using Frends.GoogleDrive.Download.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Frends.GoogleDrive.Download.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _googleDrive_CredBase64_Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? _googleDrive_CredBase64_Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly string _destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles");
    private Input _input = new();
    private Options _options = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));

        _input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            DestinationDirectory = _destinationFolder,
            FileQuery = null
        };

        _options = new()
        {
            ThrowErrorOnFailure = true,
            CreateDestinationFolder = true,
        };
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (Directory.Exists(_destinationFolder))
            Directory.Delete(_destinationFolder, true);
    }

    [TestMethod]
    public async Task DownloadTest_ALL_Success()
    {
        var result = await GoogleDrive.Download(_input, _options, default);
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
        _input.ServiceAccountKeyJSON = "filepath";
        var result = await GoogleDrive.Download(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_JPG_Success()
    {
        _input.FileQuery = $"name = 'test-photograph.jpg'";
        var result = await GoogleDrive.Download(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Count > 0);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_FileDoesntExists_Success()
    {
        _input.FileQuery = $"name = 'no file'";
        var result = await GoogleDrive.Download(_input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
    }

    [TestMethod]
    public async Task DownloadTest_InvalidDestination_Throw()
    {
        _input.DestinationDirectory = "no folder";
        _options.CreateDestinationFolder = false;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Download(_input, _options, default));
        Assert.AreEqual("Destination directory doesn't exist. (Parameter 'DestinationDirectory Value:no folder')", result.Message);
    }

    [TestMethod]
    public async Task DownloadTest_MissingJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Download(_input, _options, default));
        Assert.AreEqual("Service account key missing. (Parameter 'serviceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task DownloadTest_InvalidJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.Download(_input, _options, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }
}