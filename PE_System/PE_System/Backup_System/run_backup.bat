@echo off
setlocal enabledelayedexpansion

:: --- DATABASE CONFIGURATION ---
set DB_USER=root
set DB_PASS=sa@123
set DB_NAME=stock_management

:: --- BACKUP PATHS ---
:: Path 1: Local Backup on PC
set LOCAL_BACKUP_PATH=D:\PE_System_Backups
:: Path 2: Pen Drive Backup (Change F: to your pen drive letter)
set PEN_DRIVE_PATH=F:\PE_System_Backups

:: --- MYSQL DUMP EXE PATH ---
:: This looks for mysqldump.exe in the SAME folder as this script.
:: Put a copy of mysqldump.exe inside the Backup_System folder.
set MYSQL_DUMP="%~dp0mysqldump.exe"

:: --- GENERATE TIMESTAMP ---
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set TIMESTAMP=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%

set FILENAME=%DB_NAME%_%TIMESTAMP%.sql

:: 1. CREATE LOCAL BACKUP
if not exist "%LOCAL_BACKUP_PATH%" mkdir "%LOCAL_BACKUP_PATH%"
echo Backing up to Local Drive (D:)...
%MYSQL_DUMP% --user=%DB_USER% --password=%DB_PASS% --databases %DB_NAME% > "%LOCAL_BACKUP_PATH%\%FILENAME%"

:: 2. COPY TO PEN DRIVE IF CONNECTED
if exist "%PEN_DRIVE_PATH:~0,3%" (
    if not exist "%PEN_DRIVE_PATH%" mkdir "%PEN_DRIVE_PATH%"
    echo Pen Drive detected. Copying backup...
    copy "%LOCAL_BACKUP_PATH%\%FILENAME%" "%PEN_DRIVE_PATH%\%FILENAME%"
) else (
    echo Pen Drive not detected. Skipping USB backup.
)

:: 3. DELETE OLD BACKUPS (Older than 30 days on Local)
echo Cleaning up old backups...
forfiles /p "%LOCAL_BACKUP_PATH%" /s /m *.sql /d -30 /c "cmd /c del @path"

echo Backup process completed successfully!
