using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WatsonWebserver;
using WatsonWebserver.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using VDS.RDF;



namespace GraphWebsite
{
	public class WatsonWebserverProgramm
	{
		static WebserverSettings _Settings = null;
		static WebserverBase _Server = null;
        static string _Hostname = "localhost";
		private string _AUTH;
		private string _GRAPHDB_URL;
        private string _COMPONENT_DIRECTORY;
		private string htmlContent = @"
		        <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>RDF Visualization</title>
                    <style>
                        body {
                            font-family: 'Roboto', sans-serif;
                            background-color: #f4f7f9; /* Light background */
                            margin: 0;
                            padding: 0;
                            height: 100vh;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            color: black;
                        }
                        .container {
                            background: #fff;
                            border-radius: 12px;
                            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
                            width: 100%;
                            max-width: 100%;
                            height: 100%;
                            display: flex;
                            flex-direction: row;
                        }
                        .sidebar {
                            background-color: #2c3e50; /* Dark sidebar background */
                            width: 30%;
                            padding: 20px;
                            display: flex;
                            flex-direction: column;
                            color: white;
                            height: 100%;
                            overflow-y: auto; /* Allows scrolling if content overflows */
                        }
                        .query-box, .query-result {
                            background-color: #34495e; /* Slightly lighter dark background for query boxes */
                            padding: 20px;
                            border-radius: 8px;
                            margin-bottom: 20px;
                        }
                        .query-box h2, .query-result h2 {
                            font-size: 1.2em;
                            font-weight: 600;
                            margin-bottom: 15px;
                            color: white;
                        }
                        textarea {
                            width: 100%;
                            height: 150px;
                            border-radius: 8px;
                            border: none;
                            padding: 12px;
                            font-size: 14px;
                            box-sizing: border-box;
                            background-color: #f4f7f9; /* Light input background */
                            color: #2c3e50; /* Dark text color */
                            margin-bottom: 20px;
                            resize: none;
                        }
                        button {
                            background-color: #e67e22; /* Bright orange button */
                            color: white;
                            border: none;
                            padding: 14px 20px;
                            text-align: center;
                            text-decoration: none;
                            display: inline-block;
                            font-size: 16px;
                            cursor: pointer;
                            border-radius: 8px;
                            transition: background-color 0.3s, transform 0.2s;
                            width: 100%;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            margin-bottom: 10px; /* Add some space between buttons */
                        }
                        button:hover {
                            background-color: #d35400; /* Darker orange on hover */
                            transform: translateY(-3px);
                        }
                        .query-result {
                            background-color: #34495e;
                            padding: 20px;
                            border-radius: 8px;
                            max-height: 300px;
                            overflow-y: auto; /* Allows scrolling if the table is too large */
                        }
                        .visualization {
                            flex-grow: 1;
                            padding: 20px;
                            background-color: #e9f4f7; /* Light background for the visualization area */
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            position: relative;
                            height: 100%;
                        }
                        #graph {
                            width: 100%;
                            height: 100%;
                            background-color: #f6fafc; /* Match visualization area background */
                            border-radius: 12px;
                            box-shadow: inset 0 4px 10px rgba(0, 0, 0, 0.05);
                            overflow: auto;
                            padding: 30px;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            position: relative;
                        }

                        #queryResultTable {
                            font-size: 12px; /* Adjust the size as needed */
                        }     

                        #graph svg text {
                            fill: black !important;  /* Use !important to override other styles */
                        }
                        table {
                            width: 100%;
                            border-collapse: collapse;
                            margin-top: 10px;
                        }
                        th, td {
                            text-align: left;
                            padding: 12px;
                            border-bottom: 1px solid #ddd;
                            color: rgb(255, 255, 255); /* White text on dark background */
                        }
                        th {
                            background-color: #34495e;
                            font-weight: bold;
                        }

                        .checkbox-container {
                            display: flex; /* Use flexbox to arrange items side by side */
                            align-items: center; /* Center align items vertically */
                            gap: 20px; /* Add some space between the checkboxes */
                            margin-bottom: 20px; /* Add some space below the checkboxes */
                        }

                        .checkbox-label {
                            font-size: 14px; /* Make the text smaller */
                            color: white; /* Ensure text is white to match other sidebar elements */
                            display: flex;
                            align-items: center;
                        }

                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""sidebar"">
                            <div class=""query-box"">
                                <h2>SPARQL Query</h2>
                                <textarea id=""sparqlQuery"" placeholder=""Enter your SPARQL query""></textarea>
                                <button id=""executeQueryBtn"">Execute Query</button>
                            </div>
                            <div class=""query-result"">
                                <h2>Query Result</h2>
                                <div id=""queryResultTable""></div>
                                <!-- This div will display the number of results -->
                                <div id=""resultCount"" style=""margin-top: 10px; font-weight: bold;""></div>
                            </div>
                            <div class=""checkbox-container"">
                                <label class=""checkbox-label"">
                                    <input type=""checkbox"" id=""includeInferredData"" />
                                    Include Inferred Data
                                </label>
                                <label class=""checkbox-label"">
                                    <input type=""checkbox"" id=""showLiterals"" checked />
                                    Show Literal Nodes
                                </label>
                            </div>
                            <button id=""displayGraphBtn"">Display as Graph</button>
                            <button id=""exportCsvBtn"">Export as CSV</button>
                        </div>
                        <div class=""visualization"">
                            <div id=""graph""></div>
                        </div>
                    </div>

                    <script src=""https://d3js.org/d3.v6.min.js""></script>
                    <script>
                        const SERVER_URL = 'http://localhost:3001';
                        let parsedTriples = []; // Store parsed triples here

                        document.getElementById('executeQueryBtn').addEventListener('click', function() {
                            const sparqlQueryValue = document.getElementById('sparqlQuery').value;
                            const includeInferredData = document.getElementById('includeInferredData').checked;
                            if (!sparqlQueryValue.trim()) {
                                alert(""Please enter a SPARQL query."");
                                return;
                            }
                            executeSparqlQuery(sparqlQueryValue, includeInferredData);
                        });

                        document.getElementById('displayGraphBtn').addEventListener('click', function() {
                            if (parsedTriples.length > 0) {
                                const showLiterals = document.getElementById('showLiterals').checked;
                                displayGraphFromTriples(parsedTriples, showLiterals); // Pass the state of literals
                            } else {
                                alert(""Please execute a query first to get the data."");
                            }
                        });

                        document.getElementById('exportCsvBtn').addEventListener('click', function() {
                            if (parsedTriples.length > 0) {
                                exportTriplesAsCsv(parsedTriples);
                            } else {
                                alert(""Please execute a query first to get the data."");
                            }
                        });

                        function executeSparqlQuery(query, includeInferred) {
                            const encodedQuery = encodeURIComponent(query);
                            const fullUrl = `${SERVER_URL}/sparql?query=${encodedQuery}&reasoning=${includeInferred ? 'true' : 'false'}`;

                            console.log(""Executing SPARQL query:"", query);
                            console.log(""Full URL:"", fullUrl);

                            fetch(fullUrl, {
                                method: 'GET',
                                headers: {
                                    'Accept': 'application/rdf+xml'
                                }
                            })
                            .then(async response => {
                                console.log(""Response status:"", response.status);
                                console.log(""Response status text:"", response.statusText);
                                console.log(""Content-Type:"", response.headers.get(""content-type""));

                                if (!response.ok) {
                                    const errorText = await response.text();
                                    throw new Error(`SPARQL query failed: ${response.statusText} - ${errorText}`);
                                }

                                return response.text(); // Expecting RDF/XML text response
                            })
                            .then(data => {
                                parsedTriples = parseRdfXml(data);
                                displayTriplesAsTable(parsedTriples);
                            })
                            .catch(error => {
                                console.error('Error during SPARQL query execution:', error);
                                alert('An error occurred during SPARQL query execution: ' + error.message);
                            });
                        }

                        function parseRdfXml(rdfXml) {
                            const parser = new DOMParser();
                            const xmlDoc = parser.parseFromString(rdfXml, ""application/xml"");

                            const triples = [];
                            const descriptions = xmlDoc.getElementsByTagName(""rdf:Description"");

                            for (let i = 0; i < descriptions.length; i++) {
                                const subject = descriptions[i].getAttribute(""rdf:about"") ? trimUri(descriptions[i].getAttribute(""rdf:about"")) : null;
                                const predicates = descriptions[i].children;

                                for (let j = 0; j < predicates.length; j++) {
                                    const predicateNode = predicates[j];
                                    const predicate = trimUri(predicateNode.nodeName);
                                    const object = predicateNode.getAttribute(""rdf:resource"") 
                                                ? trimUri(predicateNode.getAttribute(""rdf:resource"")) 
                                                : predicateNode.textContent.trim();

                                    // Only add complete triples
                                    if (subject && predicate && object) {
                                        triples.push({ subject, predicate, object, isLiteral: !predicateNode.hasAttribute(""rdf:resource"") });
                                    } else {
                                        console.warn(`Skipping incomplete triple: Subject=${subject || 'N/A'}, Predicate=${predicate || 'N/A'}, Object=${object || 'N/A'}`);
                                    }
                                }
                            }

                            return triples;
                        }

                        function trimUri(uri) {
                            const parts = uri.split(/[#\/]/);
                            return parts.pop().trim(); // Return the last part of the URI
                        }

                        function displayTriplesAsTable(triples) {
                            // Check if triples array is valid
                            if (!triples || triples.length === 0) {
                                document.getElementById('queryResultTable').innerHTML = '<p>No results found.</p>';
                                document.getElementById('resultCount').innerText = `Number of Results: 0`;
                                return;
                            }

                            let result = '<table><tr><th>Subject</th><th>Predicate</th><th>Object</th></tr>';
                            triples.forEach(triple => {
                                result += `<tr><td>${triple.subject}</td><td>${triple.predicate}</td><td>${triple.object}</td></tr>`;
                            });
                            result += '</table>';
                            document.getElementById('queryResultTable').innerHTML = result;

                            // Display the number of results
                            document.getElementById('resultCount').innerText = `Number of Results: ${triples.length}`;

                            // Debugging: Log the number of results
                            console.log(`Number of Results: ${triples.length}`);
                        }

                        function displayGraphFromTriples(triples, showLiterals) {
                            const nodes = [];
                            const links = [];
                            const nodeSet = new Set();

                            // Identify nodes with specific predicates and color-code them
                            const predicateColors = {
                                'rdf:Property': '#d4864f',  // Example color for rdf:Property
                                'rdf:type': '#8da89d',      // Example color for rdf:type
                                'rdfs:Class': '#89a0ae'     // Example color for rdfs:Class
                            };

                            const defaultColor = '#e5c69a'; // Default color for other nodes

                            triples.forEach(({ subject, predicate, object, isLiteral }) => {
                                // Determine color based on predicate
                                const subjectColor = predicateColors[predicate] || defaultColor;
                                const objectColor = predicateColors[object] || defaultColor;

                                // Add nodes and links based on whether literals should be displayed
                                if (!nodeSet.has(subject)) {
                                    nodes.push({ id: subject, color: subjectColor, isLiteral: false });
                                    nodeSet.add(subject);
                                }

                                // Conditionally add literal nodes
                                if (!isLiteral || showLiterals) {
                                    if (!nodeSet.has(object)) {
                                        nodes.push({ id: object, color: objectColor, isLiteral: isLiteral });
                                        nodeSet.add(object);
                                    }
                                    links.push({ source: subject, target: object, predicate });
                                }
                            });

                            displayGraph({ nodes, links });
                        }

                        function displayGraph(data) {
                    const graphElement = document.getElementById('graph');
                    if (!graphElement) {
                        console.error(""Element with id 'graph' not found in the DOM."");
                        return;
                    }

                    d3.select(""#graph"").selectAll(""*"").remove(); // Clear any existing graph

                    const svg = d3.select('#graph').append('svg')
                        .attr('width', '100%')
                        .attr('height', '100%')
                        .attr('xmlns', 'http://www.w3.org/2000/svg');

                    const width = graphElement.clientWidth;
                    const height = graphElement.clientHeight;

                    const simulation = d3.forceSimulation(data.nodes)
                        .force('link', d3.forceLink(data.links)
                            .id(d => d.id)
                            .distance(50))  // Adjust distance to spread nodes appropriately
                        .force('charge', d3.forceManyBody()
                            .strength(-50))  // Adjust charge strength to avoid too much repulsion
                        .force('center', d3.forceCenter(width / 2, height / 2))
                        .force('collide', d3.forceCollide(d => getNodeSize(d)))  // Add collision force based on node size
                        .force('x', d3.forceX(width / 2).strength(0.1))  // Center nodes along x-axis
                        .force('y', d3.forceY(height / 2).strength(0.1))  // Center nodes along y-axis
                        .on('tick', ticked);  // Use 'ticked' function for updating positions

                    const link = svg.append('g')
                        .attr('stroke', '#34495e')
                        .attr('stroke-opacity', 0.6)
                        .selectAll('line')
                        .data(data.links)
                        .enter().append('line')
                        .attr('stroke-width', 1)
                        .attr('data-predicate', d => d.predicate);

                    const node = svg.append('g')
                        .attr('stroke', '#fff')
                        .attr('stroke-width', 1.5)
                        .selectAll('g')
                        .data(data.nodes)
                        .enter().append('g');

                    // Function to determine node size based on label length
                    function getNodeSize(d) {
                        const labelLength = d.id.length;
                        return Math.max(20, labelLength * 4);  // Minimum size is 20, size increases with label length
                    }

                    // Append circles for non-literal nodes with dynamic size
                    node.filter(d => !d.isLiteral).append('circle')
                        .attr('r', d => getNodeSize(d) / 2)  // Radius based on node size
                        .attr('fill', d => d.color)  // Use custom color from the node data
                        .attr('stroke', '#fff')
                        .attr('stroke-width', 1.5);

                    // Append rectangles for literal nodes with dynamic size
                    node.filter(d => d.isLiteral).append('rect')
                        .attr('width', d => getNodeSize(d))
                        .attr('height', d => getNodeSize(d) / 2)
                        .attr('x', d => -getNodeSize(d) / 2)
                        .attr('y', d => -getNodeSize(d) / 4)
                        .attr('fill', '#f39c12')
                        .attr('stroke', '#fff')
                        .attr('stroke-width', 1.5);

                    // Append text labels for nodes
                    node.append('text')
                        .attr('dy', 4)  // Adjust vertical alignment
                        .attr('x', 0)  // Center text horizontally
                        .attr('font-size', '9px')
                        .style('fill', 'white')  // Set text color to white for visibility on dark nodes
                        .style('pointer-events', 'none')
                        .attr('text-anchor', 'middle')  // Center text inside node
                        .text(d => d.id);

                    node.call(d3.drag()
                        .on('start', dragStarted)
                        .on('drag', dragged)
                        .on('end', dragEnded));

                    function ticked() {
                        link
                            .attr('x1', d => d.source.x = Math.max(getNodeSize(d.source) / 2, Math.min(width - getNodeSize(d.source) / 2, d.source.x)))
                            .attr('y1', d => d.source.y = Math.max(getNodeSize(d.source) / 2, Math.min(height - getNodeSize(d.source) / 2, d.source.y)))
                            .attr('x2', d => d.target.x = Math.max(getNodeSize(d.target) / 2, Math.min(width - getNodeSize(d.target) / 2, d.target.x)))
                            .attr('y2', d => d.target.y = Math.max(getNodeSize(d.target) / 2, Math.min(height - getNodeSize(d.target) / 2, d.target.y)));

                        node
                            .attr('transform', d => `translate(${Math.max(getNodeSize(d) / 2, Math.min(width - getNodeSize(d) / 2, d.x))},${Math.max(getNodeSize(d) / 2, Math.min(height - getNodeSize(d) / 2, d.y))})`);
                    }

                    function dragStarted(event, d) {
                        if (!event.active) simulation.alphaTarget(0.3).restart();
                        d.fx = d.x;
                        d.fy = d.y;
                    }

                    function dragged(event, d) {
                        d.fx = event.x;
                        d.fy = event.y;
                    }

                    function dragEnded(event, d) {
                        if (!event.active) simulation.alphaTarget(0);
                        d.fx = null;
                        d.fy = null;
                    }
                }



                        function exportTriplesAsCsv(triples) {
                            const csvContent = ""data:text/csv;charset=utf-8,"" 
                                + ""Subject,Predicate,Object\n"" 
                                + triples.map(triple => `${triple.subject},${triple.predicate},${triple.object}`).join(""\n"");

                            const encodedUri = encodeURI(csvContent);
                            const link = document.createElement(""a"");
                            link.setAttribute(""href"", encodedUri);
                            link.setAttribute(""download"", ""query_results.csv"");
                            document.body.appendChild(link); // Required for Firefox
                            link.click();
                            document.body.removeChild(link); // Clean up the DOM after the download
                        }
                    </script>
                </body>
                </html>
		              ";

		private static List<Triple> _ParsedTriples = new List<Triple>();


		public void StartServer(int port, string component_directory, string graph_db_url, string userAcc, string userPwd)
		{
            _COMPONENT_DIRECTORY = component_directory;
			_GRAPHDB_URL = graph_db_url;
			_AUTH = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userAcc}:{userPwd}"));

			_Settings = new WebserverSettings
			{
				Hostname = _Hostname,
				Port = port
			};

            // Initialize server
			_Server = new WatsonWebserver.Webserver(_Settings, DefaultRoute);

            // Set default permit
			_Server.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
			//_Server.Settings.AccessControl.DenyList.Add("1.1.1.1", "255.255.255.255

			// Enable CORS
			_Server.Routes.Preflight += CorsPreflight;

            // Ensure the upload directory exists
            if (!Directory.Exists(_COMPONENT_DIRECTORY))
            {
				_Server.Routes.PreAuthentication.Dynamic.Add(WatsonWebserver.Core.HttpMethod.GET, new Regex("/"), (ctx) =>
				{
					ctx.Response.StatusCode = 404;
					return ctx.Response.Send("_COMPONENT_DIRECTORY not found");
				});
			};

			_Server.Start();
		}

		public void StopServer()
		{
			if (_Server != null)
			{
				_Server.Dispose();
				_Server = null;
			}
		}

		// Handle CORS requests
		private async Task CorsPreflight(HttpContextBase ctx)
		{
			ctx.Response.Headers.Add("Access-Control-Allow-Origin", $"{_GRAPHDB_URL}"); // Adjust as needed
			ctx.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
			ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Content-Type");
			ctx.Response.StatusCode = 204; // No content for preflight requests
			await ctx.Response.Send();
		}


		// Default route
		private async Task DefaultRoute(HttpContextBase ctx)
		{
			FileStream fs = null;
			string filePath;

			ctx.Response.ContentType = "text/html; charset=utf-8";
			await ctx.Response.Send(htmlContent);

			switch (ctx.Request.Method)
			{
				case WatsonWebserver.Core.HttpMethod.GET:
					// Handle GET requests to serve files dynamically
					if (ctx.Request.Url.Elements != null && ctx.Request.Url.Elements.Length > 0)
					{
						// Sanitize the file name to prevent directory traversal attacks
						string fileName = Path.GetFileName(ctx.Request.Url.Elements[0]);
						filePath = Path.Combine(_COMPONENT_DIRECTORY, fileName);

						// Check if the file exists
						if (File.Exists(filePath))
						{
							long len = new FileInfo(filePath).Length;
							using (fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
							{
								ctx.Response.StatusCode = 200;
								await ctx.Response.Send(len, fs);  // Send the file content
								return;
							}
						}
						else
						{
							// File not found
							ctx.Response.StatusCode = 404;
							await ctx.Response.Send("File not found");
							return;
						}
					}
					else
					{
						// Bad request if no file name is provided
						ctx.Response.StatusCode = 400;
						await ctx.Response.Send("Bad request - Missing file name");
						return;
					}

				case WatsonWebserver.Core.HttpMethod.POST:
					// Handle POST requests to upload files
					if (ctx.Request.Url.Elements == null || ctx.Request.Url.Elements.Length != 1)
					{
						ctx.Response.StatusCode = 400;
						await ctx.Response.Send("Bad request - Invalid URL");
						return;
					}
					else if (ctx.Request.Data == null || !ctx.Request.Data.CanRead)
					{
						ctx.Response.StatusCode = 400;
						await ctx.Response.Send("Bad request - No data provided");
						return;
					}
					else
					{
						// Get the file name from the URL and sanitize it
						string fileName = Path.GetFileName(ctx.Request.Url.Elements[0]);
						filePath = Path.Combine(_COMPONENT_DIRECTORY, fileName);

						// Save the uploaded file
						using (fs = new FileStream(filePath, FileMode.Create))  // Overwrite if file exists
						{
							int bytesRead = 0;
							byte[] buffer = new byte[2048];
							while ((bytesRead = ctx.Request.Data.Read(buffer, 0, buffer.Length)) > 0)
							{
								fs.Write(buffer, 0, bytesRead);
							}
						}

						// Respond with '201 Created' status
						ctx.Response.StatusCode = 201;
						await ctx.Response.Send("File uploaded successfully");
						return;
					}

				default:
					// Respond with 400 Bad Request for unsupported methods
					ctx.Response.StatusCode = 400;
					await ctx.Response.Send("Bad request - Unsupported method");
					return;
			}
		}

		//// Route to handle RDF file upload and GraphDB interaction
		//static async Task UploadRoute(HttpContext ctx)
		//{
		//	if (!ctx.Request.Data.Equals("rdfFile"))
		//	{    
		//		ctx.Response.StatusCode = 400;
		//		await ctx.Response.Send("No file uploaded.");
		//		return;
		//	}

		//	var uploadedFile = ctx.Request.Data["rdfFile"];
  //          ctx.Request.Url.Elements.Equals("rdfFile");
		//	string contentType = "text/turtle";  // Assuming Turtle format (change if needed)

		//	try
		//	{
		//		Console.WriteLine("Uploading file to GraphDB...");

		//		// Read the file content
		//		string fileContent;
		//		using (var reader = new StreamReader(uploadedFile.Data))
		//		{
		//			fileContent = await reader.ReadToEndAsync();
		//		}

		//		Console.WriteLine("File content preview: " + fileContent.Substring(0, Math.Min(fileContent.Length, 500)));

		//		// Send file content to GraphDB
		//		var result = await PostToGraphDB(fileContent, contentType);
		//		Console.WriteLine("Received response from GraphDB: " + result.StatusCode);

		//		if (result.IsSuccessStatusCode)
		//		{
		//			await ctx.Response.Send("File uploaded successfully to GraphDB.");
		//		}
		//		else
		//		{
		//			string responseBody = await result.Content.ReadAsStringAsync();
		//			Console.WriteLine("Failed to upload: " + responseBody);
		//			await ctx.Response.Send($"Failed to upload file to GraphDB. Server said: {responseBody}", (int)result.StatusCode);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine("Error during file upload: " + ex.Message);
		//		await ctx.Response.Send("Server error during file upload.", 500);
		//	}
		//}

		//// Function to send RDF file to GraphDB
		//static async Task<HttpResponseMessage> PostToGraphDB(string content, string contentType)
		//{
		//	using (var client = new HttpClient())
		//	{
		//		client.DefaultRequestHeaders.Add("Authorization", "Basic " + AUTH);
		//		HttpContent httpContent = new StringContent(content, Encoding.UTF8, contentType);

		//		return await client.PostAsync($"{GRAPHDB_URL}/statements", httpContent);
		//	}
		//}

  
    }
}
