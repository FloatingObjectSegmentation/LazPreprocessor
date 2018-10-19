using System;
using System.Collections.Generic;
using System.Text;

namespace common
{
    public static class Filename
    {
        public static string FromFullPath(string filepath)
        {
            string[] neki = filepath.Split('\\');
            string filename = neki[neki.Length - 1];
            return filename;
        }
    }
}
