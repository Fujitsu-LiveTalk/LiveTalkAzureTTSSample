/*
 * Copyright 2021 FUJITSU LIMITED
 * クラス名　：Config
 * 概要      ：設定ファイル入力
*/

using Microsoft.Extensions.Configuration;
using System.IO;

namespace LiveTalkAzureTTSSample.Common
{
    internal class Config
    {
        private static IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json")
            .Build();

        public Config()
        {

        }

        internal static string APIKey
        {
            get { return Configuration.GetSection("AppSettings")["APIKey"]; }
        }

        internal static string Location
        {
            get { return Configuration.GetSection("AppSettings")["Location"]; }
        }
    }
}
