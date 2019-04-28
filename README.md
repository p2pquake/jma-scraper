## jma-scraper

jma-scraperは，気象庁Webサイトの地震情報・津波予報のパーサです．日本語ページのみの対応です．

jma-scraper is parser for earthquake information and tsunami forecast of Japan Meteorological Agency website (Japanese version only).

### プロジェクト

プロジェクト名|概要
:-:|---
JMAInformation|共通ライブラリです．地震情報・津波予報のURL一覧を取得する機能と，地震情報・津波予報のパーサが含まれています．
QuakeAnalyzer|地震情報のページ(HTMLファイル)を解析し，JSONとして出力するサンプルプログラムです．
QuakeReceiver|気象庁Webサイトから地震情報のページを取得するサンプルプログラムです．SQLiteを使用し，新しい地震情報のみ取得するようになっています．
TsunamiAnalyzer|QuakeAnalyzerの津波予報版です．
TsunamiReceiver|QuakeReceiverの津波予報版です．

### 使用方法

サンプルプログラムをご覧ください．

### License

ソースコードはMITライセンスです．テストデータに含まれる気象庁Webサイトのページは，CC-BYライセンスです．

Source code is provided under the MIT license.

Japan Meteorological Agency website included in the test data is provided under the CC-BY license.
