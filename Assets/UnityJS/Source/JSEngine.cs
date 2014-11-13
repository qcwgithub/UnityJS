using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSEngine : MonoBehaviour 
{
    public bool useReflection = false;
    List<string> lstLog = new List<string>();

    static JSEngine inst;
    public static bool inited = false;
	void Awake () 
    {
//         string path = "file://"+Application.streamingAssetsPath + "/JavaScript/RotateObject.javascript";
//         WWW w = new WWW(path);
//         while (true)
//         {
//             if (w.isDone)
//                 break;
//         }
//         Debug.Log(w.text);
//         return;
// #if UNITY_EDITOR_WIN
//         Debug.Log("windows editor");
//         return;
// #endif

        inited = true;
        DontDestroyOnLoad(gameObject);

        JSMgr.useReflection = this.useReflection;
        JSMgr.InitJSEngine();
        inst = this;
	}
	
	void Update () {
	
	}

    void OnGUI()
    {
        foreach (var v in lstLog)
            GUILayout.TextArea(v);
        //GUILayout.TextArea("hhhhhhhh");
        //GUI.TextArea(new Rect(0,0,50,30), "jjjjjjjjjjjjjjjjjjjjjj");
    }
    public static void log(string s)
    {
        if (inst != null)
            inst.lstLog.Add(s);
    }
}
