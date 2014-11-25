/*
 * JSValueWrap
 * author   answerwinner
 * desc   'wrap' is ONLY used for wrapping primitives, string, when passing ref/out parameters. 
 *        for struct/class, since there is an object corresponding to js object, ref/out is naturally supported. example: see Physics.Raycast function
 *        
 *  
 */

using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

public class JSValueWrap
{
    public class Wrap
    {
        public Wrap(object o) { obj = o; }
        public object obj;
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetString(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalString(cx, vp, (string)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapString(IntPtr cx, uint argc, IntPtr vp)
    {
        String b = JSApi.JSh_ArgvStringS(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetString)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetBool(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalBool(cx, vp, (bool)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapBool(IntPtr cx, uint argc, IntPtr vp)
    {
        Boolean b = JSApi.JSh_ArgvBool(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetBool)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetChar(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapChar(IntPtr cx, uint argc, IntPtr vp)
    {
        Char b = (Char)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetChar)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapByte(IntPtr cx, uint argc, IntPtr vp)
    {
        Byte b = (Byte)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetByte)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        SByte b = (SByte)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetSByte)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt16 b = (UInt16)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetUInt16)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        Int16 b = (Int16)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetInt16)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt32 b = (UInt32)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetUInt32)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        Int32 b = (Int32)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetInt32)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt64 b = (UInt64)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetUInt64)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        Int64 b = (Int64)JSApi.JSh_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetInt64)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalDouble(cx, vp, (double)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        Single b = (Single)JSApi.JSh_ArgvDouble(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetSingle)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int GetDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JSh_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JSh_SetRvalDouble(cx, vp, (double)csObj.obj);
        return JSApi.JS_TRUE;
    }
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static int WrapDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        Double b = (Double)JSApi.JSh_ArgvDouble(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JSh_NewMyClass(cx, JSMgr.mjsFinalizer);
        JSApi.JSh_DefineFunction(cx, jsObj, "Value", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetDouble)), 0/* narg */, 0);
        JSMgr.addJSCSRelation(jsObj, w);
        JSApi.JSh_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }

    public static void Register(IntPtr CS, IntPtr cx)
    {
        JSApi.JSh_DefineFunction(cx, CS, "string", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapString)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "bool", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapBool)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "char", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapChar)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "byte", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapByte)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "sbyte", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapSByte)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "uint16", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapUInt16)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "int16", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapInt16)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "uint32", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapUInt32)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "int32", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapInt32)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "uint64", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapUInt64)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "int64", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapInt64)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "float", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapSingle)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, CS, "double", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(WrapDouble)), 0/* narg */, 0);
    }
}
