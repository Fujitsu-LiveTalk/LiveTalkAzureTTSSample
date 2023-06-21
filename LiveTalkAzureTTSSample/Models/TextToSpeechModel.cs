/*
 * Copyright 2021 FUJITSU LIMITED
 * クラス名　：TextToSpeechModel
 * 概要      ：Azure Cognitive Services Speech Servicesと連携
*/
using LiveTalkAzureTTSSample.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiveTalkAzureTTSSample.Models
{
    internal class TextToSpeechModel
    {
        private string TokenUrl = "https://{0}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private string Url = "https://{0}.tts.speech.microsoft.com/cognitiveservices/v1";
        private string ProxyServer = Config.UseProxy ? Config.ProxyServerName : "";     // PROXY経由なら proxy.hogehoge.jp:8080 のように指定
        private string ProxyId = Config.ProxyServerId;                                  // 認証PROXYならIDを指定
        private string ProxyPassword = Config.ProxyServerPassword;                      // 認証PROXYならパスワードを指定
        private string AccessToken = string.Empty;
        private System.Timers.Timer ExpireTimer;

        public TextToSpeechModel()
        {
            this.TokenUrl = string.Format(this.TokenUrl, Common.Config.Location);
            this.Url = string.Format(this.Url, Common.Config.Location);
        }

        /// <summary>
        /// アクセストークンを取得する
        /// </summary>
        /// <returns></returns>
        public async Task GetToken()
        {
            this.AccessToken = await FetchTokenAsync().ConfigureAwait(false);
        }

        public async Task<(byte[], string)> TextToSpeechAsync(string text, string speeker)
        {
            try
            {
                // よみかた辞書
                text = text.Replace("&nbsp;", " ").Replace("障がい者", "しょうがいしゃ").Replace("障がい", "しょうがい");

                // パラメタ設定
                var voiceName = "ja-JP-KeitaNeural";
                var body =
                    @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='ja-JP'>" +
                    @$"<voice name='{voiceName}'>" +
                    System.Security.SecurityElement.Escape(text) +
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
                                this.ExpireTimer.Stop();
                                this.AccessToken = await FetchTokenAsync().ConfigureAwait(false);
                                return (null, response.StatusCode.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ExpireTimer.Stop();
                this.AccessToken = await FetchTokenAsync().ConfigureAwait(false);
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

            //　タイマー
            if (this.ExpireTimer == null)
            {
                this.ExpireTimer = new System.Timers.Timer();
                this.ExpireTimer.Interval = (3 * 60 + 30) * 1000;
                this.ExpireTimer.Elapsed += ExpireTimer_Tick;
            }
            this.ExpireTimer.Start();

            // 認証呼び出し
            using (var client = new HttpClient(ch))
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Common.Config.APIKey);
                UriBuilder uriBuilder = new UriBuilder(this.TokenUrl);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
                var accessToken = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (accessToken.IndexOf("{", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    Console.WriteLine("Successfully obtained an access token.\n");
                }
                else
                {
                    Console.WriteLine($"{ new String('-', 20)}\nUnSuccessfully obtained an access token.\n");

                    // デシリアライズ
                    var json = JsonSerializer.Deserialize<TReslutAccessToken>(accessToken);
                    Console.WriteLine($"{json?.error?.message}");
                    Console.WriteLine($"{ new String('-', 20)} \n");
                    accessToken = string.Empty;
                }
                return accessToken;
            }
        }

        private async void ExpireTimer_Tick(object sender, object e)
        {
            this.ExpireTimer.Stop();
            this.AccessToken = await FetchTokenAsync().ConfigureAwait(false);
        }


        private class TReslutAccessToken
        {
            public TError error { get; set; }
        }

        private class TError
        {
            public string code { get; set; }
            public string message { get; set; }
        }


    }
}
