$distPath = "dist/GameData/KwarterMaster"
$dllPath = "Plugins\net472\KwarterMaster.dll"
$iconPath = "Textures\KwarterMaster.png"


# Get the current working directory
$projectPath = Get-Location

# Navigate to the project directory
Write-Host "Navigating to project directory: $projectPath"
Set-Location -Path $projectPath

# Run the dotnet build command and capture the output
Write-Host "Building the project..."
$buildOutput = dotnet build .\KwarterMaster.csproj 2>&1

# Display the build output
Write-Host "Build Output:"
Write-Host $buildOutput

# Check if the build was successful
if ($buildOutput -match "Build succeeded.") {
	Write-Host "Build succeeded. Packaging the project..."

	# delete dist/ folder if exists
	if (Test-Path -Path "dist") {
		Remove-Item -Path "dist" -Recurse -Force
	}

	# create dist/ folder
	New-Item -ItemType Directory -Path $distPath/Plugins -Force
	New-Item -ItemType Directory -Path $distPath/Textures -Force

	# copy the built .dll file to the dist/ folder
	Copy-Item -Path $dllPath -Destination $distPath/Plugins

	# copy the icon file to the dist/ folder
	Copy-Item -Path $iconPath -Destination $distPath/Textures
} else {
	Write-Host "Build failed. Please check the error messages above."
}

# Navigate back to the original directory
Set-Location -Path $pwd