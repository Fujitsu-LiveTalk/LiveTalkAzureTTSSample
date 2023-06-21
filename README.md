# LiveTalkAzureTTSSample
LiveTalk常時ファイル出力で出力したテキストを、Microsoft Azure Cognitive Services Speech ServicesのText To Speech を使って音声合成出力するサンプルです。  
本サンプルコードは、.NET 6.0で作成しています。コードレベルでは.NET Framework 4.6と互換性があります。

![Process](https://github.com/FujitsuSSL-LiveTalk/LiveTalkAzureTTSSample/blob/images/README.png)

# 事前準備
1. 有効な Azure アカウントにサインイン、または、無料アカウントを作成して Azure Portal にサインインします。
2. LiveTalk クライアントアプリの [設定]-[クラウドオプション]-[Azure設定] メニューから Azure 設定画面を開き、 [音声サービスキーの取得] ボタンをクリックして、 LiveTalk 付属の手順に従って、 Azure Portal にて「音声」サービスを有効化します。
3. 「音声」サービスが有効化できたら、手順の流れに沿って、LiveTalk の Azure 設定画面に取得した「音声サービスキー」と「Endpoint Location」を設定します。
4. LiveTalk の [設定]-[クライアント設定] メニューから設定画面を開き、プロキシーの利用情報などを設定します。

   * 上記 3 および 4 で設定された設定値を使って本サンプルは音声合成を行います。


# 補足
音声合成した文字数がサービスの費用として加算されます。

例えば、無料プランであれば、月 0.5 million 文字まで無料（音声認識の月５時間上限とは別枠計算）、従量課金プランであれば、 文字数に応じたの課金が発生します。

# サンプルコードの動き
本サンプルでは、LiveTalk で日本語入力（音声認識、キーボード入力、スタンプ入力、定型文入力）したものを音声合成する前提となってます。

サンプルコード動作を簡単に説明すると次のような動作をします。  
1. LiveTalkで音声認識した結果がファイルに出力されるので、それを自動的に読込み、Speech Servicesの音声合成 APIを呼び出します。
2. Speech Servicesの音声合成 APIから戻ってきた音声ファイルをスピーカーの既定のデバイスで再生します。
3. サンプルコード中でSSML形式で指定 (ja-JP-KeitaNeural) してあります。
※ 日本語以外はエラーとなります。

# 連絡事項
本ソースコードは、LiveTalkの保守サポート範囲に含まれません。  
頂いたissueについては、必ずしも返信できない場合があります。  
LiveTalkそのものに関するご質問は、公式WEBサイトのお問い合わせ窓口からご連絡ください。
