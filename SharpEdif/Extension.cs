using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpEdif.User
{
    public unsafe class Extension
    {
        public const string ExtensionName = "Better Global Values";
        public const string ExtensionAuthor = "Yunivers";
        public const string ExtensionCopyright = "Copyright © 2023 Yunivers";
        public const string ExtensionComment = "A better alternative to Clickteam's Global Values, supporting integers, doubles, strings, booleans, and arrays.";
        public const string ExtensionHttp = "https://www.gamejolt.com/invite/AITYunivers";

        public static Dictionary<string, object> gV = new Dictionary<string, object>();
        public static Dictionary<string, bool> gVSet = new Dictionary<string, bool>();

        #region Actions

        // This action sets an index to a value
        [Action("Set Value", "Set value of %0 to %1 : Type: %2")]
        public static void setIndexValue(LPRDATA* rdPtr, string p1, float p2, int p3)
        {
            if (p3 < 0 || p3 > 4) return;

            object toAdd = null;
            switch (p3)
            {
                case 0:
                    toAdd = (int)p2;
                    break;
                case 1:
                    toAdd = p2;
                    break;
                case 2:
                    toAdd = p2.ToString();
                    break;
                case 3:
                    toAdd = p2 == 1;
                    break;
            }

            if (gV.ContainsKey(p1))
                gV[p1] = toAdd;
            else
                gV.Add(p1, toAdd);
        }

        // This action sets an index to a string
        [Action("Set String", "Set string of %0 to %1")]
        public static void setIndexString(LPRDATA* rdPtr, string p1, string p2)
        {
            if (gV.ContainsKey(p1))
                gV[p1] = p2;
            else
                gV.Add(p1, p2);
        }

        // This action sets an index within an array to a value
        [Action("Set Value in Array", "Set value of %1 in array %0 to %2 : Type: %3")]
        public static void setIndexArrayValue(LPRDATA* rdPtr, string p1, int p2, float p3, int p4)
        {
            if (p4 < 0 || p4 > 4) return;

            object toAdd = null;
            switch (p4)
            {
                case 0:
                    toAdd = (int)p3;
                    break;
                case 1:
                    toAdd = p3;
                    break;
                case 2:
                    toAdd = p3.ToString();
                    break;
                case 3:
                    toAdd = p3 == 1 ? true : false;
                    break;
            }

            if (gV.ContainsKey(p1) && gV[p1] is object[])
            {
                if (((object[])gV[p1]).Length > p2)
                    ((object[])gV[p1])[p2] = toAdd;
                else if (!gVSet[p1])
                {
                    object[] r = (object[])gV[p1];
                    Array.Resize(ref r, p2 + 1);
                    r[p2] = toAdd;
                    if (gV.ContainsKey(p1))
                        gV[p1] = r;
                    else
                        gV.Add(p1, r);
                }
            }
            else
            {
                object[] r = new object[p2 + 1];
                r[p2] = toAdd;
                if (gV.ContainsKey(p1))
                    gV[p1] = r;
                else
                    gV.Add(p1, r);
            }
        }

        // This action sets an index within an array to a value
        [Action("Set String in Array", "Set string of %1 in array %0 to %2")]
        public static void setIndexArrayString(LPRDATA* rdPtr, string p1, int p2, string p3)
        {
            if (gV.ContainsKey(p1) && gV[p1] is object[])
            {
                if (((object[])gV[p1]).Length > p2)
                    ((object[])gV[p1])[p2] = p3;
                else if (!gVSet[p1])
                {
                    object[] r = (object[])gV[p1];
                    Array.Resize(ref r, p2 + 1);
                    r[p2] = p3;
                    if (gV.ContainsKey(p1))
                        gV[p1] = r;
                    else
                        gV.Add(p1, r);
                }
            }
            else
            {
                object[] r = new object[p2 + 1];
                r[p2] = p3;
                if (gV.ContainsKey(p1))
                    gV[p1] = r;
                else
                    gV.Add(p1, r);
            }
        }

        // This action sets the length of an array, -1 making it infinite.
        [Action("Set Array Length", "Set length of array %0 to %1")]
        public static void setArrayLength(LPRDATA* rdPtr, string p1, int p2)
        {
            if (p2 == -1)
                gVSet[p1] = false;
            else
            {
                gVSet[p1] = true;
                if (gV.ContainsKey(p1) && gV[p1] is object[])
                {
                    object[] r = (object[])gV[p1];
                    Array.Resize(ref r, p2);
                    if (gV.ContainsKey(p1))
                        gV[p1] = r;
                    else
                        gV.Add(p1, r);
                }
                else
                    if (gV.ContainsKey(p1))
                    gV[p1] = new object[p2];
                else
                    gV.Add(p1, new object[p2]);
            }
        }

        #endregion

        #region Expressions

        // This expression returns the id of the index's type.
        [Expression("Get Value Type", "BeGlType(")]
        public static float getIndexType(LPRDATA* rdPtr, string p1)
        {
            if (!gV.ContainsKey(p1)) return 0;

            switch (gV[p1].GetType().FullName)
            {
                default:
                    return 0;
                case "System.Int32":
                    return 1;
                case "System.Single":
                    return 2;
                case "System.String":
                    return 3;
                case "System.Boolean":
                    return 4;
                case "System.Object[]":
                    return 5;
            }
        }

        // This expression returns the value of the index.
        [Expression("Get Value", "BeGlValue(")]
        public static float getIndexValue(LPRDATA* rdPtr, string p1)
        {
            if (!gV.ContainsKey(p1)) return -1;

            switch (gV[p1].GetType().FullName)
            {
                default:
                    return -1;
                case "System.Int32":
                    return (int)gV[p1];
                case "System.Single":
                    return (float)gV[p1];
                case "System.Boolean":
                    return (bool)gV[p1] ? 1 : 0;
            }
        }

        // This expression returns the string of the index.
        [Expression("Get String", "BeGlString$(")]
        public static string getIndexString(LPRDATA* rdPtr, string p1)
        {
            if (gV.ContainsKey(p1) && gV[p1] is string)
                return (string)gV[p1];
            return "";
        }

        // This expression returns the value of the index inside the array.
        [Expression("Get Array Value", "BeGlArrayValue(")]
        public static float getIndexArrayValue(LPRDATA* rdPtr, string p1, int p2)
        {
            if (!gV.ContainsKey(p1)) return -1;

            switch (((object[])gV[p1])[p2].GetType().FullName)
            {
                default:
                    return -1;
                case "System.Int32":
                    return (int)((object[])gV[p1])[p2];
                case "System.Single":
                    return (float)((object[])gV[p1])[p2];
                case "System.Boolean":
                    return (bool)((object[])gV[p1])[p2] ? 1 : 0;
            }
        }

        // This expression returns the string of the index inside the array.
        [Expression("Get Array String", "BeGlArrayString$(")]
        public static string getIndexArrayString(LPRDATA* rdPtr, string p1, int p2)
        {
            if (!gV.ContainsKey(p1) ||
                gV[p1] is not object[] ||
                ((object[])gV[p1]).Length <= p2)
                return "";

            if (((object[])gV[p1])[p2] is string)
                return (string)((object[])gV[p1])[p2];
            return "";
        }

        // This expression returns the length of an array.
        [Expression("Get Array Length", "BeGlArrayLength(")]
        public static float getArrayLength(LPRDATA* rdPtr, string p1)
        {
            if (!gV.ContainsKey(p1) ||
                gV[p1] is not object[])
                return -1;
            return ((object[])gV[p1]).Length;
        }

        #endregion
    }
}