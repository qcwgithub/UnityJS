using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
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

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, ClassCallbackNames ccbn)
    {
        /*
        * fields
        * 0 class name
        * 1 field name
        * 2 field type
        */
        string fmt = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({0})vc.csObj).{1};
    else
    [[
        (({0})vc.csObj).{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";
        string fmtValueType = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({0})vc.csObj).{1};
    else
    [[
        {0} argThis = ({0})vc.csObj;
        argThis.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
        JSMgr.changeCSObj(vc.csObj, argThis);
    ]]
]]
";

        string fmtReadOnly = @"static void {0}_{1}(JSVCall vc)
[[
    vc.result = (({0})vc.csObj).{1};
]]
";
        string fmtStatic = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = {0}.{1};
    else
        {0}.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
]]
";
        string fmtStaticReadOnly = @"static void {0}_{1}(JSVCall vc)
[[
    vc.result = {0}.{1};
]]
";
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            bool bReadOnly = (field.IsInitOnly || field.IsLiteral);

            string f = fmt;
            if (field.IsStatic) f = bReadOnly ? fmtStaticReadOnly : fmtStatic;
            else if (bReadOnly) f = fmtReadOnly;
            else if (type.IsValueType) f = fmtValueType;

            sb.AppendFormat(f, type.Name, field.Name, field.FieldType);
            ccbn.fields.Add(type.Name + "_" + field.Name);
        }

        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, ClassCallbackNames ccbn)
    {
        /*
        * property
        * 0 class name
        * 1 property name
        * 2 property type name
        * 3 class full name
        */
        string fmt = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({3})vc.csObj).{1};
    else
    [[
        (({3})vc.csObj).{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";
        string fmtValueType = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = (({3})vc.csObj).{1};
    else
    [[
        {3} argThis = ({3})vc.csObj; // unboxing
        argThis.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
        JSMgr.changeCSObj(vc.csObj, argThis);
    ]]
]]
";
        string fmtStatic = @"static void {0}_{1}(JSVCall vc)
[[
    if (vc.bGet)
        vc.result = {3}.{1};
    else
    [[
        {3}.{1} = ({2})(vc.JSValue_2_CSObject(typeof({2}), vc.currentParamCount));
    ]]
]]
";
        string fmtReadOnly = @"static void {0}_{1}(JSVCall vc) [[ vc.result = (({3})vc.csObj).{1}; ]]
";
        string fmtReadOnlyStatic = @"static void {0}_{1}(JSVCall vc) [[ vc.result = {3}.{1}; ]]
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            bool bReadOnly = !property.CanWrite;
            string f = fmt;
            if (isStatic) f = bReadOnly ? fmtReadOnlyStatic : fmtStatic;
            else if (bReadOnly) f = fmtReadOnly;
            else if (type.IsValueType) f = fmtValueType;

            sb.AppendFormat(f, type.Name, property.Name, GetTypeFullName(property.PropertyType), GetTypeFullName(type));
            ccbn.properties.Add(type.Name + "_" + property.Name);
        }
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
    static StringBuilder GenListCSParam2(ParameterInfo[] ps)
    {
        StringBuilder sb = new StringBuilder();

        string fmt = "new JSVCall.CSParam({0}, {1}, {2}, {3}{4}, {5}), ";
        for (int i = 0; i < ps.Length; i++)
        {
            ParameterInfo p = ps[i];
            Type t = p.ParameterType;
            sb.AppendFormat(fmt, t.IsByRef ? "true" : "false", p.IsOptional ? "true" : "false", t.IsArray ? "true" : "false", "typeof(" + GetTypeFullName(t) + ")", t.IsByRef ? ".MakeByRefType()" : "", "null");
        }
        fmt = "new JSVCall.CSParam[][[{0}]]";
        StringBuilder sbX = new StringBuilder();
        sbX.AppendFormat(fmt, sb);
        return sbX;
    }
    public static StringBuilder BuildSpecialFunctionCall(ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid)
    {
        List<string> lstParam = new List<string>();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ps.Length; i++)
        {
            sb.AppendFormat("({0})vc.callParams[{1}]", GetTypeFullName(ps[i].ParameterType), i);
            lstParam.Add(sb.ToString());
            sb.Remove(0, sb.Length);
        }

        // 一定是 static
        if (methodName == "op_Addition")
            sb.AppendFormat("    {0}{1} + {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);
        else if (methodName == "op_Subtraction")
            sb.AppendFormat("    {0}{1} - {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);
        else if (methodName == "op_UnaryNegation")
            sb.AppendFormat("    {0}-{1};", returnVoid ? "" : "vc.result = ", lstParam[0]);
        else if (methodName == "op_Multiply")
            sb.AppendFormat("    {0}{1} * {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);
        else if (methodName == "op_Division")
            sb.AppendFormat("    {0}{1} / {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);
        else if (methodName == "op_Equality")
            sb.AppendFormat("    {0}{1} == {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);
        else if (methodName == "op_Inequality")
            sb.AppendFormat("    {0}{1} != {2};", returnVoid ? "" : "vc.result = ", lstParam[0], lstParam[1]);

        return sb;
    }
    public static StringBuilder BuildNormalFunctionCall(ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid, bool bConstructor)
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
        sb.Append("    int len = vc.callParamsLength;\r\n");
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

            StringBuilder sbSaveRefVariable = new StringBuilder();
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                {
                    // 接一下变量才能调
                    sbSaveRefVariable.AppendFormat("        vc.callParams[{0}] = arg{0};\r\n", i);
                }
            }

            /*
             * 0 参数个数
             * 1 类名
             * 2 函数名
             * 3 实参数列表
             * 4 vc.result = (如果有的话)
             * 5 可能要加 else
             * 6 对于ref/out 参数，需要先接一下，再使用
             * 7 对于ref/out 参数，调用完后，要保存到 vc.callParams 中去
             */
            if (bConstructor)
            {
                sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {4} new {1}{2}({3});
{7}
    ]]
", j, "", GetTypeFullName(type)/* 这里不能使用methodName，因为methodName是 .ctor*/, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable, sbSaveRefVariable);
            }
            else if (bStatic)
            {
                sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {4}{1}.{2}({3});
{7}
    ]]
", j, GetTypeFullName(type)/*className*/, methodName, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable, sbSaveRefVariable);
            }
            else
            {
                if (!type.IsValueType)
                {
                    sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {4}(({1})vc.csObj).{2}({3});
{7}
    ]]
", j, GetTypeFullName(type)/*className*/, methodName, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable, sbSaveRefVariable);
                }
                else // 如果是 ValueType 需要先拆箱再调用，之后还要保存
                {
                    sb.AppendFormat(@"    {5}if (len == {0}) 
    [[
{6}
        {1} argThis = ({1})vc.csObj;
        {4}argThis.{2}({3});
        JSMgr.changeCSObj(vc.csObj, argThis);
{7}
    ]]
", j, GetTypeFullName(type)/*className*/, methodName, sbP.ToString(), (returnVoid ? "" : "vc.result = "), (j == minNeedParams) ? "" : "else ", sbRefVariable, sbSaveRefVariable);
                }
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
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, ClassCallbackNames ccbn)
    {
        /*
        * methods
        * 0 函数名
        * 1 list<CSParam> 生成
        * 2 函数调用
        */
        string fmt = @"
static bool {0}(JSVCall vc, int start, int count)
[[
{1}
    return true;
]]
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo cons = constructors[i];
            // 这里假设实例函数不会和静态函数同名
            ParameterInfo[] paramS = cons.GetParameters();

            int olIndex = i + 1; // 对于 构造函数来说，总是重载
            bool returnVoid = false;

            string functionName = type.Name + "_" + type.Name + (olIndex > 0 ? olIndex.ToString() : "") + (cons.IsStatic ? "_S" : "");

            sb.AppendFormat(fmt, functionName,
                BuildNormalFunctionCall(paramS, type.Name, cons.Name, cons.IsStatic, returnVoid, true));

            ccbn.constructors.Add(functionName);
            ccbn.constructorsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int[] olInfo, ClassCallbackNames ccbn)
    {
        /*
        * methods
        * 0 函数名
        * 1 list<CSParam> 生成
        * 2 函数调用
        */
        string fmt = @"
static bool {0}(JSVCall vc, int start, int count)
[[
{1}
    return true;
]]
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];
            // 这里假设实例函数不会和静态函数同名
            ParameterInfo[] paramS = method.GetParameters();

            int olIndex = olInfo[i];
            bool returnVoid = (method.ReturnType == typeof(void));

            string functionName = type.Name + "_" + method.Name + (olIndex > 0 ? olIndex.ToString() : "") + (method.IsStatic ? "_S" : "");

            sb.AppendFormat(fmt, functionName,
                
                method.IsSpecialName ? BuildSpecialFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid)
                : BuildNormalFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid, false));

            ccbn.methods.Add(functionName);
            ccbn.methodsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods, StringBuilder sbConstructors, StringBuilder sbRegister)
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
////////////////////// {0} ///////////////////////////////////////
// constructors
{4}
// fields
{1}
// properties
{2}
// methods
{3}

//register
{5}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, type.Name, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString(), sbConstructors.ToString(), sbRegister.ToString());
        return sb;
    }

    // 用于记录生成过程中的信息
    public class ClassCallbackNames
    {
        // 类名
        public Type type;

        // 生成的函数名
        public List<string> fields;
        public List<string> properties;
        public List<string> constructors;
        public List<string> methods;

        // 生成的，生成 CSParam 的代码
        public List<string> constructorsCSParam;
        public List<string> methodsCSParam;
    }
    public static List<ClassCallbackNames> allClassCallbackNames;
    static StringBuilder BuildRegisterFunction(ClassCallbackNames ccbn, JSMgr.ATypeInfo ti)
    {
        string fmt = @"
public static void __Register()
[[
    JSMgr.CallbackInfo ci = new JSMgr.CallbackInfo();
    ci.type = typeof({0});
    ci.fields = new JSMgr.CSCallbackField[]
    [[
{1}
    ]];
    ci.properties = new JSMgr.CSCallbackProperty[]
    [[
{2}
    ]];
    ci.constructors = new JSMgr.MethodCallBackInfo[]
    [[
{3}
    ]];
    ci.methods = new JSMgr.MethodCallBackInfo[]
    [[
{4}
    ]];
    JSMgr.allCallbackInfo.Add(ci);
]]
";
        StringBuilder sb = new StringBuilder();

        StringBuilder sbField = new StringBuilder();
        StringBuilder sbProperty = new StringBuilder();
        StringBuilder sbCons = new StringBuilder();
        StringBuilder sbMethod = new StringBuilder();

        for (int i = 0; i < ccbn.fields.Count; i++)
            sbField.AppendFormat("        {0},\r\n", ccbn.fields[i]);
        for (int i = 0; i < ccbn.properties.Count; i++)
            sbProperty.AppendFormat("        {0},\r\n", ccbn.properties[i]);
        for (int i = 0; i < ccbn.constructors.Count; i++)
            sbCons.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\r\n", ccbn.constructors[i], ccbn.constructorsCSParam[i], ti.constructors[i].Name);
        for (int i = 0; i < ccbn.methods.Count; i++)
            sbMethod.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\r\n", ccbn.methods[i], ccbn.methodsCSParam[i], ti.methods[i].Name);

        sb.AppendFormat(fmt, GetTypeFullName(ccbn.type), sbField, sbProperty, sbCons, sbMethod);
        return sb;
    }
    public static void GenerateRegisterAll()
    {
        string fmt = @"
public class CSharpGenerated
[[
    public static void RegisterAll()
    [[
{0}
    ]]
]]
";
        StringBuilder sbA = new StringBuilder();
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            sbA.AppendFormat("        {0}Generated.__Register();\r\n", JSBindingSettings.classes[i].Name);
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(fmt, sbA);
        HandleStringFormat(sb);

        string fileName = JSMgr.csGeneratedDir + "/" + "CSharpGenerated.cs";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sb.ToString());
        writer2.Close();
    }
    public static void GenerateClass()
    {
        /*if (type.IsInterface)
        {
            Debug.Log("Interface: " + type.ToString() + " ignored.");
            return;
        }*/

        JSMgr.ATypeInfo ti;
        /*int slot = */JSMgr.AddTypeInfo(type, out ti);
//         var sbCons = BuildConstructors(type, ti.constructors, slot);
//         var sbFields = BuildFields(type, ti.fields, slot);
//         var sbProperties = BuildProperties(type, ti.properties, slot);
//         var sbMethods = BuildMethods(type, ti.methods, slot);
//         var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons);
//         HandleStringFormat(sbClass);

        ClassCallbackNames ccbn = new ClassCallbackNames();
        {
            ccbn.type = type;
            ccbn.fields = new List<string>(ti.fields.Length);
            ccbn.properties = new List<string>(ti.properties.Length);
            ccbn.constructors = new List<string>(ti.constructors.Length);
            ccbn.methods = new List<string>(ti.methods.Length);

            ccbn.constructorsCSParam = new List<string>(ti.constructors.Length);
            ccbn.methodsCSParam = new List<string>(ti.methods.Length);
        }

        var sbFields = BuildFields(type, ti.fields, ccbn);
        var sbProperties = BuildProperties(type, ti.properties, ccbn);
        var sbMethods = BuildMethods(type, ti.methods, ti.methodsOLInfo, ccbn);
        var sbCons = BuildConstructors(type, ti.constructors, ccbn);
        var sbRegister = BuildRegisterFunction(ccbn, ti);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons, sbRegister);

        /*
         * 0 typeName
         * 1 class contents
         * 2 type namespace
         */
        string fmtFile = @"
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    //static Dictionary<Type, string> typeClassName = new Dictionary<Type, string>();
    //static string className = string.Empty;

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

        allClassCallbackNames = null;
        allClassCallbackNames = new List<ClassCallbackNames>(JSBindingSettings.classes.Length);

        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            CSGenerator.Clear();
            CSGenerator.type = JSBindingSettings.classes[i];
            CSGenerator.GenerateClass();
        }
        GenerateRegisterAll();

        CSGenerator.OnEnd();

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
