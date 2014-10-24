
using System;
using UnityEngine;

public class AnimationWrap
{
    public static int Constructor(IntPtr cx, UInt32 argc, IntPtr vp)
    {
        var gt = SMData.getGlobalType(typeof(Animation));
        if (gt == null)
        {
             Debug.Log("GlobalType not found:" + typeof(Animation).Name);
             return SMDll.JS_FALSE;
        }

        Animation go = new Animation();

        IntPtr obj = SMDll.JS_NewObject(cx, gt.jsClass, gt.proto, gt.parentProto);
        SMData.addNativeJSRelation(obj, go);

        return SMDll.JShelp_SetRvalObject (cx, vp, obj);
    }

    public static void Register(IntPtr cx, IntPtr glob)
    {
        IntPtr jsClass = SMDll.JShelp_NewClass("Animation", 0);

        IntPtr obj = SMDll.JS_InitClass(cx, glob,
            IntPtr.Zero, /* parentProto */
            jsClass, /* JSClass */
            AnimationWrap.Constructor, /* constructor */
            0, /* constructor argument count*/
            IntPtr.Zero, /* properties */
            IntPtr.Zero, /* functions */
            IntPtr.Zero, /* static properties */
            IntPtr.Zero /* static functions*/
        );

        SMData.addGlobalType(typeof(Animation), jsClass, obj, IntPtr.Zero);
    }

}
