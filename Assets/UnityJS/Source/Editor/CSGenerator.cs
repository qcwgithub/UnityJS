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
using UnityEngine.SocialPlatforms;
public static class CSGenerator
{
    // 输入参数
    static StringBuilder sb = null;
    public static Type type = null;

    // 一些配置

    /* 枚举统一输出到同一个地方去 */
    static string enumFile = JSMgr.jsGeneratedDir + "/enum.javascript";
    static string tempFile = JSMgr.jsDir + "/temp.javascript";

    // 开始生成，有一些事情要处理
    public static void OnBegin()
    {
        // 清除导出的枚举文件
        var writer = OpenFile(enumFile, false);
        writer.Close();

        JSMgr.ClearTypeInfo();

    }
    public static void OnEnd()
    {

    }

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields)
    {
        /*
        * fields
        * 0 class name
        * 1 field name
        * 2 field type
        */
        string fmt = @"void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({0})vc.csObj).{1};
    else
    [[
        (({0})vc.csObj).{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";
        string fmtReadOnly = @"void {0}_{1}(JSVCall vc)
[[
    vc.result = (({0})vc.csObj).{1};
]]
";
        string fmtStatic = @"void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = {0}.{1};
    else
        {0}.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
]]
";
        string fmtStaticReadOnly = @"void {0}_{1}(JSVCall vc)
[[
    vc.result = {0}.{1};
]]
";
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            // Skip Obsolete
            if (IsMemberObsolete(field))
                continue;


            bool bReadOnly = (field.IsInitOnly || field.IsLiteral);

            if (!field.IsStatic)
                sb.AppendFormat(bReadOnly ? fmtReadOnly : fmt, type.Name, field.Name, field.FieldType);
            else
                sb.AppendFormat(bReadOnly ? fmtStaticReadOnly : fmtStatic, type.Name, field.Name, field.FieldType);
        }
        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties)
    {
        /*
        * property
        * 0 class name
        * 1 property name
        * 2 property type
        */
        string fmt = @"void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({0})vc.csObj).{1};
    else
    [[
        (({0})vc.csObj).{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";.............................................................................................
        string fmtStatic = @"void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = {0}.{1};
    else
    [[
        {0}.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";
        string fmtReadOnly = @"void {0}_{1}(JSVCall vc) [[ vc.result = (({0})vc.csObj).{1}; ]]
";
        string fmtReadOnlyStatic = @"void {0}_{1}(JSVCall vc) [[ vc.result = {0}.{1}; ]]
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];

            // 在跳过不需要的 property 之前，要先处理这个
            // 否则函数又把 get_/set_ 函数加进去了
            MethodInfo[] accessors = property.GetAccessors();
            foreach (var v in accessors)
            {
                if (!classPropertyAccessors.ContainsKey(v.Name))
                    classPropertyAccessors.Add(v.Name, 0);
            }

            if (property.Name == "Item") //[] not support
                continue;

            // Skip Obsolete
            if (IsMemberObsolete(property))
                continue;

            bool isStatic = false;
            isStatic = accessors[0].IsStatic;

            bool bReadOnly = !property.CanWrite;
            if (!isStatic)
                sb.AppendFormat((bReadOnly ? fmtReadOnly : fmt), type.Name, property.Name, GetTypeFullName(property.PropertyType));
            else
                sb.AppendFormat((bReadOnly ? fmtReadOnlyStatic : fmtStatic), type.Name, property.Name, GetTypeFullName(property.PropertyType));
        }
        return sb;
    }
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
    static StringBuilder GenListCSParam(ParameterInfo[] ps)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"    vc.lstCSParam.Clear();
");

        string fmt = @"    vc.lstCSParam.Add(new JSVCall.CSParam({0}, {1}, {2}, {3}{4}, {5}));
";
        for (int i = 0; i < ps.Length; i++)
        {
            ParameterInfo p = ps[i];
            Type t = p.ParameterType;
            sb.AppendFormat(fmt, t.IsByRef?"true":"false", p.IsOptional?"true":"false", t.IsArray?"true":"false", "typeof("+GetTypeFullName(t)+")", t.IsByRef?".MakeByRefType()":"","null");
        }
        return sb;
    }
    public static StringBuilder BuildSpecialFunctionCall(MethodInfo method)
    {
        if (!method.IsSpecialName)
            return null;
        StringBuilder sb = new StringBuilder();
//        Matrix4x4
        return null;
    }
    public static StringBuilder BuildNormalFunctionCall(ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid)
    {
        // 最少需要几个参数
        int minNeedParams = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].IsOptional)
                break;
            minNeedParams++;
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("    int len = vc.callParams.Length;\r\n");
        for (int j = minNeedParams; j <= ps.Length; j++)
        {
            StringBuilder sbRefVariable = new StringBuilder();

            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                {
                    // 接一下变量才能调
                    sbRefVariable.AppendFormat("        {0} arg{1} = ({0})vc.callParams[{1}];\r\n", GetTypeFullName(p.ParameterType), i);
                }
            }

            // sP：实参
            StringBuilder sbP = new StringBuilder();
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                    sbP.AppendFormat("{0} arg{1}{2}", p.IsOut ? "out" : "ref", i, (i == ps.Length - 1 ? "" : ", "));
                else
                    sbP.AppendFormat("({0})vc.callParams[{1}]{2}", GetTypeFullName(ps[i].ParameterType), i, (i == ps.Length - 1 ? "" : ", "));
            }

            if (bStatic)
            {
                sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {4}{1}.{2}({3});
    ]]
", j, className, methodName, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable);
            }
            else
            {
                sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {4}(({1})vc.csObj).{2}({3});
    ]]
", j, className, methodName, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable);

            }

        }

        return sb;
    }
    public static string GetTypeFullName(Type type)
    {
        if (type.IsByRef)
            type = type.GetElementType();

        if (!type.IsGenericType)
        {
            string rt = type.FullName;
            rt = rt.Replace('+', '.');
            return rt;
        }
        else
        {
            string fatherName = type.Name.Substring(0, type.Name.Length - 2);
            Type[] ts = type.GetGenericArguments();
            fatherName += "<";
            for (int i = 0; i < ts.Length; i++)
            {
                fatherName += ts[i].Name;
                if (i != ts.Length - 1)
                    fatherName += ", ";
            }
            fatherName += ">";
            fatherName.Replace('+', '.');
            return fatherName;
        }
    }

    public static bool IsMemberObsolete(MemberInfo mi)
    {
        object[] attrs = mi.GetCustomAttributes(true);
        for (int j = 0; j < attrs.Length; j++)
        {
            if (attrs[j].GetType() == typeof(System.ObsoleteAttribute))
            {
                return true;
            }
        }
        return false;
    }

    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods)
    {
        /*
        * methods
        * 0 class name
        * 1 method name
        * 2 函数名后缀
        * 3 list<CSParam> 生成
        * 4 函数调用
        */
        string fmt = @"
bool {0}_{1}{2}(JSVCall vc, int start, int count)
[[
    if (!vc.ExtractJSParams(start, count)) 
        return false;
{3}
    vc.callParams = vc.BuildMethodArgs(false);
    if (null == vc.callParams)
        return false;
{4}
    return true;
]]
";

        /*
         * 先计算一下哪些是重载函数 
         */
        int overloadedIndex = 0;
        bool bOL = false;
        Dictionary<int, int> dic = new Dictionary<int, int>();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];
            // 这里假设实例函数不会和静态函数同名
            ParameterInfo[] paramS = method.GetParameters();

            if (bOL)
                dic.Add(i, overloadedIndex);

            if (i < methods.Length - 1 && method.Name == methods[i + 1].Name)
            {
                if (!bOL)
                {
                    overloadedIndex = 0;
                    bOL = true;
                    dic.Add(i, overloadedIndex);
                }
                overloadedIndex++;
            }
            else
            {
                bOL = false;
                overloadedIndex = 0;
            }
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            // skip property accessor
            if (method.IsSpecialName &&
                classPropertyAccessors.ContainsKey(method.Name))
                continue;

            // Skip Obsolete
            if (IsMemberObsolete(method))
                continue;


            // 这里假设实例函数不会和静态函数同名
            ParameterInfo[] paramS = method.GetParameters();

            int overloadIndex = -1;
            bool bOverload = false;
            if (dic.TryGetValue(i, out overloadIndex))
                bOverload = true;

            bool returnVoid = (method.ReturnType == typeof(void));

            sb.AppendFormat(fmt, type.Name, method.Name, (bOverload ? overloadIndex.ToString() : ""), GenListCSParam(paramS).ToString(), BuildNormalFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods/*, StringBuilder sbConstructors*/)
    {
        /*
        * class
        * 0 class name
        * 1 fields
        * 2 properties
        * 3 methods
        */
        string fmt = @"
////////////////////// {0} ///////////////////////////////////////

// fields
{1}
// properties
{2}
// methods
{3}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, type.Name, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString()/*, sbConstructors.ToString()*/);
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
//         var sbCons = BuildConstructors(type, ti.constructors, slot);
//         var sbFields = BuildFields(type, ti.fields, slot);
//         var sbProperties = BuildProperties(type, ti.properties, slot);
//         var sbMethods = BuildMethods(type, ti.methods, slot);
//         var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons);
//         HandleStringFormat(sbClass);

        var sbFields = BuildFields(type, ti.fields);
        var sbProperties = BuildProperties(type, ti.properties);
        var sbMethods = BuildMethods(type, ti.methods);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods);

        string fmtFile = @"
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using {2};

public class {0}Generated
[[
{1}
]]
";
        var sbFile = new StringBuilder();
        sbFile.AppendFormat(fmtFile, type.Name, sbClass, type.Namespace.ToString());
        HandleStringFormat(sbFile);

        string fileName = JSMgr.csGeneratedDir + "/" + type.Name + "Generated.cs";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sbFile.ToString());
        writer2.Close();
    }

    static void GenerateEnum()
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
        return new StreamWriter(fileName, bAppend, Encoding.UTF8);
    }

    static void HandleStringFormat(StringBuilder sb)
    {
        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }

    /* 
     * Some classes have another name
     * for example: js has 'Object'
     */
    static Dictionary<Type, string> typeClassName = new Dictionary<Type, string>();
    static string className = string.Empty;

    static Dictionary<string, int> classPropertyAccessors = new Dictionary<string, int>();

//     public class TEST2
//     {
//         public void Add()
//        
    public static void MakeJJJ(ref int i)
    {

    }
    [MenuItem("CSGenerator/Generate Class Bindings")]
    public static void GenerateClassBindings()
    {
//         typeClassName.Add(typeof(UnityEngine.Object), "UnityObject");

//         Type t = typeof(Dictionary<int,string>);
//         Debug.Log(t);
//         Debug.Log(t.Name);
//         Debug.Log(t.FullName);
//         Debug.Log(t.ToString());
//         Type tD = t.GetGenericTypeDefinition();
//         Debug.Log(tD);
//         Debug.Log(tD.Name);
//         Debug.Log(tD.FullName);
//         Debug.Log(tD.ToString());
        //int op = 1;
        //object oj = op;
        //Debug.Log(GetTypeFullName(typeof(bool).MakeByRefType()));
        //MakeJJJ(ref oj);
//         {
//             Type t = typeof(GameObject);
//             MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//             for (int i = 0; i < methods.Length; i++ )
//             {
//                 MethodInfo method = methods[i];
//                 if (method.Name != "AddComponent" || method.IsGenericMethod || method.IsGenericMethodDefinition)
//                     continue;
// 
//                 ParameterInfo[] ps = method.GetParameters();
//                 bool b1 = ps[0].ParameterType.IsGenericParameter;
//                 bool b2 = ps[0].ParameterType.IsGenericType;
//                 bool b3 = ps[0].ParameterType.IsGenericTypeDefinition;
//                 Type[] ga = ps[0].ParameterType.GetGenericArguments();
//                 Debug.Log(b1.ToString() + b2.ToString() + b3.ToString());
//             }
//         }
        //return;
// 
        CSGenerator.OnBegin();
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            CSGenerator.Clear();
            CSGenerator.type = JSBindingSettings.classes[i];
//             if (!typeClassName.TryGetValue(type, out className))
//                 className = type.Name;
            classPropertyAccessors.Clear();
            CSGenerator.GenerateClass();
        }

        
//         type = typeof(GameObject);
//         GenerateClass();

        //CSGenerator.OnEnd();

        Debug.Log("Generate Class Bindings finish. total = " + JSBindingSettings.classes.Length.ToString());
    }

    [MenuItem("CSGenerator/Output All Types in UnityEngine")]
    public static void OutputAllTypesInUnityEngine()
    {
        var asm = typeof(GameObject).Assembly;
        var alltypes = asm.GetTypes();
        var writer = new StreamWriter(tempFile, false, Encoding.UTF8);

        writer.WriteLine("// enum");
        writer.WriteLine("");
        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if (alltypes[i].IsEnum)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.WriteLine("");
        writer.WriteLine("// interface");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if (alltypes[i].IsInterface)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.WriteLine("");
        writer.WriteLine("// class");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
                alltypes[i].IsClass)
                writer.WriteLine(alltypes[i].ToString());
        }


        writer.WriteLine("");
        writer.WriteLine("// ValueType");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
                !alltypes[i].IsClass && alltypes[i].IsValueType)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.Close();

        Debug.Log("Output All Types in UnityEngine finish, file: " + tempFile);
        return;
    }
}
