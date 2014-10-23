using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameObjectWrap : MonoBehaviour
{
    public static int Constructor(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        var gt = SMData.getGlobalType(typeof(GameObject));
        if (gt == null)
            return SMDll.JS_FALSE;

        GameObject go = new GameObject();
        go.tag = "CreditCard";

        IntPtr obj = SMDll.JS_NewObject(cx, gt.jsClass, gt.proto, gt.parentProto);
        SMData.addNativeJSRelation(obj, go);

		SMDll.JShelp_SetRvalObject (cx, vp, obj);

        return 1;
    }
    public static void Register(IntPtr cx, IntPtr glob)
    {
        IntPtr jsClass = SMDll.JShelp_NewClass("GameObject", 0);

        IntPtr obj = SMDll.JS_InitClass(cx, glob,
            IntPtr.Zero, /* parentProto */
            jsClass, /* JSClass */
            GameObjectWrap.Constructor, /* constructor */
            0, /* constructor argument count*/
            IntPtr.Zero, /* properties */
            IntPtr.Zero, /* functions */
            IntPtr.Zero, /* static properties */
            IntPtr.Zero /* static functions*/
        );

        SMData.addGlobalType(typeof(GameObject), jsClass, obj, IntPtr.Zero);

        // 
        //SMDll.JS_DefineFunction(cx, obj, "tag", Marshal.GetFunctionPointerForDelegate(new SMDll.JSNative(tag)), 0, 0);
        SMDll.JS_DefineFunction(cx, obj, "tag", new SMDll.JSNative(tag), 0, 0);
    }
    static int animation(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        if (argc != 0)
            return SMDll.JS_FALSE;

        IntPtr obj = SMDll.JShelp_ThisObject(cx, vp);

        GameObject go = (GameObject)SMData.getNativeObj(obj);
        if (go == null)
            return SMDll.JS_FALSE;

        Animation ani = go.animation;
        if (ani == null)
            return SMDll.JS_FALSE;

        IntPtr jsObj = SMData.getJSObj(ani);
        if (jsObj == IntPtr.Zero)
        {
            IntPtr jsClass = SMDll.JShelp_NewClass("Animation", 0);
            jsObj = SMDll.JS_NewObject(cx, jsClass, IntPtr.Zero, IntPtr.Zero);
            SMData.addNativeJSRelation(jsObj, ani);
        }
        return SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
    }
    static int tag(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        if (argc != 0)
						return SMDll.JS_FALSE;

        IntPtr obj = SMDll.JShelp_ThisObject(cx, vp);

        GameObject go = (GameObject)SMData.getNativeObj(obj);
        if (go == null)
            return SMDll.JS_FALSE;

        string strTag = go.tag;
        Debug.Log("Tag = " + strTag);
        return SMDll.JShelp_SetRvalString(cx, vp, strTag);
    }

    // public 
}
