using Frends.GoogleDrive.Delete.Definitions;
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

namespace Frends.GoogleDrive.Delete;

/// <summary>
/// Google Drive Delete Task.
/// </summary>
public class GoogleDrive
{
    /// <summary>
    /// Permanently deletes a file owned by the user without moving it to the trash. 
    /// If the file belongs to a shared drive the user must be an organizer on the parent. If the target is a folder, all descendants owned by the user are also deleted.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleDrive.Delete)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="cancellationToken">Token received from Frends to cancel this Task.</param>
    /// <returns>Object { bool Success, List&lt;DeleteResult&gt; Data }</returns>
    public static async Task<Result> Delete([PropertyTab] Input input, CancellationToken cancellationToken)
    {
        var resultList = new List<DeleteResult>();
        DriveService service = null;

        try
        {
            InputCheck(input);
            service = await CreateDriveService(input, cancellationToken);
            var files = await GetFileList(service, input);

            if (files != null && files.Count > 0)
                foreach (var file in files)
                {
                    var deleteRequest = service.Files.Delete(file.Id);

                    // shared drives support
                    deleteRequest.SupportsAllDrives = input.IncludeSharedDrives;
                    
                    await deleteRequest.ExecuteAsync(cancellationToken);
                    resultList.Add(new DeleteResult(file.Id, file.Name));
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

        return new Result(true, resultList);
    }

    private static void InputCheck(Input input)
    {
        if (string.IsNullOrWhiteSpace(input.ServiceAccountKeyJSON))
            throw new ArgumentException(@"Service account key missing.", nameof(input.ServiceAccountKeyJSON));
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