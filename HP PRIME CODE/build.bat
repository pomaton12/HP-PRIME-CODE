@echo off
setlocal enabledelayedexpansion

if "%~1"=="" (
    echo Version number is required.
    echo Usage: build.bat [version]
    exit /b 1
)

set "version=%~1"

echo.
echo Compiling "HP_PRIME_CODE" with dotnet...

:: Compilación del proyecto con dotnet
dotnet publish -c Release -o "%~dp0publish"

echo.
echo Building Velopack Release v%version%

:: Crear el paquete con Velopack
vpk pack -u HPPrimeEdit -v %version% -p .\publish -e "HP PRIME CODE.exe"

echo.
echo Release v%version% has been built and packaged.
