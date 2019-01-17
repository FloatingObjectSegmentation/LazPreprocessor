﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common
{
    public static class MyCollections
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}