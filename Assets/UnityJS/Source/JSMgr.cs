/*
 * 
 * 
 */


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

    public static string jsDir = Application.streamingAssetsPath + "/JavaScript";
    public static string jsGeneratedDir = jsDir + "/Generated";
    public static string csDir = Application.dataPath + "/CSharp";
    public static string csGeneratedDir = csDir + "/Generated";

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printInt(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        int value = JSApi.JSh_ArgvInt(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printString(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        string value = JSApi.JSh_ArgvString(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int printDouble(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        double value = JSApi.JSh_ArgvDouble(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        string fileName = JSApi.JSh_GetErroReportFileName(report);
        int lineno = JSApi.JSh_GetErroReportLineNo(report);
        Debug.Log(fileName + "(" + lineno.ToString() + "): " + message);
        return 1;
    }

    public static bool InitJSEngine()
    {
        if (!JSApi.JSh_Init())
            return false;

        rt = JSApi.JSh_NewRuntime(8 * 1024 * 1024, 0);
        cx = JSApi.JSh_NewContext(rt, 8192);

        glob = JSApi.JSh_NewGlobalObject(cx, 1);

        JSApi.JSh_EnterCompartment(cx, glob);

        if (!JSApi.JSh_InitStandardClasses(cx, glob))
            return false;

        JSApi.JSh_InitReflect(cx, glob);

        JSApi.JSh_DefineFunction(cx, glob, "printInt", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printInt)), 1, 0/*4164*/);
        JSApi.JSh_DefineFunction(cx, glob, "printString", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printString)), 1, 0/*4164*/);
        JSApi.JSh_DefineFunction(cx, glob, "printDouble", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printDouble)), 1, 0/*4164*/);
        JSApi.JSh_SetErrorReporter(cx, new JSApi.JSErrorReporter(errorReporter));

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
        return true;
    }
    public static JSApi.SC_FINALIZE mjsFinalizer = new JSApi.SC_FINALIZE(JSObjectFinalizer);

    public static void FinishJSEngine()
    {
        JSApi.JSh_DestroyContext(cx);
        //JSApi.JSh_Finish(rt);
    }


    public static void EvaluateFile(string fullName, IntPtr obj)
    {
        JSApi.jsval val = new JSApi.jsval();
        StreamReader r = new StreamReader(fullName, Encoding.UTF8);
        string s = r.ReadToEnd();

        JSApi.JSh_EvaluateScript(cx, obj, s, (uint)s.Length, fullName, 1, ref val);
        r.Close();
    }
    static string calcFullJSFileName(string shortName)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        string fullName = "file://" + Application.streamingAssetsPath + "/JavaScript/" + shortName + ".javascript";
#else
        string fullName = Application.streamingAssetsPath + "/JavaScript/" + shortName + ".javascript";
#endif
        return fullName;
    }
    public static void EvaluateGeneratedScripts()
    {
        string[] files = Directory.GetFiles(JSMgr.jsGeneratedDir);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].IndexOf(".meta") == files[i].Length - 5)
                continue;
            EvaluateFile(files[i], glob);
        }
//         IntPtr ptr;
//         ptr = GetScript("Generated/GameObject");
//         JSMgr.ExecuteScript(ptr, glob);
//         ptr = GetScript("Generated/Transform");
//         JSMgr.ExecuteScript(ptr, glob);
//         ptr = GetScript("Generated/Vector3");
//         JSMgr.ExecuteScript(ptr, glob);
//         ptr = GetScript("Generated/GameObject");
//         JSMgr.ExecuteScript(ptr, glob);
    }
    public static IntPtr CompileScript(string shortName, IntPtr obj)
    {
        string fullName = calcFullJSFileName(shortName);

        StreamReader r = new StreamReader(fullName, Encoding.UTF8);
        string s = r.ReadToEnd();

        IntPtr ptr = JSApi.JSh_CompileScript(cx, obj, s, (uint)s.Length, shortName, 1);
        r.Close();
        return ptr;
    }
    public static IntPtr CompileScriptContent(string shortName, string content, IntPtr obj)
    {
        if (content.Length == 0)
        {
            Debug.Log(shortName + " content length = 0");
            return IntPtr.Zero;

        }

        IntPtr ptr = JSApi.JSh_CompileScript(cx, obj, content, (uint)content.Length, shortName, 1);
        compiledScript.Add(shortName, ptr.ToInt32());
        return ptr;
    }
    public static bool ExecuteScript(IntPtr ptrScript, IntPtr obj)
    {
        JSApi.jsval val = new JSApi.jsval();
        return JSApi.JSh_ExecuteScript(cx, obj, ptrScript, ref val);
    }
    public static IntPtr GetScript(string shortName/*, IntPtr obj*/)
    {
        int ptr = 0;
        if (compiledScript.TryGetValue(shortName, out ptr))
            return new IntPtr(ptr);
        else
        {
            string fullName = calcFullJSFileName(shortName);
            WWW w = new WWW(fullName);
            while (true)
            {
                if (w.isDone)
                    break;
            }
            string content = w.text;
            return CompileScriptContent(shortName, w.text, glob);
        }
    }

    static Dictionary<string, int> compiledScript = new Dictionary<string, int>();

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// callback function list
    /// </summary>
    /// 
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

    // usage
    // 1 use for calling cs from js, by directly-call
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

    // usage
    // 1 used for generating js code
    // 2 used for generating cs code
    // 3 used for calling cs from js, by reflection
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
            if (typeof(System.Delegate).IsAssignableFrom(ti.fields[i].FieldType.BaseType))
            {
                Debug.Log("[field]" + type.ToString() + "." + ti.fields[i].Name + "is delegate!");
            }

            if (!IsMemberObsolete(ti.fields[i]) && !JSBindingSettings.IsDiscard(type, ti.fields[i]))
                lstField.Add(ti.fields[i]);
        }


        for (int i = 0; i < ti.properties.Length; i++)
        {
            PropertyInfo pro = ti.properties[i];

            if (typeof(System.Delegate).IsAssignableFrom(pro.PropertyType.BaseType))
            {
                Debug.Log("[property]" + type.ToString() + "." + pro.Name + "is delegate!");
            }

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

            if (JSBindingSettings.IsDiscard(type, pro))
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
            {
                //Debug.Log(type.Name + "." + method.Name);
                continue;
            }

            if (JSBindingSettings.IsDiscard(type, method))
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
    public static JSVCall vCall = new JSVCall();
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
        IntPtr jsClass = JSApi.JSh_NewClass("CS", 0, null);
        IntPtr obj = JSApi.JSh_InitClass(cx, glob, jsClass);

        JSApi.JSh_DefineFunction(cx, obj, "Call", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(Call)), 20/* narg */, 0);
        CSOBJ = obj;
    }

    public static void JS_GC()
    {
        JSApi.JSh_GC(rt);
    }

    /*
     * record js/cs relation
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
        Debug.Log("jsObj added: " + csObj.GetType().Name + " / " + (typeof(UnityEngine.Object).IsAssignableFrom(csObj.GetType()) ? ((UnityEngine.Object)csObj).name : ""));

        int index = nextRelationIndex++;

//         JSApi.jsval val = new JSApi.jsval();
//         JSApi.JSh_SetJsvalInt(ref val, index);
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
     * record registered types
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
