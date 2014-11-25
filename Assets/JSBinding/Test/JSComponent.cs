using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security;


/*
 * JSComponent
 * A class simply transfer callbacks to js
 * 
 * This usage might cost much cpu times. Especially when there are a lot of GameObjects in the scene
 * One likely solution is call Awake, Start, Update only once per frame
 * 
 */
public class JSComponent : MonoBehaviour 
{
    public string jsScriptName = string.Empty;

    [HideInInspector][NonSerialized]
    public IntPtr go = IntPtr.Zero;
    IntPtr funAwake = IntPtr.Zero;
    IntPtr funStart = IntPtr.Zero;
    IntPtr funUpdate = IntPtr.Zero;
    IntPtr funDestroy = IntPtr.Zero;
    IntPtr funOnGUI = IntPtr.Zero;
    //JSApi.jsval rval = new JSApi.jsval();

    bool inited = false;

    public void InitScript()
    {
        if (!JSEngine.inited)
            return;

        go = JSApi.JSh_NewObjectAsClass(JSMgr.cx, JSMgr.glob, "GameObject", JSMgr.mjsFinalizer);
        if (go == IntPtr.Zero)
            return;

        JSApi.JSh_AddObjectRoot(JSMgr.cx, ref go);


        JSMgr.addJSCSRelation(go, gameObject);

        IntPtr ptrScript = JSMgr.GetScript(jsScriptName);
        if (ptrScript == IntPtr.Zero)
        {
            Debug.Log("ptrScript is null)");
            enabled = false;
            return;
        }
        if (!JSMgr.ExecuteScript(ptrScript, go))
        {
            Debug.Log("---------- ExecuteScript fail");
            enabled = false;
            return;
        }


        funAwake = JSApi.JSh_GetFunction(JSMgr.cx, go, "Awake");
        funStart = JSApi.JSh_GetFunction(JSMgr.cx, go, "Start");
        funUpdate = JSApi.JSh_GetFunction(JSMgr.cx, go, "Update");
        funDestroy = JSApi.JSh_GetFunction(JSMgr.cx, go, "Destroy");
        funOnGUI = JSApi.JSh_GetFunction(JSMgr.cx, go, "OnGUI");

        if (funAwake != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funAwake, null);
        }
        inited = true;
    }

	void Awake ()
    {
//         if (!JSEngine.inited)
//         {
//             JSEngine.log("jsengine not inited!!");
//             return;
//         }
//         else
//         {
//             JSEngine.log("jsengine inited!!");
//         }
        if (jsScriptName.Length > 0)
            InitScript();
    }

    void Start()
    {
        if (inited && funStart != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funStart, null);
        }
    }

//    float accum = 0f;
	void Update () 
    {
//         accum += Time.deltaTime;
//         if (accum > 1f) 
//         {
//             accum = 0f;
//             JSApi.JSh_GC(JSMgr.rt);
//         }

        if (inited && funUpdate != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funUpdate, null);
        }
	}

    void OnDestroy()
    {
        if (inited && funDestroy != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funDestroy, null);
        }
        if (inited)
        {
            JSApi.JSh_RemoveObjectRoot(JSMgr.cx, ref go);
        }
    }

    void OnGUI()
    {
        if (inited && funOnGUI != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funOnGUI, null);
        }
    }
}
