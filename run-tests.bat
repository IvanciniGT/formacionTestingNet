@echo off
REM =============================================================================
REM Ejecucion de pruebas — testing-dotnet
REM =============================================================================
REM Requisitos:
REM   - .NET 9 SDK (9.0.x)
REM   - Docker (solo para Rest.V1.SystemTests con Testcontainers/MariaDB)
REM   - Chromium para Playwright (ver seccion de instalacion abajo)
REM
REM Uso:
REM   run-tests.bat          &REM Ejecuta TODAS las pruebas
REM   run-tests.bat basico   &REM Solo proyectoPruebasBasico
REM   run-tests.bat web      &REM Solo proyectos (Diccionarios)
REM =============================================================================

setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0"
set FAILED=0

REM =============================================================================
REM INSTALACION DE DEPENDENCIAS
REM =============================================================================
REM
REM 1. Instalar .NET 9 SDK (si no lo tienes):
REM    ----------------------------------------
REM    Descargar desde https://dotnet.microsoft.com/download/dotnet/9.0
REM    O con winget:
REM      winget install Microsoft.DotNet.SDK.9
REM
REM    Verificar:
REM      dotnet --version   (Debe mostrar 9.0.x)
REM
REM 2. Instalar navegadores Playwright (para MVC.PlaywrightTests):
REM    ---------------------------------------------------------------
REM    Primero compilar el proyecto:
REM      cd proyectos
REM      dotnet build Apps\MVC.PlaywrightTests\
REM
REM    Opcion A: Con PowerShell (recomendado en Windows):
REM      pwsh Apps\MVC.PlaywrightTests\bin\Debug\net9.0\playwright.ps1 install chromium
REM
REM    Opcion B: Con Node.js:
REM      cd Apps\MVC.PlaywrightTests\bin\Debug\net9.0\.playwright\package
REM      node cli.js install chromium
REM
REM    Descarga Chromium (~120MB) a %LOCALAPPDATA%\ms-playwright\
REM    Solo hay que hacerlo una vez (o al actualizar Playwright).
REM
REM 3. Docker (solo para Rest.V1.SystemTests):
REM    -----------------------------------------
REM    Los tests de sistema de la API REST usan Testcontainers con MariaDB.
REM    Necesitas Docker Desktop corriendo.
REM =============================================================================

REM =============================================================================
REM COMPILAR TODO
REM =============================================================================
echo.
echo === Compilando proyectos ===

if /i "%~1" NEQ "web" (
    echo.
    echo ^> Compilando proyectoPruebasBasico
    dotnet build "%SCRIPT_DIR%proyectoPruebasBasico\" --verbosity quiet
    if errorlevel 1 (
        echo   ERROR compilando proyectoPruebasBasico
        set FAILED=1
        goto :resumen
    )
)

if /i "%~1" NEQ "basico" (
    echo.
    echo ^> Compilando proyectos (DiccionariosApp)
    dotnet build "%SCRIPT_DIR%proyectos\" --verbosity quiet
    if errorlevel 1 (
        echo   ERROR compilando proyectos
        set FAILED=1
        goto :resumen
    )
)

REM =============================================================================
REM PRUEBAS — proyectoPruebasBasico (2 proyectos: NUnit + xUnit)
REM =============================================================================
if /i "%~1" NEQ "web" (
    echo.
    echo === proyectoPruebasBasico ===

    call :run_test "%SCRIPT_DIR%proyectoPruebasBasico\LibreriaMatematica.Test\" "LibreriaMatematica.Test (NUnit)"
    call :run_test "%SCRIPT_DIR%proyectoPruebasBasico\LibreriaMatematica2.Test\" "LibreriaMatematica2.Test (xUnit + Moq)"
)

REM =============================================================================
REM PRUEBAS — proyectos/DiccionariosApp (9 proyectos de test)
REM =============================================================================
if /i "%~1" NEQ "basico" (
    echo.
    echo === proyectos (DiccionariosApp) ===

    REM --- Tests unitarios ---
    call :run_test "%SCRIPT_DIR%proyectos\Diccionarios\Ficheros.Tests\" "Ficheros.Tests (unitario)"
    call :run_test "%SCRIPT_DIR%proyectos\Diccionarios\BBDD.Tests\" "BBDD.Tests (integracion EF Core InMemory)"
    call :run_test "%SCRIPT_DIR%proyectos\Servicio\Tests\" "Servicio.Tests (unitario + Moq)"
    call :run_test "%SCRIPT_DIR%proyectos\Rest\V1\Tests\" "Rest.V1.Tests (unitario + Moq)"
    call :run_test "%SCRIPT_DIR%proyectos\Apps\MVC.Tests\" "MVC.Tests (unitario + Moq)"
    call :run_test "%SCRIPT_DIR%proyectos\UI\Consola.Tests\" "UI.Consola.Tests (unitario)"

    REM --- Tests de sistema (HTTP real, InMemory DB) ---
    call :run_test "%SCRIPT_DIR%proyectos\Apps\MVC.SystemTests\" "MVC.SystemTests (sistema, WebApplicationFactory)"
    call :run_test "%SCRIPT_DIR%proyectos\Apps\MVC.PlaywrightTests\" "MVC.PlaywrightTests (Playwright + Chromium)"

    REM --- Tests de sistema con Docker (MariaDB via Testcontainers) ---
    REM NOTA: Requiere Docker corriendo. Descomentar si tienes Docker.
    REM call :run_test "%SCRIPT_DIR%proyectos\Rest\V1\SystemTests\" "Rest.V1.SystemTests (Testcontainers + MariaDB)"
)

REM =============================================================================
REM RESUMEN
REM =============================================================================
:resumen
echo.
if %FAILED% EQU 0 (
    echo === TODAS LAS PRUEBAS PASARON ===
) else (
    echo === ALGUNAS PRUEBAS FALLARON ===
    exit /b 1
)
exit /b 0

REM =============================================================================
REM Funcion auxiliar para ejecutar un proyecto de test
REM =============================================================================
:run_test
echo.
echo ^> %~2
dotnet test %~1 --no-build --verbosity quiet 2>nul
if errorlevel 1 (
    echo   X %~2
    set FAILED=1
) else (
    echo   OK %~2
)
exit /b
