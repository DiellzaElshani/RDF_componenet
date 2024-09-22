# from http.server import BaseHTTPRequestHandler, HTTPServer
# import urllib.parse

# ### Running ###
# # python post_server.py

# class PostHandler(BaseHTTPRequestHandler):
#     def do_POST(self):
#         content_length = int(self.headers['Content-Length'])
#         post_data = self.rfile.read(content_length)
        
#         # Handle the POST data here
#         print("Received POST data:", post_data.decode())

#         self.send_response(200)
#         self.send_header('Content-type', 'text/plain')
#         self.end_headers()
#         self.wfile.write(b'POST request received')

# def run(server_class=HTTPServer, handler_class=PostHandler, port=5000):
#     server_address = ('', port)
#     httpd = server_class(server_address, handler_class)
#     print(f'Starting httpd server on port {port}...')
#     httpd.serve_forever()

# if __name__ == "__main__":
#     run()

from http.server import BaseHTTPRequestHandler, HTTPServer
import urllib.parse

class PostHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        try:
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            
            # Handle the POST data here
            print("Received POST data:", post_data.decode())
            
            self.send_response(200)
            self.send_header('Content-type', 'text/plain')
            self.end_headers()
            self.wfile.write(b'POST request received')

        except Exception as e:
            print(f"Error: {e}")
            self.send_response(500)
            self.send_header('Content-type', 'text/plain')
            self.end_headers()
            self.wfile.write(b'Internal Server Error')

def run(server_class=HTTPServer, handler_class=PostHandler, port=5000):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print(f'Starting httpd server on port {port}...')
    httpd.serve_forever()

if __name__ == "__main__":
    run()
