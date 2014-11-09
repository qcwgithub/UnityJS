using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;



public class JSVCall
{
    public enum Consts
    {
        MaxParams = 16,
    }
    public class JSParam
    {
        public int index; // 参数位置
        public object csObj; //对应的cs对象，对于基础类型，string，枚举，这个为null
        public bool isWrap { get { return csObj != null && csObj is JSValueWrap.Wrap; } }
        public object wrappedObj { get { return ((JSValueWrap.Wrap)csObj).obj; } set { ((JSValueWrap.Wrap)csObj).obj = value; } }
        public bool isArray;
        public bool isNull;
    }
    // CS函数的参数信息
    // 只存储一些要使用多次的
    public class CSParam
    {
        public bool isRef;
        public bool isOptional;
        public bool isArray;
        public Type type;
        public object defaultValue;
        public CSParam() { }
        public CSParam(bool r, bool o, bool i, Type t, object d)
        {
            isRef = r;
            isOptional = o;
            isArray = i;
            type = t;
            defaultValue = d;
        }
    }

    public JSParam[] arrJSParam = null;
    public int arrJSParamsLength = 0;

    public CSParam[] arrCSParam = null;
    public int arrCSParamsLength = 0;

    public object[] callParams = null;
    public int callParamsLength = 0;

    public MethodBase m_Method;
    public ParameterInfo[] m_ParamInfo;

    IntPtr cx;
    IntPtr vp;
    public int currIndex = 0;
    JSApi.jsval valReturn = new JSApi.jsval();

    public void Reset(IntPtr cx, IntPtr vp)
    {
        if (arrJSParam == null)
        {
            arrJSParam = new JSParam[(int)Consts.MaxParams];
            arrCSParam = new CSParam[(int)Consts.MaxParams];
            for (int i = 0; (int)Consts.MaxParams > i; i++)
            {
                arrJSParam[i] = new JSParam();
                arrCSParam[i] = new CSParam();
            }
            callParams = new object[(int)Consts.MaxParams];
        }
        arrJSParamsLength = 0;
        arrCSParamsLength = 0;
        callParamsLength = 0;

        m_Method = null;
        m_ParamInfo = null;

        this.cx = cx;
        this.vp = vp;
        currIndex = 0;
    }
    public Boolean getBool() { return JSApi.JShelp_ArgvBool(cx, vp, currIndex++); }
    public String  getString() { 
		return JSApi.JShelp_ArgvString(cx, vp, currIndex++); }
    public Char    getChar() { return (Char)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Byte    getByte() { return (Byte)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public SByte   getSByte() { return (SByte)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public UInt16  getUInt16() { return (UInt16)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Int16   getInt16() { return (Int16)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public UInt32  getUInt32() { return (UInt32)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Int32   getInt32() { return (Int32)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public UInt64  getUInt64() { return (UInt64)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Int64   getInt64() { return (Int64)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Int32   getEnum() { return (Int32)JSApi.JShelp_ArgvInt(cx, vp, currIndex++); }
    public Single  getFloat() { return (float) JSApi.JShelp_ArgvDouble(cx, vp, currIndex++); }
    public Double  getDouble() { return (Double)JSApi.JShelp_ArgvDouble(cx, vp, currIndex++); }
    public JSValueWrap.Wrap getWrap()
    {
        IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, currIndex++);
        object csObj = JSMgr.getCSObj(jsObj);
        return (JSValueWrap.Wrap)csObj;
    }
    public object getObject()
    {
        IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, currIndex++);
        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj is JSValueWrap.Wrap)
            return ((JSValueWrap.Wrap)csObj).obj;
        else
            return csObj;
    }

    public void returnBool(bool v) { JSApi.JShelp_SetRvalBool(cx, vp, v); }
    public void returnString(String v) { JSApi.JShelp_SetRvalString(cx, vp, v); }
    public void returnChar(Char v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnByte(Byte v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnSByte(SByte v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnUInt16(UInt16 v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnInt16(Int16 v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnUInt32(UInt32 v) { JSApi.JShelp_SetRvalInt(cx, vp, (Int32)v); }
    public void returnInt32(Int32 v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnUInt64(UInt64 v) { JSApi.JShelp_SetRvalInt(cx, vp, (Int32)v); }
    public void returnInt64(Int64 v) { 
        JSApi.JShelp_SetRvalInt(cx, vp, (Int32)v); 
    }
    public void returnEnum(Int32 v) { JSApi.JShelp_SetRvalInt(cx, vp, v); }
    public void returnFloat(Single v) { JSApi.JShelp_SetRvalDouble(cx, vp, v); }
    public void returnDouble(Double v) { JSApi.JShelp_SetRvalDouble(cx, vp, v); }
    public void returnObject(string className, object csObj)
    {
        JSApi.JShelp_SetJsvalUndefined(ref this.valReturn);
        if (csObj == null)
        {
            JSApi.JShelp_SetRvalJSVAL(cx, vp, ref this.valReturn);
            return;
        }
        IntPtr jsObj = JSMgr.getJSObj(csObj);
        if (jsObj == IntPtr.Zero)
        {
            jsObj = JSApi.JShelp_NewObjectAsClass(cx, JSMgr.glob, className, JSMgr.mjsFinalizer);
            if (jsObj != IntPtr.Zero)
                JSMgr.addJSCSRelation(jsObj, csObj);
        }
        if (jsObj == IntPtr.Zero)
            JSApi.JShelp_SetJsvalUndefined(ref this.valReturn);
        else
            JSApi.JShelp_SetJsvalObject(ref this.valReturn, jsObj);
        JSApi.JShelp_SetRvalJSVAL(cx, vp, ref this.valReturn);
    }

    // ref/out
    public void updateRefObject(/*object csObj, */object csObjNew)
    {

    }


    /*
     * ExtractCSParams
     *
     * extract some info to use latter
     * write into m_ParamInfo and lstCSParam
     * 仅反射版本使用
     */
    public void ExtractCSParams()
    {
        if (m_ParamInfo == null)
            m_ParamInfo = m_Method.GetParameters();

        arrCSParamsLength = m_ParamInfo.Length;
        for (int i = 0; i < m_ParamInfo.Length; i++)
        {
            ParameterInfo p = m_ParamInfo[i];

            CSParam csParam = arrCSParam[i];
            csParam.isOptional = p.IsOptional;
            csParam.isRef = p.ParameterType.IsByRef;
            csParam.isArray = p.ParameterType.IsArray;
            csParam.type = p.ParameterType;
        }
    }

    /*
     * ExtractJSParams
     * 
     * 写入lstJSParam
     * 
     * RETURN
     * false -- fail
     * true  -- success
     * 
     * 对于枚举类型、基本类型：没有处理
     */
    public bool ExtractJSParams(int start, int count)
    {
        arrJSParamsLength = 0;
        for (int i = 0; i < count; i++)
        {
            int index = i + start;
            bool bUndefined = JSApi.JShelp_ArgvIsUndefined(cx, vp, index);
            if (bUndefined)
                return true;

            JSParam jsParam = arrJSParam[arrJSParamsLength++]; //new JSParam();
            jsParam.index = index;
            jsParam.isNull = JSApi.JShelp_ArgvIsNull(cx, vp, index);
            jsParam.isArray = false;
            jsParam.csObj = null;

            IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, index);
            if (jsObj == IntPtr.Zero)
            {
                jsParam.csObj = null;
            }
            else if (JSApi.JS_IsArrayObject(cx, jsObj))
            {
                jsParam.isArray = true;
                Debug.LogError("parse js array to cs is not supported");
            }
            else
            {
                object csObj = JSMgr.getCSObj(jsObj);
                if (csObj == null)
                {
                    Debug.Log("ExtractJSParams: CSObject is not found");
                    return false;
                }
                jsParam.csObj = csObj;
            }
            //lstJSParam.Add(jsParam);
        }
        return true;
    }

    // 不牵到 ParamterInfo
    // 有使用 lstJSParam
    // 来判断第i个参数类型是否匹配
    public bool IsParamMatch(int csParamIndex, bool csIsOptional, Type csType)
    {
        if (csParamIndex < arrJSParamsLength)
        {
            if (csType.IsArray)
            {
                // todo
                // 重载函数只匹配是否数组
                // 无法识别2个都是数组参数但是类型不同的重载函数，这种情况只会调用第1个
                if (!arrJSParam[csParamIndex].isArray)
                    return false;
            }
            else if (arrJSParam[csParamIndex].isWrap)
            {
                if ((csType != arrJSParam[csParamIndex].wrappedObj.GetType()))
                    return false;
            }
            else// if (!arrJSParam[csParamIndex].isWrap)
            {
                if (arrJSParam[csParamIndex].csObj != null)
                {
                    if (csType != arrJSParam[csParamIndex].csObj.GetType())
                        return false;
                }
                else
                {
                    if (csType == typeof(bool))
                        return JSApi.JShelp_ArgvIsBool(cx, vp, arrJSParam[csParamIndex].index);
                    else if (csType == typeof(string))
                        return JSApi.JShelp_ArgvIsString(cx, vp, arrJSParam[csParamIndex].index);
                    else if (csType.IsEnum || csType.IsPrimitive)
                        return JSApi.JShelp_ArgvIsNumber(cx, vp, arrJSParam[csParamIndex].index);
                    else
                        return false;
                }
            }
        }
        else
        {
            if (!csIsOptional)
                return false;
        }
        return true;
    }

    /*
     * MatchOverloadedMethod
     * 
     * write into this.method and this.ps
     *
     * 仅反射方案使用这个函数
     */
    public int MatchOverloadedMethod(MethodBase[] methods, int methodIndex)
    {
        for (int i = methodIndex; i < methods.Length; i++)
        {
            MethodBase method = methods[i];
            if (method.Name != methods[methodIndex].Name)
                return -1;
            ParameterInfo[] ps = method.GetParameters();
            if (arrJSParamsLength > ps.Length)
                continue;

            bool matchSuccess = true;
            for (int j = 0; j < ps.Length; j++)
            {
                ParameterInfo p = ps[j];

                if (!IsParamMatch(j, p.IsOptional, p.ParameterType))
                {
                    matchSuccess = false;
                    break;
                }
            }

            if (matchSuccess)
            {
                // yes, this method is what we want
                this.m_Method = method;
                this.m_ParamInfo = ps;
                return i;
            }
        }
        return -1;
    }

    /*
     * 不使用反射的方案使用这个函数
     */
    public bool IsMethodMatch(CSParam[] arrCSParam)
    {
        for (int i = 0; i < arrCSParam.Length; i++)
        {
            if (!IsParamMatch(i, arrCSParam[i].isOptional, arrCSParam[i].type))
                return false;
        }
        return true;
    }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling property/field
    public object JSValue_2_CSObject(Type t, int paramIndex)
    {
        if (t.IsArray)
        {
            Debug.LogError("JSValue_2_CSObject: could not pass an array");
            return null;
        }

        if (t.IsByRef)
            t = t.GetElementType();

        if (t == typeof(string))
            return JSApi.JShelp_ArgvString(cx, vp, paramIndex);
        else if (t.IsEnum)
            return JSApi.JShelp_ArgvInt(cx, vp, paramIndex);
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                return JSApi.JShelp_ArgvBool(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                return JSApi.JShelp_ArgvInt(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                return JSApi.JShelp_ArgvDouble(cx, vp, paramIndex);
            }
            else
            {
                Debug.Log("ConvertJSValue2CSValue: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else// if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsValueType)
        {
            if (JSApi.JShelp_ArgvIsNull(cx, vp, paramIndex))
                return null;

            IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, paramIndex);
            if (jsObj == IntPtr.Zero)
                return null;

            object csObject = JSMgr.getCSObj(jsObj);
            return csObject;
        }
//         else
//         {
//             Debug.Log("ConvertJSValue2CSValue: Unknown CS type: " + t.ToString());
//         }
        return null;
    }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling method
    public object JSValue_2_CSObject(int index)
    {
        JSParam jsParam = arrJSParam[index];
        int paramIndex = jsParam.index;
        CSParam csParam = arrCSParam[index];
        //ParameterInfo p = m_ParamInfo[index];

        Type t = csParam.type;

        if (csParam.isRef)
            t = t.GetElementType();

        if (jsParam.isNull) return null;
        else if (jsParam.isWrap) return jsParam.wrappedObj;
        else if (jsParam.csObj != null) return jsParam.csObj;

//         if (typeof(UnityEngine.Object).IsAssignableFrom(t))
//         {
//             if (jsParam.isNull)
//                 return null;
// 
//             if (jsParam.isWrap)
//                 return jsParam.wrappedObj;
// 
//             return jsParam.csObj;
//         }

        return JSValue_2_CSObject(csParam.type, paramIndex);
    }

    /*
     * BuildMethodArgs
     * 
     * RETURN
     * null -- fail
     * not null -- success
     */
    public bool BuildMethodArgs(bool addDefaultValue)
    {
        //ArrayList args = new ArrayList();
        callParamsLength = 0;
        for (int i = 0; i < this.arrCSParamsLength; i++)
        {
            callParamsLength++;

            if (i < this.arrJSParamsLength)
            {
                JSParam jsParam = arrJSParam[i];
                if (jsParam.isWrap)
                {
                    //args.Add(jsParam.wrappedObj);
                    callParams[i] = jsParam.wrappedObj;
                }
                else if (jsParam.isArray)
                {
                    // todo
                    // 
                    Debug.Log("array parameter not supported");
                    callParams[i] = null;
                }
                else if (jsParam.isNull)
                {
                    //args.Add(null);
                    callParams[i] = null;
                }
                else
                {
                    //args.Add(JSValue_2_CSObject(i));
                    callParams[i] = JSValue_2_CSObject(i);
                }
            }
            else
            {
                if (arrCSParam[i].isOptional)
                {
                    if (addDefaultValue)//args.Add(arrCSParam[i].defaultValue);
                        callParams[i] = arrCSParam[i].defaultValue;
                    else
                        break;
                }
                else
                {
                    Debug.LogError("Not enough arguments calling function '" + m_Method.Name + "'");
                    return false;
                }
            }
        }
        //return args.ToArray();
        return true;
    }

    // CS -> JS
    // 将 cs 对象转换为 js 对象
    public JSApi.jsval CSObject_2_JSValue(object csObj)
    {
        JSApi.jsval val = new JSApi.jsval();
        JSApi.JShelp_SetJsvalUndefined(ref val);

        if (csObj == null)
        {
            return val;
        }

        Type t = csObj.GetType();
        if (t == typeof(void))
        {
            return val;
        }
        else if (t == typeof(string))
        {
            JSApi.JShelp_SetJsvalString(cx, ref val, (string)csObj);
        }
        else if (t.IsEnum)
        {
            JSApi.JShelp_SetJsvalInt(ref val, (int)csObj);
        }
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                JSApi.JShelp_SetJsvalBool(ref val, (bool)csObj);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                JSApi.JShelp_SetJsvalInt(ref val, (Int32)(Int64)csObj);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                JSApi.JShelp_SetJsvalDouble(ref val, (double)csObj);
            }
            else
            {
                Debug.Log("CS -> JS: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else if (t.IsArray)
        {
            // todo
            // 如果返回数组的数组可能会有问题
            Array arr = csObj as Array;
            if (arr.Length > 0 && arr.GetValue(0).GetType().IsArray)
            {
                Debug.LogWarning("cs return [][] may cause problems.");
            }

            IntPtr jsArr = JSApi.JS_NewArrayObject(cx, arr.Length);
            
            for (int i = 0; i < arr.Length; i++)
            {
                JSApi.jsval subVal = CSObject_2_JSValue(arr.GetValue(i));
                JSApi.JS_SetElement(cx, jsArr, (uint)i, ref subVal);
            }
            JSApi.JShelp_SetJsvalObject(ref val, jsArr);
        }
        else// if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsClass || t.IsValueType)
        {
            IntPtr jsObj = JSMgr.getJSObj(csObj);
            if (jsObj == IntPtr.Zero)
            {
                jsObj = JSApi.JShelp_NewObjectAsClass(cx, JSMgr.glob, t.Name, JSMgr.mjsFinalizer);
                if (jsObj != IntPtr.Zero)
                    JSMgr.addJSCSRelation(jsObj, csObj);
            }
            if (jsObj == IntPtr.Zero)
                JSApi.JShelp_SetJsvalUndefined(ref val);
            else
                JSApi.JShelp_SetJsvalObject(ref val, jsObj);
        }
//         else
//         {
//             Debug.Log("CS -> JS: Unknown CS type: " + t.ToString());
//             JSApi.JShelp_SetJsvalUndefined(ref val);
//         }
        return val;
    }

    public void PushResult(object csObj)
    {
        if (/*this.op == Oper.METHOD && */ arrCSParam != null)
        {
            // 处理 ref/out 参数
            for (int i = 0; i < arrCSParamsLength; i++)
            {
                if (arrCSParam[i].isRef)
                {
                    arrJSParam[i].wrappedObj = callParams[i];
                }
            }
        }

        JSApi.jsval val = CSObject_2_JSValue(csObj);
        JSApi.JShelp_SetRvalJSVAL(cx, vp, ref val);
    }


    public enum Oper
    {
        GET_FIELD = 0,
        SET_FIELD = 1,
        GET_PROPERTY = 2,
        SET_PROPERTY = 3,
        METHOD = 4,
        CONSTRUCTOR = 5,
    }

    public bool bGet = false, bStatic = false;
    public object csObj, result, arg;
    public object[] args;
    public int currentParamCount = 0;
    public Oper op;
//     public static bool CSCallback()
//     {
//         if (JSVCall.bGet)
//             result = ((GameObject)JSVCall.jsObj).activeSelf;
//         else
//         {
//             object arg = JSValue_2_CSObject(typeof(bool), JSVCall.currentParamCount);
//             ((GameObject)JSVCall.jsObj).activeSelf = (bool)JSVCall.arg;
//         }
//     }

    public int CallCallback(IntPtr cx, uint argc, IntPtr vp)
    {
        this.Reset(cx, vp);

        // 前面4个参数是固定的
        this.op = (Oper)JSApi.JShelp_ArgvInt(cx, vp, 0);
        int slot = JSApi.JShelp_ArgvInt(cx, vp, 1);
        int index = JSApi.JShelp_ArgvInt(cx, vp, 2);
        bool isStatic = JSApi.JShelp_ArgvBool(cx, vp, 3);

        if (slot < 0 || slot >= JSMgr.allCallbackInfo.Count)
        {
            Debug.LogError("Bad slot: " + slot);
            return JSApi.JS_FALSE;
        }
        JSMgr.CallbackInfo aInfo = JSMgr.allCallbackInfo[slot];

        currentParamCount = 4;
        if (!isStatic)
        {
            IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, 4);
            if (jsObj == IntPtr.Zero)
                return JSApi.JS_FALSE;

            this.csObj = JSMgr.getCSObj(jsObj);
            if (this.csObj == null)
                return JSApi.JS_FALSE;

            currentParamCount++;
        }
        
        switch (op)
        {
            case Oper.GET_FIELD:
            case Oper.SET_FIELD:
                {
                    currIndex = currentParamCount;
                    this.bGet = (op == Oper.GET_FIELD);
                    JSMgr.CSCallbackField fun = aInfo.fields[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
			        return JSApi.JS_TRUE;
                }
                break;
            case Oper.GET_PROPERTY:
            case Oper.SET_PROPERTY:
                {
                    currIndex = currentParamCount;
                    this.bGet = (op == Oper.GET_PROPERTY);
                    JSMgr.CSCallbackProperty fun = aInfo.properties[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
			        return JSApi.JS_TRUE;
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    bool overloaded = JSApi.JShelp_ArgvBool(cx, vp, currentParamCount);
                    currentParamCount++;

                    JSMgr.MethodCallBackInfo[] arrMethod;
                    if (op == Oper.METHOD)
                        arrMethod = aInfo.methods;
                    else
                        arrMethod = aInfo.constructors;

                    // JS传过来的参数个数
                    // 重载函数的参数个数由 ExtractJSParams 已经计算好了 
                    int jsParamCount = (int)argc - currentParamCount;
                    if (!overloaded)
                    {
                        // 对于无重载函数
                        // 如果没有可选参数的话 每次也就多一次判断是不是 Undefined
                        int i = (int)argc;
                        while (i > 0 && JSApi.JShelp_ArgvIsUndefined(cx, vp, --i))
                            jsParamCount--;
                    }
                    else
                    {
                        if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
                            return JSApi.JS_FALSE;

                        string methodName = arrMethod[index].methodName;

                        int i = index;
                        while (true)
                        {
                            if (IsMethodMatch(arrMethod[i].arrCSParam))
                            {
                                index = i;
                                break;
                            }
                            i++;
                            if (arrMethod[i].methodName != methodName)
                            {
                                Debug.LogError("Overloaded function can't find match: " + methodName);
                                return JSApi.JS_FALSE;
                            }
                        }

                        jsParamCount = arrJSParamsLength;
                    }
                    
                    currIndex = currentParamCount;
                    arrMethod[index].fun(this, currentParamCount, jsParamCount);
                    return JSApi.JS_TRUE;

//                     if (overloaded)
//                     {
//                         string methodName = arrMethod[index].methodName;
// 
//                         int i = index;
//                         while (true)
//                         {
//                             if (IsMethodMatch(arrMethod[i].arrCSParam))
//                             {
//                                 index = i;
//                                 break;
//                             }
//                             i++;
//                             if (arrMethod[i].methodName != methodName)
//                             {
//                                 Debug.LogError("Overloaded function can't find match: " + methodName);
//                                 return JSApi.JS_FALSE;
//                             }
//                         }
//                     }
// 
//                     JSMgr.CSCallbackMethod fun;
//                     
//                     fun = arrMethod[index].fun;
//                     arrCSParam = arrMethod[index].arrCSParam;
//                     arrCSParamsLength = arrCSParam.Length;
// 
//                     if (fun == null || arrCSParam == null) 
//                         return JSApi.JS_FALSE;
// 
//                     if (!BuildMethodArgs(false))
//                          return JSApi.JS_FALSE;
//                     
//                     if (!fun(this, currentParamCount, (int)argc - currentParamCount))
//                         return JSApi.JS_FALSE;
                }
                break;
        }

        this.PushResult(result);
        return JSApi.JS_TRUE;
    }

    public int CallReflection(IntPtr cx, uint argc, IntPtr vp)
    {
        this.Reset(cx, vp);

        // 前面4个参数是固定的
        this.op = (Oper)JSApi.JShelp_ArgvInt(cx, vp, 0);
        int slot = JSApi.JShelp_ArgvInt(cx, vp, 1);
        int index = JSApi.JShelp_ArgvInt(cx, vp, 2);
        bool isStatic = JSApi.JShelp_ArgvBool(cx, vp, 3);

        if (slot < 0 || slot >= JSMgr.allTypeInfo.Count)
        {
            Debug.LogError("Bad slot: " + slot);
            return JSApi.JS_FALSE;
        }
        JSMgr.ATypeInfo aInfo = JSMgr.allTypeInfo[slot];

        currentParamCount = 4;
        object csObj = null;
        if (!isStatic)
        {
            IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, 4);
            if (jsObj == IntPtr.Zero)
                return JSApi.JS_FALSE;

            csObj = JSMgr.getCSObj(jsObj);
            if (csObj == null)
                return JSApi.JS_FALSE;

            currentParamCount++;
        }

        //object result = null;

        switch (op)
        {
            case Oper.GET_FIELD:
                {
                    result = aInfo.fields[index].GetValue(csObj);
                }
                break;
            case Oper.SET_FIELD:
                {
                    FieldInfo field = aInfo.fields[index];
                    field.SetValue(csObj, JSValue_2_CSObject(field.FieldType, currentParamCount));
                }
                break;
            case Oper.GET_PROPERTY:
                {
                    result = aInfo.properties[index].GetValue(csObj, null);
                }
                break;
            case Oper.SET_PROPERTY:
                {
                    PropertyInfo property = aInfo.properties[index];
                    property.SetValue(csObj, JSValue_2_CSObject(property.PropertyType, currentParamCount), null);
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    bool overloaded = JSApi.JShelp_ArgvBool(cx, vp, currentParamCount);
                    currentParamCount++;

                    if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
                        return JSApi.JS_FALSE;

                    if (overloaded)
                    {
                        MethodBase[] methods = aInfo.methods;
                        if (op == Oper.CONSTRUCTOR)
                            methods = aInfo.constructors;

                        if (-1 == MatchOverloadedMethod(methods, index))
                            return JSApi.JS_FALSE;
                    }
                    else
                    {
                        m_Method = aInfo.methods[index];
                        if (op == Oper.CONSTRUCTOR)
                            m_Method = aInfo.constructors[index];
                    }

                    this.ExtractCSParams();

                    //!!!!!!!!!!!!!!!!!!!!
                    if (!BuildMethodArgs(true))
                         return JSApi.JS_FALSE;

                    object[] cp = new object[callParamsLength];
                    for (int i = 0; callParamsLength > i; i++)
                        cp[i] = callParams[i];

                    result = this.m_Method.Invoke(csObj, cp);
                }
                break;
        }

        this.PushResult(result);
        return JSApi.JS_TRUE;
    }
}