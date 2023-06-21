/*
 * Copyright 2021 FUJITSU LIMITED
 * クラス名　：Config
 * 概要      ：設定ファイル入力
*/

using System;
using System.IO;
using System.Xml.Linq;

namespace LiveTalkAzureTTSSample.Common
{
    internal class Config
    {
        private static XElement XmlData;

        internal static void Load()
        {
            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingPath = dataPath + "\\LiveTalk\\Settings";
            var fileName = settingPath + "\\Settings.xml";
            XmlData = XElement.Load(fileName);
        }

        /// <summary>
        /// MSSpeechServiceKey
        /// </summary>
        internal static string APIKey
        {
            get { return XmlData.Element("MSSpeechServiceKey").Value; }
        }

        /// <summary>
        /// MSSpeechLocation
        /// </summary>
        internal static string Location
        {
            get { return XmlData.Element("MSSpeechLocation").Value; }
        }

        /// <summary>
        /// SpeechLanguage
        /// </summary>
        internal static string SpeechLanguage
        {
            get { return XmlData.Element("SpeechLanguage").Value; }
        }

        /// <summary>
        /// UseProxy
        /// </summary>
        internal static bool UseProxy
        {
            get { return (XmlData.Element("UseProxy").Value == "ON"); }
        }

        /// <summary>
        /// ProxyServerName
        /// </summary>
        internal static string ProxyServerName
        {
            get { return XmlData.Element("ProxyServerName").Value; }
        }

        /// ProxyServerId
        /// </summary>
        internal static string ProxyServerId
        {
            get { return XmlData.Element("ProxyServerId").Value; }
        }

        /// ProxyServerPassword
        /// </summary>
        internal static string ProxyServerPassword
        {
            get { return XmlData.Element("ProxyServerPassword").Value; }
        }
    }
}
