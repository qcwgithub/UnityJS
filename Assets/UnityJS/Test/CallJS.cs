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
        JSMgr.useReflection = this.useReflection;
        JSMgr.InitJSEngine();
        JSMgr.EvaluateFile(Application.dataPath + "/StreamingAssets/JavaScript/test.javascript");
        JSMgr.JS_GC();
    }

	void Update () {
        
	}
}
