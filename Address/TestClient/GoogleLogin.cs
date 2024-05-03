using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.TestClient
{
    internal static class GoogleLogin
    {
        public static async Task Login(AppSettings settings)
        {
            const string CODE_CHALLENGE_METHOD = "S256";
            string state = RandomDataBase64url(32);
            string code_verifier = RandomDataBase64url(32);
            string code_challenge = Base64urlencodeNoPadding(Sha256(code_verifier));

            string redirectURI = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            Console.WriteLine("redirect URI: " + redirectURI);

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(redirectURI);
            Console.WriteLine("Listening..");
            httpListener.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format(
                CultureInfo.InvariantCulture,
                "{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                settings.GoogleAuthorizationEndpoint,
                Uri.EscapeDataString(redirectURI),
                settings.GoogleClientId,
                state,
                code_challenge,
                CODE_CHALLENGE_METHOD);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = authorizationRequest,
                UseShellExecute = true,
                Verb = "Open"
            };
            // Opens request in the browser.
            _ = Process.Start(startInfo);

            // Waits for the OAuth authorization response.
            HttpListenerContext context = await httpListener.GetContextAsync();

            // Sends an HTTP response to the browser.
            HttpListenerResponse response = context.Response;
            string responseString = "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                httpListener.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                Console.WriteLine("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            string code = context.Request.QueryString.Get("code");
            string incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Received request with invalid state ({0})", incoming_state));
                return;
            }
            Console.WriteLine("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            await PerformCodeExchange(settings, code, code_verifier, redirectURI);
        }

        private static async Task PerformCodeExchange(AppSettings settings, string code, string code_verifier, string redirectURI)
        {
            Console.WriteLine("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestBody = string.Format(
                CultureInfo.InvariantCulture,
                "code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectURI),
                settings.GoogleClientId,
                code_verifier,
                settings.GoogleClientSecret);

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(settings.GoogleTokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, default);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    Console.WriteLine(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    AccessToken.Get.GoogleToken = tokenEndpointDecoded;

                    //string access_token = tokenEndpointDecoded["access_token"];
                    //UserinfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError
                    && ex.Response is HttpWebResponse response)
                {
                    Console.WriteLine("HTTP: " + response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // reads response body
                        string responseText = await reader.ReadToEndAsync();
                        Console.WriteLine(responseText);
                    }
                }
            }
        }

        public static int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static byte[] Sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            return SHA256.HashData(bytes);
        }

        private static string RandomDataBase64url(uint length)
        {
            byte[] bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return Base64urlencodeNoPadding(bytes);
        }

        private static string Base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", string.Empty);

            return base64;
        }
    }
}
