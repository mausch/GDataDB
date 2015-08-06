using System;

namespace GDataDB.Impl {
    public static class Utils {
        public static string UrlEncode(string s) {
            return Uri.EscapeDataString(s).Replace("%20", "+");
        }
    }
}
