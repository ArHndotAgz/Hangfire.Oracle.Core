@echo off
setlocal enabledelayedexpansion

REM Kavosh.Hangfire.Oracle.Core Build & Package Script
set Configuration=Release
set OutputPath=.\nupkg
set Version=2.0.0
set ProjectFile=Kavosh.Hangfire.Oracle.Core.csproj

echo ================================
echo Kavosh.Hangfire.Oracle.Core
echo Build ^& Package Script v%Version%
echo ================================
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

REM Restore packages
echo.
echo Restoring NuGet packages...
dotnet restore "%ProjectFile%"
if errorlevel 1 (
    echo [X] Package restore failed!
    pause
    exit /b 1
)

REM Build
echo.
echo Building project (%Configuration%)...
dotnet build "%ProjectFile%" --configuration %Configuration% --no-restore
if errorlevel 1 (
    echo [X] Build failed!
    pause
    exit /b 1
)

REM Pack
echo.
echo Creating NuGet package...
dotnet pack "%ProjectFile%" --configuration %Configuration% --no-build --output "%OutputPath%"
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
pause

endlocal