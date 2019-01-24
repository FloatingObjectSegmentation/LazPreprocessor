using System;
using System.Collections.Generic;
using System.Numerics;
using common.structs;

using System.IO;

using external_tools.filters;
using core;
using System.Linq;
using common;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //UndergroundTest.ExecuteTest(args);
            new DmrAugsMergeTest().ExecuteTest(args);
        }
    }
}
