using System;
using System.Data.SqlTypes;
using UnityEngine;

namespace _TevLib.CustomUtility
{
    public static class CtUtils
    {
        public static bool IsNull(params object[] nullable)
        {
            foreach (object t in nullable)
            {
                if (t == null)
                    return true;
            }
            return false;
        }
    }
}