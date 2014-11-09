using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;


public class CallJS : MonoBehaviour 
{
    public bool useReflection = true;
	void Awake ()
    {
//         string o = "abc";
//         object oo = o;
//         Type type = typeof(string);
//         Debug.Log(type == oo.GetType());
//         return;
//         Int64 i64 = 121;
//         Debug.Log((Int32)i64);
//         return;
        JSMgr.useReflection = this.useReflection;
        JSMgr.InitJSEngine();
        JSMgr.EvaluateFile(Application.dataPath + "/StreamingAssets/JavaScript/test.javascript");
        JSMgr.JS_GC();
    }

	void Update () {
        
	}
}
