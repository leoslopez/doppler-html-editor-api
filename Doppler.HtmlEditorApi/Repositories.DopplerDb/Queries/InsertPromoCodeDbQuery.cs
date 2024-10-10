using Doppler.HtmlEditorApi.DataAccess;

namespace Doppler.HtmlEditorApi.Repositories.DopplerDb.Queries;

public record InsertPromoCodeDbQuery(
    string Type,
    decimal Value,
    bool IncludeShipping,
    bool FirstPurchase,
    bool AllowCombines,
    decimal MinPrice,
    int ExpireDays,
    int MaxUses,
    string? Categories,
    int IdCampaign
) : ISingleItemDbQuery<InsertPromoCodeDbQuery.Result>
{
    public string GenerateSqlQuery() => @"
INSERT INTO DynamicContentPromoCode (
    Type,
    Value,
    IncludeShipping,
    FirstPurchase,
    AllowCombines,
    MinPrice,
    ExpireDays,
    MaxUses,
    Categories,
    IdCampaign
) VALUES (
    @Type,
    @Value,
    @IncludeShipping,
    @FirstPurchase,
    @AllowCombines,
    @MinPrice,
    @ExpireDays,
    @MaxUses,
    @Categories,
    @IdCampaign
)

SELECT @@Identity AS IdDynamicContentPromoCode";

    public class Result
    {
        public int IdDynamicContentPromoCode { get; init; }
    }
}
