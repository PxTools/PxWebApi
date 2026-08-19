param(
    [Parameter(Mandatory=$true)]
    [string]$WebRoot
)

Import-Module WebAdministration

# Find IIS site and application for the given path
$found = $null
foreach ($site in Get-ChildItem IIS:\Sites) {
    # Resolve environment variables and normalize case
    $sitePath = [Environment]::ExpandEnvironmentVariables($site.PhysicalPath).ToLowerInvariant()
    $inputPath = $WebRoot.ToLowerInvariant()
    Write-Host "Checking site: $($site.Name) with path $sitePath"
    if ($sitePath -eq $inputPath) {
        $found = @{
            Site = $site
            App = $null
            AppPool = $site.applicationPool
        }
        break
    }
    # Check applications under site
    foreach ($app in Get-ChildItem "IIS:\Sites\$($site.Name)") {
        $appPath = [Environment]::ExpandEnvironmentVariables($app.PhysicalPath).ToLowerInvariant()
        if ($appPath -eq $inputPath) {
            $found = @{
                Site = $site
                App = $app
                AppPool = $app.applicationPool
            }
            break
        }
    }
    if ($found) { break }
}

if (-not $found) {
    Write-Host "No IIS site or application found with physical path $WebRoot"
    exit 1
}

$siteName = $found.Site.Name
$appName = if ($found.App) { $found.App.Path } else { "" }
$appPoolName = $found.AppPool
$appPoolUser = "IIS APPPOOL\$appPoolName"

# Get binding info (use first http binding from site)
$binding = $found.Site.Bindings.Collection | Where-Object { $_.protocol -eq "http" } | Select-Object -First 1
if (-not $binding) {
    Write-Host "No HTTP binding found for site $siteName"
    exit 1
}
$hostx = if ($binding.Host) { $binding.Host } else { "localhost" }

# Add application path to baseUrl if not root
if ($appName -and $appName -ne "/") {
    $baseUrl = "http://$($hostx)$($appName)"
} else {
    $baseUrl = "http://$hostx"
}

Write-Host "Site: $siteName"
Write-Host "Application: ${(}[string]::IsNullOrEmpty($appName) ? '(root)' : $appName}"
Write-Host "AppPool: $appPoolName"
Write-Host "AppPoolUser: $appPoolUser"
Write-Host "BaseUrl: $baseUrl"

# Create required folders
$folders = @("$WebRoot\wwwroot\ControllerStates", "$WebRoot\wwwroot\sq", "$WebRoot\wwwroot\Database")
foreach ($folder in $folders) {
    if (-not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
    }
}

# Set permissions
foreach ($folder in $folders) {
    $acl = Get-Acl $folder
    $permission = "$appPoolUser","Modify","Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
    $acl.SetAccessRule($accessRule)
    Set-Acl $folder $acl
}

Write-Host "Folders created and permissions set."
Write-Host ""
Write-Host "Please copy your database files to:"
Write-Host "  $WebRoot\wwwroot\Database"
Write-Host ""
Write-Host "When you have finished copying your files, press Enter to continue..."
Read-Host

# Optional: Download tiny demo database
# $demoDbUrl = "https://github.com/PxTools/PxWebApi/raw/refs/heads/main/docker/pxwebapi/Database/tinydatabase.zip"
# $demoDbZip = "$WebRoot\Database\tinydatabase.zip"
# Invoke-WebRequest -Uri $demoDbUrl -OutFile $demoDbZip
# Expand-Archive -Path $demoDbZip -DestinationPath "$WebRoot\Database"
# Remove-Item $demoDbZip

# Generate Menu.xml
Invoke-WebRequest -Uri "$baseUrl/api/v2/admin/database" -Method Put -Headers @{'API_ADMIN_KEY' = 'test'}
Write-Host "Menu.xml generation started. Waiting for completion..."

# Wait for Menu.xml to be finished
do {
    Start-Sleep -Seconds 5
    $menuStatus = Invoke-WebRequest -Uri "$baseUrl/api/v2/admin/database" -Method Get -Headers @{'API_ADMIN_KEY' = 'test'} | Select-Object -Expand Content | ConvertFrom-Json
    Write-Host "Menu.xml status: $($menuStatus.State)"
} while ($menuStatus.State -ne "finished")

Write-Host "Menu.xml generation finished."

# Generate search index
Invoke-WebRequest -Uri "$baseUrl/api/v2/admin/searchindex" -Method POST -Headers @{'API_ADMIN_KEY' = 'test'}
Write-Host "Search index generation started. Waiting for completion..."

# Wait for search index to be finished
do {
    Start-Sleep -Seconds 5
    $searchStatus = Invoke-WebRequest -Uri "$baseUrl/api/v2/admin/searchindex" -Method Get -Headers @{'API_ADMIN_KEY' = 'test'} | Select-Object -Expand Content | ConvertFrom-Json
    Write-Host "Search index status: $($searchStatus.State)"
} while ($searchStatus.State -ne "finished")

Write-Host "Search index generation finished."

# Verify API is running
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v2/tables" -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "PxWebApi installation complete. API is running."
    } else {
        Write-Host "API responded with status code: $($response.StatusCode)"
    }
} catch {
    Write-Host "Could not verify API. Please check IIS and application logs."
}