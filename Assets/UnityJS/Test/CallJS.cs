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

    IntPtr go;
    IntPtr funUpdate;
    JSApi.jsval rval = new JSApi.jsval();
	void Awake ()
    {
        go = JSApi.JSh_NewObjectAsClass(JSMgr.cx, JSMgr.glob, "GameObject", JSMgr.mjsFinalizer);
        JSMgr.addJSCSRelation(go, gameObject);
        JSMgr.EvaluateFile(Application.dataPath + "/StreamingAssets/JavaScript/" + jsScriptName + ".javascript", go);
        funUpdate = JSApi.JSh_GetFunction(JSMgr.cx, go, "Update");
    }

	void Update () {
        if (funUpdate != IntPtr.Zero)
            JSApi.JSh_CallFunction(JSMgr.cx, go, funUpdate, 0, IntPtr.Zero, ref rval);
	}
}
