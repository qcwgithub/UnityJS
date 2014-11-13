using UnityEngine;

// public class SMDLL : MonoBehaviour {
// 
// 	// Use this for initialization
// 	void Start () {
// 	
// 	}
// 	
// 	// Update is called once per frame
// 	void Update () {
// 	
// 	}
// }

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

public class JSApi
{

/***********************************************************************
    *
    * Part I
    * Constants & Types & Delegates
    *    
************************************************************************/
#if UNITY_EDITOR_WIN
    const string JSDll = "mozjswrap";
#elif UNITY_IPHONE
    const string JSDll = "__Internal";
#else
    const string JSDll = "mozjswrap";
#endif

    enum JSConstant
    {

    }

    public static int JS_TRUE = 1;
    public static int JS_FALSE = 0;

    public struct JSHandleObject { public IntPtr _; }
    public struct JSHandleId { public IntPtr _; }
    public struct JSMutableHandleValue{ public IntPtr _; }

    //[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //public delegate int JSPROPERTYOP(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

// 
//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate int JS_STRICTPROPERTYSTUB(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_EnumerateStub(IntPtr cx, JSHandleObject obj);
//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate int JS_ENUMERATESTUB(IntPtr cx, JSHandleObject obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ResolveStub(IntPtr cx, JSHandleObject obj, JSHandleId id);
//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate int JS_RESOLVESTUB(IntPtr cx, JSHandleObject obj, JSHandleId id);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ConvertStub(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);
//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate int JS_CONVERTSTUB(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);

    public static void sc_finalize(IntPtr freeOp, IntPtr obj) { }
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate void SC_FINALIZE(IntPtr freeOp, IntPtr obj);

//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JSCheckAccessOp(IntPtr cx, JSHandleObject obj, JSHandleId id, int mode, IntPtr vp);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate int JSNative(IntPtr cx, uint argc, IntPtr vp);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate int JSErrorReporter(IntPtr cx, string message, IntPtr report);

//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate int JSHasInstanceOp(IntPtr cx, JSHandleObject obj, int v, IntPtr bp);

//     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public delegate void JSTraceOp(IntPtr trc, IntPtr obj);

    /*[StructLayout(LayoutKind.Sequential)]
    public class JSClass
    {
        public string name;
        public uint flags;

        // Mandatory non-null function pointer members. 
        public JSPROPERTYOP addProperty;
        public JSPROPERTYOP delProperty;
        public JSPROPERTYOP getProperty;
        public JS_STRICTPROPERTYSTUB setProperty;
        public JS_ENUMERATESTUB enumerate;
        public JS_RESOLVESTUB resolve;
        public JS_CONVERTSTUB convert;
        public SC_FINALIZE finalize;

        // Optionally non-null members start here. 
        public JSCheckAccessOp checkAccess;
        public JSNative call;
        public JSHasInstanceOp hasInstance;
        public JSNative construct;
        public JSTraceOp trace;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public UInt64[] reserved;
    };*/


/***********************************************************************
     
 * Part II
 * SpiderMonkey functions
     
************************************************************************/

    [DllImport("mozjs-28", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSBB_Init();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_Init();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewRuntime(UInt32 maxbytes, int useHelperThreads);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewContext(IntPtr rt, int stackChunkSize);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_InitStandardClasses(IntPtr cx, IntPtr obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_InitReflect(IntPtr cx, IntPtr global);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewObject(IntPtr cx, IntPtr clasp, IntPtr proto, IntPtr parent);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_DestroyContext(IntPtr cx);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_Finish(IntPtr rt);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_InitClass(IntPtr cx, IntPtr obj, IntPtr clasp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_DefineFunction(IntPtr cx, IntPtr obj, string name, /*JSNative*/IntPtr call, UInt32 nargs, UInt32 flags);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_SetErrorReporter(IntPtr cx, JSErrorReporter er);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JSh_NewArrayObject", CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewArrayObject_(IntPtr cx, int length, IntPtr vector);
    public static IntPtr JSh_NewArrayObject(IntPtr cx, int length) { return JSh_NewArrayObject_(cx, length, IntPtr.Zero); }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetElement(IntPtr cx, IntPtr obj, uint index, ref jsval vp);

    public delegate IntPtr JSh_NewGlobalObject_Del(IntPtr cx, IntPtr clasp, IntPtr principals);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_IsArrayObject(IntPtr cx, IntPtr obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewGlobalObject(IntPtr cx, int hookOption);
    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewClass(string name, UInt32 flag, SC_FINALIZE finalizeOp);

//     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public static extern IntPtr JSh_InitClass(IntPtr cx, IntPtr jsClass);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ThisObject(IntPtr cx, IntPtr vp);



    /*
     * Arguments from JavaScript
     */
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsUndefined(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsNull(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsBool(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsString(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsNumber(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvIsDouble(IntPtr cx, IntPtr vp, int i);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvBool(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double JSh_ArgvDouble(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_ArgvInt(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, EntryPoint = "JSh_ArgvString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ArgvString_(IntPtr cx, IntPtr vp, int i);
    public static string JSh_ArgvString(IntPtr cx, IntPtr vp, int i) { return Marshal.PtrToStringAnsi(JSh_ArgvString_(cx, vp, i)); }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ArgvObject(IntPtr cx, IntPtr vp, int i);

    /*
     * Return values to JavaScript
     */
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalBool(IntPtr cx, IntPtr vp, bool value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalDouble(IntPtr cx, IntPtr vp, double value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalInt(IntPtr cx, IntPtr vp, int value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalUInt(IntPtr cx, IntPtr vp, UInt32 value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalString(IntPtr cx, IntPtr vp, string value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalObject(IntPtr cx, IntPtr vp, IntPtr value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalUndefined(IntPtr cx, IntPtr vp);
    /*
     * 生成 jsval，有些函数需要传递 jsval，或者 jsval*
     */
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalBool(ref jsval vp, bool value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalDouble(ref jsval vp, double value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalInt(ref jsval vp, int value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalUInt(ref jsval vp, UInt32 value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalString(IntPtr cx, ref jsval vp, string value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern  void JSh_SetJsvalObject(ref jsval vp, IntPtr value);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalUndefined(ref jsval vp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalJSVAL(IntPtr cx, IntPtr vp, ref jsval value);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JSh_GetErroReportFileName", CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetErroReportFileName_(IntPtr report);
    public static string JSh_GetErroReportFileName(IntPtr report)
    {
        IntPtr str = JSh_GetErroReportFileName_(report);
        return Marshal.PtrToStringAnsi(str);
    }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GetErroReportLineNo(IntPtr report);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewObjectAsClass(IntPtr cx, IntPtr glob, string className, SC_FINALIZE finalizeOp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_SetClassFinalize(IntPtr cx, string className, SC_FINALIZE finalizeOp);

    [StructLayout(LayoutKind.Explicit)]
    public struct jsval
    {
        [FieldOffset(0)]
        UInt64 asBits;
    }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_EvaluateScript(IntPtr cx, IntPtr obj,
                 string bytes, uint length,
                 string filename, uint lineno,
                 ref jsval rval);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_CompileScript(IntPtr cx, IntPtr obj, string bytes, uint length, string filename, uint lineno);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ExecuteScript(IntPtr cx, IntPtr obj,IntPtr script, ref jsval rval);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ValueToString(IntPtr cx, jsval v);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_EncodeString(IntPtr cx, IntPtr str);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_ValueToInt32(IntPtr cx, jsval v, ref int ip);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GC(IntPtr rt);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_EnterCompartment(IntPtr cx, IntPtr target);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_LeaveCompartment(IntPtr cx, IntPtr oldCompartment);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetFunction(IntPtr cx, IntPtr obj, string name);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_CallFunction(IntPtr cx, IntPtr obj, IntPtr fun, UInt32 argc,
        IntPtr argv, ref jsval rval);
}