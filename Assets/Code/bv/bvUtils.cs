using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bvData;

namespace bvUtils
{
    public static class Utilities
    {
        public static T[][] SplitArray<T>(T[] array, int size)
        {
            int numOfArrays = (int)Math.Ceiling((double)array.Length / size);
            T[][] result = new T[numOfArrays][];

            for (int i = 0; i < numOfArrays; i++)
            {
                int elementsInThisArray = Math.Min(size, array.Length - i * size);
                result[i] = new T[elementsInThisArray];
                Array.Copy(array, i * size, result[i], 0, elementsInThisArray);
            }

            return result;
        }
    }

    public delegate void FactionApiCallback(bvFaction[] factions);
    public delegate void SystemApiCallback(bvSystem[] systems);
}
