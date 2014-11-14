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

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, ClassCallbackNames ccbn)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            var sbCall = new StringBuilder();

            FieldInfo field = fields[i];

            sb.AppendFormat("static void {0}_{1}(JSVCall vc)\r\n[[\r\n", type.Name, field.Name);

            bool bReadOnly = (field.IsInitOnly || field.IsLiteral);
            if (!bReadOnly)
                sb.Append("if (vc.bGet)\r\n");

            
            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);

            // get 部分
            if (field.IsStatic)
                sbCall.AppendFormat("{0}.{1}", type.Name, field.Name);
            else
                sbCall.AppendFormat("(({0})vc.csObj).{1}", type.Name, field.Name);
            sb.AppendFormat("    {0};\r\n", BuildReturnObject(field.FieldType, sbCall.ToString()));

            // set 部分
            if (!bReadOnly)
            {
                sb.Append("else\r\n");
                if (field.IsStatic)
                    sb.AppendFormat("{0}.{1} = ({2}){3};", type.Name, field.Name, field.FieldType, BuildRetriveParam(field.FieldType));
                else
                {
                    if (type.IsValueType)
                    {
                        sb.AppendFormat("[[\r\n    {0} argThis = ({0})vc.csObj;\r\n", type.Name);
                        sb.AppendFormat("    argThis.{0} = ({1}){2};\r\n", field.Name, field.FieldType, BuildRetriveParam(field.FieldType));
                        sb.Append("    JSMgr.changeCSObj(vc.csObj, argThis);\r\n]]\r\n");
                    }
                    else
                    {
                        sb.AppendFormat("(({0})vc.csObj).{1} = ({2});", GetTypeFullName(type), field.Name, BuildRetriveParam(field.FieldType));
                    }
                }
            }

            sb.AppendFormat("]]\r\n");

//             string f = fmt;
//             if (field.IsStatic) f = bReadOnly ? fmtStaticReadOnly : fmtStatic;
//             else if (bReadOnly) f = fmtReadOnly;
//             else if (type.IsValueType) f = fmtValueType;
// 
//             sb.AppendFormat(f, type.Name, field.Name, field.FieldType);
            ccbn.fields.Add(type.Name + "_" + field.Name);
        }

        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, ClassCallbackNames ccbn)
    {
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            var sbCall = new StringBuilder();

            PropertyInfo property = properties[i];

            sb.AppendFormat("static void {0}_{1}(JSVCall vc)\r\n[[\r\n", type.Name, property.Name);

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            bool bReadOnly = !property.CanWrite;


            if (!bReadOnly)
                sb.Append("if (vc.bGet)\r\n");


            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);

            // get 部分
            if (isStatic)
                sbCall.AppendFormat("{0}.{1}", GetTypeFullName(type), property.Name);
            else
                sbCall.AppendFormat("(({0})vc.csObj).{1}", GetTypeFullName(type), property.Name);
            sb.AppendFormat("    {0};\r\n", BuildReturnObject(property.PropertyType, sbCall.ToString()));

            // set 部分
            if (!bReadOnly)
            {
                sb.Append("else\r\n");
                if (isStatic)
                    sb.AppendFormat("{0}.{1} = ({2}){3};", GetTypeFullName(type), property.Name, GetTypeFullName(property.PropertyType), BuildRetriveParam(property.PropertyType));
                else
                {
                    if (type.IsValueType)
                    {
                        sb.AppendFormat("[[\r\n    {0} argThis = ({0})vc.csObj;\r\n", GetTypeFullName(type));
                        sb.AppendFormat("    argThis.{0} = ({1}){2};\r\n", property.Name, GetTypeFullName(property.PropertyType), BuildRetriveParam(property.PropertyType));
                        sb.Append("    JSMgr.changeCSObj(vc.csObj, argThis);\r\n]]\r\n");
                    }
                    else
                    {
                        sb.AppendFormat("(({0})vc.csObj).{1} = ({2});", GetTypeFullName(type), property.Name, BuildRetriveParam(property.PropertyType));
                    }
                }
            }

            sb.AppendFormat("]]\r\n");

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
    // 取得：获取参数并转换为相应类型的表达式
    public static string BuildRetriveParam(Type paramType)
    {
        if (paramType == typeof(Boolean)) return "vc.getBool()";
        else if (paramType == typeof(String)) return "vc.getString()";
        else if (paramType == typeof(Char)) return "vc.getChar()";
        else if (paramType == typeof(Byte)) return "vc.getByte()";
        else if (paramType == typeof(SByte)) return "vc.getSByte()";
        else if (paramType == typeof(UInt16)) return "vc.getUInt16()";
        else if (paramType == typeof(Int16)) return "vc.getInt16()";
        else if (paramType == typeof(UInt32)) return "vc.getUInt32()";
        else if (paramType == typeof(Int32)) return "vc.getInt32()";
        else if (paramType == typeof(UInt64)) return "vc.getUInt64()";
        else if (paramType == typeof(Int64)) return "vc.getInt64()";
        else if (paramType.IsEnum) return "(" + GetTypeFullName(paramType) + ")" + "vc.getEnum()";
        else if (paramType == typeof(Single)) return "vc.getFloat()";
        else if (paramType == typeof(Double)) return "vc.getDouble()";
        else return "(" + GetTypeFullName(paramType) + ")" + "vc.getObject()";
    }
    public static string BuildReturnObject(Type paramType, string callString)
    {
        if (paramType == typeof(Boolean)) return "vc.returnBool(" + callString + ")";
        else if (paramType == typeof(String)) return "vc.returnString(" + callString + ")";
        else if (paramType == typeof(Char)) return "vc.returnChar(" + callString + ")";
        else if (paramType == typeof(Byte)) return "vc.returnByte(" + callString + ")";
        else if (paramType == typeof(SByte)) return "vc.returnSByte(" + callString + ")";
        else if (paramType == typeof(UInt16)) return "vc.returnUInt16(" + callString + ")";
        else if (paramType == typeof(Int16)) return "vc.returnInt16(" + callString + ")";
        else if (paramType == typeof(UInt32)) return "vc.returnUInt32(" + callString + ")";
        else if (paramType == typeof(Int32)) return "vc.returnInt32(" + callString + ")";
        else if (paramType == typeof(UInt64)) return "vc.returnUInt64(" + callString + ")";
        else if (paramType == typeof(Int64)) return "vc.returnInt64(" + callString + ")";
        else if (paramType.IsEnum) return "vc.returnEnum((Int32)" + callString + ")";
        else if (paramType == typeof(Single)) return "vc.returnFloat(" + callString + ")";
        else if (paramType == typeof(Double)) return "vc.returnDouble(" + callString + ")";
        else return "vc.returnObject(\"" + paramType.Name + "\", " + callString + ")";
    }
    // 是否直接返回
    // 如果是  就是直接return 扣号里是调用语句
    // 如果不是  就是先用 object 接一下  再 return
    public static bool IsDirectReturn(Type paramType)
    {
        if (paramType == typeof(Boolean)) return true;
        else if (paramType == typeof(String)) return true;
        else if (paramType == typeof(Char)) return true;
        else if (paramType == typeof(Byte)) return true;
        else if (paramType == typeof(SByte)) return true;
        else if (paramType == typeof(UInt16)) return true;
        else if (paramType == typeof(Int16)) return true;
        else if (paramType == typeof(UInt32)) return true;
        else if (paramType == typeof(Int32)) return true;
        else if (paramType == typeof(UInt64)) return true;
        else if (paramType == typeof(Int64)) return true;
        else if (paramType.IsEnum) return true;
        else if (paramType == typeof(Single)) return true;
        else if (paramType == typeof(Double)) return true;
        else return false;
    }
    public static StringBuilder BuildNormalFunctionCall(ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid, Type returnType, bool bConstructor)
    {
        bool directReturn = true;
        if (!bConstructor)
            IsDirectReturn(returnType);

        // minimal params needed
        int minNeedParams = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].IsOptional)
                break;
            minNeedParams++;
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("    int len = count;\r\n");
        for (int j = minNeedParams; j <= ps.Length; j++)
        {
            StringBuilder sbRefVariable = new StringBuilder();

            // receive ref/out first
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                {
                    if (IsDirectReturn(p.ParameterType))
                    {
                        sbRefVariable.AppendFormat("        JSValueWrap.Wrap wrap{0} = vc.getWrap();\r\n", i);
                        sbRefVariable.AppendFormat("        {0} arg{1} = ({0})wrap{1}.obj;\r\n", GetTypeFullName(p.ParameterType), i);
                    }
                    else
                    {
                        sbRefVariable.AppendFormat("        object csObj{0} = vc.getObject();\r\n", i);
                        sbRefVariable.AppendFormat("        {0} arg{1} = ({0})csObj{1};\r\n", GetTypeFullName(p.ParameterType), i);
                    }
                }
                else
                {
                    sbRefVariable.AppendFormat("        {0} arg{1} = ({0}){2};\r\n", GetTypeFullName(p.ParameterType), i, BuildRetriveParam(p.ParameterType));
                }
            }

            // sP: actual parameters
            StringBuilder sbP = new StringBuilder();
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                    sbP.AppendFormat("{0} arg{1}{2}", p.IsOut ? "out" : "ref", i, (i == ps.Length - 1 ? "" : ", "));
                else
                    sbP.AppendFormat("arg{0}{1}", i, (i == ps.Length - 1 ? "" : ", "));
            }

            // write ref/out variables back
            StringBuilder sbSaveRefVariable = new StringBuilder();
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (p.ParameterType.IsByRef || p.IsOut)
                {
                    if (IsDirectReturn(p.ParameterType))
                        sbSaveRefVariable.AppendFormat("        wrap{0}.obj = arg{0};\r\n", i);
                    else
                        sbSaveRefVariable.AppendFormat("        JSMgr.changeCSObj(csObj{0}, arg{0});", i);
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
                sb.AppendFormat(@"    {4}if (len == {0}) 
    [[
{5}
        vc.returnObject( '{7}', new {1}{2}({3}) );
{6}
    ]]
", j, "", GetTypeFullName(type)/* can't use methodName here, it's .ctor*/, sbP.ToString(), (j == minNeedParams) ? "" : "else ", sbRefVariable, sbSaveRefVariable, type.Name);
            }
            else
            {
                StringBuilder sbCall = new StringBuilder();
                if (bStatic)
                    sbCall.AppendFormat("{0}.{1}({2})", GetTypeFullName(type), methodName, sbP.ToString());
                else if (!type.IsValueType)
                    sbCall.AppendFormat("(({0})vc.csObj).{1}({2})", GetTypeFullName(type), methodName, sbP.ToString());
                else
                    sbCall.AppendFormat("argThis.{0}({1})", methodName, sbP.ToString());
                StringBuilder sbFullCall = new StringBuilder();
                if (returnVoid) sbFullCall.AppendFormat("{0};", sbCall);
                else if (directReturn) sbFullCall.AppendFormat("{0};", BuildReturnObject(returnType, sbCall.ToString()));
                else sbFullCall.AppendFormat("object ret = {0};\r\n{1};", sbCall, BuildReturnObject(returnType, "ret"));

                StringBuilder sbStruct = null;
                if (type.IsValueType)
                {
                    sbStruct = new StringBuilder();
                    sbStruct.AppendFormat("{0} argThis = ({0})vc.csObj;", GetTypeFullName(type));
                }

                sb.AppendFormat(@"    {1}if (len == {0}) 
    [[
{5}
        {3}
        {2}
        {4}
{6}
    ]]
", j, (j == minNeedParams) ? "" : "else ", sbFullCall, type.IsValueType?sbStruct.ToString():"", type.IsValueType?"JSMgr.changeCSObj(vc.csObj, argThis);":"", sbRefVariable, sbSaveRefVariable);

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
        * 0 function name
        * 1 list<CSParam> generation
        * 2 function call
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
            ParameterInfo[] paramS = cons.GetParameters();

            int olIndex = i + 1; // for constuctors, they are always overloaded
            bool returnVoid = false;

            string functionName = type.Name + "_" + type.Name + (olIndex > 0 ? olIndex.ToString() : "") + (cons.IsStatic ? "_S" : "");

            sb.AppendFormat(fmt, functionName,
                BuildNormalFunctionCall(paramS, type.Name, cons.Name, cons.IsStatic, returnVoid, null, true));

            ccbn.constructors.Add(functionName);
            ccbn.constructorsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int[] olInfo, ClassCallbackNames ccbn)
    {
        /*
        * methods
        * 0 function name
        * 1 list<CSParam> generation
        * 2 function call
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
            ParameterInfo[] paramS = method.GetParameters();

            int olIndex = olInfo[i];
            bool returnVoid = (method.ReturnType == typeof(void));

            string functionName = type.Name + "_" + method.Name + (olIndex > 0 ? olIndex.ToString() : "") + (method.IsStatic ? "_S" : "");

            sb.AppendFormat(fmt, functionName,
                
                method.IsSpecialName ? BuildSpecialFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid)
                : BuildNormalFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid, method.ReturnType, false));

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

    // used for record information
    public class ClassCallbackNames
    {
        // class type
        public Type type;

        public List<string> fields;
        public List<string> properties;
        public List<string> constructors;
        public List<string> methods;

        // genetated, generating CSParam code
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

        // comment line
        string fmtComment = @"// {0}
";
        sb.AppendFormat(fmtComment, type.ToString());

        // remove namespace
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
    [MenuItem("JS for Unity/Generate CS Class Bindings")]
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

        Debug.Log("Generate CS Class Bindings finish. total = " + JSBindingSettings.classes.Length.ToString());
    }

    [MenuItem("JS for Unity/Output All Types in UnityEngine")]
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
