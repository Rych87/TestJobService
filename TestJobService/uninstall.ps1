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