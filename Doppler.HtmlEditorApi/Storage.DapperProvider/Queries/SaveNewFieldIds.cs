
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppler.HtmlEditorApi.Storage.DapperProvider.Queries;

/// <summary>
/// It keeps existing DB entries and only adds new ones without deleting anything.
/// </summary>
public class SaveNewFieldIds : DbQuery<SaveNewFieldIds.Parameters, int>
{
    public SaveNewFieldIds(IDbContext dbContext) : base(dbContext) { }

    protected override string SqlQuery => @"
    DECLARE @T TABLE (IdField INT)
    INSERT INTO @T (IdField) VALUES {{FieldIds}}

    INSERT INTO ContentXField (IdContent, IdField)
    SELECT @IdContent, t.IdField
    From @T t
    LEFT JOIN dbo.ContentXFIeld CxF ON CxF.IdField = t.IdField AND CxF.IdContent = @IdContent
    WHERE CxF.IdContent IS NULL";

    public override Task<int> ExecuteAsync(Parameters parameters)
    {
        var serializedFieldsId = string.Join(",", parameters.FieldIds.Select(x => $"({x})"));
        var query = SqlQuery.Replace("{{FieldIds}}", serializedFieldsId);

        return DbContext.ExecuteAsync(query, parameters);
    }

    public record Parameters(int IdContent, IEnumerable<int> FieldIds);

}