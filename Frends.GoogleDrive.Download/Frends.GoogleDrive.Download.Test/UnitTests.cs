using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotenv.net;
using Frends.GoogleDrive.Download.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.Download.Tests;

[TestClass]
public class UnitTests
{
    // Service accounts have no storage quota, but empty folders and 0-byte files don't use any.
    private static readonly (string Name, string MimeType)[] Fixtures =
    {
        ("test-pdf-document.pdf", "application/pdf"),
        ("test-photograph.jpg", "image/jpeg"),
    };

    private readonly string temporaryDirectory = Path.Combine(Path.GetTempPath(), "Frends.GoogleDrive.Download.Tests", Guid.NewGuid().ToString("N"));
    private DriveService? driveService;
    private string testDriveFolderId = string.Empty;
    private string credentialsFilePath = string.Empty;
    private string destinationFolder = string.Empty;
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public async Task Setup()
    {
        try
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6, overwriteExistingVars: false));

            var credentialsBase64Part1 = GetRequiredEnvironmentVariable("GoogleDrive_CredBase64_Part1");
            var credentialsBase64Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2") ?? string.Empty;
            var parentFolderId = GetRequiredEnvironmentVariable("GoogleDrive_FolderId");
            var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(credentialsBase64Part1 + credentialsBase64Part2));

            destinationFolder = Path.Combine(temporaryDirectory, "Downloads");
            credentialsFilePath = Path.Combine(temporaryDirectory, "service-account.json");
            Directory.CreateDirectory(temporaryDirectory);
            File.WriteAllText(credentialsFilePath, credentialsJson);

            driveService = await CreateDriveService(credentialsJson);
            await CreateTestDriveFixtures(parentFolderId);

            input = new()
            {
                ServiceAccountKeyJSON = credentialsJson,
                DestinationDirectory = destinationFolder,
                FileQuery = $"'{testDriveFolderId}' in parents",
            };

            options = new()
            {
                ThrowErrorOnFailure = true,
                CreateDestinationFolder = true,
            };
        }
        catch (Exception setupException)
        {
            try
            {
                await CleanUpResources();
            }
            catch (Exception cleanupException)
            {
                throw new AggregateException("Test setup failed and cleanup also failed.", setupException, cleanupException);
            }

            throw;
        }
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        await CleanUpResources();
    }

    [TestMethod]
    public async Task DownloadFromSharedDriveTest()
    {
        input.IncludeSharedDrives = true;
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-pdf-document.pdf")));
    }

    [TestMethod]
    public async Task DownloadTest_ALL_Success()
    {
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-pdf-document.pdf")));
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
        Assert.IsTrue(File.Exists(Path.Combine(destinationFolder, "test-pdf-document.pdf")));
        Assert.IsTrue(File.Exists(Path.Combine(destinationFolder, "test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_JSONasFile()
    {
        input.ServiceAccountKeyJSON = credentialsFilePath;
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
    }

    [TestMethod]
    public async Task DownloadTest_JPG_Success()
    {
        input.FileQuery = $"'{testDriveFolderId}' in parents and name = 'test-photograph.jpg'";
        var result = await GoogleDrive.Download(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(0, result.ErrorMessage.Count);
        Assert.IsTrue(result.Data.Any(x => x.DownloadedFilename.Equals("test-photograph.jpg")));
    }

    [TestMethod]
    public async Task DownloadTest_FileDoesntExists_Success()
    {
        input.FileQuery = $"'{testDriveFolderId}' in parents and name = 'no file'";
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
        Assert.AreEqual("Destination directory doesn't exist. (Parameter 'destinationDirectory Value:no folder')", result.Message);
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

    private static string GetRequiredEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Required test environment variable '{name}' is not set.");

        return value;
    }

    private static async Task<DriveService> CreateDriveService(string credentialsJson)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(credentialsJson));
        var credential = await GoogleCredential.FromStreamAsync(stream, default);

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential.CreateScoped(DriveService.ScopeConstants.Drive),
            ApplicationName = "Frends iPaaS",
        });
    }

    private async Task CreateTestDriveFixtures(string parentFolderId)
    {
        testDriveFolderId = await CreateDriveItem($"Frends Download Tests {Guid.NewGuid():N}", "application/vnd.google-apps.folder", parentFolderId);

        foreach (var (name, mimeType) in Fixtures)
            await CreateDriveItem(name, mimeType, testDriveFolderId);
    }

    // Metadata-only create: no content is uploaded, so the file is 0 bytes.
    private async Task<string> CreateDriveItem(string name, string mimeType, string parentId)
    {
        var request = driveService!.Files.Create(new Google.Apis.Drive.v3.Data.File
        {
            Name = name,
            MimeType = mimeType,
            Parents = new List<string> { parentId },
        });
        request.Fields = "id";
        request.SupportsAllDrives = true;
        var item = await request.ExecuteAsync();
        return item.Id;
    }

    private async Task CleanUpResources()
    {
        var cleanupErrors = new List<Exception>();

        if (driveService != null)
        {
            // Deleting the folder also deletes the files inside it.
            if (!string.IsNullOrWhiteSpace(testDriveFolderId))
            {
                try
                {
                    var deleteRequest = driveService.Files.Delete(testDriveFolderId);
                    deleteRequest.SupportsAllDrives = true;
                    await deleteRequest.ExecuteAsync();
                }
                catch (Exception exception)
                {
                    cleanupErrors.Add(exception);
                }
            }

            driveService.Dispose();
            driveService = null;
        }

        if (Directory.Exists(temporaryDirectory))
        {
            try
            {
                Directory.Delete(temporaryDirectory, true);
            }
            catch (Exception exception)
            {
                cleanupErrors.Add(exception);
            }
        }

        if (cleanupErrors.Count > 0)
            throw new AggregateException("One or more test fixtures could not be cleaned up.", cleanupErrors);
    }
}
