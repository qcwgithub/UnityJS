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
    public static IntPtr rt;
    public static IntPtr cx;
    public static IntPtr glob;

    //[MonoPInvokeCallbackAttribute(typeof(SMDll.JSNative))]
    static int printInt(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        int value = SMDll.JShelp_ArgvInt(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    static int printString(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        string value = SMDll.JShelp_ArgvString(cx, vp, 0);
        Debug.Log(value);
        return 1;
    }
    static GameObject testGameObject;
    static int getTestObject(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        string className = SMDll.JShelp_ArgvString(cx, vp, 0);
        IntPtr jsObj = SMDll.JShelp_NewObjectAsClass(cx, glob, className);
        if (jsObj == IntPtr.Zero)
            Debug.Log("jsObj == IntPtr.Zero");
        SMData.addNativeJSRelation(jsObj, testGameObject);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return 1;
    }
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        Debug.Log(message);
        return 1;
    }
//     static int printString(IntPtr cx, UInt32 argc, IntPtr vp)
//     {
// 
//    
	// Use this for initialization
	void Awake ()
    {
//         MethodInfo ms = typeof(GameObject).GetMethod("Find");
//         object[] para = new object[] { (object)"Main Camera" };
//         GameObject go = (GameObject)ms.Invoke((object)null, para);
//         Debug.Log(go.name);
//         return;

//         GameObject[] gos = GameObject.FindGameObjectsWithTag("CreditCard");
//         for (int i = 0; i < gos.Length; i++)
//         {
//             Debug.Log(gos[i].name);
//             
//         }
//         retur
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

            global_class.addProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            global_class.delProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            global_class.getProperty = new SMDll.JSPROPERTYOP(SMDll.JS_PropertyStub);
            global_class.setProperty = new SMDll.JS_STRICTPROPERTYSTUB(SMDll.JS_StrictPropertyStub);
            global_class.enumerate = new SMDll.JS_ENUMERATESTUB(SMDll.JS_EnumerateStub);
            global_class.resolve = new SMDll.JS_RESOLVESTUB(SMDll.JS_ResolveStub);
            global_class.convert = new SMDll.JS_CONVERTSTUB(SMDll.JS_ConvertStub);
            global_class.finalize = new SMDll.SC_FINALIZE(SMDll.sc_finalize);

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
        glob = SMDll.JS_CreateGlobal(cx);
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
        SMDll.JS_DefineFunction(cx, glob, "printString", new SMDll.JSNative(printString), 1, 0/*4164*/);
        SMDll.JS_DefineFunction(cx, glob, "getTestObject", new SMDll.JSNative(getTestObject), 1, 0/*4164*/);
        SMDll.JS_SetErrorReporter(cx, new SMDll.JSErrorReporter(errorReporter));
        // GameObjectWrap.Register(cx, glob);

        JSMgr.RegisterCS(cx, glob);
        ValueTypeWrap2.Register(cx);
        JSMgr.AddTypeInfo(typeof(GameObject));
        JSMgr.AddTypeInfo(typeof(Camera));

        testGameObject = gameObject;// new GameObject("i love you");
        //testGameObject.tag = "Finish";

        JSMgr.EvaluateGeneratedScripts(cx, glob);
        JSMgr.EvaluateFile(cx, glob, Application.dataPath + "/StreamingAssets/JavaScript/test.javascript");

        SMDll.jsval rval = new SMDll.jsval();
        string filename = "noname";
        uint lineno = 0;

        int executeOK = SMDll.JS_EvaluateScript(cx, glob, Test.testScript, (uint)Test.testScript.Length, filename, lineno, ref rval);
        Debug.Log("evaluate result = " + executeOK);
        // string source = @"var test = {};";


        
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
