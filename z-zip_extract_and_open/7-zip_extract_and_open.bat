@echo off
setlocal enabledelayedexpansion
rem ////////////////////////  Setting  /////////////////////////

set PATH_OF_7-ZIP_G_EXE="%~dp0..\\7zG.exe"
if not exist %PATH_OF_7-ZIP_G_EXE% (
    echo 7zG.exe does not exist
    echo %PATH_OF_7-ZIP_G_EXE%
    pause
)

rem //////////////////////// Variables /////////////////////////
rem %* gets all args for file name separated by spaces
set "SRC_FILE_PATH="
for %%I in (%*) do (
    set "BUF=%%I"
    if not defined SRC_FILE_PATH (
        set "SRC_FILE_PATH=!BUF!"
    ) else (
        set "SRC_FILE_PATH=!SRC_FILE_PATH! !BUF!"
    )
)
rem Check by replacing with + since it is unclear whether the src file is enclosed in double quotes or not
set FIRST_CHAR=%SRC_FILE_PATH:~0,1%
set LAST_CHAR=%SRC_FILE_PATH:~-1%
set FIRST_CHAR=%FIRST_CHAR:"=+%
set LAST_CHAR=%LAST_CHAR:"=+%
if not "%FIRST_CHAR%"=="+" if not "%LAST_CHAR%"=="+" (
    set SRC_FILE_PATH="!SRC_FILE_PATH!"
)
rem get desktop path
FOR /F "usebackq tokens=3" %%i in (`REG QUERY "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v Desktop`) DO SET DESKTOP_PATH=%%i
rem get only the filename, even if the filename is separated by many spaces
set "FILENAME_WITHOUT_EXT="
for %%I in (%SRC_FILE_PATH%) do (
    set "BUF=%%~nI"
    if not defined FILENAME_WITHOUT_EXT (
        set "FILENAME_WITHOUT_EXT=!BUF!"
    ) else (
        set "FILENAME_WITHOUT_EXT=!FILENAME_WITHOUT_EXT! !BUF!"
    )
)

set DEST_PATH=%DESKTOP_PATH%

rem //////////////////// Debugging Echoes //////////////////////
rem echo check
rem echo src %SRC_FILE_PATH%
rem echo filename without ext %FILENAME_WITHOUT_EXT%
rem echo desktop %DESKTOP_PATH%
rem echo dest %DEST_PATH%
rem echo dest2 %DEST_PATH%\!FILENAME_WITHOUT_EXT!
rem pause
rem echo %PATH_OF_7-ZIP_G_EXE% x %SRC_FILE_PATH% -o"%DEST_PATH%\*"
rem pause
rem open folder when successed
rem echo "%DEST_PATH%\!FILENAME_WITHOUT_EXT!"
rem pause

rem //////////////////// Main Process //////////////////////////
%PATH_OF_7-ZIP_G_EXE% x %SRC_FILE_PATH% -o"%DEST_PATH%\*"
if %ERRORLEVEL% neq 0 exit 1
explorer.exe "%DEST_PATH%\!FILENAME_WITHOUT_EXT!"

exit 0
