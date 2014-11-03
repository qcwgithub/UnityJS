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

public static class ToJS
{
    // 输入参数
    static StringBuilder sb = null;
    public static Type type = null;
    public static string className = "TestEnum";
    public static string baseClassName = "";

    // 一些配置

    /* 枚举统一输出到同一个地方去 */
    static string enumFile = Application.dataPath + "/StreamingAssets/JavaScript/Generated/enum.javascript";
    static string tempFile = Application.dataPath + "/StreamingAssets/JavaScript/temp.javascript";

    // 开始生成，有一些事情要处理
    public static void OnBegin()
    {
        // 清除导出的枚举文件
        var writer = OpenFile(enumFile, false);
        writer.Close();

    }
    public static void OnEnd()
    {

    }

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, int slot)
    {
        /*
        * fields
        * 0 class name
        * 1 field name
        * 2 slot
        * 3 index
        * 4 GET_FIELD
        * 5 SET_FIELD
        * 6 field type
        * 7 READ only / WRITE only / READ & WRITE
        */
        string fmt = @"
/* {7} {6} */
Object.defineProperty({0}.prototype, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, false, this); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, false, this, v); ]]
]]);
";
        string fmtStatic = @"
/* {7} static {6} */
Object.defineProperty({0}, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, true); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, true, v); ]]
]]);
";
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            if (!field.IsStatic)
                sb.AppendFormat(fmt, type.Name, field.Name, slot, i, (int)VCall.Oper.GET_FIELD, (int)VCall.Oper.SET_FIELD, field.FieldType.Name,
                    (field.IsInitOnly || field.IsLiteral) ? "ReadOnly" :""
                    );
            else
                sb.AppendFormat(fmtStatic, type.Name, field.Name, slot, i, (int)VCall.Oper.GET_FIELD, (int)VCall.Oper.SET_FIELD, field.FieldType.Name,
                    (field.IsInitOnly || field.IsLiteral) ? "ReadOnly" : ""
                    );
        }
        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, int slot)
    {
        /*
        * properties
        * 0 class name
        * 1 property name
        * 2 slot
        * 3 index in field array
        * 4 GET_PROPERTY
        * 5 SET_PROPERTY
        * 6 return type
        * 7 READ only / WRITE only
        */
        string fmt = @"
/* {7} {6} */
Object.defineProperty({0}.prototype, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, false, this); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, false, this, v); ]]
]]);
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            sb.AppendFormat(fmt, type.Name, property.Name, slot, i, (int)VCall.Oper.GET_PROPERTY, (int)VCall.Oper.SET_PROPERTY, property.PropertyType.Name,
                (property.CanRead && property.CanWrite) ? "" : (property.CanRead ? "ReadOnly" : "WriteOnly")
                );
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int slot)
    {
        /*
        * methods
        * 0 class name
        * 1 method name
        * 2 formal parameters
        * 3 slot
        * 4 index
        * 5 actual parameters
        * 6 return type
        * 7 op
        * 8 is override
        */
        string fmt = @"
/* {6} */
{0}.prototype.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, false, this, {8}{5}); ]]";
        string fmtStatic = @"
/* static {6} */
{0}.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, true, {8}{5}); ]]";

        int overloadedIndex = 0;
        int overloadedCount = 0;
        int overloadedMaxParamCount = 0;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            // 这里假设实例函数不会和静态函数同名
            ParameterInfo[] paramS = method.GetParameters();
            if (i < methods.Length - 1 && method.Name == methods[i + 1].Name)
            {
                if (overloadedCount == 0)
                {
                    overloadedCount = 2;
                    overloadedIndex = i;
                }
                else
                {
                    overloadedCount++;
                }
                overloadedMaxParamCount = Math.Max(overloadedMaxParamCount, paramS.Length);
                continue;
            }
            StringBuilder sbFormalParam = new StringBuilder();
            StringBuilder sbActualParam = new StringBuilder();

            if (overloadedCount > 0)
            {
                for (int j = 0; j < overloadedMaxParamCount; j++)
                {
                    sbFormalParam.AppendFormat("a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == overloadedMaxParamCount  - 1 ? "" : ","), (j == 0 ? ", " : ""));
                }
                sb.AppendFormat(@"
/* overloaded {0} */", overloadedCount);
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, type.Name, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)VCall.Oper.METHOD, "true");
                else
                    sb.AppendFormat(fmtStatic, type.Name, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)VCall.Oper.METHOD, "true");
            }
            else
            {
                for (int j = 0; j < paramS.Length; j++)
                {
                    ParameterInfo param = paramS[j];
                    sbFormalParam.AppendFormat("a{0}/* {1} */{2}", j, param.ParameterType.Name, (j == paramS.Length - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == paramS.Length - 1 ? "" : ","), (j == 0 ? ", " : ""));
                }
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, type.Name, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)VCall.Oper.METHOD, "false");
                else
                    sb.AppendFormat(fmtStatic, type.Name, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)VCall.Oper.METHOD, "false");
            }

            overloadedCount = 0;
            overloadedIndex = 0;
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods)
    {
        /*
        * class
        * 0 class name
        * 1 fields
        * 2 properties
        * 3 methods
        */
        string fmt = @"
{0} = function() [[]]

// fields
{1}
// properties
{2}
// methods
{3}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, type.Name, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString());
        return sb;
    }

    public static void Generate()
    {
        /* 如果是枚举，单独处理 */
        if (type.IsEnum)
        {
            GenEnum();
            return;
        }

        if (type.IsInterface)
        {
            Debug.Log("Interface: " + type.ToString() + " ignored.");
            return;
        }

        JSMgr.ATypeInfo ti;
        int slot = JSMgr.AddTypeInfo(type, out ti);
        var sbFields = BuildFields(type, ti.fields, slot);
        var sbProperties = BuildProperties(type, ti.properties, slot);
        var sbMethods = BuildMethods(type, ti.methods, slot);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods);
        HandleStringFormat(sbClass);

        string fileName = Application.dataPath + "/StreamingAssets/JavaScript/Generated/" + type.Name + ".javascript";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sbClass.ToString());
        writer2.Close();

        return;


        var sb = new StringBuilder();
        



        MethodInfo[] methods = type.GetMethods(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
        for (int i = 0; i < methods.Length; i++)
        {
            sb.AppendLine(methods[i].Name);
        }
        var writer = OpenFile(tempFile, false);
        writer.Write(sb.ToString());
        writer.Close();

        return;
        
        
        {
            GenBegin();
            GenConstructor();
            GenRegister();
            GenEnd();
        }

        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }

    static void GenEnum()
    {
        var writer = OpenFile(enumFile, true/* append */);

        var sb = new StringBuilder();

        // 先写一句注释
        string fmtComment = @"// {0}
";
        sb.AppendFormat(fmtComment, type.ToString());

        // 把名字空间去掉
        string typeName = type.ToString();
        int lastDot = typeName.LastIndexOf('.');
        if (lastDot >= 0)
        {
            typeName = typeName.Substring(lastDot + 1);
        }

        string fmt = @"{0} = {0} || [[]];
";

        typeName.Replace('+', '.');

        // 处理+号
        // +号表示类里面的枚举
        //
        //例如有个枚举是 Hello+World+Flag，要先生成这2行：
        // Hello = Hello || {}
        // Hello.World = Hello.World || {}
        int start = 0;
        while (true)
        {
            int index = typeName.IndexOf('.', start);
            if (index <= 0)
                break;

            sb.AppendFormat(fmt, typeName.Substring(0, index));
            start = index + 1;
        }

        FieldInfo[] fields = type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
        
        sb.AppendFormat(fmt, typeName);

        string fmtField = @"{0}.{1} = {2};
";

        for (int i = 0; i < fields.Length; i++)
        {
            sb.AppendFormat(fmtField, typeName, fields[i].Name, (int)fields[i].GetValue(null));
        }
        string fmtEnter = @"
";
        sb.Append(fmtEnter);

        HandleStringFormat(sb);
        writer.Write(sb.ToString());
        writer.Close();
    }

    public static void Clear()
    {
        className = null;
        type = null;
        sb = new StringBuilder();
    }    
    /*
    public static void Generate()
    {
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
    */
    static void GenBegin()
    {
        string fmt = @"
using System;
using UnityEngine;

public class {0}Wrap
[[
";

        sb.AppendFormat(fmt, type.Name);
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
        sb.AppendFormat(fmt, type.Name);
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
        sb.AppendFormat(fmt, type.Name);

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


    static void WriteUsingSection(StreamWriter writer)
    {
        string fmt = @"using System;
using UnityEngine;
";
        writer.Write(fmt);
    }
    static StreamWriter OpenFile(string fileName, bool bAppend = false)
    {
        return new StreamWriter(fileName, bAppend, Encoding.UTF8);
    }

    static void HandleStringFormat(StringBuilder sb)
    {
        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }
}
