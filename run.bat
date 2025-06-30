@echo off
echo ========================================
echo Java Ogrenme Otomasyonu - Run Script
echo ========================================
echo.

echo Starting Java Learning Automation App...
echo.
echo Default Admin Login:
echo   Email: admin@javaotomasyon.com
echo   Password: admin123
echo.
echo Press Ctrl+C to stop the application
echo.

dotnet run --project JavaOtomasyonApp/JavaOtomasyonApp.csproj

echo.
echo Application stopped.
pause
