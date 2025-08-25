using System.Text;
using System;

namespace Fnf.Framework
{
    public static class Base64
    {
        public static string To(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static string From(string text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }
    }
}