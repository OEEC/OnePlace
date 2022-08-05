using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class RemoverCaracteres
    {
        public static string RemoverCaracteresInvalidosXml(string str)
        {
            str = str.Replace("\r\n", "");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            str = str.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", @"<?xml version=""1.0"" encoding=""utf-8""?>").Trim();
            str = str.Replace("﻿", "");
            str = str.Replace(@"
", "");
            str = str.Replace("\0", "");
            return str;
        }
    }
}
