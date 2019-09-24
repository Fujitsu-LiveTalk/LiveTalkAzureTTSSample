# LiveTalkAzureTTSSample
LiveTalk常時ファイル出力で出力したテキストを、Microsoft Azure Cognitive Services Speech ServicesのText To Speech を使って音声合成出力するサンプルです。  
本サンプルコードは、.NET Core 3.0で作成しています。コードレベルでは.NET Framework 4.6と互換性があります。

![Process](https://github.com/FujitsuSSL-LiveTalk/LiveTalkAzureTTSSample/blob/images/README.png)

# サンプルコードの動き
本サンプルでは、日本語発話を英語翻訳したものを音声合成する前提となっており、サンプルコード動作を簡単に説明すると次のような動作をします。  
1. LiveTalkで音声認識した結果がファイルに出力されるので、それを自動的に読込み、Speech Servicesの音声合成 APIを呼び出します。
2. Speech Servicesの音声合成 APIから戻ってきた音声ファイルをスピーカーの既定のデバイスで再生します。
3. サンプルコード中でSSML形式で指定してありますので、英語翻訳されたものを音声合成（話者はZiraRUS）します。
※ 英語以外はエラーとなります。


# 事前準備
1. LiveTalkに添付されている手順に基づき、Microsoft音声サービスを有効化します（現状、東アジア指定、リソース名LiveTalkを前提としたサンプルコードになっております）。
2. 有効化するとAPIキーが取得できますので、サンプルコードに指定します。
3. インターネットとの接続がPROXY経由の場合、PROXYサーバーや認証情報を設定してください。

# 連絡事項
本ソースコードは、LiveTalkの保守サポート範囲に含まれません。  
頂いたissueについては、必ずしも返信できない場合があります。  
LiveTalkそのものに関するご質問は、公式WEBサイトのお問い合わせ窓口からご連絡ください。
