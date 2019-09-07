using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace common
{
    public class CountLines
    {
        public static long CountLinesReader(FileInfo file)
        {
            var lineCounter = 0L;
            using (var reader = new StreamReader(file.FullName))
            {
                while (reader.ReadLine() != null)
                {
                    lineCounter++;
                }
                return lineCounter;
            }
        }
    }
}
