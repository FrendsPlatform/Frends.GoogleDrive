using Frends.GoogleDrive.Download.Definitions;
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

namespace Frends.GoogleDrive.Download;

/// <summary>
/// Google Drive Download Task.
/// </summary>
public class GoogleDrive
{
    /// <summary>
    /// Download objects from Google Drive.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleDrive.Download)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Token received from Frends to cancel this Task.</param>
    /// <returns>Object { bool success, List&lt;DownloadResult&gt; data, List&lt;dynamic&gt; errorMessages }</returns>
    public static async Task<Result> Download([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var resultList = new List<DownloadResult>();
        var errorList = new List<dynamic>();
        DriveService service = null;

        try
        {
            InputCheck(input.ServiceAccountKeyJSON, input.DestinationDirectory, options.CreateDestinationFolder);
            service = await CreateDriveService(input, cancellationToken);
            var files = GetFileList(service, input.FileQuery);
            var success = true;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    FilesResource.GetRequest fileRequest = service.Files.Get(file.Id);
                    FileStream fileStream = new(
                        Path.Combine(input.DestinationDirectory, file.Name),
                        FileMode.OpenOrCreate,
                        FileAccess.Write
                    );

                    // Add a handler which will be notified on progress changes.
                    // It will notify on each chunk download and when the download is completed or failed.
                    fileRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                    {
                        switch (progress.Status)
                        {
                            case Google.Apis.Download.DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case Google.Apis.Download.DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    fileStream.Flush();
                                    fileStream.Close();
                                    break;
                                }
                            case Google.Apis.Download.DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                    var result = await fileRequest.DownloadAsync(fileStream, cancellationToken: cancellationToken);

                    if (result.Status.Equals(Google.Apis.Download.DownloadStatus.Failed))
                    {
                        success = false;
                        if (options.ThrowErrorOnFailure)
                            throw new Exception($"Error downloading file: {file.Name}. Error: {result.Exception.Message}");
                        errorList.Add($"Error downloading file: {file.Name}. Error: {result.Exception.Message}");
                    }

                    if (success)
                        resultList.Add(new DownloadResult(file.Id, file.Name));
                }
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

    private static void InputCheck(string serviceAccountKeyJSON, string DestinationDirectory, bool createDir)
    {
        if (string.IsNullOrWhiteSpace(serviceAccountKeyJSON))
            throw new ArgumentException(@"Service account key missing.", nameof(serviceAccountKeyJSON));
        if (!Directory.Exists(DestinationDirectory))
        {
            if (createDir)
                Directory.CreateDirectory(DestinationDirectory);
            else
                throw new ArgumentException(@"Destination directory doesn't exist.", $"{nameof(DestinationDirectory)} Value:{DestinationDirectory}");
        }
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

    private static IList<Google.Apis.Drive.v3.Data.File> GetFileList(DriveService service, string fileQuery)
    {
        FilesResource.ListRequest fileListRequest = service.Files.List();
        fileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";
        fileListRequest.Q = fileQuery;
        return fileListRequest.Execute().Files;
    }
}