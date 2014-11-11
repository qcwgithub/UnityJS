using UnityEngine;
using System.Collections;

public class JSEngine : MonoBehaviour {
    public bool useReflection = true;
    
	
	void Awake () {
        JSMgr.useReflection = this.useReflection;
        JSMgr.InitJSEngine();
	}
	
	
	void Update () {
	
	}
}
