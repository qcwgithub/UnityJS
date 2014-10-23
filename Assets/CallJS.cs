using System;
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

#pragma warning disable 414
public class MonoPInvokeCallbackAttribute : System.Attribute
{
    private Type type;
    public MonoPInvokeCallbackAttribute(Type t) { type = t; }
}
#pragma warning restore 414
public class CallJS : MonoBehaviour 
{
    IntPtr rt;
    IntPtr cx;

    //[MonoPInvokeCallbackAttribute(typeof(SMDll.JSNative))]
    static int printInt(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        int value = SMDll.JShelp_argv0_int(cx, vp);
        Debug.Log(value);
        return 1;
    }
//     static int printString(IntPtr cx, UInt32 argc, IntPtr vp)
//     {
// 
//    
	// Use this for initialization
	void Awake ()
    {
        rt = SMDll.JS_Init(10 * 1024 * 1024);
        //Debug.Log("rt: " + rt + "\n");
        cx = SMDll.JS_NewContext(rt, 8192);
        //Debug.Log("cx: " + cx + "\n");

        int sizeofJSClass = Marshal.SizeOf(typeof(SMDll.JSClass));


        // 
        //         SMDll.JS_SetOptions(cx, SMDll.JS_GetOptions(cx) & ~JSOPTION_METHODJIT);
        //         JS_SetOptions(cx, JS_GetOptions(cx) & ~JSOPTION_METHODJIT_ALWAYS);
        //         JS_SetErrorReporter(cx, reportError);
        // 
        SMDll.JSClass global_class = new SMDll.JSClass();
        {
            global_class.name = "global";
            global_class.flags = 168704;

            //                 global_class.addProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            //                 global_class.delProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            //                 global_class.getProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            //                 global_class.setProperty = new SMDll.JS_STRICTPROPERTYSTUB(SMDll.JS_StrictPropertyStub);
            //                 global_class.enumerate = new SMDll.JS_ENUMERATESTUB(SMDll.JS_EnumerateStub);
            //                 global_class.resolve = new SMDll.JS_RESOLVESTUB(SMDll.JS_ResolveStub);
            //                 global_class.convert = new SMDll.JS_CONVERTSTUB(SMDll.JS_ConvertStub);
            //                 global_class.finalize = new SMDll.SC_FINALIZE(SMDll.sc_finalize);

            global_class.addProperty = Marshal.GetFunctionPointerForDelegate(new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub));
            global_class.delProperty = Marshal.GetFunctionPointerForDelegate(new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub));
            global_class.getProperty = Marshal.GetFunctionPointerForDelegate(new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub));
            global_class.setProperty = Marshal.GetFunctionPointerForDelegate(new SMDll.JS_STRICTPROPERTYSTUB(SMDll.JS_StrictPropertyStub));
            global_class.enumerate = Marshal.GetFunctionPointerForDelegate(new SMDll.JS_ENUMERATESTUB(SMDll.JS_EnumerateStub));
            global_class.resolve = Marshal.GetFunctionPointerForDelegate(new SMDll.JS_RESOLVESTUB(SMDll.JS_ResolveStub));
            global_class.convert = Marshal.GetFunctionPointerForDelegate(new SMDll.JS_CONVERTSTUB(SMDll.JS_ConvertStub));
            global_class.finalize = Marshal.GetFunctionPointerForDelegate(new SMDll.SC_FINALIZE(SMDll.sc_finalize));

            //                 global_class.addProperty = new IntPtr(0);
            //                 global_class.delProperty = new IntPtr(0);
            //                 global_class.getProperty = new IntPtr(0);
            //                 global_class.setProperty = new IntPtr(0);
            //                 global_class.enumerate = new IntPtr(0);
            //                 global_class.resolve = new IntPtr(0);
            //                 global_class.convert = new IntPtr(0);
            //                 global_class.finalize = new IntPtr(0);

            //                 global_class.checkAccess = null;
            //                 global_class.call = null;
            //                 global_class.hasInstance = null;
            //                 global_class.construct = null;
            //                 global_class.trace = null;

            global_class.checkAccess = new IntPtr(0);
            global_class.call = new IntPtr(0);
            global_class.hasInstance = new IntPtr(0);
            global_class.construct = new IntPtr(0);
            global_class.trace = new IntPtr(0);

            {
                global_class.reserved = new UInt64[40];
                for (int i = 0; i < 40; i++)
                    global_class.reserved[i] = 0;
            }
        }

        //IntPtr glob = SMDll.JS_NewGlobalObject(cx, global_class, new IntPtr(0));
        IntPtr glob = SMDll.JS_CreateGlobal(cx);
        //Debug.Log("glob: " + glob + "\n");

        //         var ho = new SMDll.JSHandleObject(); ho._ = new IntPtr(0);
        //         var hid = new SMDll.JSHandleId(); hid._ = new IntPtr(0);
        //         var mhv = new SMDll.JSMutableHandleValue(); mhv._ = new IntPtr(0);
        //         Debug.Log("JS_ResolveStub " + SMDll.JS_ResolveStub(cx, ho, hid));

        //SMDll.JSAutoCompartment(cx, glob);

        int b;
        b = SMDll.JS_InitStandardClasses(cx, glob);
        SMDll.JS_InitReflect(cx, glob);

        SMDll.JS_DefineFunction(cx, glob, "printInt", new SMDll.JSNative(printInt), 1, 0/*4164*/);
        GameObjectWrap.Register(cx, glob);

        string source = "var go = new GameObject();\ngo.tag();\nprintInt(200);";

        SMDll.jsval rval = new SMDll.jsval();
        string filename = "noname";
        uint lineno = 0;

        int executeOK = SMDll.JS_EvaluateScript(cx, glob, source, (uint)source.Length, filename, lineno, ref rval);
        //string str = Marshal.PtrToStringAnsi(SMDll.JS_EncodeString(cx, SMDll.JS_ValueToString(cx, rval)));
        
        SMDll.JS_DestroyContext(cx);
        SMDll.JS_Finish(rt); 
    }

    void InitClass()
    {
        IntPtr jsClass = SMDll.JShelp_NewClass("GameObject", 0);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
