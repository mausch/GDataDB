using System;

namespace GDataDB.Impl {
    public class OAuth2Token {
        public readonly string AuthToken;
        public readonly DateTime Expiration;

        public OAuth2Token(string authToken, DateTime expiration) {
            AuthToken = authToken;
            Expiration = expiration;
        }
    }
}