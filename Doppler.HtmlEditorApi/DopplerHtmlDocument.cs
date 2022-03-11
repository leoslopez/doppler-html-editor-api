using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Doppler.HtmlEditorApi;

/// <summary>
/// This class deals with the conversion between a standard HTML content
/// and the set of strings that represents the content in Doppler DB.
/// It is named Doppler because it should replicate current Doppler logic.
/// </summary>
public class DopplerHtmlDocument
{
    // Old Doppler code:
    // https://github.com/MakingSense/Doppler/blob/ed24e901c990b7fb2eaeaed557c62c1adfa80215/Doppler.HypermediaAPI/ApiMappers/ToDoppler/CampaignContent_To_DtoContent.cs#L27-L29

    // TODO: consider remove the linebreaks in hrefs:
    // * https://github.com/MakingSense/Doppler/blob/ed24e901c990b7fb2eaeaed557c62c1adfa80215/Doppler.Domain.Core/Classes/Utils.cs#L147-L151
    // TODO: consider sanitize field names
    // Searching the canonical name in FieldsRes.resx (replacing " " and "%20" by "_" in the key)
    // * https://github.com/MakingSense/Doppler/blob/e0bf2aa982ac8b9902430395fd293fdcd3c231a8/Doppler.Application.CampaignsModule/Services/Classes/CampaignContentService.cs#L3428
    // * https://github.com/MakingSense/Doppler/blob/e0bf2aa982ac8b9902430395fd293fdcd3c231a8/Doppler.Application.ListsModule/Utils/FieldHelper.cs#L65-L81
    // * https://github.com/MakingSense/Doppler/blob/e0bf2aa982ac8b9902430395fd293fdcd3c231a8/Doppler.Application.ListsModule/Utils/FieldHelper.cs#L113-L134
    // * https://github.com/MakingSense/Doppler/blob/develop/Doppler.Recursos/FieldsRes.resx

    private const string FIELD_NAME_TAG_START_DELIMITER = "[[[";
    private const string FIELD_NAME_TAG_END_DELIMITER = "]]]";
    private const string FIELD_ID_TAG_START_DELIMITER = "|*|";
    private const string FIELD_ID_TAG_END_DELIMITER = "*|*";
    // &, # and ; are here to accept HTML Entities
    private static readonly Regex FIELD_NAME_TAG_REGEX = new Regex($@"{Regex.Escape(FIELD_NAME_TAG_START_DELIMITER)}([a-zA-Z0-9 \-_ñÑáéíóúÁÉÍÓÚ%&;#]+){Regex.Escape(FIELD_NAME_TAG_END_DELIMITER)}");
    private static readonly Regex FIELD_ID_TAG_REGEX = new Regex($@"{Regex.Escape(FIELD_ID_TAG_START_DELIMITER)}(\d+){Regex.Escape(FIELD_ID_TAG_END_DELIMITER)}");

    private readonly HtmlNode _headNode;
    private readonly HtmlNode _contentNode;

    public DopplerHtmlDocument(string inputHtml)
    {
        var htmlDocument = HtmlAgilityPackUtils.LoadHtml(inputHtml);

        _headNode = htmlDocument.DocumentNode.SelectSingleNode("//head");

        _contentNode = _headNode == null ? htmlDocument.DocumentNode
            : htmlDocument.DocumentNode.SelectSingleNode("//body")
            ?? HtmlAgilityPackUtils.LoadHtml(inputHtml.Replace(_headNode.OuterHtml, string.Empty)).DocumentNode;
    }

    public string GetDopplerContent()
        => EnsureContent(_contentNode.InnerHtml);

    public string GetHeadContent()
        => _headNode?.InnerHtml;

    public IEnumerable<int> GetFieldsId()
        => FIELD_ID_TAG_REGEX.Matches(_contentNode.InnerHtml)
            .Select(x => int.Parse(x.Groups[1].ValueSpan))
            .Distinct();

    public void ReplaceFieldNameTagsByFieldIdTags(Func<string, int?> getFieldIdOrNullFunc)
    {
        _contentNode.TraverseAndReplaceTextsAndAttributeValues(text => FIELD_NAME_TAG_REGEX.Replace(
            text,
            match =>
            {
                var fieldName = HtmlEntity.DeEntitize(match.Groups[1].Value.Replace("%20", " "));
                var fieldId = getFieldIdOrNullFunc(fieldName);
                return fieldId.HasValue
                    ? CreateFieldIdTag(fieldId.GetValueOrDefault())
                    // keep the name when field doesn't exist
                    : match.Value;
            }));
    }

    public void RemoveUnknownFieldIdTags(Func<int, bool> fieldIdExistFunc)
    {
        _contentNode.TraverseAndReplaceTextsAndAttributeValues(text => FIELD_ID_TAG_REGEX.Replace(
            text,
            match => fieldIdExistFunc(int.Parse(match.Groups[1].ValueSpan))
                ? match.Value
                : string.Empty));
    }

    private static string CreateFieldIdTag(int? fieldId)
        => $"{FIELD_ID_TAG_START_DELIMITER}{fieldId}{FIELD_ID_TAG_END_DELIMITER}";

    private static string EnsureContent(string htmlContent)
        => string.IsNullOrWhiteSpace(htmlContent) ? "<BR>" : htmlContent;

}
