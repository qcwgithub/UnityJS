using UnityEngine;
using System.Collections;

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

using JSBool = System.Int32;

public class SMDll
{

/***********************************************************************
 *
 * Part I
 * Constants & Types & Delegates
 *    
************************************************************************/

    const string SMDLL = "mozjs";
    const string SMHelpDll = "dllParseToUnity";

    enum JSConstant
    {

    }

    public static int JS_TRUE = 1;
    public static int JS_FALSE = 0;

    //     typedef struct { JSObject **_; } JSHandleObject;
    public struct JSHandleObject { public IntPtr _; }

    //     typedef struct { jsval _; } JSHandleValue;
    //     typedef struct { JSString **_; } JSHandleString;
    //     typedef struct { JSObject **_; } JSMutableHandleObject;
    //     typedef struct { jsid *_; } JSHandleId;
    public struct JSHandleId { public IntPtr _; }
    public struct JSMutableHandleValue{ public IntPtr _; }

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSPROPERTYOP(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_PropertyStub(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_StrictPropertyStub(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JS_STRICTPROPERTYSTUB(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_EnumerateStub(IntPtr cx, JSHandleObject obj);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JS_ENUMERATESTUB(IntPtr cx, JSHandleObject obj);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ResolveStub(IntPtr cx, JSHandleObject obj, JSHandleId id);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JS_RESOLVESTUB(IntPtr cx, JSHandleObject obj, JSHandleId id);

    // extern JS_PUBLIC_API(JSBool) JS_ConvertStub(JSContext *cx, JSHandleObject obj, JSType type, JSMutableHandleValue vp);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ConvertStub(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JS_CONVERTSTUB(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);

    // void sc_finalize(JSFreeOp* freeOp, JSObject* obj)
    public static void sc_finalize(IntPtr freeOp, IntPtr obj) { }
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void SC_FINALIZE(IntPtr freeOp, IntPtr obj);

    //     typedef JSBool
    // (* JSCheckAccessOp)(JSContext *cx, JSHandleObject obj, JSHandleId id, JSAccessMode mode,
    //                     jsval *vp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSCheckAccessOp(IntPtr cx, JSHandleObject obj, JSHandleId id, int mode, IntPtr vp);

    // typedef JSBool (* JSNative)(JSContext *cx, unsigned argc, jsval *vp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSNative(IntPtr cx, uint argc, IntPtr vp);

    // typedef void(* JSErrorReporter)(JSContext *cx, const char *message, JSErrorReport *report);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSErrorReporter(IntPtr cx, string message, IntPtr report);

    // typedef JSBool (* JSHasInstanceOp)(JSContext *cx, JSHandleObject obj, const jsval *v, JSBool *bp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSHasInstanceOp(IntPtr cx, JSHandleObject obj, int v, IntPtr bp);

    //    typedef void(* JSTraceOp)(JSTracer *trc, JSObject *obj);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void JSTraceOp(IntPtr trc, IntPtr obj);

    [StructLayout(LayoutKind.Sequential)]
    public class JSClass
    {
        public string name;
        public uint flags;

        /* Mandatory non-null function pointer members. */
        public JSPROPERTYOP addProperty;
        public JSPROPERTYOP delProperty;
        public JSPROPERTYOP getProperty;
        public JS_STRICTPROPERTYSTUB setProperty;
        public JS_ENUMERATESTUB enumerate;
        public JS_RESOLVESTUB resolve;
        public JS_CONVERTSTUB convert;
        public SC_FINALIZE finalize;

        /* Optionally non-null members start here. */
        public JSCheckAccessOp checkAccess;
        public JSNative call;
        public JSHasInstanceOp hasInstance;
        public JSNative construct;
        public JSTraceOp trace;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public UInt64[] reserved;
    };


/***********************************************************************
     
 * Part II
 * SpiderMonkey functions
     
************************************************************************/


    /* 
     * JSRuntime* JS_NewRuntime(uint32_t maxbytes); 
     */
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_Init(int maxbytes);

    /* 
     * JSContext* JS_NewContext(JSRuntime *rt, size_t stackChunkSize); 
     */
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_NewContext(IntPtr rt, int stackChunkSize);

    /* 
     * uint32_t JS_SetOptions(JSContext *cx, uint32_t options); 
     */
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_SetOptions(IntPtr cx, int options);

    /* 
     * uint32_t JS_GetOptions(JSContext *cx); 
     */
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern uint JS_GetOptions(IntPtr cx);

    // JSBool JS_InitStandardClasses(JSContext *cx, JSObject *obj);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_InitStandardClasses(IntPtr cx, IntPtr obj);


    // JSObject* JS_InitReflect(JSContext *cx, JSObject *global); 
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_InitReflect(IntPtr cx, IntPtr global);

    /*
    * JSObject* JS_NewGlobalObject(JSContext *cx, JSClass *clasp, JSPrincipals *principals);
    */
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_NewGlobalObject(IntPtr cx, JSClass clasp, IntPtr principals);

    // extern JS_PUBLIC_API(JSObject *) JS_NewObject(JSContext *cx, JSClass *clasp, JSObject *proto, JSObject *parent);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_NewObject(IntPtr cx, IntPtr clasp, IntPtr proto, IntPtr parent);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JS_DestroyContext(IntPtr cx);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JS_Finish(IntPtr rt);

//     JSObject* JS_InitClass(JSContext* cx, JSObject* obj,
//         JSObject* parent_proto, JSClass* clasp,
//         JSNative constructor, uintN nargs,
//         JSPropertySpec* ps, JSFunctionSpec* fs,
//         JSPropertySpec* static_ps, JSFunctionSpec* static_fs);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_InitClass(IntPtr cx, IntPtr obj,
        IntPtr parent_proto, IntPtr clasp,
        JSNative constructor, UInt32 nargs,
        IntPtr ps, IntPtr fs,
        IntPtr static_ps, IntPtr static_fs);

//    JSFunction * JS_DefineFunction(JSContext *cx, JSObject *obj,
//    const char *name, JSNative call, uintN nargs, uintN flags);

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_DefineFunction(IntPtr cx, IntPtr obj, string name, JSNative call, UInt32 nargs, UInt32 flags);

    // extern JS_PUBLIC_API(JSErrorReporter) JS_SetErrorReporter(JSContext *cx, JSErrorReporter er);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_SetErrorReporter(IntPtr cx, JSErrorReporter er);

//     JSBool JS_DefineProperty(JSContext *cx, JSObject *obj,
//         const char *name, jsval value, JSPropertyOp getter,
//         JSStrictPropertyOp setter, unsigned attrs);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_DefineProperty(IntPtr cx, IntPtr obj, string name, jsval value, JSPROPERTYOP getter, JS_STRICTPROPERTYSTUB setter, UInt32 attrs );

    // extern JS_PUBLIC_API(JSObject *) JS_NewArrayObject(JSContext *cx, int length, jsval *vector);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JS_NewArrayObject", CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_NewArrayObject_(IntPtr cx, int length, IntPtr vector);
    public static IntPtr JS_NewArrayObject(IntPtr cx, int length) { return JS_NewArrayObject_(cx, length, IntPtr.Zero); }

    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    // extern JS_PUBLIC_API(JSBool)JS_SetElement(JSContext *cx, JSObject *obj, uint32_t index, jsval *vp);
    public static extern int JS_SetElement(IntPtr cx, IntPtr obj, uint index, ref jsval vp);

    public delegate IntPtr JS_NewGlobalObject_Del(IntPtr cx, IntPtr clasp, IntPtr principals);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_CreateGlobal(IntPtr cx);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_NewClass(string name, UInt32 flag);    

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_ThisObject(IntPtr cx, IntPtr vp);



    /*
     * Arguments from JavaScript
     */
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JShelp_ArgvIsUndefined(IntPtr cx, IntPtr vp, int i);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JShelp_ArgvIsNull(IntPtr cx, IntPtr vp, int i);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JShelp_ArgvBool(IntPtr cx, IntPtr vp, int i);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double JShelp_ArgvDouble(IntPtr cx, IntPtr vp, int i);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_ArgvInt(IntPtr cx, IntPtr vp, int i);
    [DllImport(SMHelpDll, EntryPoint = "JShelp_ArgvString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_ArgvString_(IntPtr cx, IntPtr vp, int i);
    public static string JShelp_ArgvString(IntPtr cx, IntPtr vp, int i)
    {
        return Marshal.PtrToStringAnsi(JShelp_ArgvString_(cx, vp, i));
    }
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_ArgvObject(IntPtr cx, IntPtr vp, int i);

    /*
     * Return values to JavaScript
     */
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalBool(IntPtr cx, IntPtr vp, bool value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalDouble(IntPtr cx, IntPtr vp, double value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalInt(IntPtr cx, IntPtr vp, int value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalUInt(IntPtr cx, IntPtr vp, UInt32 value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalString(IntPtr cx, IntPtr vp, string value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalObject(IntPtr cx, IntPtr vp, IntPtr value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalUndefined(IntPtr cx, IntPtr vp);
    /*
     * 生成 jsval，有些函数需要传递 jsval，或者 jsval*
     */
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalBool(ref jsval vp, bool value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalDouble(ref jsval vp, double value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalInt(ref jsval vp, int value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalUInt(ref jsval vp, UInt32 value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalString(IntPtr cx, ref jsval vp, string value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern  void JShelp_SetJsvalObject(ref jsval vp, IntPtr value);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JShelp_SetJsvalUndefined(ref jsval vp);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_NewObjectAsClass(IntPtr cx, IntPtr glob, string className);
    

    [StructLayout(LayoutKind.Explicit)]
    public struct jsval
    {
        [FieldOffset(0)]
        UInt64 asBits;
    }

    //     extern JS_PUBLIC_API(JSBool)
    //     JS_EvaluateScript(JSContext *cx, JSObject *obj,
    //                   const char *bytes, unsigned length,
    //                   const char *filename, unsigned lineno,
    //                   jsval *rval);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_EvaluateScript(IntPtr cx, IntPtr obj,
                 string bytes, uint length,
                 string filename, uint lineno,
                 ref jsval rval);

//     extern JS_PUBLIC_API(JSScript *)
// JS_CompileScript(JSContext *cx, JSObject *obj,
//                  const char *bytes, size_t length,
//                  const char *filename, unsigned lineno);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_CompileScript(IntPtr cx, IntPtr obj, string bytes, uint length, string filename, uint lineno);

    //     extern JS_PUBLIC_API(JSString *)
    //     JS_ValueToString(JSContext *cx, jsval v);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_ValueToString(IntPtr cx, jsval v);

    //     JS_PUBLIC_API(char *)
    // JS_EncodeString(JSContext *cx, JSString *str);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_EncodeString(IntPtr cx, IntPtr str);

    //extern JS_PUBLIC_API(JSBool)
    //JS_ValueToInt32(JSContext *cx, jsval v, int32_t *ip);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ValueToInt32(IntPtr cx, jsval v, ref int ip);

    //     [StructLayout(LayoutKind.Sequential)]
    //     public class SystemTime
    //     {
    //         public ushort year;
    //         public ushort month;
    //         public ushort weekday;
    //         public ushort day;
    //         public ushort hour;
    //         public ushort minute;
    //         public ushort second;
    //         public ushort millisecond;
    //     }
    //     [DllImport("Kernel32.dll")]
    //     public static extern void GetSystemTime([In, Out] SystemTime st);
}