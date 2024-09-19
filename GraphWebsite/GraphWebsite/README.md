# RDF Visualization

This project provides a web-based interface for visualizing RDF data and executing SPARQL queries. It includes a front-end web application and a back-end server that interacts with a GraphDB instance.

## Table of Contents

1. [Features](#features)
2. [Technologies Used](#technologies-used)
3. [Installation](#installation)
4. [Usage](#usage)
5. [API Endpoints](#api-endpoints)
6. [Contributing](#contributing)
7. [License](#license)

## Features

- **SPARQL Query Execution**: Execute SPARQL queries against a GraphDB repository.
- **RDF Visualization**: Visualize RDF data as a graph using D3.js.
- **CSV Export**: Export query results as CSV files.
- **Inference Toggle**: User can select if they want to include inferences in their results or not.
- **Include literals Toggle**: User can select if they want to display the literals node or not in the graph.
- **Currently only CONSTRUCT queries are accepted.**

## Technologies Used

- **Front-End**: HTML, CSS, JavaScript, D3.js
- **Back-End**: Node.js, Express.js
- **Database**: GraphDB (RDF store)

## Installation

### Prerequisites

Ensure you have the following installed on your machine:

- [Node.js](https://nodejs.org/en/) (v12.x or higher)
- [npm](https://www.npmjs.com/) (Node Package Manager)
- [GraphDB](https://www.ontotext.com/products/graphdb/) instance running locally or accessible over the network

### Steps

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/rdf-visualization.git
   cd rdf-visualization
   
2. **Install dependencies:**

   ```bash
   npm install
3. **Set up GraphDB:**
     - Ensure your GraphDB instance is running.
     - Update the GRAPHDB_URL in the server file (server.js) to match your GraphDB endpoint.
4. **Run the Server:**

   ```bash
   node server2.js 
5. **Open the front-end application:**
   Open index.html in your web browser, or if you're using a live server, ensure it's serving on http://localhost:8080.
   
## Usage

### Running SPARQL Queries

1. Open the front-end application.
2. Enter your SPARQL query in the provided text area.
3. Click the **Execute Query** button to run the query against the GraphDB instance.
4. View the results in the "Query Result" section.

### Visualizing RDF Data

1. After executing a SPARQL query, click the **Display as Graph** button to visualize the RDF triples as a graph.
2. Use the checkboxes to include inferred data or show literal nodes.

### Exporting Query Results

- Click the **Export as CSV** button to download the query results in CSV format.

### Uploading RDF Files
Upload logic exixts in the server2.js file, if you want you can add an upload section in the webpage and connect with it.

## API Endpoints

- **GET /**: Server status check.
- **POST /upload**: Upload RDF files to GraphDB.
  - **Request**: Multipart/form-data with the RDF file.
- **GET /sparql**: Execute SPARQL queries.
  - **Query Parameters**:
    - `query`: The SPARQL query to execute.
    - `reasoning`: Boolean (`true` or `false`) to include inferred data.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request for any changes you'd like to make.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

