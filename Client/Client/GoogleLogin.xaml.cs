using Autofac;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BrassLoon.Client
{
    /// <summary>
    /// Interaction logic for GoogleLogin.xaml
    /// </summary>
    public partial class GoogleLogin : Window
    {
        public GoogleLogin()
        {
            InitializeComponent();
        }

        public static int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            AppSettings appSettings = scope.Resolve<AppSettings>();

            const string CODE_CHALLENGE_METHOD = "S256";
            string state = RandomDataBase64url(32);
            string code_verifier = RandomDataBase64url(32);
            string code_challenge = Base64urlencodeNoPadding(Sha256(code_verifier));

            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            Output("redirect URI: " + redirectURI);

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(redirectURI);
            Output("Listening..");
            httpListener.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                appSettings.GoogleAuthorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                appSettings.GoogleClientId,
                state,
                code_challenge,
                CODE_CHALLENGE_METHOD);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = authorizationRequest,
                UseShellExecute = true
            };
            // Opens request in the browser.
            System.Diagnostics.Process.Start(startInfo);

            // Waits for the OAuth authorization response.
            HttpListenerContext context = await httpListener.GetContextAsync();

            // Brings this app back to the foreground.
            this.Activate();

            // Sends an HTTP response to the browser.
            HttpListenerResponse response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
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
                Output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                Output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            string code = context.Request.QueryString.Get("code");
            string incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                Output(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }
            Output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            await PerformCodeExchange(appSettings, code, code_verifier, redirectURI);
            await GetApiToken();
            this.Close();
        }

        private async Task PerformCodeExchange(AppSettings appSettings, string code, string code_verifier, string redirectURI)
        {
            Output("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                appSettings.GoogleClientId,
                code_verifier,
                appSettings.GoogleClientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(appSettings.GoogleTokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    Output(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    AccessToken.Get.GoogleToken = tokenEndpointDecoded;

                    //string access_token = tokenEndpointDecoded["access_token"];
                    //UserinfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            Output(responseText);
                        }
                    }

                }
            }
        }

        private async Task GetApiToken()
        {
            try
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    ITokenService tokenService = scope.Resolve<ITokenService>();
                    AccessToken.Get.Token = await tokenService.Create(settingsFactory.CreateAccountSettings(AccessToken.Get.GetGoogleIdToken()));
                    //NotifyOwner();
                    Output("Token received");
                }
            }
            catch (Exception ex)
            {
                Output(ex.ToString());
            }
        }

        //private void NotifyOwner()
        //{
        //    if (Owner != null && Owner.GetType().Equals(typeof(MainWindow)))
        //    {
        //        MainWindow mainWindow = (MainWindow)Owner;
        //        mainWindow.AfterTokenRefresh();
        //    }
        //}

        //private async void UserinfoCall(string access_token)
        //{
        //    Output("Making API Call to Userinfo...");

        //    // sends the request
        //    HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.GoogleUserInfoEndpoint);
        //    userinfoRequest.Method = "GET";
        //    userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
        //    userinfoRequest.ContentType = "application/x-www-form-urlencoded";
        //    userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        //    // gets the response
        //    WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
        //    using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
        //    {
        //        // reads response body
        //        string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
        //        Output(userinfoResponseText);
        //    }
        //}

        private void Output(string output)
        {
            textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Console.WriteLine(output);
        }

        private static byte[] Sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(bytes);
        }

        private static string RandomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64urlencodeNoPadding(bytes);
        }

        private static string Base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        public static void ShowLoginDialog(bool checkAccessToken = true, Window owner = null)
        {
            if (!checkAccessToken || string.IsNullOrEmpty(AccessToken.Get?.Token))
            {
                GoogleLogin googleLogin = new GoogleLogin() { Owner = owner };
                googleLogin.ShowDialog();
            }
        }
    }
}
