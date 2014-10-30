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
    public enum Oper
    {
        GET_FIELD = 0,
        SET_FIELD = 1,
        GET_PROPERTY = 2,
        SET_PROPERTY = 3,
        METHOD,
    }

    // CS -> JS 用于数组
    public static SMDll.jsval ConvertCSValue2JSValue(IntPtr cx, IntPtr vp, object csObj)
    {
        SMDll.jsval val = new SMDll.jsval();

        if (csObj == null)
        {
            SMDll.JShelp_SetJsvalUndefined(ref val);
            return val;
        }

        Type t = csObj.GetType();

        if (t == typeof(string))
        {
            SMDll.JShelp_SetJsvalString(cx, ref val, (string)csObj);
        }
        else if (t.IsEnum)
        {
            SMDll.JShelp_SetJsvalInt(ref val, (int)csObj);
        }
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                SMDll.JShelp_SetJsvalBool(ref val, (bool)csObj);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                SMDll.JShelp_SetJsvalInt(ref val, (int)csObj);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                SMDll.JShelp_SetJsvalDouble(ref val, (double)csObj);
            }
            else
            {
                Debug.Log("CS -> JS: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            IntPtr jsObj = SMData.getJSObj(csObj);
            if (jsObj == IntPtr.Zero)
            {
                jsObj = SMDll.JShelp_NewObjectAsClass(cx, CallJS.glob, t.Name);
                if (jsObj != null)
                    SMData.addNativeJSRelation(jsObj, csObj);
            }
            if (jsObj == IntPtr.Zero)
                SMDll.JShelp_SetJsvalUndefined(ref val);
            else
                SMDll.JShelp_SetJsvalObject(ref val, jsObj);
        }
        else
        {
            Debug.Log("CS -> JS: Unknown CS type: " + t.ToString());
            SMDll.JShelp_SetJsvalUndefined(ref val);
        }
        return val;
    }

    // 根据 CS 对象的类型，向 JS 返回值
    // CS -> JS
    public static void PushResult(IntPtr cx, IntPtr vp, object csObj)
    {
        if (csObj == null)
        {
            SMDll.JShelp_SetRvalUndefined(cx, vp);
            return;
        }

        Type t = csObj.GetType();
        
        if (t == typeof(string))
        {
            JSMgr.Push(cx, vp, (string)csObj);
        }
        else if (t.IsEnum)
        {
            JSMgr.Push(cx, vp, (int)csObj);
        }
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                JSMgr.Push(cx, vp, (bool)csObj);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                JSMgr.Push(cx, vp, (int)csObj);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                JSMgr.Push(cx, vp, (double)csObj);
            }
            else
            {
                Debug.Log("PushResult: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            IntPtr jsObj = SMData.getJSObj(csObj);
            if (jsObj == IntPtr.Zero)
            {
                jsObj = SMDll.JShelp_NewObjectAsClass(cx, CallJS.glob, t.Name);
                if (jsObj != null)
                    SMData.addNativeJSRelation(jsObj, csObj);
            }
            if (jsObj == IntPtr.Zero)
                SMDll.JShelp_SetRvalUndefined(cx, vp);
            else
                JSMgr.Push(cx, vp, (IntPtr)jsObj);
        }
        else if (t.IsArray)
        {
            Array arr = csObj as Array;
            IntPtr jsArr = SMDll.JS_NewArrayObject(cx, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                SMDll.jsval val = ConvertCSValue2JSValue(cx, vp, arr.GetValue(i));
                SMDll.JS_SetElement(cx, jsArr, (uint)i, ref val);
            }
            JSMgr.Push(cx, vp, (IntPtr)jsArr);
        }
        else
        {
            Debug.Log("PushResult: Unknown CS type: " + t.ToString());
            SMDll.JShelp_SetRvalUndefined(cx, vp);
        }
    }

    // JS -> CS
    // 是根据所需要的 C# 参数类型来转换 js 参数
    public static object ConvertJSValue2CSValue(Type t, IntPtr cx, IntPtr vp, int paramIndex, Oper op)
    {
        if (t == typeof(string))
            return SMDll.JShelp_ArgvString(cx, vp, paramIndex);
        else if (t.IsEnum)
            return SMDll.JShelp_ArgvInt(cx, vp, paramIndex);
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                return SMDll.JShelp_ArgvBool(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                return SMDll.JShelp_ArgvInt(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                return SMDll.JShelp_ArgvDouble(cx, vp, paramIndex);
            }
            else
            {
                Debug.Log("ConvertJSValue2CSValue: Unknown primitive type: " + t.ToString());
            }
        }
//         else if (t.IsValueType)
//         {
// 
//         }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            if (SMDll.JShelp_ArgvIsNull(cx, vp, paramIndex))
                return null;
            IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, paramIndex);
            if (jsObj == IntPtr.Zero)
                return null;
            object csObject = SMData.getNativeObj(jsObj);
            return csObject;
        }
        else
        {
            Debug.Log("ConvertJSValue2CSValue: Unknown CS type: " + t.ToString());
        }
        return null;
    }

    // paramCount是指从 js 传过来的参数
    public static object[] BuildMethodArgs(IntPtr cx, IntPtr vp, MethodInfo method, int paramCount, int paramStartIndex)
    {
        if (paramCount == 0)
            return null;
        if (paramCount < 0)
        {
            Debug.LogError("paramCount < 0");
            return null;
        }
        ParameterInfo[] ps = method.GetParameters();
        if (paramCount != ps.Length)
        {
            Debug.LogError("paramCount != ps.Length");
        }

        ArrayList args = new ArrayList();
        for (int i = 0; i < paramCount; i++)
        {
            int paramIndex = paramStartIndex + i;

            Type t = ps[i].ParameterType;
            if (ps[i].IsOptional && SMDll.JShelp_ArgvIsUndefined(cx, vp, paramIndex))
                args.Add(ps[i].DefaultValue);
            else
                args.Add(ConvertJSValue2CSValue(t, cx, vp, paramIndex, Oper.METHOD));
        }
        return args.ToArray();
    }
    

    public static void AddTestObject(IntPtr cx, string className, object csObj, string jsName)
    {
        IntPtr jsObj = SMData.getJSObj(csObj);
        if (jsObj == IntPtr.Zero)
        {
            jsObj = SMDll.JShelp_NewObjectAsClass(cx, CallJS.glob, className);
            SMData.addNativeJSRelation(jsObj, csObj);
        }
    }

    static int Call(IntPtr cx, uint argc, IntPtr vp)
    {
        // 前面4个参数是固定的
        Oper op = (Oper)SMDll.JShelp_ArgvInt(cx, vp, 0);
        int slot = SMDll.JShelp_ArgvInt(cx, vp, 1);
        int index = SMDll.JShelp_ArgvInt(cx, vp, 2);
        bool isStatic = SMDll.JShelp_ArgvBool(cx, vp, 3);

        if (slot < 0 || slot >= allTypeInfo.Count)
        {
            Debug.LogError("Bad slot: " + slot);
            return SMDll.JS_FALSE;
        }
        ATypeInfo aInfo = allTypeInfo[slot];

        int paramCount = 4;
        object csObj = null;
        if (!isStatic)
        {
            IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 4);
            if (jsObj == IntPtr.Zero)
                return SMDll.JS_FALSE;

            csObj = SMData.getNativeObj(jsObj);
            if (csObj == null)
                return SMDll.JS_FALSE;

            paramCount++;
        }

        object result = null;

        switch (op)
        {
        case Oper.GET_FIELD:
            {
                result = aInfo.fields[index].GetValue(csObj);
            }
            break;
        case Oper.SET_FIELD:
            {
                FieldInfo field = aInfo.fields[index];
                field.SetValue(csObj, ConvertJSValue2CSValue(field.FieldType, cx, vp, 4, Oper.SET_FIELD));
            }
            break;
        case Oper.GET_PROPERTY:
            {
                result = aInfo.properties[index].GetValue(csObj, null);
            }
            break;
        case Oper.SET_PROPERTY:
            {
                PropertyInfo property = aInfo.properties[index];
                property.SetValue(csObj, ConvertJSValue2CSValue(property.PropertyType, cx, vp, 4, Oper.SET_PROPERTY), null);
            }
            break;
        case Oper.METHOD:
            {
                int methodParamCount = (int)argc - paramCount;

                MethodInfo method = aInfo.methods[index];
                object[] args = BuildMethodArgs(cx, vp, method, methodParamCount, paramCount);
                result = method.Invoke(csObj, args);
            }
            break;
        }

        JSMgr.PushResult(cx, vp, result);
        return SMDll.JS_TRUE;
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

        int slot = allTypeInfo.Count;
        allTypeInfo.Add(ti);
        tiOut = ti;
        return slot;
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
    }

    
}
