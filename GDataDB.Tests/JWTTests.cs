using System;
using NUnit.Framework;
using System.IO;
using GDataDB.Impl;

namespace GDataDB.Tests {
    [TestFixture]
    public class JWTTests {
        private static readonly GDataDBRequestFactory client =
            new GDataDBRequestFactory(
                clientEmail: "315184672897-s1q9gb7bghm39f27d655fb8iqo27lv1o@developer.gserviceaccount.com",
                privateKey: File.ReadAllBytes(@"g:\Users\mausch\Downloads\gdatadb-test-cfa565a0a063.p12"));

        [Test]
        public void GetJWT() {
            var jwt = client.GetJWT();
            Console.WriteLine(jwt);
        }

        [Test]
        public void GetAuthToken() {
            var token = client.GetToken();
            Console.WriteLine(token);
        }
    }
}
