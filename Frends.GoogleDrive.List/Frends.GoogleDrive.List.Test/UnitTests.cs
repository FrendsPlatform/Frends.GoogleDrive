using Frends.GoogleDrive.List.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
namespace Frends.GoogleDrive.List.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _googleDrive_CredBase64_Part1 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part1");
    private readonly string? _googleDrive_CredBase64_Part2 = Environment.GetEnvironmentVariable("GoogleDrive_CredBase64_Part2");
    private Input _input = new();

    [TestInitialize]
    public void Setup()
    {
        var credentialsJson = Encoding.ASCII.GetString(Convert.FromBase64String(_googleDrive_CredBase64_Part1 + _googleDrive_CredBase64_Part2));

        _input = new()
        {
            ServiceAccountKeyJSON = credentialsJson,
            FileQuery = null,
        };
    }

    [TestMethod]
    public async Task ListTest_ALL_Success()
    {
        var result = await GoogleDrive.List(_input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [Ignore("Can't test this in github")]
    [TestMethod]
    public async Task ListTest_JSONasFile()
    {
        _input.ServiceAccountKeyJSON = "filepath";
        var result = await GoogleDrive.List(_input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_JPG_Success()
    {
        _input.FileQuery = $"name = 'test-photograph.jpg'";
        var result = await GoogleDrive.List(_input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_FileDoesntExists_Success()
    {
        _input.FileQuery = $"name = 'no file'";
        var result = await GoogleDrive.List(_input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task ListTest_MissingJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = null;
        var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await GoogleDrive.List(_input, default));
        Assert.AreEqual("Service account key missing. (Parameter 'ServiceAccountKeyJSON')", result.Message);
    }

    [TestMethod]
    public async Task ListTest_InvalidJSON_Throw()
    {
        _input.ServiceAccountKeyJSON = "NoJsonHere";
        var result = await Assert.ThrowsExceptionAsync<AggregateException>(async () => await GoogleDrive.List(_input, default));
        Assert.AreEqual("One or more errors occurred. (Error deserializing JSON credential data.)", result.Message);
    }
}