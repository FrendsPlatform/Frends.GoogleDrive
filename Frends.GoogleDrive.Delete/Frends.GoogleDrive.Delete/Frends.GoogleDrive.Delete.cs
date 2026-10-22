using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Frends.GoogleDrive.Delete.Definitions;
using Frends.GoogleDrive.Delete.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Frends.GoogleDrive.Delete;

/// <summary>
/// Google Drive Delete Task.
/// </summary>
public static class GoogleDrive
{
    /// <summary>
    /// Permanently deletes a file owned by the user without moving it to the trash.
    /// If the file belongs to a shared drive the user must be an organizer on the parent. If the target is a folder, all descendants owned by the user are also deleted.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleDrive.Delete)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">Token received from Frends to cancel this Task.</param>
    /// <returns>object { bool Success, List&lt;DeleteResult&gt; Data, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> Delete([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var resultList = new List<DeleteResult>();
        DriveService service = null;

        try
        {
            ValidationHandler.Run(input, options);
            service = await CreateDriveService(input, cancellationToken);
            var files = GetFileList(service, input.FileQuery);

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    await service.Files.Delete(file.Id).ExecuteAsync(cancellationToken);
                    resultList.Add(new DeleteResult(file.Id, file.Name));
                }
            }
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
        finally
        {
            service?.Dispose();
        }

        return new Result { Success = true, Data = resultList, Error = null };
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
            ApplicationName = "Frends iPaaS",
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