using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Doppler.HtmlEditorApi.Storage;
using Doppler.HtmlEditorApi.Storage.DapperProvider;
using Doppler.HtmlEditorApi.Storage.DapperProvider.Queries;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Doppler.HtmlEditorApi
{
    public class SaveCampaignTest
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.E3RHjKx9p0a-64RN2YPtlEMysGM45QBO9eATLBhtP4tUQNZnkraUr56hAWA-FuGmhiuMptnKNk_dU3VnbyL6SbHrMWUbquxWjyoqsd7stFs1K_nW6XIzsTjh8Bg6hB5hmsSV-M5_hPS24JwJaCdMQeWrh6cIEp2Sjft7I1V4HQrgzrkMh15sDFAw3i1_ZZasQsDYKyYbO9Jp7lx42ognPrz_KuvPzLjEXvBBNTFsVXUE-ur5adLNMvt-uXzcJ1rcwhjHWItUf5YvgRQbbBnd9f-LsJIhfkDgCJcvZmGDZrtlCKaU1UjHv5c3faZED-cjL59MbibofhPjv87MK8hhdg";
        const string TOKEN_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjIwMDAwMDAwMDB9.mll33c0kstVIN9Moo4HSw0CwRjn0IuDc2h1wkRrv2ahQtIG1KV5KIxYw-H3oRfd-PiCWHhIVIYDP3mWDZbsOHTlnpRGpHp4f26LAu1Xp1hDJfOfxKYEGEE62Xt_0qp7jSGQjrx-vQey4l2mNcWkOWiE0plOws7cX-wLUvA3NLPoOvEegjM0Wx6JFcvYLdMGcTGT5tPd8Pq8pe9VYstCbhOClzI0bp81iON3f7VQP5d0n64eb_lvEPFu5OfURD4yZK2htyQK7agcNNkP1c5mLEfUi39C7Qtx96aAhOjir6Wfhzv_UEs2GQKXGTHl6_-HH-ecgOdIvvbqXGLeDmTkXUQ";
        const string TOKEN_SUPERUSER_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjEwMDAwMDAwMDB9.FYOpOxrXSHDif3lbQLPEStMllzEktWPKQ2T4vKUq5qgVjiH_ki0W0Ansvt0PMlaLHqq7OOL9XGFebtgUcyU6aXPO9cZuq6Od196TWDLMdnxZ-Ct0NxWxulyMbjTglUiI3V6g3htcM5EaurGvfu66kbNDuHO-WIQRYFfJtbm7EuOP7vYBZ26hf5Vk5KvGtCWha4zRM55i1-CKMhXvhPN_lypn6JLENzJGYHkBC9Cx2DwzaT683NWtXiVzeMJq3ohC6jvRpkezv89QRes2xUW4fRgvgRGQvaeQ4huNW_TwQKTTikH2Jg7iHbuRqqwYuPZiWuRkjqfd8_80EdlSAnO94Q";
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoxMDAwMDAwMDAwfQ.JBmiZBgKVSUtB4_NhD1kiUhBTnH2ufGSzcoCwC3-Gtx0QDvkFjy2KbxIU9asscenSdzziTOZN6IfFx6KgZ3_a3YB7vdCgfSINQwrAK0_6Owa-BQuNAIsKk-pNoIhJ-OcckV-zrp5wWai3Ak5Qzg3aZ1NKZQKZt5ICZmsFZcWu_4pzS-xsGPcj5gSr3Iybt61iBnetrkrEbjtVZg-3xzKr0nmMMqe-qqeknozIFy2YWAObmTkrN4sZ3AB_jzqyFPXN-nMw3a0NxIdJyetbESAOcNnPLymBKZEZmX2psKuXwJxxekvgK9egkfv2EjKYF9atpH5XwC0Pd4EWvraLAL2eg";
        const string TOKEN_SUPERUSER_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjIwMDAwMDAwMDB9.rUtvRqMxrnQzVHDuAjgWa2GJAJwZ-wpaxqdjwP7gmVa7XJ1pEmvdTMBdirKL5BJIE7j2_hsMvEOKUKVjWUY-IE0e0u7c82TH0l_4zsIztRyHMKtt9QE9rBRQnJf8dcT5PnLiWkV_qEkpiIKQ-wcMZ1m7vQJ0auEPZyyFBKmU2caxkZZOZ8Kw_1dx-7lGUdOsUYad-1Rt-iuETGAFijQrWggcm3kV_KmVe8utznshv2bAdLJWydbsAUEfNof0kZK5Wu9A80DJd3CRiNk8mWjQxF_qPOrGCANOIYofhB13yuYi48_8zVPYku-llDQjF77BmQIIIMrCXs8IMT3Lksdxuw";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public SaveCampaignTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Theory]
        [InlineData("/accounts/x@x.com/campaigns/123/content", HttpStatusCode.Unauthorized)]
        public async Task PUT_campaign_should_require_token(string url, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new { htmlContent = "", meta = new { } }));
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/x@x.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/x@x.com/campaigns/123/content", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        public async Task PUT_campaign_should_not_accept_the_token_of_another_account(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new { htmlContent = "", meta = new { } }));
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized)]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized)]
        public async Task PUT_campaign_should_not_accept_a_expired_token(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new { htmlContent = "", meta = new { } }));
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Contains("Bearer", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains("invalid_token", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains("token expired", response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, "test1@test.com")]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_SUPERUSER_EXPIRE_20330518, "test1@test.com")]
        [InlineData("/accounts/otro@test.com/campaigns/123/content", TOKEN_SUPERUSER_EXPIRE_20330518, "otro@test.com")]
        public async Task PUT_campaign_should_accept_right_tokens_and_return_Ok(string url, string token, string expectedAccountName)
        {
            // Arrange
            var repositoryMock = new Mock<IRepository>();
            repositoryMock.Setup(x => x.SaveCampaignContent(expectedAccountName, It.IsAny<BaseHtmlContentData>()))
                .Returns(Task.CompletedTask);

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(repositoryMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = "unlayer",
                htmlContent = "<html></html>",
                meta = new { }
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518)]
        public async Task PUT_campaign_should_return_error_when_htmlContent_is_not_present(string url, string token)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new { meta = new { } }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"htmlContent\":\\[\"The htmlContent field is required.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518)]
        public async Task PUT_campaign_should_return_error_when_htmlContent_is_a_empty_string(string url, string token)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new { htmlContent = "", meta = new { } }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"htmlContent\":\\[\"The htmlContent field is required.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518)]
        public async Task PUT_campaign_should_return_error_when_type_is_unlayer_and_meta_is_not_present(string url, string token)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = "unlayer",
                htmlContent = "<html></html>"
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"meta\":\\[\"The meta field is required for unlayer content.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518)]
        public async Task PUT_campaign_should_return_error_when_type_is_unlayer_and_meta_is_empty_string(string url, string token)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = "unlayer",
                htmlContent = "<html></html>",
                meta = ""
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"meta\":\\[\"The meta field is required for unlayer content.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518)]
        public async Task PUT_campaign_should_return_error_when_type_is_not_defined(string url, string token)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                htmlContent = "<html></html>",
                meta = ""
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"type\":\\[\"The type field is required.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, "")]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, null)]
        [InlineData("/accounts/test1@test.com/campaigns/123/content", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, "noexisto")]
        public async Task PUT_campaign_should_return_error_when_type_is_invalid(string url, string token, string type)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type,
                htmlContent = "<html></html>",
                meta = ""
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Matches("\"$.type\":\\[\"The JSON value could not be converted to Doppler.HtmlEditorApi.Model.CampaignContent. Path: $.type | LineNumber: \\d+ | BytePositionInLine: \\d+.\"\\]", responseContent);
        }

        [Theory]
        [InlineData("html")]
        [InlineData("unlayer")]
        public async Task PUT_campaign_should_return_500_error_when_campaign_does_not_exist(string type)
        {
            // Arrange
            var url = "/accounts/test1@test.com/campaigns/123/content";
            var token = TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518;
            var expectedAccountName = "test1@test.com";
            var expectedIdCampaign = 123;
            var htmlContent = "My HTML Content";

            var dbContextMock = new Mock<IDbContext>();
            dbContextMock
                .Setup(x => x.QueryFirstOrDefaultAsync<FirstOrDefaultCampaignStatusDbQuery.Result>(
                    It.IsAny<string>(),
                    It.Is<ByCampaignIdAndAccountNameParameters>(x =>
                        x.AccountName == expectedAccountName
                        && x.IdCampaign == expectedIdCampaign)))
                .ReturnsAsync(new FirstOrDefaultCampaignStatusDbQuery.Result()
                {
                    OwnCampaignExists = false,
                    ContentExists = false,
                    EditorType = null,
                });

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(dbContextMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = type,
                htmlContent,
                meta = "true" // it does not care
            }));

            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            dbContextMock.VerifyAll();
            dbContextMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("html")]
        [InlineData("unlayer")]
        public async Task PUT_campaign_should_return_500_error_when_user_does_not_exist(string type)
        {
            // Arrange
            var url = "/accounts/test1@test.com/campaigns/123/content";
            var token = TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518;
            var expectedAccountName = "test1@test.com";
            var expectedIdCampaign = 123;
            var htmlContent = "My HTML Content";

            var dbContextMock = new Mock<IDbContext>();
            dbContextMock
                .Setup(x => x.QueryFirstOrDefaultAsync<FirstOrDefaultCampaignStatusDbQuery.Result>(
                    It.IsAny<string>(),
                    It.Is<ByCampaignIdAndAccountNameParameters>(x =>
                        x.AccountName == expectedAccountName
                        && x.IdCampaign == expectedIdCampaign)))
                .ReturnsAsync(new FirstOrDefaultCampaignStatusDbQuery.Result()
                {
                    OwnCampaignExists = false,
                    ContentExists = false,
                    EditorType = null,
                });

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(dbContextMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = type,
                htmlContent,
                meta = "true" // it does not care
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            dbContextMock.VerifyAll();
            dbContextMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null, true, "UPDATE")]
        [InlineData(55, true, "UPDATE")]
        [InlineData(4, true, "UPDATE")]
        [InlineData(5, true, "UPDATE")]
        [InlineData(null, false, "INSERT")]
        public async Task PUT_campaign_should_store_html_content(int? currentEditorType, bool contentExists, string sqlQueryStartsWith)
        {
            // Arrange
            var url = "/accounts/test1@test.com/campaigns/123/content";
            var token = TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518;
            var expectedAccountName = "test1@test.com";
            var expectedIdCampaign = 123;
            var htmlContent = "My HTML Content";

            var dbContextMock = new Mock<IDbContext>();
            dbContextMock
                .Setup(x => x.QueryFirstOrDefaultAsync<FirstOrDefaultCampaignStatusDbQuery.Result>(
                    It.IsAny<string>(),
                    It.Is<ByCampaignIdAndAccountNameParameters>(x =>
                        x.AccountName == expectedAccountName
                        && x.IdCampaign == expectedIdCampaign)))
                .ReturnsAsync(new FirstOrDefaultCampaignStatusDbQuery.Result()
                {
                    OwnCampaignExists = true,
                    ContentExists = contentExists,
                    EditorType = currentEditorType,
                });

            dbContextMock
                .Setup(x => x.ExecuteAsync(
                    It.Is<string>(s => s.Trim().StartsWith(sqlQueryStartsWith)),
                    It.Is<ContentRow>(c =>
                        c.IdCampaign == expectedIdCampaign
                        && c.Content == htmlContent
                        && c.Meta == null)))
                .ReturnsAsync(1);

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(dbContextMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = "html",
                htmlContent
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dbContextMock.VerifyAll();
            dbContextMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null, true, "UPDATE")]
        [InlineData(55, true, "UPDATE")]
        [InlineData(4, true, "UPDATE")]
        [InlineData(5, true, "UPDATE")]
        [InlineData(null, false, "INSERT")]
        public async Task PUT_campaign_should_store_unlayer_content(int? currentEditorType, bool contentExists, string sqlQueryStartsWith)
        {
            // Arrange
            var url = "/accounts/test1@test.com/campaigns/123/content";
            var token = TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518;
            var expectedAccountName = "test1@test.com";
            var expectedIdCampaign = 123;
            var htmlContent = "My HTML Content";
            var metaAsString = "{\"data\":\"My Meta Content\"}";

            var dbContextMock = new Mock<IDbContext>();
            dbContextMock
                .Setup(x => x.QueryFirstOrDefaultAsync<FirstOrDefaultCampaignStatusDbQuery.Result>(
                    It.IsAny<string>(),
                    It.Is<ByCampaignIdAndAccountNameParameters>(x =>
                        x.AccountName == expectedAccountName
                        && x.IdCampaign == expectedIdCampaign)))
                .ReturnsAsync(new FirstOrDefaultCampaignStatusDbQuery.Result()
                {
                    OwnCampaignExists = true,
                    ContentExists = contentExists,
                    EditorType = currentEditorType,
                });

            dbContextMock
                .Setup(x => x.ExecuteAsync(
                    It.Is<string>(s => s.Trim().StartsWith(sqlQueryStartsWith)),
                    It.Is<ContentRow>(c =>
                        c.IdCampaign == expectedIdCampaign
                        && c.Content == htmlContent
                        && c.Meta == metaAsString)))
                .ReturnsAsync(1);

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(dbContextMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PutAsync(url, JsonContent.Create(new
            {
                type = "unlayer",
                htmlContent,
                meta = Utils.ParseAsJsonElement(metaAsString)
            }));
            _output.WriteLine(response.GetHeadersAsString());
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dbContextMock.VerifyAll();
            dbContextMock.VerifyNoOtherCalls();
        }
    }
}
