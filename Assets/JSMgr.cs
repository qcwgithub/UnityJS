using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class JSMgr
{
    /*
    * Push，返回某个类型的对象给JS
    */
    public static void Push(IntPtr cx, IntPtr vp, bool v) { SMDll.JShelp_SetRvalBool(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, double v) { SMDll.JShelp_SetRvalDouble(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, int v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, UInt32 v) { SMDll.JShelp_SetRvalUInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, string v) { SMDll.JShelp_SetRvalString(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, IntPtr v) { SMDll.JShelp_SetRvalObject(cx, vp, v); }

    public static void Push(IntPtr cx, IntPtr vp, char v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, sbyte v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, byte v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, short v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, ushort v) { SMDll.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, long v) { SMDll.JShelp_SetRvalInt(cx, vp, (int)v); } // !!
    public static void Push(IntPtr cx, IntPtr vp, ulong v) { SMDll.JShelp_SetRvalUInt(cx, vp, (UInt32)v); } // !!
    public static void Push(IntPtr cx, IntPtr vp, float v) { SMDll.JShelp_SetRvalDouble(cx, vp, (double)v); }
    public static void Push(IntPtr cx, IntPtr vp, decimal v) { SMDll.JShelp_SetRvalDouble(cx, vp, (double)v); }

    

    public static void RegisterEnum(string name, JSEnum[] enums)
    {
        // 导出到 js 文件中
        StringBuilder sb = new StringBuilder();
        string fmt = @"{0} = {0} || [[]];
";
        sb.AppendFormat(fmt, name);

        string fmtField = @"{0}.{1} = {2};
";

        for (int i = 0; i < enums.Length; i++)
        {
            sb.AppendFormat(fmtField, name, enums[i].name, enums[i].val);
        }

        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");

        string file = Application.dataPath + "/StreamingAssets/" + "Enum.javascript";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        } 
    }
}
