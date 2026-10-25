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
    private Input _input = new();
    private string _folderId = string.Empty;

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

    // Restricts every delete/list to direct children of the per-test folder.
    private string FolderQuery => $"'{_folderId}' in parents";

    [TestInitialize]
    public async Task Setup()
    {
        await CreateTestFolderWithFiles();
        _input = new()
        {
            ServiceAccountKeyJSON = CredentialsJson,
            FileQuery = FolderQuery,
        };
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        if (string.IsNullOrEmpty(_folderId))
            return;

        // Deleting the folder also removes anything left inside it.
        using var service = CreateService();
        try
        {
            await service.Files.Delete(_folderId).ExecuteAsync();
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
        }
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

    [TestMethod]
    public async Task DeleteTest_JSONasFile()
    {
        var keyFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(keyFile, CredentialsJson);
            _input.ServiceAccountKeyJSON = keyFile;
            var result = await GoogleDrive.Delete(_input, new Options(), default);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Count);
            Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-pdf-document.pdf")));
            Assert.IsTrue(result.Data.Any(x => x.Filename.Contains("test-photograph.jpg")));
            Assert.AreEqual(0, GetFileList().Count);
        }
        finally
        {
            File.Delete(keyFile);
        }
    }

    [TestMethod]
    public async Task DeleteTest_JPG_Success()
    {
        _input.FileQuery = $"{FolderQuery} and name = 'test-photograph.jpg'";
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
        _input.FileQuery = $"{FolderQuery} and name = 'no file'";
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

    private static DriveService CreateService()
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(CredentialsJson));
        var credential = GoogleCredential.FromStreamAsync(memoryStream, default).Result.CreateScoped(DriveService.ScopeConstants.Drive);
        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Frends iPaaS",
        });
    }

    // Folders and zero-byte files use no Drive storage quota, so a service account can own them.
    private async Task CreateTestFolderWithFiles()
    {
        using var service = CreateService();

        var folder = await service.Files.Create(new Google.Apis.Drive.v3.Data.File
        {
            Name = $"frends-delete-test-{Guid.NewGuid()}",
            MimeType = "application/vnd.google-apps.folder",
        }).ExecuteAsync();
        _folderId = folder.Id;

        foreach (var name in new[] { "test-pdf-document.pdf", "test-photograph.jpg" })
        {
            await service.Files.Create(new Google.Apis.Drive.v3.Data.File
            {
                Name = name,
                Parents = new[] { _folderId },
            }).ExecuteAsync();
        }
    }

    private IList<Google.Apis.Drive.v3.Data.File> GetFileList()
    {
        using var service = CreateService();
        FilesResource.ListRequest fileListRequest = service.Files.List();
        fileListRequest.Q = FolderQuery;
        return fileListRequest.Execute().Files;
    }
}