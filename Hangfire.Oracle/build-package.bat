@echo off
setlocal enabledelayedexpansion

REM Kavosh.Hangfire.Oracle.Core Build & Package Script
set Configuration=Release
set OutputPath=.\nupkg
set ProjectFile=Kavosh.Hangfire.Oracle.Core.csproj

REM Get current timestamp for version suffix
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set TIMESTAMP=%datetime:~0,4%%datetime:~4,2%%datetime:~6,2%%datetime:~8,2%%datetime:~10,2%

echo ====================================
echo Kavosh.Hangfire.Oracle.Core
echo Build ^& Package Script
echo By: Amirhossein Aghazadeh
echo Company: Rayan Pardaz Kavosh
echo ====================================
echo.

REM Check if project file exists
if not exist "%ProjectFile%" (
    echo [X] Project file not found: %ProjectFile%
    echo Please run this script from the project directory.
    pause
    exit /b 1
)

REM Clean previous builds
echo Cleaning previous builds...
if exist "%OutputPath%" rmdir /s /q "%OutputPath%"
mkdir "%OutputPath%"

echo Cleaning bin and obj folders...
if exist ".\bin" rmdir /s /q ".\bin"
if exist ".\obj" rmdir /s /q ".\obj"

REM Clear NuGet caches
echo.
echo Clearing NuGet caches...
dotnet nuget locals all --clear
if errorlevel 1 (
    echo [!] Warning: Failed to clear NuGet cache
)

REM Restore packages
echo.
echo Restoring NuGet packages...
dotnet restore "%ProjectFile%" --force --no-cache
if errorlevel 1 (
    echo [X] Package restore failed!
    pause
    exit /b 1
)

REM Build
echo.
echo Building project (%Configuration%)...
dotnet build "%ProjectFile%" --configuration %Configuration% --no-restore --force
if errorlevel 1 (
    echo [X] Build failed!
    pause
    exit /b 1
)

REM Pack with version suffix
echo.
echo Creating NuGet package with timestamp: %TIMESTAMP%
dotnet pack "%ProjectFile%" --configuration %Configuration% --no-build --output "%OutputPath%" --version-suffix "dev%TIMESTAMP%"
if errorlevel 1 (
    echo [X] Package creation failed!
    pause
    exit /b 1
)

REM Display results
echo.
echo ================================
echo [√] Build ^& Package Complete!
echo ================================
echo.
echo Package created:
dir /b "%OutputPath%\*.nupkg"
echo.
echo Location: %cd%\%OutputPath%
echo.
echo [!] IMPORTANT: Clear your project's NuGet cache:
echo    1. Delete packages from your consuming project
echo    2. Run: dotnet nuget locals all --clear
echo    3. Restore packages in consuming project
echo.
pause

endlocal