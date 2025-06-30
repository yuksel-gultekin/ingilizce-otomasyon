@echo off
echo ========================================
echo Java Ogrenme Otomasyonu - Build Script
echo ========================================
echo.

echo [1/4] Restoring NuGet packages...
dotnet restore JavaOtomasyon.sln
if %ERRORLEVEL% neq 0 (
    echo ERROR: Package restore failed!
    pause
    exit /b 1
)

echo.
echo [2/4] Building solution...
dotnet build JavaOtomasyon.sln --configuration Release
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo [3/4] Running tests...
dotnet test JavaOtomasyonApp.Tests/JavaOtomasyonApp.Tests.csproj --configuration Release --verbosity normal
if %ERRORLEVEL% neq 0 (
    echo WARNING: Some tests failed!
    echo Continuing with build...
)

echo.
echo [4/4] Build completed successfully!
echo.
echo To run the application:
echo   dotnet run --project JavaOtomasyonApp/JavaOtomasyonApp.csproj
echo.
echo Or navigate to:
echo   JavaOtomasyonApp/bin/Release/net6.0-windows/JavaOtomasyonApp.exe
echo.
pause
