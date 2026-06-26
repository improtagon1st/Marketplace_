param(
    [switch]$SkipInstaller
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiProject = Join-Path $projectRoot "MarketplaceAPI\MarketplaceAPI.csproj"
$wpfProject = Join-Path $projectRoot "MarketplaceWPF\MarketplaceWPF.csproj"
$launcherProject = Join-Path $projectRoot "MarketplaceLauncher\MarketplaceLauncher.csproj"
$webProjectDir = Join-Path $projectRoot "MarketplaceWeb"
$webDistDir = Join-Path $webProjectDir "dist"
$apiWwwrootDir = Join-Path $projectRoot "MarketplaceAPI\wwwroot"
$releaseRoot = Join-Path $projectRoot "ReleaseBuild"
$stagingRoot = Join-Path $projectRoot "artifacts\release-staging"
$apiOutput = Join-Path $releaseRoot "API"
$wpfOutput = Join-Path $releaseRoot "WPF"
$launcherOutput = Join-Path $releaseRoot "Launcher"
$apiStagingOutput = Join-Path $stagingRoot "API"
$wpfStagingOutput = Join-Path $stagingRoot "WPF"
$launcherStagingOutput = Join-Path $stagingRoot "Launcher"
$installerScript = Join-Path $projectRoot "installer_script.iss"

function Reset-Directory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (Test-Path $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Path $Path | Out-Null
}

function Find-InnoCompiler {
    $candidates = @(
        (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
        (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe")
    )

    foreach ($candidate in $candidates) {
        if ($candidate -and (Test-Path $candidate)) {
            return $candidate
        }
    }

    return $null
}

function Invoke-NativeCommand {
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$Command,
        [Parameter(Mandatory = $true)]
        [string]$ErrorMessage
    )

    & $Command

    if ($LASTEXITCODE -ne 0) {
        throw $ErrorMessage
    }
}

function Sync-WebStaticFiles {
    Write-Host "Building MarketplaceWeb..."
    Invoke-NativeCommand -Command {
        Push-Location $webProjectDir
        try {
            npm run build
        }
        finally {
            Pop-Location
        }
    } -ErrorMessage "MarketplaceWeb build failed."

    Write-Host "Syncing MarketplaceWeb dist into API wwwroot..."

    if (Test-Path $apiWwwrootDir) {
        Remove-Item -LiteralPath $apiWwwrootDir -Recurse -Force
    }

    New-Item -ItemType Directory -Path $apiWwwrootDir | Out-Null
    Copy-Item -Path (Join-Path $webDistDir '*') -Destination $apiWwwrootDir -Recurse -Force
}

Write-Host "Preparing staging directories..."
Reset-Directory -Path $apiStagingOutput
Reset-Directory -Path $wpfStagingOutput
Reset-Directory -Path $launcherStagingOutput

Sync-WebStaticFiles

Write-Host "Cleaning MarketplaceAPI..."
Invoke-NativeCommand -Command {
    dotnet clean $apiProject -c Release --nologo
} -ErrorMessage "MarketplaceAPI clean failed."

Write-Host "Publishing MarketplaceAPI..."
Invoke-NativeCommand -Command {
    dotnet publish $apiProject -c Release -o $apiStagingOutput --no-restore
} -ErrorMessage "MarketplaceAPI publish failed."

Write-Host "Publishing MarketplaceWPF..."
Invoke-NativeCommand -Command {
    dotnet publish $wpfProject -c Release -o $wpfStagingOutput --no-restore
} -ErrorMessage "MarketplaceWPF publish failed."

Write-Host "Publishing MarketplaceLauncher..."
Invoke-NativeCommand -Command {
    dotnet publish $launcherProject -c Release -o $launcherStagingOutput --no-restore
} -ErrorMessage "MarketplaceLauncher publish failed."

Write-Host "Updating release directories..."
Reset-Directory -Path $apiOutput
Reset-Directory -Path $wpfOutput
Reset-Directory -Path $launcherOutput
Copy-Item -Path (Join-Path $apiStagingOutput '*') -Destination $apiOutput -Recurse -Force
Copy-Item -Path (Join-Path $wpfStagingOutput '*') -Destination $wpfOutput -Recurse -Force
Copy-Item -Path (Join-Path $launcherStagingOutput '*') -Destination $launcherOutput -Recurse -Force

if (-not $SkipInstaller) {
    $innoCompiler = Find-InnoCompiler

    if (-not $innoCompiler) {
        throw "Inno Setup compiler not found. Install Inno Setup 6 or rerun with -SkipInstaller."
    }

    Write-Host "Compiling installer..."
    Invoke-NativeCommand -Command {
        & $innoCompiler $installerScript
    } -ErrorMessage "Installer compilation failed."
}

Write-Host "Release build is ready."
