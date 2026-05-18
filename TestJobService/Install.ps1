$exePath = Join-Path $PSScriptRoot "SysMonAgent.exe"
$displayServiceName = "System Monitor Agent"
$serviceName = "SysMonAgent"

$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($service) {
    Write-Host "Service exists. Stopping..."

    if ($service.Status -ne "Stopped") {
        Stop-Service $serviceName -Force
    }

    Write-Host "Removing service..."
    sc.exe delete $serviceName
    Start-Sleep -Seconds 2
}

sc.exe create SysMonAgent binPath= $exePath DisplayName= $displayServiceName start= auto
if (-not [System.Diagnostics.EventLog]::SourceExists($serviceName)) {
    New-EventLog -LogName Application -Source $serviceName
}
sc.exe start $serviceName