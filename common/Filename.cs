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

        public static string FolderFromFullPath(string filepath) {
            string[] neki = filepath.Split('\\');
            
            string[] dirpath = new string[neki.Length - 1];
            for (int i = 0; i < neki.Length - 1; i++)
                dirpath[i] = neki[i];

            return string.Join("\\", dirpath);
        } 
    }
}
