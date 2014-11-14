using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


public static class JSGenerator
{
    // input
    static StringBuilder sb = null;
    public static Type type = null;

    static string enumFile = JSMgr.jsGeneratedDir + "/enum.javascript";
    static string tempFile = JSMgr.jsDir + "/temp.javascript";

    public static void OnBegin()
    {
        // clear generated enum files
        var writer = OpenFile(enumFile, false);
        writer.Close();

        JSMgr.ClearTypeInfo();

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
                sb.AppendFormat(fmt, className, field.Name, slot, i, (int)JSVCall.Oper.GET_FIELD, (int)JSVCall.Oper.SET_FIELD, field.FieldType.Name,
                    (field.IsInitOnly || field.IsLiteral) ? "ReadOnly" : ""
                    );
            else
                sb.AppendFormat(fmtStatic, className, field.Name, slot, i, (int)JSVCall.Oper.GET_FIELD, (int)JSVCall.Oper.SET_FIELD, field.FieldType.Name,
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
        * 8 isStatic
        */
        string fmt = @"
/* {7} {6} */
Object.defineProperty({0}.prototype, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, {8}, this); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, {8}, this, v); ]]
]]);
";
        string fmtStatic = @"
/* {7} {6} */
Object.defineProperty({0}, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, {8}); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, {8}, v); ]]
]]);
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            if (property.Name == "Item") //[] not support
                continue;

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            //             if (property.Name == "Item")
            //             {
            //                 Debug.Log("");
            //             }
            //             if (property.IsSpecialName)
            //             {
            //                 if (!mDictJJ.ContainsKey(property.Name))
            //                     mDictJJ.Add(property.Name, "");
            //             }

            sb.AppendFormat(isStatic ? fmtStatic : fmt, className, property.Name, slot, i, (int)JSVCall.Oper.GET_PROPERTY, (int)JSVCall.Oper.SET_PROPERTY, property.PropertyType.Name,
                (property.CanRead && property.CanWrite) ? "" : (property.CanRead ? "ReadOnly" : "WriteOnly"), (isStatic ? "true" : "false")
                );
        }
        return sb;
    }
    //static Dictionary<string, string> mDictJJ = new Dictionary<string, string>();
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, int slot)
    {
        /*
         * 0 op
         * 1 slot
         * 2 index
         * 3 true (isStatic)
         * 4 args
         * 5 Class name
         * 6 overload count
         * 7 formal parameters
         */
        string fmt = @"{5} = function({7}) [[
    /* overloaded {6} */
    return CS.Call({0}, {1}, {2}, {3}, {8}{4});
]]";
        bool bOverload = constructors.Length > 0;
        int overloadedMaxParamCount = 0;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo con = constructors[i];
            ParameterInfo[] ps = con.GetParameters();
            overloadedMaxParamCount = Math.Max(ps.Length, overloadedMaxParamCount);

        }
        StringBuilder sbFormalParam = new StringBuilder();
        StringBuilder sbActualParam = new StringBuilder();
        for (int j = 0; j < overloadedMaxParamCount; j++)
        {
            sbFormalParam.AppendFormat("a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "));
            sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "), (j == 0 ? ", " : ""));
        }

        sb.AppendFormat(fmt, (int)JSVCall.Oper.CONSTRUCTOR, slot, 0, "true", sbActualParam, className, constructors.Length, sbFormalParam, bOverload ? "true" : "false");

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
         * 9 some information
        */
        string fmt = @"
/* {6} {9} */
{0}.prototype.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, false, this, {8}{5}); ]]";
        string fmtStatic = @"
/* static {6} {9} */
{0}.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, true, {8}{5}); ]]";

        int overloadedIndex = 0;
        int overloadedCount = 0;
        int overloadedMaxParamCount = 0;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            // here assumes static functions don't have same name with instance functions
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
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "), (j == 0 ? ", " : ""));
                }
                sb.AppendFormat(@"
/* overloaded {0} */", overloadedCount);
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, className, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "true", "");
                else
                    sb.AppendFormat(fmtStatic, className, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "true", "");
            }
            else
            {
                StringBuilder sbInfo = new StringBuilder();
                sbInfo.AppendFormat("{0}", method.IsSpecialName ? "SPECIAL" : "");

                for (int j = 0; j < paramS.Length; j++)
                {
                    ParameterInfo param = paramS[j];
                    sbFormalParam.AppendFormat("a{0}/* {1} */{2}", j, param.ParameterType.Name, (j == paramS.Length - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == paramS.Length - 1 ? "" : ", "), (j == 0 ? ", " : ""));
                }
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, className, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", sbInfo);
                else
                    sb.AppendFormat(fmtStatic, className, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", sbInfo);
            }

            overloadedCount = 0;
            overloadedIndex = 0;
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods, StringBuilder sbConstructors)
    {
        /*
        * class
        * 0 class name
        * 1 fields
        * 2 properties
        * 3 methods
        * 4 constructors
        */
        string fmt = @"
{4}

// fields
{1}
// properties
{2}
// methods
{3}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, className, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString(), sbConstructors.ToString());
        return sb;
    }

    public static void GenerateClass()
    {
        /*if (type.IsInterface)
        {
            Debug.Log("Interface: " + type.ToString() + " ignored.");
            return;
        }*/

        JSMgr.ATypeInfo ti;
        int slot = JSMgr.AddTypeInfo(type, out ti);
        var sbCons = BuildConstructors(type, ti.constructors, slot);
        var sbFields = BuildFields(type, ti.fields, slot);
        var sbProperties = BuildProperties(type, ti.properties, slot);
        var sbMethods = BuildMethods(type, ti.methods, slot);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons);
        HandleStringFormat(sbClass);

        string fileName = JSMgr.jsGeneratedDir + "/" + className + ".javascript";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sbClass.ToString());
        writer2.Close();
    }

    static void GenerateEnum()
    {
        var writer = OpenFile(enumFile, true/* append */);

        var sb = new StringBuilder();

        // comment line
        string fmtComment = @"// {0}
";
        sb.AppendFormat(fmtComment, type.ToString());

        // remove name space
        string typeName = type.ToString();
        int lastDot = typeName.LastIndexOf('.');
        if (lastDot >= 0)
        {
            typeName = typeName.Substring(lastDot + 1);
        }

        string fmt = @"{0} = {0} || [[]];
";

        typeName.Replace('+', '.');

        // handle '+'
        // '+' means an enum inside a class
        //
        // for example, Hello+World+Flag, 2 lines will be generated:
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
        type = null;
        sb = new StringBuilder();
    }
    static void GenEnd()
    {
        string fmt = @"
]]
";
        sb.Append(fmt);
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
        Encoding utf8NoBom = new UTF8Encoding(false);
        return new StreamWriter(fileName, bAppend, utf8NoBom);
    }

    static void HandleStringFormat(StringBuilder sb)
    {
        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }

    [MenuItem("JS for Unity/Generate JS Enum Bindings")]
    public static void GenerateEnumBindings()
    {
        JSGenerator.OnBegin();

        for (int i = 0; i < JSBindingSettings.enums.Length; i++)
        {
            JSGenerator.Clear();
            JSGenerator.type = JSBindingSettings.enums[i];
            JSGenerator.GenerateEnum();
        }

        JSGenerator.OnEnd();

        Debug.Log("Generate JS Enum Bindings finish. total = " + JSBindingSettings.enums.Length.ToString());
    }

    /* 
     * Some classes have another name
     * for example: js has 'Object'
     */
    static Dictionary<Type, string> typeClassName = new Dictionary<Type, string>();
    static string className = string.Empty;


    [MenuItem("JS for Unity/Generate JS class Bindings")]
    public static void GenerateClassBindings()
    {
        if (!typeClassName.ContainsKey(typeof(UnityEngine.Object)))
            typeClassName.Add(typeof(UnityEngine.Object), "UnityObject");

        JSGenerator.OnBegin();
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            JSGenerator.Clear();
            JSGenerator.type = JSBindingSettings.classes[i];
            //             if (type != typeof(Physics))
            //                 continue;
            if (!typeClassName.TryGetValue(type, out className))
                className = type.Name;
            JSGenerator.GenerateClass();
        }

        JSGenerator.OnEnd();

        Debug.Log("Generate JS Class Bindings finish. total = " + JSBindingSettings.classes.Length.ToString());
    }

    //     [MenuItem("JS for Unity/Output All Types in UnityEngine")]
    //     public static void OutputAllTypesInUnityEngine()
    //     {
    //         var asm = typeof(GameObject).Assembly;
    //         var alltypes = asm.GetTypes();
    //         var writer = new StreamWriter(tempFile, false, Encoding.UTF8);
    // 
    //         writer.WriteLine("// enum");
    //         writer.WriteLine("");
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if (alltypes[i].IsEnum)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// interface");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if (alltypes[i].IsInterface)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// class");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
    //                 alltypes[i].IsClass)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// ValueType");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
    //                 !alltypes[i].IsClass && alltypes[i].IsValueType)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.Close();
    // 
    //         Debug.Log("Output All Types in UnityEngine finish, file: " + tempFile);
    //         return;
    //    }
}