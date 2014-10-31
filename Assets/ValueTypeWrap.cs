using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class ValueTypeWrap
{
    public class ValueTypeWrap
    {
        public ValueTypeWrap(object o) { obj = o; }
        public object obj;
    }

    static int GetBool(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        object csObj = SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalBool(cx, vp, (bool)csObj);
        return SMDll.JS_TRUE;
    }
    static int WrapBool(IntPtr cx, uint argc, IntPtr vp)
    {
        bool b = SMDll.JShelp_ArgvBool(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JShelp_NewObjectAsClass(cx, CallJS.glob, "");
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }

    public static void Register()
    {

    }
}
