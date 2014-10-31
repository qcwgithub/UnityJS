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

public class ValueTypeWrap2
{
    public class ValueTypeWrap
    {
        public ValueTypeWrap(object o) { obj = o; }
        public object obj;
    }

    static int GetString(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalString(cx, vp, (string)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapString(IntPtr cx, uint argc, IntPtr vp)
    {
        String b = SMDll.JShelp_ArgvString(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetString), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetBool(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalBool(cx, vp, (bool)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapBool(IntPtr cx, uint argc, IntPtr vp)
    {
        Boolean b = SMDll.JShelp_ArgvBool(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetBool), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetChar(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapChar(IntPtr cx, uint argc, IntPtr vp)
    {
        Char b = (Char)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetChar), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapByte(IntPtr cx, uint argc, IntPtr vp)
    {
        Byte b = (Byte)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetByte), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapSByte(IntPtr cx, uint argc, IntPtr vp)
    {
        SByte b = (SByte)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetSByte), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapUInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt16 b = (UInt16)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetUInt16), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapInt16(IntPtr cx, uint argc, IntPtr vp)
    {
        Int16 b = (Int16)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetInt16), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapUInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt32 b = (UInt32)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetUInt32), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapInt32(IntPtr cx, uint argc, IntPtr vp)
    {
        Int32 b = (Int32)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetInt32), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapUInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        UInt64 b = (UInt64)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetUInt64), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }

    static int GetInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalInt(cx, vp, (int)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapInt64(IntPtr cx, uint argc, IntPtr vp)
    {
        Int64 b = (Int64)SMDll.JShelp_ArgvInt(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetInt64), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalDouble(cx, vp, (double)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapSingle(IntPtr cx, uint argc, IntPtr vp)
    {
        Single b = (Single)SMDll.JShelp_ArgvDouble(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetSingle), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }
    static int GetDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        IntPtr jsObj = SMDll.JShelp_ArgvObject(cx, vp, 0);
        ValueTypeWrap csObj = (ValueTypeWrap)SMData.getNativeObj(jsObj);
        SMDll.JShelp_SetRvalDouble(cx, vp, (double)csObj.obj);
        return SMDll.JS_TRUE;
    }
    static int WrapDouble(IntPtr cx, uint argc, IntPtr vp)
    {
        Double b = (Double)SMDll.JShelp_ArgvDouble(cx, vp, 0);
        var w = new ValueTypeWrap(b);
        IntPtr jsObj = SMDll.JS_NewObject(cx, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        SMDll.JS_DefineFunction(cx, jsObj, "Value", new SMDll.JSNative(GetDouble), 0/* narg */, 0);
        SMData.addNativeJSRelation(jsObj, w);
        SMDll.JShelp_SetRvalObject(cx, vp, jsObj);
        return SMDll.JS_TRUE;
    }

    public static void Register(IntPtr cx)
    {
        IntPtr CS = JSMgr.CSOBJ;
        SMDll.JS_DefineFunction(cx, CS, "string", new SMDll.JSNative(WrapString), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "bool", new SMDll.JSNative(WrapBool), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "char", new SMDll.JSNative(WrapChar), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "byte", new SMDll.JSNative(WrapByte), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "sbyte", new SMDll.JSNative(WrapSByte), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "uint16", new SMDll.JSNative(WrapUInt16), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "int16", new SMDll.JSNative(WrapInt16), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "uint32", new SMDll.JSNative(WrapUInt32), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "int32", new SMDll.JSNative(WrapInt32), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "uint64", new SMDll.JSNative(WrapUInt64), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "int64", new SMDll.JSNative(WrapInt64), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "float", new SMDll.JSNative(WrapSingle), 0/* narg */, 0);
        SMDll.JS_DefineFunction(cx, CS, "double", new SMDll.JSNative(WrapDouble), 0/* narg */, 0);
    }
}
