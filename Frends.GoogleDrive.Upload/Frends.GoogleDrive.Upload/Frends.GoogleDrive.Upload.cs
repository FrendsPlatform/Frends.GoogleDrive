using Frends.GoogleDrive.Upload.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.GoogleDrive.Upload;

/// <summary>
/// Google Drive Upload Task.
/// </summary>
public class GoogleDrive
{
    /// <summary>
    /// Upload objects to Google Drive.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleDrive.Upload)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Token received from Frends to cancel this Task.</param>
    /// <returns>Object { bool success, List&lt;UploadResult&gt; data, List&lt;dynamic&gt; errorMessages }</returns>
    public static async Task<Result> Upload([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var resultList = new List<UploadResult>();
        var errorList = new List<dynamic>();
        DriveService service = null;

        try
        {
            InputCheck(input.ServiceAccountKeyJSON, input.SourceDirectory);
            var filesToCopy = GetFileInfo(input, options);
            service = await CreateDriveService(input, cancellationToken);

            foreach (var file in filesToCopy)
            {
                var success = true;
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = file.Name,
                    Parents = string.IsNullOrWhiteSpace(input.TargetFolderId) ? null : new string[] { input.TargetFolderId }
                };

                FilesResource.CreateMediaUpload request;
                await using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    request = service.Files.Create(fileMetadata, stream, null);
                    request.Fields = "id";
                    request.SupportsAllDrives = input.IncludeSharedDrives;

                    var results = await request.UploadAsync(cancellationToken);

                    if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
                    {
                        success = false;
                        if (options.ThrowErrorOnFailure)
                            throw new Exception($"Error uploading file: {file.Name}. Error: {results.Exception.Message}");
                        errorList.Add($"Error uploading file: {file.Name}. Error: {results.Exception.Message}");
                    }
                }

                if (success)
                    resultList.Add(new UploadResult(request.ResponseBody?.Id, file.Name));
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            service?.Dispose();
        }

        return new Result(errorList.Count == 0, resultList, errorList);
    }

    private static void InputCheck(string serviceAccountKeyJSON, string sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(serviceAccountKeyJSON))
            throw new ArgumentException(@"Service account key missing.", nameof(serviceAccountKeyJSON));
        if (!Directory.Exists(sourceDirectory))
            throw new ArgumentException(@"Source path not found.", $"{nameof(sourceDirectory)} Value:{sourceDirectory}");
    }

    private static async Task<DriveService> CreateDriveService(Input input, CancellationToken cancellationToken)
    {
        byte[] byteArray;
        if (File.Exists(input.ServiceAccountKeyJSON))
            byteArray = await File.ReadAllBytesAsync(input.ServiceAccountKeyJSON, cancellationToken: cancellationToken);
        else
            byteArray = Encoding.UTF8.GetBytes(input.ServiceAccountKeyJSON);

        using var stream = new MemoryStream(byteArray);
        GoogleCredential credential = GoogleCredential.FromStreamAsync(stream, cancellationToken: cancellationToken).Result.CreateScoped(DriveService.ScopeConstants.Drive);
        var service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Frends iPaaS"
        });

        return service;
    }

    private static FileInfo[] GetFileInfo(Input input, Options options)
    {
        var localRoot = new DirectoryInfo(input.SourceDirectory);
        // If filemask is not set, get all files.
        var filesToCopy = localRoot.GetFiles(input.FileMask ?? "*", SearchOption.TopDirectoryOnly);
        if (options.ThrowErrorIfNoMatch && filesToCopy.Length < 1)
            throw new Exception($"No files match the filemask '{input.FileMask}' within supplied path.");

        return filesToCopy;
    }
}