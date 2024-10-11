# GraphWebsite 
C# Grasshopper Plugin

## Overview

**GraphWebsite** is a Grasshopper plugin that launches a local web server to query and visualize graph-data like RDF (Resource Description Framework) in a web browser.
It connects to a predefinded host server (most-likely GraphDB server) where the graph data is stored.

This plugin leverages an Eto Forms interface for user credentials input 
and employs the Watson Webserver to host the local GraphWebsite on `http://localhost:9000`. 
The server can be started or stopped directly from the Grasshopper component.

## Features

- User-friendly credential input with Eto Forms.
- Start a local website to query and visualize GraphDB data.
- Grasshopper inputs to control the process dynamically.
- Caching mechanism to avoid redundant recomputation of inputs.

## Requirements

- Rhino 7 or Rhino 8 with Grasshopper installed.
- Visual Studio for compiling the code.
- Watson Webserver + Eto Forms NuGet package for handling the web server.

### Optional

- A GraphDB instance with proper credentials (server address, username, password). 

## Installation

1. Clone the repository or download the source code.
2. Open the solution in Visual Studio and restore the NuGet packages (including Watson Webserver).
3. Build the project and place the resulting `.gha` file in your Grasshopper `Components` folder (`C:\Users\{YourUsername}\AppData\Roaming\Grasshopper\Libraries`).
4. Launch Grasshopper in Rhino, and the `GraphWebsite` component will appear under the `BHoM -> RDF` tab.

## Usage

### Component Inputs

1. **Credentials (Boolean)**: Set to `true` to open the credentials input window (Eto Forms) where you can provide 
the server address, username, and password for your GraphDB repository. The username and password are optional.
   
2. **RunGraphWebsite (Boolean)**: Set to `true` to start the Watson Webserver, hosting the local GraphWebsite on `http://localhost:9000`. 
Ensure that credentials are provided before running the server.

### Component Outputs

- Currently, the component does not have specific outputs but will generate runtime messages to inform you of the server's status (started/stopped, errors, etc.).

### Workflow

1. Place the `GraphWebsite` component on your Grasshopper canvas.
2. Set `Credentials` to `true` to open the credential dialog.
3. Enter your `server address` (**mandatory**), `Username` and `Password` (**optional**) in the pop-up window.
4. Set `RunGraphWebsite` to `true` to start the web server, which will automatically open the website in your default web browser at `http://localhost:9000`.
5. Stop the server by setting `RunGraphWebsite` to `false`.

### Caching Mechanism

To prevent unnecessary recomputation of inputs, 
the component uses an internal caching system. When the `Credentials` input value remains unchanged, 
the previous credentials are reused, and the component avoids recomputing the same operations.

## Icon and UI

The component uses a custom 24x24 pixel icon, 
located in the `GraphWebsite.Properties.Resources` as `GraphWebsite_icon_small`. 
This icon appears in the Grasshopper user interface next to the component name.

## Error Handling

If the server address is left empty when trying to run the server, an error message will be displayed. 
Likewise, if any issues occur during the starting or stopping of the server, 
detailed error messages will be logged in Grasshopper's runtime messages.

## Known Issues

- The component does not yet handle situations where the server is already running in the background. 
Restarting the server multiple times might require a manual reset.
- Ensure that the server address is correctly entered, as it's a mandatory input for starting the server.

## License

_

## Outlook

Uploading RDF data directly from the webbrowser or as input in the grasshopper component to the host server repository. 

---

Enjoy using GraphWebsite! Feel free to contribute by submitting issues or pull requests.
