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

        
        DontDestroyOnLoad(gameObject);

        JSMgr.useReflection = this.useReflection;
        if (JSMgr.InitJSEngine())
        {
            inited = true;
            inst = this;
            Debug.Log("----------InitJSEngine OK ---");
        }
        else
            Debug.Log("----------InitJSEngine FAIL ---");
    }

    float accum = 0f;
	void Update () 
    {
        accum += Time.deltaTime;
        if (accum > 1f)
        {
            accum = 0f;
            JSApi.JSh_GC(JSMgr.rt);
        }
	}

    void OnGUI()
    {
        foreach (var v in lstLog)
            GUILayout.TextArea(v);
    }
    public static void log(string s)
    {
        if (inst != null)
            inst.lstLog.Add(s);
    }
}
