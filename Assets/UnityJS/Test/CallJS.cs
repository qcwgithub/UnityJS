using System;
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;


public class CallJS : MonoBehaviour 
{
    public object Make(object v)
    {
        return v;
    }

	void Awake ()
    {
        Vector3 v = Vector3.zero;
        object obj1 = v;
        object obj2 = Make(null);
        if (obj1 == obj2)
            Debug.Log("Same!");
        return;

        JSMgr.InitJSEngine();
        JSMgr.EvaluateFile(Application.dataPath + "/StreamingAssets/JavaScript/test.javascript");
        JSMgr.JS_GC();
    }

	void Update () {
        
	}
}
