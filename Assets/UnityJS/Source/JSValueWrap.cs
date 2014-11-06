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

public class JSValueWrap
{
    public class Wrap
    {
        public Wrap(object o) { obj = o; }
        public object obj;
    }

    static int GetString(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalString(cx, vp, (string)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapString(IntPtr cx, uint argc, IntPtr vp)
    {
        String b = JSApi.JShelp_ArgvString(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetString), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetBool(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalBool(cx, vp, (bool)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapBool(IntPtr cx, uint argc, IntPtr vp)
    {
        Boolean b = JSApi.JShelp_ArgvBool(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetBool), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetChar(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapChar(IntPtr cx, uint argc, IntPtr vp)
    {
        Char b = (Char)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetChar), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapByte(IntPtr cx, uint argc, IntPtr vp)
    {
        Byte b = (Byte)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetByte), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        SByte b = (SByte)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetSByte), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt16 b = (UInt16)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetUInt16), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        Int16 b = (Int16)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetInt16), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt32 b = (UInt32)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetUInt32), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        Int32 b = (Int32)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetInt32), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt64 b = (UInt64)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetUInt64), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }

    static int GetInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        Int64 b = (Int64)JSApi.JShelp_ArgvInt(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetInt64), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalDouble(cx, vp, (double)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        Single b = (Single)JSApi.JShelp_ArgvDouble(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetSingle), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }
    static int GetDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = JSApi.JShelp_ThisObject(cx, vp);
        Wrap csObj = (Wrap)JSMgr.getCSObj(jsObj);
        JSApi.JShelp_SetRvalDouble(cx, vp, (double)csObj.obj);
        return JSApi.JS_TRUE;
    }
    static int WrapDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        Double b = (Double)JSApi.JShelp_ArgvDouble(cx, vp, 0);
        var w = new Wrap(b);
        IntPtr jsObj = JSApi.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        JSApi.JS_DefineFunction(cx, jsObj, "Value", new JSApi.JSNative(GetDouble), 0/* narg */, 0);
        JSMgr.addNativeJSRelation(jsObj, w);
        JSApi.JShelp_SetRvalObject(cx, vp, jsObj);
        return JSApi.JS_TRUE;
    }

    public static void Register(IntPtr CS, IntPtr cx)
    {
        JSApi.JS_DefineFunction(cx, CS, "string", new JSApi.JSNative(WrapString), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "bool", new JSApi.JSNative(WrapBool), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "char", new JSApi.JSNative(WrapChar), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "byte", new JSApi.JSNative(WrapByte), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "sbyte", new JSApi.JSNative(WrapSByte), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "uint16", new JSApi.JSNative(WrapUInt16), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "int16", new JSApi.JSNative(WrapInt16), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "uint32", new JSApi.JSNative(WrapUInt32), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "int32", new JSApi.JSNative(WrapInt32), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "uint64", new JSApi.JSNative(WrapUInt64), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "int64", new JSApi.JSNative(WrapInt64), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "float", new JSApi.JSNative(WrapSingle), 0/* narg */, 0);
        JSApi.JS_DefineFunction(cx, CS, "double", new JSApi.JSNative(WrapDouble), 0/* narg */, 0);
    }
}
