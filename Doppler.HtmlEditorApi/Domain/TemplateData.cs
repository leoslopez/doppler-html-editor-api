namespace Doppler.HtmlEditorApi.Domain;

public abstract record TemplateData(
    string HtmlCode,
    string Meta,
    string PreviewImage,
    int EditorType,
    bool IsPublic)
{ }

public sealed record UnlayerTemplateData(
    string HtmlCode,
    string Meta,
    string PreviewImage,
    int EditorType,
    bool IsPublic)
    : TemplateData(HtmlCode, Meta, PreviewImage, EditorType, IsPublic);

public sealed record UnknownTemplateData(
    string HtmlCode,
    string PreviewImage,
    int EditorType,
    bool IsPublic)
    : TemplateData(HtmlCode, null, PreviewImage, EditorType, IsPublic);
