# Define the base directory as the user's home directory
$baseDirectory = [System.Environment]::GetFolderPath('UserProfile')

# Construct the full path to the target directory
$relativePath = "AppData\Roaming\Grasshopper\Libraries\GraphWebsite"
$workingDirectory = Join-Path -Path $baseDirectory -ChildPath $relativePath

# Install fnm (Fast Node Manager)
Write-Output "Installing fnm (Fast Node Manager)..."
winget install Schniz.fnm

# Configure fnm environment
Write-Output "Configuring fnm environment..."
fnm env --use-on-cd | Out-String | Invoke-Expression

# Download and install Node.js
Write-Output "Downloading and installing Node.js..."
fnm use --install-if-missing 22

# Verify the right Node.js version is in the environment
$nodeVersion = & node -v
Write-Output "Node.js version: $nodeVersion"

# Verify the right npm version is in the environment
$npmVersion = & npm -v
Write-Output "npm version: $npmVersion"

# Check if npm is installed
if (-not $npmVersion) {
    Write-Output "npm is not installed. Installing npm..."
    # Use fnm to install the correct npm version for the Node.js version
    fnm use --install-if-missing 22
    $npmVersion = & npm -v
    Write-Output "npm version after installation: $npmVersion"
} else {
    Write-Output "npm is already installed."
}

# Navigate to the directory containing package.json
Write-Output "Navigating to the directory with package.json..."
Set-Location -Path $workingDirectory

# Check if package.json exists
if (Test-Path "package.json") {
    Write-Output "package.json found. Installing dependencies..."
    # Install dependencies
    npm install

    # Verify node_modules directory creation
    if (Test-Path "node_modules") {
        Write-Output "node_modules directory created successfully."
    } else {
        Write-Output "Failed to create node_modules directory."
    }

    # Optionally start the server
    Write-Output "Starting the server..."
    npm start
} else {
    Write-Output "package.json not found in the directory."
}
