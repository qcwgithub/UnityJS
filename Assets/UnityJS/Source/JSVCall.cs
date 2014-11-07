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



public class JSVCall
{
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

    public List<JSParam> lstJSParam;
    public int jsParamCount { get { return lstJSParam.Count; } }

    public int csParamCount { get { return arrCSParam.Length; } }
    public CSParam[] arrCSParam;

    public MethodBase m_Method;
    public ParameterInfo[] m_ParamInfo;
    public object[] callParams;

    IntPtr cx;
    IntPtr vp;

    public void Reset(IntPtr cx, IntPtr vp)
    {
        if (lstJSParam == null)
            lstJSParam = new List<JSParam>();
        else
            lstJSParam.Clear();

        arrCSParam = null;

        m_Method = null;
        m_ParamInfo = null;
        callParams = null;

        this.cx = cx;
        this.vp = vp;
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

        arrCSParam = new CSParam[m_ParamInfo.Length];

        for (int i = 0; i < m_ParamInfo.Length; i++)
        {
            ParameterInfo p = m_ParamInfo[i];

            CSParam csParam = new CSParam();
            csParam.isOptional = p.IsOptional;
            csParam.isRef = p.ParameterType.IsByRef;
            csParam.isArray = p.ParameterType.IsArray;
            csParam.type = p.ParameterType;
            arrCSParam[i] = csParam;
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
        for (int i = 0; i < count; i++)
        {
            int index = i + start;
            bool bUndefined = JSApi.JShelp_ArgvIsUndefined(cx, vp, index);
            if (bUndefined)
                return true;

            JSParam jsParam = new JSParam();
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
            lstJSParam.Add(jsParam);
        }
        return true;
    }

    // 不牵到 ParamterInfo
    // 有使用 lstJSParam
    // 来判断第i个参数类型是否匹配
    public bool IsParamMatch(int csParamIndex, bool csIsOptional, Type csType)
    {
        if (csParamIndex < jsParamCount)
        {
            if (csType.IsArray)
            {
                // todo
                // 重载函数只匹配是否数组
                // 无法识别2个都是数组参数但是类型不同的重载函数，这种情况只会调用第1个
                if (!lstJSParam[csParamIndex].isArray)
                    return false;
            }
            else if (!lstJSParam[csParamIndex].isWrap)
            {
                if (lstJSParam[csParamIndex].csObj == null || csType != lstJSParam[csParamIndex].csObj.GetType())
                    return false;
            }
            else if (lstJSParam[csParamIndex].isWrap)
            {
                if (csType != lstJSParam[csParamIndex].wrappedObj.GetType())
                    return false;
            }
            else
                return false;
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
            if (jsParamCount > ps.Length)
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
        else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            if (JSApi.JShelp_ArgvIsNull(cx, vp, paramIndex))
                return null;

            IntPtr jsObj = JSApi.JShelp_ArgvObject(cx, vp, paramIndex);
            if (jsObj == IntPtr.Zero)
                return null;

            object csObject = JSMgr.getCSObj(jsObj);
            return csObject;
        }
        else
        {
            Debug.Log("ConvertJSValue2CSValue: Unknown CS type: " + t.ToString());
        }
        return null;
    }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling method
    public object JSValue_2_CSObject(int index)
    {
        JSParam jsParam = lstJSParam[index];
        int paramIndex = jsParam.index;
        CSParam csParam = arrCSParam[index];
        //ParameterInfo p = m_ParamInfo[index];

        Type t = csParam.type;

        if (csParam.isRef)
            t = t.GetElementType();

        if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            if (jsParam.isNull)
                return null;

            if (jsParam.isWrap)
                return jsParam.wrappedObj;

            return jsParam.csObj;
        }

        return JSValue_2_CSObject(csParam.type, paramIndex);
    }

    /*
     * BuildMethodArgs
     * 
     * RETURN
     * null -- fail
     * not null -- success
     */
    public object[] BuildMethodArgs(bool addDefaultValue)
    {
        ArrayList args = new ArrayList();
        for (int i = 0; i < this.arrCSParam.Length; i++)
        {
            if (i < this.lstJSParam.Count)
            {
                JSParam jsParam = lstJSParam[i];
                if (jsParam.isWrap)
                {
                    args.Add(jsParam.wrappedObj);
                }
                else if (jsParam.isArray)
                {
                    // todo
                    // 
                    Debug.Log("array parameter not supported");
                }
                else if (jsParam.isNull)
                {
                    args.Add(null);
                }
                else
                {
                    args.Add(JSValue_2_CSObject(i));
                }
            }
            else
            {
                if (arrCSParam[i].isOptional)
                {
                    if (addDefaultValue)
                        args.Add(arrCSParam[i].defaultValue);
                    else
                        break;
                }
                else
                {
                    Debug.LogError("Not enough arguments calling function '" + m_Method.Name + "'");
                    return null;
                }
            }
        }

        return args.ToArray();
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
                JSApi.JShelp_SetJsvalInt(ref val, (int)csObj);
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
        else if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsClass)
        {
            IntPtr jsObj = JSMgr.getJSObj(csObj);
            if (jsObj == IntPtr.Zero)
            {
                jsObj = JSApi.JShelp_NewObjectAsClass(cx, JSMgr.glob, t.Name, JSMgr.mjsFinalizer);
                if (jsObj != null)
                    JSMgr.addJSCSRelation(jsObj, csObj);
            }
            if (jsObj == IntPtr.Zero)
                JSApi.JShelp_SetJsvalUndefined(ref val);
            else
                JSApi.JShelp_SetJsvalObject(ref val, jsObj);
        }
        else
        {
            Debug.Log("CS -> JS: Unknown CS type: " + t.ToString());
            JSApi.JShelp_SetJsvalUndefined(ref val);
        }
        return val;
    }

    public void PushResult(object csObj)
    {
        if (/*this.op == Oper.METHOD && */ arrCSParam != null)
        {
            // 处理 ref/out 参数
            for (int i = 0; i < arrCSParam.Length; i++)
            {
                if (arrCSParam[i].isRef)
                {
                    lstJSParam[i].wrappedObj = callParams[i];
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
                    this.bGet = (op == Oper.GET_FIELD);
                    JSMgr.CSCallbackField fun = aInfo.fields[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
                }
                break;
            case Oper.GET_PROPERTY:
            case Oper.SET_PROPERTY:
                {
                    this.bGet = (op == Oper.GET_PROPERTY);
                    JSMgr.CSCallbackProperty fun = aInfo.properties[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    bool overloaded = JSApi.JShelp_ArgvBool(cx, vp, currentParamCount);
                    currentParamCount++;

                    if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
                        return JSApi.JS_FALSE;

                    JSMgr.MethodCallBackInfo[] arrMethod;
                    if (op == Oper.METHOD)
                        arrMethod = aInfo.methods;
                    else
                        arrMethod = aInfo.constructors;

                    if (overloaded)
                    {
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
                    }

                    JSMgr.CSCallbackMethod fun;
                    
                    fun = arrMethod[index].fun;
                    arrCSParam = arrMethod[index].arrCSParam;

                    if (fun == null || arrCSParam == null) 
                        return JSApi.JS_FALSE;

                    callParams = BuildMethodArgs(false);
                    if (null == callParams)
                        return JSApi.JS_FALSE;
                    
                    if (!fun(this, currentParamCount, (int)argc - currentParamCount))
                        return JSApi.JS_FALSE;
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

                    callParams = BuildMethodArgs(true);
                    if (null == callParams)
                        return JSApi.JS_FALSE;

                    result = this.m_Method.Invoke(csObj, callParams);
                }
                break;
        }

        this.PushResult(result);
        return JSApi.JS_TRUE;
    }
}