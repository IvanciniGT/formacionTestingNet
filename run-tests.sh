#!/usr/bin/env bash
# =============================================================================
# Ejecución de pruebas — testing-dotnet
# =============================================================================
# Requisitos:
#   - .NET 9 SDK (9.0.x)
#   - Docker (solo para Rest.V1.SystemTests con Testcontainers/MariaDB)
#   - Chromium para Playwright (ver sección de instalación abajo)
#
# Uso:
#   chmod +x run-tests.sh
#   ./run-tests.sh          # Ejecuta TODAS las pruebas
#   ./run-tests.sh basico   # Solo proyectoPruebasBasico
#   ./run-tests.sh web      # Solo proyectos (Diccionarios)
# =============================================================================

set -e

# --- Configuración del SDK .NET 9 -------------------------------------------
# Si tienes .NET 9 en ~/.dotnet (instalación local), descomentar estas líneas:
# export DOTNET_ROOT=$HOME/.dotnet
# export PATH="$DOTNET_ROOT:$PATH"

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Colores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

run_test() {
    local project="$1"
    local name="$2"
    echo -e "\n${YELLOW}▶ $name${NC}"
    if dotnet test "$project" --no-build --verbosity quiet 2>&1; then
        echo -e "${GREEN}  ✓ $name${NC}"
    else
        echo -e "${RED}  ✗ $name${NC}"
        FAILED=1
    fi
}

FAILED=0

# =============================================================================
# INSTALACIÓN DE DEPENDENCIAS
# =============================================================================
#
# 1. Instalar .NET 9 SDK (si no lo tienes):
#    ----------------------------------------
#    # Opción A: Script oficial de Microsoft (instala en ~/.dotnet)
#    curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 9.0
#
#    # Opción B: Homebrew (requiere macOS compatible)
#    brew install dotnet@9
#
#    # Verificar
#    dotnet --version   # Debe mostrar 9.0.x
#
# 2. Instalar navegadores Playwright (para MVC.PlaywrightTests):
#    ---------------------------------------------------------------
#    # Primero compilar el proyecto para que se descargue el paquete NuGet
#    cd proyectos && dotnet build Apps/MVC.PlaywrightTests/
#
#    # Luego instalar Chromium usando el CLI de Playwright via Node.js
#    cd Apps/MVC.PlaywrightTests/bin/Debug/net9.0/.playwright/package
#    node cli.js install chromium
#
#    # Esto descarga Chromium (~120MB) a ~/Library/Caches/ms-playwright/
#    # Solo hay que hacerlo una vez (o cuando se actualice la versión de Playwright).
#
#    # NOTA: El método oficial usa PowerShell (pwsh) pero si no lo tienes
#    # instalado, el método con node funciona igual:
#    # pwsh bin/Debug/net9.0/playwright.ps1 install chromium
#
# 3. Docker (solo para Rest.V1.SystemTests):
#    -----------------------------------------
#    # Los tests de sistema de la API REST usan Testcontainers con MariaDB.
#    # Necesitas Docker Desktop corriendo.
#    # Si no tienes Docker, puedes ejecutar todo excepto esos tests.
#
# =============================================================================

# =============================================================================
# COMPILAR TODO
# =============================================================================
echo -e "${YELLOW}═══ Compilando proyectos ═══${NC}"

if [[ "$1" != "web" ]]; then
    echo -e "\n${YELLOW}▶ Compilando proyectoPruebasBasico${NC}"
    dotnet build "$SCRIPT_DIR/proyectoPruebasBasico/" --verbosity quiet
fi

if [[ "$1" != "basico" ]]; then
    echo -e "\n${YELLOW}▶ Compilando proyectos (DiccionariosApp)${NC}"
    dotnet build "$SCRIPT_DIR/proyectos/" --verbosity quiet
fi

# =============================================================================
# PRUEBAS — proyectoPruebasBasico (2 proyectos: NUnit + xUnit)
# =============================================================================
if [[ "$1" != "web" ]]; then
    echo -e "\n${YELLOW}═══ proyectoPruebasBasico ═══${NC}"

    run_test "$SCRIPT_DIR/proyectoPruebasBasico/LibreriaMatematica.Test/" \
             "LibreriaMatematica.Test (NUnit)"

    run_test "$SCRIPT_DIR/proyectoPruebasBasico/LibreriaMatematica2.Test/" \
             "LibreriaMatematica2.Test (xUnit + Moq)"
fi

# =============================================================================
# PRUEBAS — proyectos/DiccionariosApp (9 proyectos de test)
# =============================================================================
if [[ "$1" != "basico" ]]; then
    echo -e "\n${YELLOW}═══ proyectos (DiccionariosApp) ═══${NC}"

    # --- Tests unitarios ---
    run_test "$SCRIPT_DIR/proyectos/Diccionarios/Ficheros.Tests/" \
             "Ficheros.Tests (unitario)"

    run_test "$SCRIPT_DIR/proyectos/Diccionarios/BBDD.Tests/" \
             "BBDD.Tests (integración EF Core InMemory)"

    run_test "$SCRIPT_DIR/proyectos/Servicio/Tests/" \
             "Servicio.Tests (unitario + Moq)"

    run_test "$SCRIPT_DIR/proyectos/Rest/V1/Tests/" \
             "Rest.V1.Tests (unitario + Moq)"

    run_test "$SCRIPT_DIR/proyectos/Apps/MVC.Tests/" \
             "MVC.Tests (unitario + Moq)"

    run_test "$SCRIPT_DIR/proyectos/UI/Consola.Tests/" \
             "UI.Consola.Tests (unitario)"

    # --- Tests de sistema (HTTP real, InMemory DB) ---
    run_test "$SCRIPT_DIR/proyectos/Apps/MVC.SystemTests/" \
             "MVC.SystemTests (sistema, WebApplicationFactory + InMemory)"

    # --- Tests Playwright (navegador Chromium real) ---
    run_test "$SCRIPT_DIR/proyectos/Apps/MVC.PlaywrightTests/" \
             "MVC.PlaywrightTests (Playwright + Chromium headless)"

    # --- Tests de sistema con Docker (MariaDB via Testcontainers) ---
    # NOTA: Requiere Docker corriendo. Comentar si no tienes Docker.
    # run_test "$SCRIPT_DIR/proyectos/Rest/V1/SystemTests/" \
    #          "Rest.V1.SystemTests (sistema, Testcontainers + MariaDB)"
fi

# =============================================================================
# RESUMEN
# =============================================================================
echo ""
if [[ $FAILED -eq 0 ]]; then
    echo -e "${GREEN}═══ TODAS LAS PRUEBAS PASARON ✓ ═══${NC}"
else
    echo -e "${RED}═══ ALGUNAS PRUEBAS FALLARON ✗ ═══${NC}"
    exit 1
fi
