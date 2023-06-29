# 7-zip_extract_and_open
This application is an extension for 7-zip that unzips the file to the desktop when double-clicked and opens the folder in Explorer.

# Installation
## 1. Install 7-zip
   Download 7-zip and install it from https://www.7-zip.org/download.html
   
## 2. Place this 7-zip_extract_and_open folder in the 7-zip installation folder
   Compile or download 7-zip_extract_and_open from release page<br>
   https://github.com/Ponpok0/7-zip_extract_and_open/releases<br>
   
   Example: When you have installed 7-zip in C:\Program Files\7-Zip, place to C:\Program Files\7-Zip\7-zip_extract_and_open

## 3. Associate compressed file with 7-zip_extract_and_open.bat
   If the folder is generated on the desktop and Explorer is opened in the extracted folder,   
   you have succeeded😀

## 4.(Optional) Change compressed file icons
  If you associate 7-zip_extract_and_open.bat, the icon will not be set. The icons have been prepared for you, install the following optionally.
  However, please note that this will make changes to the Windows registry, so use at your own risk.
  I recommend using nirsoft's FileTypesMan to change the icons as you wish instead of installing the following.<br>
  https://www.nirsoft.net/utils/file_types_manager.html
  
  1. This app needs dotnet6.0 runtime. Download and install it if you have not installed.<br>
  https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0

  2. Execute 7-zip_custom_icon_associator.exe
  If you agree with the content, please press "y" and press enter.

  3. Re-associate compressed file with 7-zip_extract_and_open.bat
  If the compressed file icons changed, you have succeeded😀

  4. If you want uninstall it, execute "7-zip_custom_icon_associator.exe -u " with administrator privileges.
  If you agree with the content, please press "y" and press enter.

  Example:  
  1. Create a shortcut of 7-zip_custom_icon_associator.exe.  
  2. Open its property and add a space-separated -u at the end of the target.  
  3. Execute the shortcut with administrator privileges.  

─────────────────────────────────────────────────────────────────────

# 7-zip_extract_and_open
このアプリは7-zip向けの、ダブルクリックしたときにデスクトップにファイルを解凍してエクスプローラーでそのフォルダーを開くという拡張機能です。

# インストール手順
## 1. 7-zipをインストールしてください。
こちらから https://www.7-zip.org/download.html
ダウンロードとインストールを行ってください。

## 2. 7-zip_extract_and_openフォルダーを7-zipをインストールしたフォルダーに配置してください。
7-zip_extract_and_openをコンパイルするかReleaseページからダウンロードしてください。<br>
https://github.com/Ponpok0/7-zip_extract_and_open/releases<br>

C:\Program Files\7-Zip にインストールしていたら C:\Program Files\7-Zip\7-zip_extract_and_open です。

## 3. 圧縮ファイルと7-zip_extract_and_open.batを関連付けてください。
ダブルクリックしてデスクトップに解凍され、エクスプローラーでそのフォルダが開かれたら成功です😀

## 4.(任意) 圧縮ファイルのアイコンを変更
7-zip_extract_and_open.batを関連付けるとアイコンが設定されなくなります。アイコンを用意しましたので任意で以下のインストールを行ってください。
ただし、これはWindowsのレジストリに変更を行うため使用は自己の責任においてお願いします。

下記の代わりにアイコンを変更するにはnirsoftのFileTypesManを使用してお好きなように変更することをお勧めします。<br>
https://www.nirsoft.net/utils/file_types_manager.html

1. dotnet6.0ランタイムが必要です。下記からインストールしてください。<br>
https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0

2. 7-zip_custom_icon_associator.exeを実行してください。
内容に同意出来たらyを押してエンターキーを押してください。

3. 圧縮ファイルに7-zip_extract_and_open.batを再び関連付けしてください。
圧縮ファイルのアイコンが変更されたら成功です😀

4. アイコンをアンインストールする場合は"7-zip_custom_icon_associator.exe -u "を管理者権限で実行してください。

例:
  1. 7-zip_custom_icon_associator.exeのショートカットを作成します。
  2. そのプロパティを開いてリンク先の末尾に半角スペース区切りで -uを追加します。
  3. 管理者権限でそのショートカットを実行します。
