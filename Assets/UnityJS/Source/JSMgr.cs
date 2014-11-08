using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

#pragma warning disable 414
public class MonoPInvokeCallbackAttribute : System.Attribute
{
    private Type type;
    public MonoPInvokeCallbackAttribute(Type t) { type = t; }
}
#pragma warning restore 414

public static class JSMgr
{
    public static IntPtr rt;
    public static IntPtr cx;
    public static IntPtr glob;
    public static bool useReflection = false;

    public static string jsDir = Application.dataPath + "/StreamingAssets/JavaScript";
    public static string jsGeneratedDir = jsDir + "/Generated";
    public static string csDir = Application.dataPath + "/CSharp";
    public static string csGeneratedDir = csDir + "/Generated";

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printInt(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        int value = JSApi.JShelp_ArgvInt(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printString(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        string value = JSApi.JShelp_ArgvString(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printDouble(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        double value = JSApi.JShelp_ArgvDouble(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        string fileName = JSApi.JShelp_GetErroReportFileName(report);
        int lineno = JSApi.JShelp_GetErroReportLintNo(report);
        Debug.Log(fileName + "(" + lineno.ToString() + "): " + message);
        return 1;
    }

    public static void InitJSEngine()
    {
        rt = JSApi.JS_Init(10 * 1024 * 1024);
        //Debug.Log("rt: " + rt + "\n");
        cx = JSApi.JS_NewContext(rt, 8192);
        //Debug.Log("cx: " + cx + "\n");

        //int sizeofJSClass = Marshal.SizeOf(typeof(JSApi.JSClass));


        // 
        //         SMDll.JS_SetOptions(cx, SMDll.JS_GetOptions(cx) & ~JSOPTION_METHODJIT);
        //         JS_SetOptions(cx, JS_GetOptions(cx) & ~JSOPTION_METHODJIT_ALWAYS);
        //         JS_SetErrorReporter(cx, reportError);
        // 

        JSApi.JSClass global_class = new JSApi.JSClass();
        {
            global_class.name = "global";
            global_class.flags = 168704;

            global_class.addProperty = new JSApi.JSPROPERTYOP(JSApi.JS_PropertyStub);
            global_class.delProperty = new JSApi.JSPROPERTYOP(JSApi.JS_PropertyStub);
            global_class.getProperty = new JSApi.JSPROPERTYOP(JSApi.JS_PropertyStub);
            global_class.setProperty = new JSApi.JS_STRICTPROPERTYSTUB(JSApi.JS_StrictPropertyStub);
            global_class.enumerate = new JSApi.JS_ENUMERATESTUB(JSApi.JS_EnumerateStub);
            global_class.resolve = new JSApi.JS_RESOLVESTUB(JSApi.JS_ResolveStub);
            global_class.convert = new JSApi.JS_CONVERTSTUB(JSApi.JS_ConvertStub);
            global_class.finalize = new JSApi.SC_FINALIZE(JSApi.sc_finalize);

            global_class.checkAccess = null;
            global_class.call = null;
            global_class.hasInstance = null;
            global_class.construct = null;
            global_class.trace = null;

            {
                global_class.reserved = new UInt64[40];
                for (int i = 0; i < 40; i++)
                    global_class.reserved[i] = 0;
            }
        }

        //IntPtr glob = SMDll.JS_NewGlobalObject(cx, global_class, new IntPtr(0));
        glob = JSApi.JS_CreateGlobal(cx);
        //Debug.Log("glob: " + glob + "\n");

        //         var ho = new SMDll.JSHandleObject(); ho._ = new IntPtr(0);
        //         var hid = new SMDll.JSHandleId(); hid._ = new IntPtr(0);
        //         var mhv = new SMDll.JSMutableHandleValue(); mhv._ = new IntPtr(0);
        //         Debug.Log("JS_ResolveStub " + SMDll.JS_ResolveStub(cx, ho, hid));

        //SMDll.JSAutoCompartment(cx, glob);

        /*int b = */JSApi.JS_InitStandardClasses(cx, glob);
        JSApi.JS_InitReflect(cx, glob);

        JSApi.JS_DefineFunction(cx, glob, "printInt", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printInt)), 1, 0/*4164*/);
        JSApi.JS_DefineFunction(cx, glob, "printString", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printString)), 1, 0/*4164*/);
        JSApi.JS_DefineFunction(cx, glob, "printDouble", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printDouble)), 1, 0/*4164*/);
        JSApi.JS_SetErrorReporter(cx, new JSApi.JSErrorReporter(errorReporter));

        JSMgr.RegisterCS(cx, glob);
        JSValueWrap.Register(CSOBJ, cx);
        if (useReflection)
        {
            for (int i = 0; i < JSBindingSettings.classes.Length; i++)
                JSMgr.AddTypeInfo(JSBindingSettings.classes[i]);
        }
        else
        {
            CSharpGenerated.RegisterAll();
        }
        JSMgr.EvaluateGeneratedScripts();
    }
    public static JSApi.SC_FINALIZE mjsFinalizer = new JSApi.SC_FINALIZE(JSObjectFinalizer);

    public static void FinishJSEngine()
    {
        JSApi.JS_DestroyContext(cx);
        JSApi.JS_Finish(rt);
    }


    /*
    * Push，返回某个类型的对象给JS
    */
    /*public static void Push(IntPtr cx, IntPtr vp, bool v) { JSApi.JShelp_SetRvalBool(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, double v) { JSApi.JShelp_SetRvalDouble(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, int v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, UInt32 v) { JSApi.JShelp_SetRvalUInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, string v) { JSApi.JShelp_SetRvalString(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, IntPtr v) { JSApi.JShelp_SetRvalObject(cx, vp, v); }

    public static void Push(IntPtr cx, IntPtr vp, char v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, sbyte v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, byte v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, short v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, ushort v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public static void Push(IntPtr cx, IntPtr vp, long v) { JSApi.JShelp_SetRvalInt(cx, vp, (int)v); } // !!
    public static void Push(IntPtr cx, IntPtr vp, ulong v) { JSApi.JShelp_SetRvalUInt(cx, vp, (UInt32)v); } // !!
    public static void Push(IntPtr cx, IntPtr vp, float v) { JSApi.JShelp_SetRvalDouble(cx, vp, (double)v); }
    public static void Push(IntPtr cx, IntPtr vp, decimal v) { JSApi.JShelp_SetRvalDouble(cx, vp, (double)v); }*/

    public static void EvaluateFile(string file)
    {
        JSApi.jsval val = new JSApi.jsval();
        StreamReader r = new StreamReader(file, Encoding.UTF8);
        string s = r.ReadToEnd();

        JSApi.JS_EvaluateScript(cx, glob, s, (uint)s.Length, file, 1, ref val);
        r.Close();
    }
    public static void EvaluateGeneratedScripts()
    {
        string[] files = Directory.GetFiles(JSMgr.jsGeneratedDir);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].IndexOf(".meta") == files[i].Length - 5)
                continue;
            EvaluateFile(files[i]);
        }
    }

    /*public static void RegisterEnum(string name, JSEnum[] enums)
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
    }*/

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// callback function list
    /// </summary>
    /// 

    // 对于 field，property，get和set放在同一个函数中
    // 对于
    public delegate void CSCallbackField(JSVCall vc);
    public delegate void CSCallbackProperty(JSVCall vc);
    public delegate bool CSCallbackMethod(JSVCall vc, int start, int count);

    public class MethodCallBackInfo
    {
        public MethodCallBackInfo(CSCallbackMethod f, string n, JSVCall.CSParam[] a) { fun = f; methodName = n;  arrCSParam = a; }
        public CSCallbackMethod fun;
        public string methodName;//这个用于判断是否重载函数
        public JSVCall.CSParam[] arrCSParam;
    }

    // 用途：
    // 用于js对cs的非反射调用
    public class CallbackInfo
    {
        public Type type;
        public CSCallbackField[] fields;
        public CSCallbackProperty[] properties;

        public MethodCallBackInfo[] constructors;
        public MethodCallBackInfo[] methods;
    }
    public static List<CallbackInfo> allCallbackInfo = new List<CallbackInfo>();

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// type info list
    /// </summary>

    // 用途：
    // 用于生成js代码
    // 用于生成cs代码
    // 用于js对cs的反射调用
    public class ATypeInfo
    {
        public FieldInfo[] fields;
        public PropertyInfo[] properties;
        public ConstructorInfo[] constructors;
        public MethodInfo[] methods;
        public int[] methodsOLInfo;//0 not overloaded >0 overloaded index
    }
    public static List<ATypeInfo> allTypeInfo = new List<ATypeInfo>();

    public static void ClearTypeInfo()
    {
//         CallbackInfo cbi = new CallbackInfo();
//         cbi.fields = new List<CSCallbackField>();
//         cbi.fields.Add(Vector3Generated.Vector3_x);

        allTypeInfo.Clear();
    }
    public static int AddTypeInfo(Type type)
    {
        ATypeInfo tiOut = new ATypeInfo();
        return AddTypeInfo(type, out tiOut);
    }
    public static int AddTypeInfo(Type type, out ATypeInfo tiOut)
    {
        ATypeInfo ti = new ATypeInfo();
        ti.fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Static);
        ti.properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static);
        ti.methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);        
        ti.constructors = type.GetConstructors();

        FilterTypeInfo(type, ti);

        int slot = allTypeInfo.Count;
        allTypeInfo.Add(ti);
        tiOut = ti;
        return slot;
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
    public static int MethodInfoComparison(MethodInfo m1, MethodInfo m2)
    {
        if (!m1.IsStatic && m2.IsStatic)
            return -1;
        if (m1.IsStatic && !m2.IsStatic)
            return 1;
        return string.Compare(m1.Name, m2.Name);
    }
    public static void FilterTypeInfo(Type type, ATypeInfo ti)
    {
        List<FieldInfo> lstField = new List<FieldInfo>();
        List<PropertyInfo> lstPro = new List<PropertyInfo>();
        Dictionary<string, int> proAccessors = new Dictionary<string, int>();
        List<MethodInfo> lstMethod = new List<MethodInfo>();


        for (int i = 0; i < ti.fields.Length; i++)
        {
            if (!IsMemberObsolete(ti.fields[i]))
                lstField.Add(ti.fields[i]);
        }


        for (int i = 0; i < ti.properties.Length; i++)
        {
            PropertyInfo pro = ti.properties[i];

            MethodInfo[] accessors = pro.GetAccessors();
            foreach (var v in accessors)
            {
                if (!proAccessors.ContainsKey(v.Name))
                    proAccessors.Add(v.Name, 0);
            }

            if (pro.Name == "Item") //[] not support
                continue;

            // Skip Obsolete
            if (IsMemberObsolete(pro))
                continue;

            lstPro.Add(pro);
        }

        for (int i = 0; i < ti.methods.Length; i++)
        {
            MethodInfo method = ti.methods[i];
            // skip property accessor
            if (method.IsSpecialName &&
                proAccessors.ContainsKey(method.Name))
                continue;

            if (method.IsSpecialName)
            {
                if (method.Name == "op_Addition" ||
                    method.Name == "op_Subtraction" ||
                    method.Name == "op_UnaryNegation" ||
                    method.Name == "op_Multiply" ||
                    method.Name == "op_Division" ||
                    method.Name == "op_Equality" ||
                    method.Name == "op_Inequality")
                {
                    if (!method.IsStatic)
                    {
                        Debug.LogWarning("IGNORE not-static special-name function: " + type.Name + "." + method.Name);
                        continue;
                    }
                }
                else
                {
                    Debug.LogWarning("IGNORE special-name function:" + type.Name + "." + method.Name);
                    continue;
                }
            }

            // Skip Obsolete
            if (IsMemberObsolete(method))
                continue;

            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
                continue;

            lstMethod.Add(method);
        }

        if (lstMethod.Count == 0)
            ti.methodsOLInfo = null;
        else 
        {
            // sort methods
            lstMethod.Sort(MethodInfoComparison);
            ti.methodsOLInfo = new int[lstMethod.Count];
        }

        int overloadedIndex = 1;
        bool bOL = false;
        for (int i = 0; i < lstMethod.Count; i++)
        {
            ti.methodsOLInfo[i] = 0;
            if (bOL)
            {
                ti.methodsOLInfo[i] = overloadedIndex;
            }

            if (i < lstMethod.Count - 1 && lstMethod[i].Name == lstMethod[i + 1].Name &&
                ((lstMethod[i].IsStatic && lstMethod[i + 1].IsStatic) || (!lstMethod[i].IsStatic && !lstMethod[i + 1].IsStatic)))
            {
                if (!bOL)
                {
                    overloadedIndex = 1;
                    bOL = true;
                    ti.methodsOLInfo[i] = overloadedIndex;
                }
                overloadedIndex++;
            }
            else
            {
                bOL = false;
                overloadedIndex = 1;
            }
        }

        ti.fields = lstField.ToArray();
        ti.properties = lstPro.ToArray();
        ti.methods = lstMethod.ToArray();
    }

    public static IntPtr CSOBJ = IntPtr.Zero;
    static JSVCall vCall = new JSVCall();
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int Call(IntPtr cx, uint argc, IntPtr vp)
    {
        if (useReflection)
            return vCall.CallReflection(cx, argc, vp);
        else
            return vCall.CallCallback(cx, argc, vp);
    }

    /*
     * Create a 'CS' global object to use in JS
     */
    public static void RegisterCS(IntPtr cx, IntPtr glob)
    {
        IntPtr jsClass = JSApi.JShelp_NewClass("CS", 0, null);

        IntPtr obj = JSApi.JS_InitClass(cx, glob,
            IntPtr.Zero, /* parentProto */
            jsClass, /* JSClass */
            null, /* constructor */
            0, /* constructor argument count*/
            IntPtr.Zero, /* properties */
            IntPtr.Zero, /* functions */
            IntPtr.Zero, /* static properties */
            IntPtr.Zero /* static functions*/
        );

        JSApi.JS_DefineFunction(cx, obj, "Call", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(Call)), 20/* narg */, 0);
        CSOBJ = obj;
    }

    public static void JS_GC()
    {
        JSApi.JS_GC(rt);
    }

    /*
     * 这2个是记录 C#对象和js对象的对应关系
     */
    class JS_CS_Relation
    {
        public IntPtr jsObj;
        public object csObj;
        public JS_CS_Relation(IntPtr a, object b)
        {
            jsObj = a;
            csObj = b;
        }
    }

    public static void addJSCSRelation(IntPtr jsObj, object csObj)
    {
        Debug.Log("jsObj added: " + jsObj.ToInt32().ToString());

        int index = nextRelationIndex++;

//         JSApi.jsval val = new JSApi.jsval();
//         JSApi.JShelp_SetJsvalInt(ref val, index);
//         JSApi.JS_SetProperty(cx, jsObj, "__resourceID", ref val);
        mDict1.Add(jsObj.ToInt64(), new JS_CS_Relation(jsObj, csObj));
        mDict2.Add(csObj, new JS_CS_Relation(jsObj, csObj));
    }
    public static object getCSObj(IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.ToInt64(), out obj))
            return obj.csObj;
        return null;
    }
    public static IntPtr getJSObj(object csObj)
    {
        JS_CS_Relation obj;
        if (mDict2.TryGetValue(csObj, out obj))
            return obj.jsObj;
        return IntPtr.Zero;
    }
    public static void changeCSObj(object csObj, object csObjNew)
    {
        IntPtr jsObj = getJSObj(csObj);
        if (jsObj == IntPtr.Zero)
            return;

        mDict1.Remove(jsObj.ToInt64());
        mDict2.Remove(csObj);
        addJSCSRelation(jsObj, csObj);
    }
    static Dictionary<long, JS_CS_Relation> mDict1 = new Dictionary<long, JS_CS_Relation>(); // key = jsObj.hashCode()
    static Dictionary<object, JS_CS_Relation> mDict2 = new Dictionary<object, JS_CS_Relation>(); // key = nativeObj.hashCode()
    static int nextRelationIndex = 0;

    static void JSObjectFinalizer(IntPtr freeOp, IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.ToInt64(), out obj))
        {
            string name = obj.csObj.GetType().Name;
            mDict1.Remove(jsObj.ToInt64());
            Debug.Log(name + " finalized, left " + mDict1.Count.ToString());
        }
        else
        {
            Debug.Log("Finalizer: csObj not found: " + jsObj.ToInt32().ToString());
        }
    }
    
    /*
     * 记录已注册的类型
     */
    public class GlobalType
    {
        public IntPtr jsClass; // JSClass*
        public IntPtr proto;   // JSObject* returned by JS_InitClass
        public IntPtr parentProto; // JSObject* parent
    }
    public static void addGlobalType(Type type, IntPtr jsClass, IntPtr proto, IntPtr parentProto)
    {
        int hash = type.GetHashCode();
        GlobalType gt = new GlobalType();
        gt.jsClass = jsClass;
        gt.proto = proto;
        gt.parentProto = parentProto;
        mGlobalType.Add(hash, gt);
    }
    public static GlobalType getGlobalType(Type type)
    {
        GlobalType gt;
        if (mGlobalType.TryGetValue(type.GetHashCode(), out gt))
        {
            return gt;
        }
        return null;
    }
    static Dictionary<int, GlobalType> mGlobalType = new Dictionary<int, GlobalType>();
}
