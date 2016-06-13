# Requires psake to run, see README.md for more details.

if ($PSVersionTable.PSVersion.Major -lt 3)
{
    throw "PowerShell 3 is required.`nhttp://www.microsoft.com/en-us/download/details.aspx?id=40855"
}

Import-Module .\scripts\Saritasa.Psake.psd1
Import-Module .\scripts\Saritasa.Build.psd1
Import-Module .\scripts\Saritasa.Test.psd1
Register-HelpTask

Properties `
{
    $version = '0.1.0'
    $signKey = './saritasa-tools.snk'
    $nuspecFile = './build/Saritasa.Tools.nuspec'
    $libDirectory = './build/lib'
}

$builds = @(
    @{Id = 'v4.5'; Framework = 'net452'; symbol = 'NET4_5'}
    @{Id = 'v4.6'; Framework = 'net461'; symbol = 'NET4_6'}
)
$packages = @(
    'Saritasa.Tools'
    'Saritasa.Tools.Ef6'
    'Saritasa.Tools.Mvc5'
    'Saritasa.Tools.NLog4'
)

function Get-PackageName([string]$package)
{
    If ([System.Char]::IsDigit($package[$package.Length-1])) {$package.Substring(0, $package.Length-1)} Else {$package}
}

Task pack -description 'Build the library, test it and prepare nuget packages' `
{
    # nuget restore
    Invoke-NugetRestore './src/Saritasa.Tools.sln'

    # update version
    Update-AssemblyInfoFiles $version

    # build all versions, sign, test and prepare package directory
    foreach ($package in $packages)
    {
        Get-PackageName($package)
        Remove-Item $libDirectory -Recurse -Force -ErrorAction SilentlyContinue
        New-Item $libDirectory -ItemType Directory
        foreach ($build in $builds)
        {
            # build & sign
            $id = $build.Id
            $symbol = $build.symbol
            $args = @("/p:TargetFrameworkVersion=$id", "/p:DefineConstants=$symbol")
            if (Test-Path $signKey)
            {
                $args += "/p:AssemblyOriginatorKeyFile=$signKey" + '/p:SignAssembly=true'
            }
            Invoke-ProjectBuild -ProjectPath "./src/$package/$package.csproj" -Configuration 'Release' -BuildParams $args

            # copy
            New-Item (Join-Path $libDirectory $build.Framework) -ItemType Directory
            $packageFile = Get-PackageName $package
            Copy-Item "./src/$package/bin/Release/$packageFile.dll" (Join-Path $libDirectory $build.Framework)
            Copy-Item "./src/$package/bin/Release/$packageFile.XML" (Join-Path $libDirectory $build.Framework)
        }

        # pack, we already have nuget in current folder
        $nugetExePath = "$PSScriptRoot\scripts\nuget.exe"
        $buildDirectory = (Get-Item $libDirectory).Parent.FullName
        &"$nugetExePath" @('pack', (Join-Path $buildDirectory "$package.nuspec"), '-Version', $version, '-NonInteractive', '-Exclude', '*.snk')
        if ($LASTEXITCODE)
        {
            throw 'Nuget pack failed.'
        }
    }

    # little clean up
    Remove-Item $libDirectory -Recurse -Force -ErrorAction SilentlyContinue

    # build docs
    Compile-Docs
}

Task clean -description 'Clean solution' `
{
    Remove-Item $libDirectory -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item './Saritasa.*.nupkg' -ErrorAction SilentlyContinue
    Remove-Item './src/*.suo' -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item './src/Saritasa.Tools/bin' -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item './src/Saritasa.Tools/obj' -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item './src/StyleCop.Cache' -Force -ErrorAction SilentlyContinue
    Remove-Item './docs/_build' -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item './docs/conf.py' -ErrorAction SilentlyContinue
    Remove-Item './scripts/nuget.exe' -ErrorAction SilentlyContinue
}

Task docs -description 'Compile and open documentation' `
{
    Compile-Docs
    Invoke-Item './docs/_build/html/index.html'
}

function Compile-Docs
{
    Set-Location '.\docs'
    Copy-Item '.\conf.py.template' '.\conf.py'
    (Get-Content '.\conf.py').replace('VX.VY', $version) | Set-Content '.\conf.py'
    &'.\make.cmd' @('html')
    if ($LASTEXITCODE)
    {
        throw 'Cannot compile documentation.'
    }
    Set-Location '..'
}