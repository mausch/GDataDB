using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace GDataDB.Impl {
    public class GDataDBRequestFactory  {
        private readonly string clientEmail;
        private readonly RSACryptoServiceProvider privateKey;

        private Lazy<OAuth2Token> oauthToken;

        public GDataDBRequestFactory(string clientEmail, byte[] privateKey) {
            this.clientEmail = clientEmail;
            this.privateKey = GetPrivateKey(privateKey);
            oauthToken = GetOAuth2Token();
        }

        private Lazy<OAuth2Token> GetOAuth2Token() {
            return new Lazy<OAuth2Token>(() => {
                var tokenResponse = RequestNewToken();
                var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenResponse);
                var accessToken = (string)response["access_token"];
                var expiresSeconds = (long)response["expires_in"];
                var expiration = DateTime.Now.AddSeconds(expiresSeconds);
                return new OAuth2Token(accessToken, expiration);
            });
        }

        public string GetToken() {
            if (oauthToken.Value.Expiration < DateTime.Now)
                oauthToken = GetOAuth2Token();
            return oauthToken.Value.AuthToken;
        }

        private static RSACryptoServiceProvider GetPrivateKey(byte[] p12) {
            var certificate = new X509Certificate2(p12, "notasecret", X509KeyStorageFlags.Exportable);
            var rsa = (RSACryptoServiceProvider)certificate.PrivateKey;
            byte[] privateKeyBlob = rsa.ExportCspBlob(true);
            var privateKey = new RSACryptoServiceProvider();
            privateKey.ImportCspBlob(privateKeyBlob);
            return privateKey;
        }

        private string RequestNewToken() {
            var http = new WebClient();
            var response = http.UploadValues("https://accounts.google.com/o/oauth2/token", new NameValueCollection {
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"assertion", GetJWT()},
            });
            return Encoding.UTF8.GetString(response);
        }

        private static string UrlBase64Encode(string value) {
            var bytesValue = Encoding.UTF8.GetBytes(value);
            return UrlBase64Encode(bytesValue);
        }

        private static string UrlBase64Encode(byte[] bytes) {
            return Convert.ToBase64String(bytes)
                .Replace("=", String.Empty)
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static readonly string serializedHeader = 
            JsonConvert.SerializeObject(new {
                typ = "JWT",
                alg = "RS256",
            });

        private static readonly DateTime zeroDate = 
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly string scope = string.Join(" ", new[] { 
            "https://www.googleapis.com/auth/drive", 
            "https://spreadsheets.google.com/feeds",
        });

        public static string GetJWT(string clientEmail, RSACryptoServiceProvider privateKey, DateTime now) {
            var payload = new {
                scope = scope,
                iss = clientEmail,
                aud = "https://accounts.google.com/o/oauth2/token",
                exp = (int)(now - zeroDate + TimeSpan.FromHours(1)).TotalSeconds,
                iat = (int)(now - zeroDate).TotalSeconds,
                //sub = "mauricioscheffer@gmail.com",
            };

            string serializedPayload = JsonConvert.SerializeObject(payload);

            using (var hashAlg = new SHA256Managed()) {
                hashAlg.Initialize();
                var headerAndPayload = UrlBase64Encode(serializedHeader) + "." + UrlBase64Encode(serializedPayload);
                var headerPayloadBytes = Encoding.ASCII.GetBytes(headerAndPayload);
                var signature = UrlBase64Encode(privateKey.SignData(headerPayloadBytes, hashAlg));
                return headerAndPayload + "." + signature;
            }

        }

        public string GetJWT() {
            return GetJWT(clientEmail, privateKey, DateTime.UtcNow);
        }

        public WebClient CreateRequest() {
            var http = new WebClient();
            http.Headers.Add("Authorization", "Bearer " + GetToken());
            http.Headers.Add("Content-Type", "application/atom+xml; charset=UTF-8");
            http.Headers.Add("GData-Version", "3.0");
            return http;
        }
    }
}