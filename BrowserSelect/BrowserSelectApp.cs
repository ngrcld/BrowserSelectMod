using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Windows.Forms;

using BrowserSelect.Properties;

namespace BrowserSelect
{
    //=============================================================================================================
    class BrowserSelectApp
    //=============================================================================================================
    {
        public static string url { get; set; } = "https://www.google.com";
        public static bool launchWithUrl { get; set; } = false;

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        //-------------------------------------------------------------------------------------------------------------
        {
            BrowserSelectSetup.RegisterAsBrowser();
            ConfigureUriParser();

            // to prevent loss of settings when updating to new version
            if (Settings.Default.Version == 0)
            {
                Settings.Default.Upgrade();
                Settings.Default.Version = 2;
                // to prevent null reference in case settings file did not exist
                if (Settings.Default.HiddenBrowsers == null)
                    Settings.Default.HiddenBrowsers = new StringCollection();
                if (Settings.Default.Rules == null)
                    Settings.Default.Rules = new StringCollection();
                Settings.Default.Save();
            }

            //checking if a url is being opened or app is run from start menu (without arguments)
            if (args.Length > 0)
            {
                //check to see if auto select rules match
                url = args[0];
                //normalize the url
                Uri uri = new UriBuilder(url).Uri;
                url = uri.AbsoluteUri;
                launchWithUrl = true;

                UrlProcessor processor = new UrlProcessor();
                if (processor.ProcessUrl(uri))
                    return;
            }

            // display main view
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BrowserSelectView());
        }

        //-------------------------------------------------------------------------------------------------------------
        private static Uri UriExpander(Uri uri)
        //-------------------------------------------------------------------------------------------------------------
        {
            // always expand microsoft safelinks
            if (uri.Host.EndsWith("safelinks.protection.outlook.com"))
            {
                var queryDict = HttpUtility.ParseQueryString(uri.Query);
                if (queryDict != null && queryDict.Get("url") != null)
                {
                    uri = new UriBuilder(HttpUtility.UrlDecode(queryDict.Get("url"))).Uri;
                }
            }

            if (Settings.Default.RedirectPolicy == "First Redirect" || Settings.Default.RedirectPolicy == "All Redirects")
            {
                bool followAllRedirects = (Settings.Default.RedirectPolicy == "All Redirects");
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertificates);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3; //SecurityProtocolType.Tls12;
                var webRequest = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv: 85.0) Gecko/20100101 Firefox/85.0";
                webRequest.AllowAutoRedirect = followAllRedirects;
                try
                {
                    var response = (HttpWebResponse)webRequest.GetResponse();
                    if ((int)response.StatusCode == 307)
                    {
                        uri = UriExpander(new UriBuilder(response.Headers["Location"]).Uri);
                    }
                    else if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302)
                    {
                        uri = new UriBuilder(response.Headers["Location"]).Uri;
                    }
                    else
                    {
                        ServicePoint sp = webRequest.ServicePoint;
                        //Console.WriteLine("Final URL address is " + sp.Address.ToString());
                        uri = new UriBuilder(sp.Address.ToString()).Uri;
                    }
                }
                catch (Exception /*ex*/)
                {
                }
            }

            return uri;
        }

        //-------------------------------------------------------------------------------------------------------------
        private static bool AcceptAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //-------------------------------------------------------------------------------------------------------------
        {
            return true;
        }

        //-------------------------------------------------------------------------------------------------------------
        // https://stackoverflow.com/a/7202560/1461004
        private static void ConfigureUriParser()
        //-------------------------------------------------------------------------------------------------------------
        {
            var getSyntaxMethod = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            if (getSyntaxMethod == null)
            {
                throw new MissingMethodException("UriParser", "GetSyntax");
            }

            var uriParser = getSyntaxMethod.Invoke(null, new object[] { "http" });

            var setUpdatableFlagsMethod = uriParser.GetType().GetMethod("SetUpdatableFlags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (setUpdatableFlagsMethod == null)
            {
                throw new MissingMethodException("UriParser", "SetUpdatableFlags");
            }
            setUpdatableFlagsMethod.Invoke(uriParser, new object[] { 0 });
        }
    }
}
