using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doppler.HtmlEditorApi.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContentType
{
    unlayer = 5,
}
