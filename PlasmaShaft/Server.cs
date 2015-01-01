using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;

namespace PlasmaShaft
{
	public static class Server
	{
		static TcpListener listener;
		public static IPAddress InternalIP { get; private set; }
		public static IPAddress ExternalIP { get; private set; }
		public static Config Config = new Config("config");

		public static void InitConfig()
		{
            Config.LoadConfig("config");
            try
            {
                Config.GetValue("server-name");
            } catch {
                Config.SetValue("server-name", "[PlasmaShaft] Default");
            }
            try
            {
                Config.GetValue("server-port");
            } catch {
                Config.SetValue("server-port", "25565");
            }
            try
            {
                Config.GetValue("verify-names");
            }
            catch {
                Config.SetValue("verify-names", "true");
            }
            try
            {
                Config.GetValue("max-players");
            }
            catch {
                Config.SetValue("max-players", "20");
            }
                Config.SaveConfig("config");
		}
        //Borrowed from fCraft
        public static IPEndPoint BindIPEndPointCallback( ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount ) {
            return new IPEndPoint( InternalIP, 0 );
        }
        static IPAddress CheckExternalIP() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "http://checkip.dyndns.org/" );
            request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint( BindIPEndPointCallback );
            request.Timeout = 30000;
            request.CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore );

            try {
                using( WebResponse response = request.GetResponse() ) {
                    // ReSharper disable AssignNullToNotNullAttribute
                    using( StreamReader responseReader = new StreamReader( response.GetResponseStream() ) ) {
                        // ReSharper restore AssignNullToNotNullAttribute
                        string responseString = responseReader.ReadToEnd();
                        int startIndex = responseString.IndexOf( ":" ) + 2;
                        int endIndex = responseString.IndexOf( '<', startIndex ) - startIndex;
                        IPAddress result;
                        if( IPAddress.TryParse( responseString.Substring( startIndex, endIndex ), out result ) ) {
                            return result;
                        } else {
                            return null;
                        }
                    }
                }
            } catch( WebException ex ) {
                Console.WriteLine("Could not check external IP: " + ex.ToString());
                return null;
            }
        }
	}
}

