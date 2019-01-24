using common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    public class Test
    {
        public static string TESTS_SUBDIR = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.TESTS_SUBDIR);
        public virtual string TEST_SUBSUBDIR { get; set; }

        public Test() {
            if (!Directory.Exists(TESTS_SUBDIR))
                Directory.CreateDirectory(TESTS_SUBDIR);
            if (!Directory.Exists(TEST_SUBSUBDIR))
                Directory.CreateDirectory(TEST_SUBSUBDIR);
        }
        
    }
}
