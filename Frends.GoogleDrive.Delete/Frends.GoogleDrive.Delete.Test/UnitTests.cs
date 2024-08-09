using Frends.GoogleDrive.Delete.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Frends.GoogleDrive.Delete.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _googleDrive_CredBase64_Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? _googleDrive_CredBase64_Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly string _testFileDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles");
    private readonly string? _driveFolder = Environment.GetEnvironmentVariable("GoogleDrive_FolderId");
    private Input _input = new();
    private List<string> _uploadedIDs = new();

    [TestInitialize]
    public async Task Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));
        await UploadTestFiles();
        _input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileQuery = null,
            IncludeSharedDrives = true
        };
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        _input.ServiceAccountKeyJSON = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));
        _input.FileQuery = null;
        await GoogleDrive.Delete(_input, default);
    }

    [TestMethod]
    public async Task DeleteTest_ALL_Success()
    {
        var result = await GoogleDrive.Delete(_input, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-photograph.jpg")));
        Assert.AreEqual(0, GetFileList().Count);
    }

    [Ignore("Can't test this in github")]
    [TestMethod]
    public async Task DeleteTest_JSONasFile()
    {
        _input.ServiceAccountKeyJSON = "filepath";
        var result = await GoogleDrive.Delete(_input, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-photograph.jpg")));
        Assert.AreEqual(0, GetFileList().Count);
    }

    [TestMethod]
    public async Task DeleteTest_JPG_Success()
    {
        _input.FileQuery = $"name = 'test-photograph.jpg'";
        var result = await GoogleDrive.Delete(_input, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.IsFalse(result.Data.Any(x => x.Filename.Contains("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-photograph.jpg")));
        Assert.AreEqual(1, GetFileList().Count);
    }

    [TestMethod]
    public async Task DeleteTest_FileDoesntExists_Success()
    {
        _input.FileQuery = $"name = 'no file'";
        var result = await GoogleDrive.Delete(_input, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
        Assert.IsFalse(result.Data.Any(x => x.Filename.Contains("test-pdf-document.pdf")));
        Assert.IsFalse(result.Data.Any(x => x.Filename.Contains("test-photograph.jpg")));
        Assert.AreEqual(2, GetFileList().Count);
    }

    [TestMethod]
    public async Task DeleteTest_MissingJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Delete(_input, default));
        Assert.AreEqual("Service account key missing. (Parameter 'ServiceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task DeleteTest_InvalidJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.Delete(_input, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }

    private async Task UploadTestFiles()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(credentialsJson));
        var credential = GoogleCredential.FromStreamAsync(memoryStream, default).Result.CreateScoped(DriveService.ScopeConstants.Drive);
        var service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Frends iPaaS"
        });

        var filesToCopy = Directory.GetFiles(_testFileDir);

        foreach (var file in filesToCopy)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = new FileInfo(file).Name,
                Parents = string.IsNullOrWhiteSpace(_driveFolder) ? null : new string[] { _driveFolder }
            };

            FilesResource.CreateMediaUpload request;
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            request = service.Files.Create(fileMetadata, stream, null);
            request.Fields = "id";
            request.SupportsAllDrives = true;
            var result = await request.UploadAsync();
            _uploadedIDs.Add(request.ResponseBody.Id);
        }
    }

    private IList<Google.Apis.Drive.v3.Data.File> GetFileList()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(credentialsJson));
        var credential = GoogleCredential.FromStreamAsync(memoryStream, default).Result.CreateScoped(DriveService.ScopeConstants.Drive);
        var service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Frends iPaaS"
        });

        FilesResource.ListRequest fileListRequest = service.Files.List();
        return fileListRequest.Execute().Files;
    }
}