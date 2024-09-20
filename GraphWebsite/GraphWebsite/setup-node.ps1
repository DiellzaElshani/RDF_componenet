# Define the base directory as the user's home directory
$baseDirectory = [System.Environment]::GetFolderPath('UserProfile')

# Construct the full path to the target directory
$relativePath = "AppData\Roaming\Grasshopper\Libraries\GraphWebsite"
$workingDirectory = Join-Path -Path $baseDirectory -ChildPath $relativePath

# Step 1: Install fnm (Fast Node Manager) if not installed
Write-Output "Installing fnm (Fast Node Manager)..."
winget install Schniz.fnm

# Step 2: Configure fnm environment
Write-Output "Configuring fnm environment..."
fnm env --use-on-cd | Out-String | Invoke-Expression

# Step 3: Install Node.js (version 22 or specify the version you need)
Write-Output "Downloading and installing Node.js..."
fnm use --install-if-missing 22

# Step 4: Verify the Node.js and npm versions
$nodeVersion = & node -v
$npmVersion = & npm -v
Write-Output "Node.js version: $nodeVersion"
Write-Output "npm version: $npmVersion"

# Step 5: Navigate to the directory containing the package.json
Write-Output "Navigating to the directory with package.json..."
Set-Location -Path $workingDirectory

# Step 6: Check if package.json exists and install dependencies
if (Test-Path "package.json") {
    Write-Output "package.json found. Installing dependencies..."

    # Install dependencies (such as express)
    npm install

    # Verify if node_modules is created
    if (Test-Path "node_modules") {
        Write-Output "Dependencies installed successfully."

        # Step 7: Start the web server using Node.js
        Write-Output "Starting the server..."
        npm start
    } else {
        Write-Output "Failed to install dependencies. 'node_modules' directory not found."
    }
} else {
    Write-Output "Error: package.json not found. Please ensure the file exists in $workingDirectory."
}
