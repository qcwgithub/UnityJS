using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security;


/*
 * CallJS
 * A class simply transfer callbacks to js
 * 
 * This usage might cost much cpu times. Especially when there are a lot of GameObjects in the scene
 * One likely solution is call Awake, Start, Update only once per frame
 * 
 */
public class CallJS : MonoBehaviour 
{
    public string jsScriptName = string.Empty;

    IntPtr go = IntPtr.Zero;
    IntPtr funAwake = IntPtr.Zero;
    IntPtr funStart = IntPtr.Zero;
    IntPtr funUpdate = IntPtr.Zero;
    IntPtr funDestroy = IntPtr.Zero;
    JSApi.jsval rval = new JSApi.jsval();


    Transform mTrans;
    Vector3 rotateVar = new Vector3(0.5f, 0f, 0f);

    bool inited = false;
	void Awake ()
    {
        if (!JSEngine.inited)
        {
            JSEngine.log("jsengine not inited!!");
            return;
        }
        else
        {
            JSEngine.log("jsengine inited!!");
        }

        mTrans = transform;
        go = JSApi.JSh_NewObjectAsClass(JSMgr.cx, JSMgr.glob, "GameObject", JSMgr.mjsFinalizer);
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

        if (funAwake != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funAwake, null);
        }
        JSMgr.JS_GC();
        inited = true;
    }

    void Start()
    {
        if (inited && funStart != IntPtr.Zero)
        {
            JSMgr.vCall.CallJSFunction(go, funStart, null);
        }

        dict.Add(GameObject.Find("Cafe"),0);
    }

//    float accum = 0f;
    Dictionary<object, int> dict = new Dictionary<object, int>();
    
	void Update () 
    {
//         accum += Time.deltaTime;
//         if (accum > 1f) 
//         {
//             accum = 0f;
//             JSApi.JSh_GC(JSMgr.rt);
//         }
        Debug.Log("dict count:"+dict.Count.ToString());
        foreach (var v in dict)
        {
            Debug.Log(dict.Count.ToString() + v.Key.ToString());
        }
        dict.Remove((object)null);

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
        //Destroy();

        JSApi.JSh_RemoveObjectRoot(JSMgr.cx, ref go);
        //JSApi.JSh_GC(JSMgr.rt);
    }
}
