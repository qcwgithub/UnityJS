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
    public static bool useReflection = true;

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

        int sizeofJSClass = Marshal.SizeOf(typeof(JSApi.JSClass));


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

        int b;
        b = JSApi.JS_InitStandardClasses(cx, glob);
        JSApi.JS_InitReflect(cx, glob);

        JSApi.JS_DefineFunction(cx, glob, "printInt", new JSApi.JSNative(printInt), 1, 0/*4164*/);
        JSApi.JS_DefineFunction(cx, glob, "printString", new JSApi.JSNative(printString), 1, 0/*4164*/);
        JSApi.JS_SetErrorReporter(cx, new JSApi.JSErrorReporter(errorReporter));

        JSMgr.RegisterCS(cx, glob);
        JSValueWrap.Register(CSOBJ, cx);

        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            JSMgr.AddTypeInfo(JSBindingSettings.classes[i]);
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
    public delegate void CSCallback(JSVCall vc);
    public class CallbackInfo
    {
        public List<CSCallback> fields;
        public List<CSCallback> properties;
        public List<CSCallback> constructors;
        public List<CSCallback> methods;
    }
    public static List<CallbackInfo> allCallbackInfo = new List<CallbackInfo>();

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// type info list
    /// </summary>

    public class ATypeInfo
    {
        public FieldInfo[] fields;
        public PropertyInfo[] properties;
        public ConstructorInfo[] constructors;
        public MethodInfo[] methods;
    }
    public static List<ATypeInfo> allTypeInfo = new List<ATypeInfo>();

    public static void ClearTypeInfo()
    {
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
//         var lstField = new List<FieldInfo>();
//         foreach (var field in ti.fields)
//         {
//             if (field.IsInitOnly)
//                 continue;
//             lstField.Add(field);
//         }

        ti.properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static);
        ti.methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        List<MethodInfo> lMethods = new List<MethodInfo>();
        for (int i = 0; i < ti.methods.Length; i++)
        {
            // 泛型函数不支持
            if (ti.methods[i].IsGenericMethod || ti.methods[i].IsGenericMethodDefinition)
                continue;
            else
                lMethods.Add(ti.methods[i]);
        }
        ti.constructors = type.GetConstructors();
        ti.methods = lMethods.ToArray();

        int slot = allTypeInfo.Count;
        allTypeInfo.Add(ti);
        tiOut = ti;
        return slot;
    }

    public static IntPtr CSOBJ = IntPtr.Zero;
    static JSVCall vCall = new JSVCall();
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

        JSApi.JS_DefineFunction(cx, obj, "Call", new JSApi.JSNative(Call), 0/* narg */, 0);
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

    public static void addNativeJSRelation(IntPtr jsObj, object nativeObj)
    {
        Debug.Log("jsObj added: " + jsObj.ToInt32().ToString());

        mDict1.Add(jsObj.GetHashCode(), new JS_CS_Relation(jsObj, nativeObj));
        mDict2.Add(nativeObj.GetHashCode(), new JS_CS_Relation(jsObj, nativeObj));
    }
    public static object getCSObj(IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.GetHashCode(), out obj))
            return obj.csObj;
        return null;
    }
    public static IntPtr getJSObj(object nativeObj)
    {
        JS_CS_Relation obj;
        if (mDict2.TryGetValue(nativeObj.GetHashCode(), out obj))
            return obj.jsObj;
        return IntPtr.Zero;
    }
    static Dictionary<int, JS_CS_Relation> mDict1 = new Dictionary<int, JS_CS_Relation>(); // key = jsObj.hashCode()
    static Dictionary<int, JS_CS_Relation> mDict2 = new Dictionary<int, JS_CS_Relation>(); // key = nativeObj.hashCode()

    static void JSObjectFinalizer(IntPtr freeOp, IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.GetHashCode(), out obj))
        {
            string name = obj.csObj.GetType().Name;
            mDict1.Remove(jsObj.GetHashCode());
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
