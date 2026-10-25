using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Frends.GoogleDrive.List.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleDrive.List.Tests;

[TestClass]
public class UnitTests
{
    private readonly string googleDriveCredBase64Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string googleDriveCredBase64Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private readonly Options options = new();
    private Input input = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(googleDriveCredBase64Part1 + googleDriveCredBase64Part2));

        input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileQuery = null,
        };
    }

    [TestMethod]
    public async Task ListTest_ALL_Success()
    {
        var result = await GoogleDrive.List(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [Ignore("Can't test this in github")]
    [TestMethod]
    public async Task ListTest_JSONasFile()
    {
        input.ServiceAccountKeyJSON = "filepath";
        var result = await GoogleDrive.List(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_JPG_Success()
    {
        input.FileQuery = $"name = 'test-photograph.jpg'";
        var result = await GoogleDrive.List(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_FileDoesntExists_Success()
    {
        input.FileQuery = $"name = 'no file'";
        var result = await GoogleDrive.List(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_MissingJSON_Throw()
    {
        input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ValidationException>(async () => await GoogleDrive.List(input, options, default));
        StringAssert.Contains(result.Message, "ServiceAccountKeyJSON");
    }

    [TestMethod]
    public async Task ListTest_InvalidJSON_Throw()
    {
        input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.List(input, options, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }
}