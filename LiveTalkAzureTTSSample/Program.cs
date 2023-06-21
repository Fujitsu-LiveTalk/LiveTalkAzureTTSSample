/*
 * Copyright 2021 FUJITSU LIMITED
 * システム名：LiveTalkAzureTTSSample
 * 概要      ：LiveTalk-Speech Services連携サンプルアプリ
*/
using LiveTalkAzureTTSSample.Common;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveTalkAzureTTSSample
{
    class Program
    {
        static LiveTalk.FileCollaboration FileInterface;
        static BlockingCollection<byte[]> AudioQueue = new BlockingCollection<byte[]>();
        static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Config.Load();

            var model = new Models.TextToSpeechModel();
            var param = new string[]
            {
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "LiveTalkOutput.csv"),
            };
            if (args.Length >= 1)
            {
                param[0] = args[0];
            }
            Console.WriteLine("InputCSVFileName  :" + param[0]);
            FileInterface = new LiveTalk.FileCollaboration(param[0], "");

            // ファイル入力(LiveTalk常時ファイル出力からの入力)
            FileInterface.RemoteMessageReceived += async (s) =>
            {
                var reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var items = reg.Split(s);

                Console.WriteLine(">>>>>>>");
                Console.WriteLine("DateTime:" + items[0]);
                Console.WriteLine("Speaker:" + items[1]);
                Console.WriteLine("Speech contents:" + items[2]);
                Console.WriteLine("Translate content:" + items[3]);

                (byte[] waveData, string errorMessage) = await model.TextToSpeechAsync(items[3] == "\"\"" ? items[2] : items[3], items[1]);
                if (waveData != null)
                {
                    // 音声合成キューにエントリ
                    AudioQueue.Add(waveData);
                }
                else
                {
                    // エラーメッセージ表示
                    Console.WriteLine(errorMessage);
                }
            };

            // 事前に認証トークンを取得（別途、トークン有効期間切れ対応が必要）
            Task.Factory.StartNew(async () =>
            {
                await model.GetToken();
            });

            // 音声合成キュー処理
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // 音声合成の再生
                    if (AudioQueue.TryTake(out byte[] data, -1, TokenSource.Token))
                    {
                        try
                        {
                            using (var ms = new MemoryStream(data))
                            {
                                using (var audio = new WaveFileReader(ms))
                                {
                                    using (var outputDevice = new WaveOutEvent())
                                    {
                                        outputDevice.Init(audio);
                                        outputDevice.Play();

                                        // 再生終了待ち
                                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                                        {
                                            Thread.Sleep(1000);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            });

            // ファイル監視開始
            if (System.IO.File.Exists(param[0]))
            {
                System.IO.File.Delete(param[0]);
            }
            FileInterface.WatchFileStart();

            // 処理終了待ち
            var message = Console.ReadLine();

            // ファイル監視終了
            TokenSource.Cancel(true);
            FileInterface.WatchFileStop();
        }
    }
}
