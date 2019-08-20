/*
 * Copyright 2019 FUJITSU SOCIAL SCIENCE LABORATORY LIMITED
 * クラス名　：TextToSpeechModel
 * 概要      ：Azure Cognitive Services Speech Servicesと連携
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveTalkAzureTTSSample.Models
{
    internal class TextToSpeechModel
    {
        private string APIKey = "<<<<YOUR_SUBSCRIPTION_KEY>>>>";
        private string TokenUrl = "https://eastasia.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private string Url = "https://eastasia.tts.speech.microsoft.com/cognitiveservices/v1";
        private string ProxyServer = "";    // PROXY経由なら proxy.hogehoge.jp:8080 のように指定
        private string ProxyId = "";        // 認証PROXYならIDを指定
        private string ProxyPassword = "";  // 認証PROXYならパスワードを指定
        private string AccessToken = string.Empty;

        /// <summary>
        /// アクセストークンを取得する
        /// </summary>
        /// <returns></returns>
        public async Task GetToken()
        {
            this.AccessToken = await FetchTokenAsync().ConfigureAwait(false);
            Console.WriteLine("Successfully obtained an access token. \n");
        }

        public async Task<(byte[], string)> TextToSpeechAsync(string text)
        {
            try
            {
                // パラメタ設定
                var body =
                    @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" +
                    @"<voice name='Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)'>" +
                    text +
                    "</voice></speak>";

                // プロキシ設定
                var ch = new HttpClientHandler() { UseCookies = true };
                if (!string.IsNullOrEmpty(this.ProxyServer))
                {
                    var proxy = new System.Net.WebProxy(this.ProxyServer);
                    if (!string.IsNullOrEmpty(this.ProxyId) && !string.IsNullOrEmpty(this.ProxyPassword))
                    {
                        proxy.Credentials = new System.Net.NetworkCredential(this.ProxyId, this.ProxyPassword);
                    }
                    ch.Proxy = proxy;
                }
                else
                {
                    ch.Proxy = null;
                }

                // Web API呼び出し
                using (var client = new HttpClient(ch))
                {
                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(this.Url);
                        request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                        request.Headers.Add("Authorization", "Bearer " + this.AccessToken);
                        request.Headers.Add("Connection", "Keep-Alive");
                        request.Headers.Add("User-Agent", "LiveTalk");
                        request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");


                        using (var response = await client.SendAsync(request).ConfigureAwait(false))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                return (await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false), string.Empty);
                            }
                            else
                            {
                                return (null, response.StatusCode.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        /// <summary>
        /// アクセストークンを取得
        /// </summary>
        /// <returns></returns>
        private async Task<string> FetchTokenAsync()
        {
            // プロキシ設定
            var ch = new HttpClientHandler() { UseCookies = true };
            if (!string.IsNullOrEmpty(this.ProxyServer))
            {
                var proxy = new System.Net.WebProxy(this.ProxyServer);
                if (!string.IsNullOrEmpty(this.ProxyId) && !string.IsNullOrEmpty(this.ProxyPassword))
                {
                    proxy.Credentials = new System.Net.NetworkCredential(this.ProxyId, this.ProxyPassword);
                }
                ch.Proxy = proxy;
            }
            else
            {
                ch.Proxy = null;
            }

            // 認証呼び出し
            using (var client = new HttpClient(ch))
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.APIKey);
                UriBuilder uriBuilder = new UriBuilder(this.TokenUrl);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
                return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
