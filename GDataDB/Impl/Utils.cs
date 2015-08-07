using System;
using System.Xml.Linq;

namespace GDataDB.Impl {
    public static class Utils {
        public static string UrlEncode(string s) {
            return Uri.EscapeDataString(s).Replace("%20", "+");
        }

        public static readonly XNamespace SpreadsheetsNs = "http://schemas.google.com/spreadsheets/2006";
        public static readonly XNamespace BatchNs = "http://schemas.google.com/gdata/batch";
        public static readonly XNamespace AtomNs = "http://www.w3.org/2005/Atom";

    }
}
