using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace Doppler.HtmlEditorApi.Infrastructure
{
    public class DummyRepository : IRepository
    {
        private static readonly object _demoMeta = new
        {
            counters = new
            {
                u_row = 1,
                u_column = 2,
                u_content_text = 1,
                u_content_heading = 1,
                u_content_menu = 1
            },
            body = new
            {
                rows = new object[0],
                values = new
                {
                    textColor = "#000000",
                    backgroundColor = "#e7e7e7",
                    backgroundImage = new
                    {
                        url = "",
                        fullWidth = true,
                        repeat = false,
                        center = true,
                        cover = false
                    },
                    contentWidth = "500px",
                    contentAlign = "center",
                    fontFamily = new
                    {
                        label = "Arial",
                        value = "arial,helvetica,sans-serif"
                    },
                    preheaderText = "",
                    linkStyle = new
                    {
                        body = true,
                        linkColor = "#0000ee",
                        linkHoverColor = "#0000ee",
                        linkUnderline = true,
                        linkHoverUnderline = true
                    },
                    _meta = new
                    {
                        htmlID = "u_body",
                        htmlClassNames = "u_body"
                    }
                }
            },
            schemaVersion = 6
        };

        public Task<ContentRow> GetCampaignModel(string accountName, int campaignId)
        {
            var contentRow = ContentRow.CreateUnlayerContentRow(
                meta: JsonSerializer.Serialize(_demoMeta),
                content: "<html></html>",
                idCampaign: campaignId);

            return Task.FromResult(contentRow);
        }

        public Task SaveCampaignContent(string accountName, ContentRow campaignModel)
        {
            return Task.CompletedTask;
        }
    }
}
