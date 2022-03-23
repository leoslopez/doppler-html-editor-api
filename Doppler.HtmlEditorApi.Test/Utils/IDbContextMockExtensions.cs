using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Doppler.HtmlEditorApi.Storage.DapperProvider;
using Doppler.HtmlEditorApi.Storage.DapperProvider.Queries;
using Moq;

namespace Doppler.HtmlEditorApi.Test.Utils;

public static class IDbContextMockExtensions
{
    public static void SetupContentWithCampaignStatus(
        this Mock<IDbContext> dbContextMock,
        string expectedAccountName,
        int expectedIdCampaign,
        FirstOrDefaultContentWithCampaignStatusDbQuery.Result result)
    {
        var setup = dbContextMock.Setup(x => x.QueryFirstOrDefaultAsync<FirstOrDefaultContentWithCampaignStatusDbQuery.Result>(
            It.IsAny<string>(),
            It.Is<ByCampaignIdAndAccountNameParameters>(x =>
                x.AccountName == expectedAccountName
                && x.IdCampaign == expectedIdCampaign)));

        setup.ReturnsAsync(result);
    }

    public static void SetupBasicFields(
        this Mock<IDbContext> dbContextMock)
    {
        dbContextMock.Setup(x => x.QueryAsync<DbField>(
            It.Is<string>(y => y.Contains("IsBasicField = 1")),
            null))
        .ReturnsAsync(new DbField[]
        {
            new() { IdField = 319, Name = "FIRST_NAME" },
            new() { IdField = 320, Name = "LAST_NAME" },
            new() { IdField = 321, Name = "EMAIL" },
            new() { IdField = 322, Name = "GENDER" },
            new() { IdField = 323, Name = "BIRTHDAY" },
            new() { IdField = 324, Name = "COUNTRY" },
            new() { IdField = 325, Name = "CONSENT" },
            new() { IdField = 326, Name = "ORIGIN" },
            new() { IdField = 327, Name = "SCORE" },
            new() { IdField = 106667, Name = "GDPR" }
        });
    }

    public static void SetupCustomFields(
        this Mock<IDbContext> dbContextMock,
        string expectedAccountName,
        IEnumerable<DbField> result)
    {
        dbContextMock.Setup(x => x.QueryAsync<DbField>(
            It.Is<string>(y => y.Contains("IsBasicField = 0") && y.Contains("Email = @accountName")),
            It.Is<ByAccountNameParameters>(y => y.AccountName == expectedAccountName)))
        .ReturnsAsync(result);
    }
}