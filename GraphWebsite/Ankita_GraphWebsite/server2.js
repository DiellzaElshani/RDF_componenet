const express = require('express');
const multer = require('multer');
const fetch = require('node-fetch');
const fs = require('fs');
const cors = require('cors');

const app = express();
app.use(express.json());
app.use(cors({
    origin: 'http://localhost:8080' // Adjust as needed for your front-end origin
}));

const upload = multer({ dest: 'uploads/' }); // Temporarily save files here

const GRAPHDB_URL = 'http://localhost:7200/repositories/BotOntology'; // Replace with your actual repository ID
const AUTH = 'Basic ' + Buffer.from('admin:admin').toString('base64');

app.get('/', (req, res) => {
    res.send('Server is up and running!');
});

app.post('/upload', upload.single('rdfFile'), async (req, res) => {
    if (!req.file) {
        return res.status(400).send('No file uploaded.');
    }

    const contentType = 'text/turtle'; // Ensure this matches the type of RDF data you're uploading

    try {
        console.log('Uploading file to GraphDB...');
        const fileContent = fs.readFileSync(req.file.path, 'utf8');
        console.log('File content preview:', fileContent.substring(0, 500));
        
        const response = await fetch(`${GRAPHDB_URL}/statements`, {
            method: 'POST',
            headers: {
                'Authorization': AUTH,
                'Content-Type': contentType
            },
            body: fileContent,
        });

        console.log('Received response from GraphDB:', response.status, response.statusText);
        
        if (response.ok) {
            res.send('File uploaded successfully to GraphDB.');
        } else {
            const responseBody = await response.text();
            console.error('Failed to upload file to GraphDB. Server response:', responseBody);
            res.status(response.status).send(`Failed to upload file to GraphDB. Server said: ${responseBody}`);
        }
    } catch (error) {
        console.error('Error during file upload:', error.message);
        res.status(500).send('Server error during file upload.');
    } finally {
        fs.unlink(req.file.path, err => {
            if (err) console.error('Error removing uploaded file:', err);
        });
    }
});

// Handle SPARQL query execution
app.get('/sparql', async (req, res) => {
    const sparqlQuery = req.query.query;
    const includeInferredData = req.query.reasoning === 'true'; // Read the reasoning parameter

    if (!sparqlQuery) {
        return res.status(400).send('No SPARQL query provided.');
    }

    try {
        console.log('Executing SPARQL query:', sparqlQuery);
        
        // Adjust the URL based on reasoning
        let fullUrl = `${GRAPHDB_URL}?query=${encodeURIComponent(sparqlQuery)}`;

        if (includeInferredData) {
            fullUrl += '&infer=true'; // Enable reasoning/inference
        } else {
            fullUrl += '&infer=false'; // Disable reasoning/inference
        }

        // Use 'application/rdf+xml' for all queries
        const acceptHeader = 'application/rdf+xml';

        const graphDbResponse = await fetch(fullUrl, {
            method: 'GET',
            headers: {
                'Authorization': AUTH,
                'Accept': acceptHeader
            }
        });

        console.log('Received response from GraphDB for SPARQL query:', graphDbResponse.status, graphDbResponse.statusText);

        if (graphDbResponse.ok) {
            const data = await graphDbResponse.text(); // Get the response as text (RDF/XML)
            res.header('Content-Type', `${acceptHeader};charset=utf-8`);
            res.send(data);
        } else {
            const errorText = await graphDbResponse.text();
            res.status(graphDbResponse.status).send(`SPARQL query failed: ${graphDbResponse.statusText} - ${errorText}`);
        }

    } catch (error) {
        console.error('Error during SPARQL query execution:', error.message);
        res.status(500).send(error.message);
    }
});

const PORT = 3001;
app.listen(PORT, () => {
    console.log(`Server running on http://localhost:${PORT}`);
});
