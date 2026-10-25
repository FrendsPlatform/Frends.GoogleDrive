using dotenv.net;
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
    private readonly string _testFileDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestFiles");
    private Input _input = new();
    private List<string> _uploadedIDs = new();

    static UnitTests()
    {
        // Existing environment variables (e.g. from CI) take precedence over values in .env.
        DotEnv.Fluent()
            .WithProbeForEnv(probeLevelsToSearch: 6)
            .WithTrimValues()
            .WithoutOverwriteExistingVars()
            .Load();
    }

    private static string CredentialsJson => Encoding.ASCII.GetString(Convert.FromBase64String(
        Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1") +
        Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2")));

    private static string? DriveFolder => Environment.GetEnvironmentVariable("GoogleDrive_FolderId");

    [TestInitialize]
    public async Task Setup()
    {
        await UploadTestFiles();
        _input = new()
        {
            ServiceAccountKeyJSON = CredentialsJson,
            FileQuery = null
        };
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        _input.ServiceAccountKeyJSON = CredentialsJson;
        _input.FileQuery = null;
        await GoogleDrive.Delete(_input, new Options(), default);
    }

    [TestMethod]
    public async Task DeleteTest_ALL_Success()
    {
        var result = await GoogleDrive.Delete(_input, new Options(), default);
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
        var result = await GoogleDrive.Delete(_input, new Options(), default);
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
        var result = await GoogleDrive.Delete(_input, new Options(), default);
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
        var result = await GoogleDrive.Delete(_input, new Options(), default);
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
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.Delete(_input, new Options(), default));
        Assert.AreEqual("Service account key missing. (Parameter 'ServiceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task DeleteTest_InvalidJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.Delete(_input, new Options(), default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }

    private async Task UploadTestFiles()
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(CredentialsJson));
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
                Parents = string.IsNullOrWhiteSpace(DriveFolder) ? null : new string[] { DriveFolder }
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
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(CredentialsJson));
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