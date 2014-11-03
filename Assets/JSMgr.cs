using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
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

    public static void EvaluateFile(IntPtr cx, IntPtr glob, string file)
    {
        SMDll.jsval val = new SMDll.jsval();
        StreamReader r = new StreamReader(file, Encoding.UTF8);
        string s = r.ReadToEnd();

        SMDll.JS_EvaluateScript(cx, glob, s, (uint)s.Length, file, 1, ref val);
        r.Close();
    }
    public static void EvaluateGeneratedScripts(IntPtr cx, IntPtr glob)
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/StreamingAssets/JavaScript/Generated");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].IndexOf(".meta") == files[i].Length - 5)
                continue;
            EvaluateFile(cx, glob, files[i]);
        }
    }

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

    public enum CallType
    {
        Field,
        Property,
        Method,
    }
    
    public class ATypeInfo
    {
        public FieldInfo[] fields;
        public PropertyInfo[] properties;
        public MethodInfo[] methods;
    }
    public static List<ATypeInfo> allTypeInfo = new List<ATypeInfo>();

    public static int AddTypeInfo(Type type)
    {
        ATypeInfo tiOut = new ATypeInfo();
        return AddTypeInfo(type, out tiOut);
    }
    public static int AddTypeInfo(Type type, out ATypeInfo tiOut)
    {
        ATypeInfo ti = new ATypeInfo();
        ti.fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Static);
//         var lstField = new List<FieldInfo>();
//         foreach (var field in ti.fields)
//         {
//             if (field.IsInitOnly)
//                 continue;
//             lstField.Add(field);
//         }

        ti.properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance/* | BindingFlags.Static*/);
        ti.methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        List<MethodInfo> lMethods = new List<MethodInfo>();
        for (int i = 0; i < ti.methods.Length; i++)
        {
            // generic method is not supported
            if (ti.methods[i].IsGenericMethod || ti.methods[i].IsGenericMethodDefinition)
                continue;
            else
                lMethods.Add(ti.methods[i]);
        }
        ti.methods = lMethods.ToArray();

        int slot = allTypeInfo.Count;
        allTypeInfo.Add(ti);
        tiOut = ti;
        return slot;
    }

    public static IntPtr CSOBJ = IntPtr.Zero;
    static VCall vCall = new VCall();

    static int Call(IntPtr cx, uint argc, IntPtr vp)
    {
        return vCall.Call(cx, argc, vp);
    }

    /*
     * Create a 'CS' global object to use in JS
     */
    public static void RegisterCS(IntPtr cx, IntPtr glob)
    {
        IntPtr jsClass = SMDll.JShelp_NewClass("CS", 0);

        IntPtr obj = SMDll.JS_InitClass(cx, glob,
            IntPtr.Zero, /* parentProto */
            jsClass, /* JSClass */
            null, /* constructor */
            0, /* constructor argument count*/
            IntPtr.Zero, /* properties */
            IntPtr.Zero, /* functions */
            IntPtr.Zero, /* static properties */
            IntPtr.Zero /* static functions*/
        );

        SMDll.JS_DefineFunction(cx, obj, "Call", new SMDll.JSNative(Call), 0/* narg */, 0);
        CSOBJ = obj;
    }

    
}
