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

public class SMDll
{
    const string SMDLL = "mozjs";
    const string SMHelpDll = "dllParseToUnity";
    public static int JS_TRUE = 1;
    public static int JS_FALSE = 0;



    //     typedef struct { JSObject **_; } JSHandleObject;
    public struct JSHandleObject
    {
        public IntPtr _;
    }
    //     typedef struct { jsval _; } JSHandleValue;
    //     typedef struct { JSString **_; } JSHandleString;
    //     typedef struct { JSObject **_; } JSMutableHandleObject;
    //     typedef struct { jsid *_; } JSHandleId;
    public struct JSHandleId
    {
        public IntPtr _;
    }
    //     typedef struct { jsval *_; } JSMutableHandleValue;
    public struct JSMutableHandleValue
    {
        public IntPtr _;
    }
    //     typedef JSObject *JSRawObject;

    public delegate int JSPROPERTYOP(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

    // extern JS_PUBLIC_API(JSBool)JS_PropertyStub(JSContext *cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_PropertyStub(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

    // extern JS_PUBLIC_API(JSBool) JS_StrictPropertyStub(JSContext *cx, JSHandleObject obj, JSHandleId id, JSBool strict, JSMutableHandleValue vp);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_StrictPropertyStub(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);
    public delegate int JS_STRICTPROPERTYSTUB(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);

    // extern JS_PUBLIC_API(JSBool) JS_EnumerateStub(JSContext *cx, JSHandleObject obj);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_EnumerateStub(IntPtr cx, JSHandleObject obj);
    public delegate int JS_ENUMERATESTUB(IntPtr cx, JSHandleObject obj);

    // extern JS_PUBLIC_API(JSBool) JS_ResolveStub(JSContext *cx, JSHandleObject obj, JSHandleId id);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ResolveStub(IntPtr cx, JSHandleObject obj, JSHandleId id);
    public delegate int JS_RESOLVESTUB(IntPtr cx, JSHandleObject obj, JSHandleId id);

    // extern JS_PUBLIC_API(JSBool) JS_ConvertStub(JSContext *cx, JSHandleObject obj, JSType type, JSMutableHandleValue vp);
    [DllImport(SMDLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JS_ConvertStub(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);
    public delegate int JS_CONVERTSTUB(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);

    // void sc_finalize(JSFreeOp* freeOp, JSObject* obj)
    public static void sc_finalize(IntPtr freeOp, IntPtr obj) { }
    public delegate void SC_FINALIZE(IntPtr freeOp, IntPtr obj);

    //     typedef JSBool
    // (* JSCheckAccessOp)(JSContext *cx, JSHandleObject obj, JSHandleId id, JSAccessMode mode,
    //                     jsval *vp);
    public delegate int JSCheckAccessOp(IntPtr cx, JSHandleObject obj, JSHandleId id, int mode, IntPtr vp);

    // typedef JSBool (* JSNative)(JSContext *cx, unsigned argc, jsval *vp);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int JSNative(IntPtr cx, uint argc, IntPtr vp);

    // typedef JSBool (* JSHasInstanceOp)(JSContext *cx, JSHandleObject obj, const jsval *v, JSBool *bp);
    public delegate int JSHasInstanceOp(IntPtr cx, JSHandleObject obj, int v, IntPtr bp);

    //    typedef void(* JSTraceOp)(JSTracer *trc, JSObject *obj);
    public delegate void JSTraceOp(IntPtr trc, IntPtr obj);

    //     struct JSClass {
    //         const char          *name;
    //         uint32_t            flags;
    // 
    //         /* Mandatory non-null function pointer members. */
    //         JSPropertyOp        addProperty;
    //         JSPropertyOp        delProperty;
    //         JSPropertyOp        getProperty;
    //         JSStrictPropertyOp  setProperty;
    //         JSEnumerateOp       enumerate;
    //         JSResolveOp         resolve;
    //         JSConvertOp         convert;
    //         JSFinalizeOp        finalize;
    // 
    //         /* Optionally non-null members start here. */
    //         JSCheckAccessOp     checkAccess;
    //         JSNative            call;
    //         JSHasInstanceOp     hasInstance;
    //         JSNative            construct;
    //         JSTraceOp           trace;
    // 
    //         void                *reserved[40];
    //     };

    [StructLayout(LayoutKind.Sequential)]
    public class JSClass
    {
        public string name;
        public uint flags;

        /* Mandatory non-null function pointer members. */
        //             public JSPROPERTYOP addProperty;
        //             public JSPROPERTYOP delProperty;
        //             public JSPROPERTYOP getProperty;
        //             public JS_STRICTPROPERTYSTUB setProperty;
        //             public JS_ENUMERATESTUB enumerate;
        //             public JS_RESOLVESTUB resolve;
        //             public JS_CONVERTSTUB convert;
        //             public SC_FINALIZE finalize;

        public IntPtr addProperty;
        public IntPtr delProperty;
        public IntPtr getProperty;
        public IntPtr setProperty;
        public IntPtr enumerate;
        public IntPtr resolve;
        public IntPtr convert;
        public IntPtr finalize;

        /* Optionally non-null members start here. */
        //             public JSCheckAccessOp checkAccess;
        //             public JSNative call;
        //             public JSHasInstanceOp hasInstance;
        //             public JSNative construct;
        //             public JSTraceOp trace;

        public IntPtr checkAccess;
        public IntPtr call;
        public IntPtr hasInstance;
        public IntPtr construct;
        public IntPtr trace;

        //         [MarshalAs(UnmanagedType.LPArray, SizeConst = 40)]
        //         public IntPtr[] reserved;
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        // public int[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public UInt64[] reserved;
    };


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
    public static extern IntPtr JS_DefineFunction(IntPtr cx, IntPtr obj,
    string name, JSNative call, UInt32 nargs, UInt32 flags);

    public delegate IntPtr JS_NewGlobalObject_Del(IntPtr cx, IntPtr clasp, IntPtr principals);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JS_CreateGlobal(IntPtr cx);

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_NewClass(string name, UInt32 flag);    

    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_ARGV(IntPtr cx, IntPtr vp);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_argv0_int(IntPtr cx, IntPtr vp);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JShelp_ThisObject(IntPtr cx, IntPtr vp);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalInt(IntPtr cx, IntPtr vp, int value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalBool(IntPtr cx, IntPtr vp, bool value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalObject(IntPtr cx, IntPtr vp, IntPtr value);
    [DllImport(SMHelpDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JShelp_SetRvalString(IntPtr cx, IntPtr vp, string value);


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