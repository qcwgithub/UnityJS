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

public static class ToLua
{
    static string enumFile = Application.dataPath + "/StreamingAssets/" + "Enum.cs";
    static StreamWriter enumWriter;

    [MenuItem("ToLua/Generate JS Bindings")]
    public static void GenerateJSBindingFiles()
    {
        Type t = typeof(BindingFlags);
        FieldInfo[] fis  = t.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
        int v = (int)fis[11].GetValue(null);

        return;

        SendMessageOptionsWrap.Register();
        return;

        mType = typeof(SendMessageOptions);
        sb = new StringBuilder();

        if (mType.IsEnum)
        {
            GenEnum();
        }
        else
        {
            GenBegin();
            GenConstructor();
            GenRegister();
            GenEnd();
        }

        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");

        SaveFile();
    }

    static StringBuilder sb = null;
    public static Type mType = null;

    public static void GenerateType(Type type)
    {
        mType = type;
        sb = new StringBuilder();
        string s = "";

        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        List<MethodInfo> list = new List<MethodInfo>();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo m = methods[i];
            if (m.Name.IndexOf("get_") == 0)
                continue;
            if (m.IsGenericMethod)
                continue;

             ParameterInfo[] param = m.GetParameters();
//             if (param != null && param.Length > 0)
//             {
//                 for (int j = 0; j < param.Length; j++)
//                 {
//                     if (param[j].ParameterType.IsArray)
//                 }
//             }

            if (m.ReturnType.IsArray)
                continue;

            s += m.ReturnType.ToString() + " " + m.Name + "(";
            for (int j = 0; j < param.Length; j++)
            {
                s += param[j].ToString();
                if (j < param.Length - 1)
                    s += ", ";
            }
            s += ");\n";
        }

        Debug.Log(s);
    }

    static void GenBegin()
    {
        string fmt = @"
using System;
using UnityEngine;

public class {0}Wrap
[[
";

        sb.AppendFormat(fmt, mType.Name);
    }

    static void GenEnd()
    {
        string fmt = @"
]]
";
        sb.Append(fmt);
    }

    static void GenConstructor()
    {
string fmt = @"
    public static int Constructor(IntPtr cx, UInt32 argc, IntPtr vp)
    [[
        var gt = SMData.getGlobalType(typeof({0}));
        if (gt == null)
        [[
             Debug.Log('GlobalType not found:' + typeof({0}).Name);
             return SMDll.JS_FALSE;
        ]]

        {0} go = new {0}();

        IntPtr obj = SMDll.JS_NewObject(cx, gt.jsClass, gt.proto, gt.parentProto);
        SMData.addNativeJSRelation(obj, go);

        return SMDll.JShelp_SetRvalObject (cx, vp, obj);
    ]]
";
        sb.AppendFormat(fmt, mType.Name);
        Debug.Log(sb);
    }

    static void GenRegister()
    {
string fmt = @"
    public static void Register(IntPtr cx, IntPtr glob)
    [[
        IntPtr jsClass = SMDll.JShelp_NewClass('{0}', 0);

        IntPtr obj = SMDll.JS_InitClass(cx, glob,
            IntPtr.Zero, /* parentProto */
            jsClass, /* JSClass */
            {0}Wrap.Constructor, /* constructor */
            0, /* constructor argument count*/
            IntPtr.Zero, /* properties */
            IntPtr.Zero, /* functions */
            IntPtr.Zero, /* static properties */
            IntPtr.Zero /* static functions*/
        );

        SMData.addGlobalType(typeof({0}), jsClass, obj, IntPtr.Zero);
    ]]
";
        sb.AppendFormat(fmt, mType.Name);

        Debug.Log(sb);
    }

    static string GetPushFunction(Type t)
    {
        if (t.IsEnum)
        {
            return "PushEnum";
        }
        else if (t == typeof(bool) || t.IsPrimitive || t == typeof(string))
        {
            return "Push";
        }
        return "Push";
    }

    static void GenAFunction(string functionName)
    {
string fmt = @"
    static int animation(IntPtr cx, UInt32 argc, IntPtr vp)
    [[
        if (argc != 0)
            return SMDll.JS_FALSE;

        IntPtr obj = SMDll.JShelp_ThisObject(cx, vp);

        var go = ({0})SMData.getNativeObj(obj);
        if (go == null)
            return SMDll.JS_FALSE;

        Animation ani = go.{1};
        if (ani == null)
            return SMDll.JS_FALSE;

        IntPtr jsObj = SMData.getJSObj(ani);
        if (jsObj == IntPtr.Zero)
        [[
            IntPtr jsClass = SMDll.JShelp_NewClass('Animation', 0);
            jsObj = SMDll.JS_NewObject(cx, jsClass, IntPtr.Zero, IntPtr.Zero);
            SMData.addNativeJSRelation(jsObj, ani);
        ]]
        return SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
    ]]
";
    }

    static void GenEnum()
    {
        FieldInfo[] fields = mType.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);

        string fmt = @"
public class {0}Wrap
[[
    static JSEnum[] enums = new JSEnum[]
    [[{1}
    ]];

    public static void Register()
    [[
        JSMgr.RegisterEnum('{0}', enums);
    ]]
]]
";

        string fmtField = @"
        new JSEnum('{0}', (int){1}.{0}),";

        StringBuilder stringField = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            stringField.AppendFormat(fmtField, fields[i].Name, mType.Name);
        }

        sb.AppendFormat(fmt, mType.Name, stringField.ToString());
    }

    static void SaveFile()
    {
        string file = Application.dataPath + "/Wrap/" + mType.Name + "Wrap.cs";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            StringBuilder sb_front = new StringBuilder();
            string fmt = 
@"using System;
using UnityEngine;
";
            sb_front.Append(fmt);

            textWriter.Write(sb_front.ToString());

            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }  
    }
}
