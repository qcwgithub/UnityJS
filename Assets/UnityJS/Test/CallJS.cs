using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;


public class CallJS : MonoBehaviour 
{
    public string jsScriptName = string.Empty;

    IntPtr go = IntPtr.Zero;
    IntPtr funUpdate = IntPtr.Zero;
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
            JSEngine.log("jsengine inited!!");

        mTrans = transform;
        go = JSApi.JSh_NewObjectAsClass(JSMgr.cx, JSMgr.glob, "GameObject", JSMgr.mjsFinalizer);
        JSMgr.addJSCSRelation(go, gameObject);

        IntPtr ptrScript = JSMgr.GetScript(jsScriptName/*, go*/);
        if (ptrScript == IntPtr.Zero)
        {
            enabled = false;
            return;
        }
//         if (ptrScript==IntPtr.Zero)
//         {
//             StartCoroutine(dlScript());
//         }
//         else
        {
            if (!JSMgr.ExecuteScript(ptrScript, go))
            {
                enabled = false;
                return;
            }
            funUpdate = JSApi.JSh_GetFunction(JSMgr.cx, go, "Update");

            {
                IntPtr myAdd = JSApi.JSh_GetFunction(JSMgr.cx, go, "myAdd");
                JSMgr.vCall.CallJSFunction(IntPtr.Zero, myAdd, 6, 9);
            }
            inited = true;
        }
    }
    bool bbb = false;
    GameObject goThis;
	void Update () {
        if (inited && funUpdate != IntPtr.Zero)
        {
            if (!JSMgr.vCall.CallJSFunction(go, funUpdate, null))
                Debug.Log("call function fail");
        }
	}
    /*public IEnumerator dlScript()
    {
        WWW w = new WWW(Application.streamingAssetsPath + "/JavaScript/" + jsScriptName + ".javascript");
        yield return w;
        string content = w.text;
        IntPtr ptrScript = JSMgr.CompileScriptContent(jsScriptName, content, JSMgr.glob);

        if (!JSMgr.ExecuteScript(ptrScript, go))
        {
            enabled = false;
        }
        else
        {
            funUpdate = JSApi.JSh_GetFunction(JSMgr.cx, go, "Update");
            inited = true;
        }
    }*/
}
