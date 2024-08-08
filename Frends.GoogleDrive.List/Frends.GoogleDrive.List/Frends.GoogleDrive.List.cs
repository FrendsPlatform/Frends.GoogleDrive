using Frends.GoogleDrive.List.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.GoogleDrive.List;

/// <summary>
/// Google Drive List Task.
/// </summary>
public class GoogleDrive
{
    /// <summary>
    /// List files in Google Drive.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleDrive.List)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="cancellationToken">Token received from Frends to cancel this Task.</param>
    /// <returns>Object { bool Success, string Data }</returns>
    public static async Task<Result> List([PropertyTab] Input input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.ServiceAccountKeyJSON))
            throw new ArgumentException(@"Service account key missing.", nameof(input.ServiceAccountKeyJSON));

        DriveService service = null;
        var json = string.Empty;
        try
        {
            service = await CreateDriveService(input, cancellationToken);
            var files = await GetFileList(service, input);
            var fileDict = new Dictionary<string, object>();

            foreach (var file in files) 
            {
                // Skip over folders
                if (file.MimeType == "application/vnd.google-apps.folder")
                    continue;

                fileDict[file.Id] = new { file.Id, file.Name, file.Size, file.Version, file.CreatedTime };
            }

            json = JsonConvert.SerializeObject(fileDict);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            service?.Dispose();
        }

        return new Result(true, json);
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

    private static async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFileList(DriveService service, Input input)
    {
        FilesResource.ListRequest fileListRequest = service.Files.List();
        fileListRequest.Fields = "nextPageToken, files(id, kind, name, size, version, createdTime, mimeType)";
        fileListRequest.Q = input.FileQuery;
        fileListRequest.PageSize = 1000;

        // shared drives support
        fileListRequest.SupportsAllDrives = input.IncludeSharedDrives;
        fileListRequest.IncludeItemsFromAllDrives = input.IncludeSharedDrives;

        var ret = await fileListRequest.ExecuteAsync();
        return ret.Files;
    }
}