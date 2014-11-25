// mozjswrap.cpp
// author: answerwinner
// desc: wraps for SpiderMonkey api

#include "jsapi.h"

#ifdef _WINDOWS
#define DLLEXPORT __declspec( dllexport ) 
#else
#define DLLEXPORT __cdecl
#endif
extern "C"
{
    void sc_finalize(JSFreeOp* freeOp, JSObject* obj)
    {

    }

    static JSClass global_class = 
    {
        "global", JSCLASS_GLOBAL_FLAGS,
        JS_PropertyStub, JS_DeletePropertyStub, JS_PropertyStub, JS_StrictPropertyStub,
        JS_EnumerateStub, JS_ResolveStub, JS_ConvertStub, sc_finalize,
        JSCLASS_NO_OPTIONAL_MEMBERS
    };

    static JSClass qiucw_class = 
    {
        "qicuw",0,
        JS_PropertyStub, JS_DeletePropertyStub, JS_PropertyStub, JS_StrictPropertyStub,
        JS_EnumerateStub, JS_ResolveStub, JS_ConvertStub, NULL,
        JSCLASS_NO_OPTIONAL_MEMBERS
    };

    DLLEXPORT bool JSh_Init(void) 
    { 
        return JS_Init(); 
    }
    DLLEXPORT void JSh_ShutDown(void) 
    { 
        JS_ShutDown(); 
    }
    DLLEXPORT JSRuntime* JSh_NewRuntime(uint32_t maxbytes, int useHelperThreads) 
    { 
        return JS_NewRuntime(maxbytes, (JSUseHelperThreads)useHelperThreads); 
    }
    DLLEXPORT void JSh_DestroyRuntime(JSRuntime *rt) 
    { 
        JS_DestroyRuntime(rt); 
    }
    DLLEXPORT void JSh_SetGCParameter(JSRuntime *rt, int key, uint32_t value) 
    { 
        return JS_SetGCParameter(rt, (JSGCParamKey)key, value); 
    }

    DLLEXPORT JSContext* JSh_NewContext(JSRuntime *rt, size_t stackChunkSize) 
    { 
        return JS_NewContext(rt, stackChunkSize); 
    }
    DLLEXPORT void JSh_DestroyContext(JSContext *cx) 
    { 
        JS_DestroyContext(cx); 
    }
    DLLEXPORT void JSh_DestroyContextNoGC(JSContext *cx) 
    { 
        JS_DestroyContextNoGC(cx); 
    }

    DLLEXPORT JSErrorReporter JSh_SetErrorReporter(JSContext *cx, JSErrorReporter er) 
    { 
        return JS_SetErrorReporter(cx, er); 
    }
    DLLEXPORT void* JSh_GetContextPrivate(JSContext *cx)
    {
        return JS_GetContextPrivate(cx);
    }
    DLLEXPORT void JSh_SetContextPrivate(JSContext *cx, void *data)
    {
        JS_SetContextPrivate(cx, data);
    }
    DLLEXPORT void* JSh_GetPrivate(JSObject *obj) 
    {
        return JS_GetPrivate(obj);
    }
    DLLEXPORT void JSh_SetPrivate(JSObject *obj, void *data)
    {
        JS_SetPrivate(obj, data);
    }
    DLLEXPORT JSObject* JSh_GetParent(JSObject *obj)
    {
        return JS_GetParent(obj);
    }
    DLLEXPORT bool JSh_SetParent(JSContext *cx, JSObject *obj, JSObject *parent)
    {
        return JS_SetParent(cx, obj, parent);
    }

    DLLEXPORT JSObject* JSh_NewGlobalObject(JSContext *cx, int hookOption)
    {
        JS::CompartmentOptions options;
        options.setVersion(JSVERSION_LATEST);

        return JS_NewGlobalObject(cx, &global_class, 0/*principals*/, (JS::OnNewGlobalHookOption)hookOption, options);
    }

    DLLEXPORT bool JSh_InitStandardClasses(JSContext *cx, JSObject *obj)
    { 
        return JS_InitStandardClasses(cx, obj); 
    }
    DLLEXPORT JSObject* JSh_InitReflect(JSContext *cx, JSObject *global) 
    { 
        return JS_InitReflect(cx, global); 
    }
    DLLEXPORT JSFunction* JSh_DefineFunction(JSContext *cx, JSObject *obj, const char *name, JSNative call, unsigned nargs, unsigned attrs)
    {
        return JS_DefineFunction(cx, obj ,name, call, nargs, attrs);
    }
    DLLEXPORT int JSh_GetErroReportLineNo(JSErrorReport* report) 
    { 
        return report->lineno;
    }
    DLLEXPORT const char* JSh_GetErroReportFileName(JSErrorReport* report) 
    { 
        if (!report->filename) 
            return "no_file_name"; 
        else 
            return report->filename; 
    }
    DLLEXPORT JSObject* JSh_NewArrayObject(JSContext *cx, int length, jsval *vector) 
    { 
        return JS_NewArrayObject(cx, length, vector); 
    }
    DLLEXPORT bool JSh_IsArrayObject(JSContext *cx, JSObject *obj)
    {
        return !!JS_IsArrayObject(cx, obj);
    }
    // return -1: fail
    DLLEXPORT int JSh_GetArrayLength(JSContext *cx, JSObject *obj)
    {
        uint32_t length = 0;
        if (!JS_GetArrayLength(cx, obj, &length))
            return -1;
        return (int)length;
    }
    DLLEXPORT bool JSh_GetElement(JSContext *cx, JSObject *obj, uint32_t index, jsval* val)
    {
        JS::RootedValue v(cx);
        JS_GetElement(cx, obj, index, &v);
        *val = v.get();
        return true;
    }
    DLLEXPORT bool JSh_SetElement(JSContext *cx, JSObject *obj, uint32_t index, jsval* pVal)
    {
        JS::RootedValue arrElement(cx);
        arrElement = *pVal;
        return JS_SetElement(cx, obj, index, &arrElement);
    }
    // new a JSClass with specified flag and finalizer
    DLLEXPORT JSClass* JSh_NewClass(const char* name, unsigned int flag, JSFinalizeOp finalizeOp) 
    {
        JSClass* cls = new JSClass();
        memcpy(cls, &global_class, sizeof(JSClass));
        int len = strlen(name);
        char* pName = (char*)malloc(len + 1);
        memcpy(pName, name, len);
        pName[len] = 0;
        cls->name = pName;
        cls->flags = flag;
        cls->finalize = finalizeOp;
        return cls;
    }
    
    // init a class with default value
    DLLEXPORT JSObject* JSh_InitClass(JSContext* cx, JSObject* glob, JSClass* jsClass)
    {
        JSObject* obj = JS_InitClass(cx, glob, 
            NULL, /* parentProto*/
            jsClass, /* JSClass*/
            NULL, /* constructor*/
            0, /* constructor nargs */
            NULL, /* JSPropertySpec* */
            NULL, /* JSFunctionSpec* */
            NULL, /* static JSPropertySpec* */
            NULL /* static JSFunctionSpec* */
            );
        return obj;
    }

    DLLEXPORT void JSh_GC(JSRuntime *rt) 
    { 
        JS_GC(rt); 
    }
    DLLEXPORT void JSh_MaybeGC(JSContext *cx) 
    { 
        JS_MaybeGC(cx); 
    }
    DLLEXPORT bool JSh_EvaluateScript(JSContext *cx, JSObject *obj,
        const char *bytes, unsigned length,
        const char *filename, unsigned lineno,
        jsval *rval)
    {
        return JS_EvaluateScript(cx, obj, bytes, length, filename, lineno, rval);
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // new a class and assign it a class

    DLLEXPORT JSObject* JSh_NewObjectAsClass(JSContext* cx, JSObject* glob, const char* className, JSFinalizeOp finalizeOp)
    {
        JS::RootedValue nsval(cx);

        JS_GetProperty(cx, glob, className, &nsval);
        JSObject* jsObject = JSVAL_TO_OBJECT(nsval);
        if (jsObject == 0) 
            return 0;

        JS_GetProperty(cx, jsObject, "prototype", &nsval);
        JSObject* proto = JSVAL_TO_OBJECT(nsval);
        if (proto == 0) 
            return 0;

        JSClass* jsClass = &qiucw_class;
        jsClass->finalize = finalizeOp;
        JSObject* obj = JS_NewObject(cx, jsClass, proto, 0/* parentProto */);        
        return obj;
    }

    DLLEXPORT JSObject* JSh_NewObject(JSContext *cx, JSClass *clasp, JSObject *proto, JSObject *parent)
    {
        return JS_NewObject(cx, clasp, proto, parent);
    }

    DLLEXPORT JSObject* JSh_NewMyClass(JSContext *cx, JSFinalizeOp finalizeOp)
    {
        JSClass* jsClass = &qiucw_class;
        jsClass->finalize = finalizeOp;
        JSObject* obj = JS_NewObject(cx, jsClass, 0/* proto */, 0/* parentProto */);        
        return obj;
    }

    DLLEXPORT bool JSh_ArgvIsUndefined(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isUndefined(); }
    DLLEXPORT bool JSh_ArgvIsNull(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNull(); }
    DLLEXPORT bool JSh_ArgvIsNullOrUndefined(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNullOrUndefined(); }
    DLLEXPORT bool JSh_ArgvIsInt32(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isInt32(); }
    DLLEXPORT bool JSh_ArgvIsDouble(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isDouble(); }
    DLLEXPORT bool JSh_ArgvIsBool(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isBoolean(); }
    DLLEXPORT bool JSh_ArgvIsString(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isString(); }
    DLLEXPORT bool JSh_ArgvIsNumber(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNumber(); }
    DLLEXPORT bool JSh_ArgvIsObject(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isObject(); }    

    DLLEXPORT bool JSh_ArgvBool(JSContext* cx, jsval* vp, int i) { return JSVAL_TO_BOOLEAN(JS_ARGV(cx, vp)[i]); }
    DLLEXPORT double JSh_ArgvDouble(JSContext* cx, jsval* vp, int i) { return JSVAL_TO_DOUBLE(JS_ARGV(cx, vp)[i]); }
    DLLEXPORT int JSh_ArgvInt(JSContext* cx, jsval* vp, int i) { return JSVAL_TO_INT(JS_ARGV(cx, vp)[i]); }
    DLLEXPORT const char* JSh_ArgvString(JSContext* cx, jsval* vp, int i) 
    { 
        JSString* jsStr = JSVAL_TO_STRING(JS_ARGV(cx, vp)[i]); 
        return JS_EncodeString(cx, jsStr);
    }
    DLLEXPORT JSObject* JSh_ArgvObject(JSContext* cx, jsval* vp, int i) 
    { 
        jsval* pVal = &(JS_ARGV(cx, vp)[i]);
        if (pVal->isObject()) 
            return JSVAL_TO_OBJECT(*pVal);
        else
            return 0;
    }
    DLLEXPORT JSFunction* JSh_ArgvFunction(JSContext* cx, jsval* vp, int i) { 
        jsval val = (JS_ARGV(cx, vp)[i]);         
        if (!val.isObject()) 
            return 0;

        JSObject* obj = JSVAL_TO_OBJECT(val);
        if (!JS_ObjectIsFunction(cx, obj))
            return 0;

        return JS_ValueToFunction(cx, JS::RootedValue(cx, val));
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // returns

    DLLEXPORT void JSh_SetRvalBool(JSContext* cx, jsval* vp, bool value) { JS_SET_RVAL(cx, vp, BOOLEAN_TO_JSVAL(value)); }
    DLLEXPORT void JSh_SetRvalDouble(JSContext* cx, jsval* vp, double value) { JS_SET_RVAL(cx, vp, DOUBLE_TO_JSVAL(value)); }
    DLLEXPORT void JSh_SetRvalInt(JSContext* cx, jsval* vp, int value) { JS_SET_RVAL(cx, vp, INT_TO_JSVAL(value)); }
    DLLEXPORT void JSh_SetRvalUInt(JSContext* cx, jsval* vp, unsigned int value) { JS_SET_RVAL(cx, vp, UINT_TO_JSVAL(value)); }
    DLLEXPORT void JSh_SetRvalString(JSContext* cx, jsval* vp, const char* value) { JSString* jsString = JS_NewStringCopyZ(cx, value); JS_SET_RVAL(cx, vp, STRING_TO_JSVAL(jsString)); }
    DLLEXPORT void JSh_SetRvalObject(JSContext* cx, jsval* vp, JSObject* value) { JS_SET_RVAL(cx, vp, OBJECT_TO_JSVAL(value)); }
    DLLEXPORT void JSh_SetRvalUndefined(JSContext* cx, jsval* vp) { jsval v; v.setUndefined(); JS_SET_RVAL(cx, vp, v); }
    DLLEXPORT void JSh_SetRvalJSVAL(JSContext* cx, jsval* vp, jsval* value) { JS_SET_RVAL(cx, vp, *value); }

    ////////////////////////////////////////////////////////////////////////////////////
    // generate jsval

    DLLEXPORT void JSh_SetJsvalBool(jsval* vp, bool value)  { *vp = BOOLEAN_TO_JSVAL(value);  }
    DLLEXPORT void JSh_SetJsvalDouble(jsval* vp, double value) { *vp = DOUBLE_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalInt(jsval* vp, int value) {*vp = INT_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalUInt(jsval* vp, unsigned int value) {  *vp = UINT_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalString(JSContext* cx, jsval* vp, const char* value) { JSString* jsString = JS_NewStringCopyZ(cx, value); *vp = STRING_TO_JSVAL(jsString); }
    DLLEXPORT void JSh_SetJsvalObject(jsval* vp, JSObject* value) { *vp = OBJECT_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalUndefined(jsval* vp) { vp->setUndefined(); }

    DLLEXPORT bool JSh_GetJsvalBool(jsval* vp)  { return vp->toBoolean();  }
    DLLEXPORT double JSh_GetJsvalDouble(jsval* vp) { return vp->toDouble(); }
    DLLEXPORT int JSh_GetJsvalInt(jsval* vp) { return vp->toInt32(); }
    DLLEXPORT unsigned int JSh_GetJsvalUInt(jsval* vp) {  return vp->toInt32(); }
    DLLEXPORT const char* JSh_GetJsvalString(JSContext* cx, jsval* vp) { 
        JSString* jsString = vp->toString(); 
        return JS_EncodeString(cx, jsString); 
    }
    DLLEXPORT JSObject* JSh_GetJsvalObject(jsval* vp) { return vp->toObjectOrNull(); }


    DLLEXPORT JSCompartment* JSh_EnterCompartment(JSContext *cx, JSObject *target) 
    { 
        return JS_EnterCompartment(cx, target); 
    }
    DLLEXPORT void JSh_LeaveCompartment(JSContext *cx, JSCompartment *oldCompartment) 
    { 
        JS_LeaveCompartment(cx, oldCompartment); 
    }

    ////////////////////////////////////////////////////////////////////////////////////

    DLLEXPORT void  JSh_SetTrustedPrincipals(JSRuntime *rt, const JSPrincipals *prin) 
    { 
        JS_SetTrustedPrincipals(rt, prin); 
    }
    DLLEXPORT JSScript* JSh_CompileScript(JSContext *cx, JSObject* global, const char *ascii, size_t length) 
    {
        return JS_CompileScript(cx, JS::RootedObject(cx, global), ascii, length, JS::CompileOptions(cx));
    }
    DLLEXPORT bool JSh_ExecuteScript(JSContext *cx, JSObject *obj, JSScript *script, jsval *rval) 
    {
        return JS_ExecuteScript(cx, obj, script, rval);
    }
    DLLEXPORT JSFunction* JSh_GetFunction(JSContext* cx, JSObject* obj, const char* name)
    {
        JS::RootedValue nsval(cx);
        JS::RootedObject ns(cx);
        if (!JS_GetProperty(cx, obj, name, &nsval))
            return 0;

        JSFunction* fun = JS_ValueToFunction(cx, nsval);
        return fun;
    }
    DLLEXPORT bool JSh_ObjectIsFunction(JSContext *cx, JSObject *obj)
    {
        return JS_ObjectIsFunction(cx, obj);
    }

    DLLEXPORT bool JSh_CallFunction(JSContext *cx, JSObject *obj, JSFunction *fun, unsigned argc, jsval *argv, jsval *rval)
    {
        return JS_CallFunction(cx, obj, fun, argc, argv, rval);
    }

    DLLEXPORT bool JSh_IsNative(JSObject *obj) 
    { 
        return JS_IsNative(obj);
    }
    DLLEXPORT JSRuntime* JSh_GetObjectRuntime(JSObject *obj) 
    { 
        return JS_GetObjectRuntime(obj); 
    }
    DLLEXPORT JSObject* JSh_NewObjectWithGivenProto(JSContext *cx, const JSClass *clasp, JSObject *proto, JSObject *parent) 
    {
        return JS_NewObjectWithGivenProto(cx, clasp, proto, parent);
    }
    DLLEXPORT bool JSh_DeepFreezeObject(JSContext *cx, JSObject *obj) 
    {
        return JS_DeepFreezeObject(cx, obj);
    }
    DLLEXPORT bool JSh_FreezeObject(JSContext *cx, JSObject *obj) 
    {
        return JS_FreezeObject(cx, obj);
    }


    DLLEXPORT bool JSh_StringHasBeenInterned(JSContext *cx, JSString *str)
    {
        return JS_StringHasBeenInterned(cx, str);
    }

//     DLLEXPORT jsid INTERNED_STRING_TO_JSID(JSContext *cx, JSString *str)
//     {
//         return INTERNED_STRING_TO_JSID(cx, str);
//     }

    DLLEXPORT bool JSh_CallOnce(JSCallOnceType *once, JSInitCallback func)
    {
        return JS_CallOnce(once, func);
    }
    DLLEXPORT int64_t JSh_Now(void) 
    { 
        return JS_Now(); 
    }
    DLLEXPORT jsval JSh_GetNaNValue(JSContext *cx)
    {
        return JS_GetNaNValue(cx);
    }

    DLLEXPORT jsval JSh_GetNegativeInfinityValue(JSContext *cx)
    {
        return JS_GetNegativeInfinityValue(cx);
    }

    DLLEXPORT jsval JSh_GetPositiveInfinityValue(JSContext *cx)
    {
        return JS_GetPositiveInfinityValue(cx);
    }

    DLLEXPORT jsval JSh_GetEmptyStringValue(JSContext *cx)
    {
        return JS_GetEmptyStringValue(cx);
    }

    DLLEXPORT JSString* JSh_GetEmptyString(JSRuntime *rt)
    {
        return JS_GetEmptyString(rt);
    }

//     DLLEXPORT bool JS_ConvertValue(JSContext *cx, JS::HandleValue v, JSType type, JS::MutableHandleValue vp);
//     DLLEXPORT bool JS_ValueToObject(JSContext *cx, JS::HandleValue v, JS::MutableHandleObject objp);
    DLLEXPORT JSFunction* JSh_ValueToFunction(JSContext *cx, jsval* v)
    {
        JS::RootedValue rval(cx);
        rval = *v;
        JS::HandleValue hval(&rval);
        return JS_ValueToFunction(cx, hval);
    }
    DLLEXPORT JSFunction* JSh_ValueToConstructor(JSContext *cx, jsval* v)
    {
        JS::RootedValue rval(cx);
        rval = *v;
        JS::HandleValue hval(&rval);
        return JS_ValueToConstructor(cx, hval);
    }

    DLLEXPORT JSString* JSh_ValueToSource(JSContext *cx, jsval v)
    {
        return JS_ValueToSource(cx, v);
    }
    DLLEXPORT bool JSh_DoubleIsInt32(double d, int32_t *ip)
    {
        return JS_DoubleIsInt32(d, ip);
    }
    DLLEXPORT int32_t JSh_DoubleToInt32(double d)
    {
        return JS_DoubleToInt32(d);
    }

    DLLEXPORT uint32_t JSh_DoubleToUint32(double d)
    {
        return JS_DoubleToUint32(d);
    }

    DLLEXPORT JSType JSh_TypeOfValue(JSContext *cx, jsval v)
    {
        return JS_TypeOfValue(cx, v);
    }
    DLLEXPORT const char * JSh_GetTypeName(JSContext *cx, JSType type)
    {
        return JS_GetTypeName(cx, type);
    }

    DLLEXPORT bool JSh_StrictlyEqual(JSContext *cx, jsval v1, jsval v2, bool *equal)
    {
        return JS_StrictlyEqual(cx, v1, v2, equal);
    }
    DLLEXPORT bool JSh_LooselyEqual(JSContext *cx, jsval v1, jsval v2, bool *equal)
    {
        return JS_LooselyEqual(cx, v1, v2, equal);
    }
    DLLEXPORT bool JSh_SameValue(JSContext *cx, jsval v1, jsval v2, bool *same) 
    {
        return JS_SameValue(cx, v1, v2, same);
    }

    DLLEXPORT bool JSh_IsBuiltinEvalFunction(JSFunction *fun)
    {
        return JS_IsBuiltinEvalFunction(fun);
    }
    DLLEXPORT bool JSh_IsBuiltinFunctionConstructor(JSFunction *fun)
    {
        return JS_IsBuiltinFunctionConstructor(fun);
    }
    DLLEXPORT void* JSh_GetRuntimePrivate(JSRuntime *rt)
    {
        return JS_GetRuntimePrivate(rt);
    }
    DLLEXPORT void JSh_SetRuntimePrivate(JSRuntime *rt, void *data)
    {
        return JS_SetRuntimePrivate(rt, data);
    }
    DLLEXPORT void JSh_BeginRequest(JSContext *cx)
    {
        return JS_BeginRequest(cx);
    }
    DLLEXPORT void JSh_EndRequest(JSContext *cx)
    {
        return JS_EndRequest(cx);
    }
    DLLEXPORT bool JSh_IsInRequest(JSRuntime *rt)
    {
        return JS_IsInRequest(rt);
    } 
    DLLEXPORT void JSh_SetJitHardening(JSRuntime *rt, bool enabled)
    {
        return JS_SetJitHardening(rt, enabled);
    }
    DLLEXPORT const char * JSh_GetImplementationVersion(void)
    {
        return JS_GetImplementationVersion();
    }
    DLLEXPORT void JSh_SetDestroyCompartmentCallback(JSRuntime *rt, JSDestroyCompartmentCallback callback)
    {
        return JS_SetDestroyCompartmentCallback(rt, callback);
    }
    DLLEXPORT void JSh_SetDestroyZoneCallback(JSRuntime *rt, JSZoneCallback callback)
    {
        return JS_SetDestroyZoneCallback(rt, callback);
    }
    DLLEXPORT void JSh_SetSweepZoneCallback(JSRuntime *rt, JSZoneCallback callback)
    {
        return JS_SetSweepZoneCallback(rt, callback);
    }
    DLLEXPORT void JSh_SetCompartmentNameCallback(JSRuntime *rt, JSCompartmentNameCallback callback)
    {
        return JS_SetCompartmentNameCallback(rt, callback);
    }
    DLLEXPORT void JSh_SetWrapObjectCallbacks(JSRuntime *rt, const JSWrapObjectCallbacks *callbacks)
    {
        return JS_SetWrapObjectCallbacks(rt, callbacks);
    }
    DLLEXPORT void JSh_SetCompartmentPrivate(JSCompartment *compartment, void *data)
    {
        return JS_SetCompartmentPrivate(compartment, data);
    }
    DLLEXPORT void* JSh_GetCompartmentPrivate(JSCompartment *compartment)
    {
        return JS_GetCompartmentPrivate(compartment);
    }
    DLLEXPORT void JSh_SetZoneUserData(JS::Zone *zone, void *data)
    {
        return JS_SetZoneUserData(zone, data);
    }
    DLLEXPORT void* JSh_GetZoneUserData(JS::Zone *zone)
    {
        return JS_GetZoneUserData(zone);
    }

//     DLLEXPORT bool JS_WrapObject(JSContext *cx, JS::MutableHandleObject objp);
//     DLLEXPORT bool JS_WrapValue(JSContext *cx, JS::MutableHandleValue vp);
//     DLLEXPORT bool JS_WrapId(JSContext *cx, jsid *idp);
//     DLLEXPORT JSObject* JS_TransplantObject(JSContext *cx, JS::Handle<JSObject*> origobj, JS::Handle<JSObject*> target);
//     DLLEXPORT bool JS_RefreshCrossCompartmentWrappers(JSContext *cx, JSObject *ob);
//     DLLEXPORT void JS_IterateCompartments(JSRuntime *rt, void *data, JSIterateCompartmentCallback compartmentCallback);
// 
// 
//     DLLEXPORT bool JS_ResolveStandardClass(JSContext *cx, JS::Handle<JSObject*> obj, JS::Handle<jsid> id, bool *resolved);
//     DLLEXPORT bool JS_EnumerateStandardClasses(JSContext *cx, JS::Handle<JSObject*> obj);
//     DLLEXPORT bool JS_GetClassObject(JSContext *cx, JSObject *obj, JSProtoKey key, JSObject **objp);
//     DLLEXPORT bool JS_GetClassPrototype(JSContext *cx, JSProtoKey key, JSObject **objp);

    DLLEXPORT JSProtoKey JSh_IdentifyClassPrototype(JSContext *cx, JSObject *obj)
    {
        return JS_IdentifyClassPrototype(cx, obj);
    }

    DLLEXPORT JSObject* JSh_ThisObject(JSContext* cx, jsval* vp)
    {
        return JS_THIS_OBJECT(cx, vp);
    }

    DLLEXPORT bool JSh_AddValueRoot(JSContext *cx, jsval *vp)
    {
        return JS_AddValueRoot(cx, vp);
    }
    DLLEXPORT bool JSh_AddStringRoot(JSContext *cx, JSString **rp)
    {
        return JS_AddStringRoot(cx, rp);
    }
    DLLEXPORT bool JSh_AddObjectRoot(JSContext *cx, JSObject **rp)
    {
        return JS_AddObjectRoot(cx, rp);
    }
    DLLEXPORT bool JSh_AddNamedValueRoot(JSContext *cx, jsval *vp, const char *name)
    {
        return JS_AddNamedValueRoot(cx, vp, name);
    }
    DLLEXPORT bool JSh_AddNamedValueRootRT(JSRuntime *rt, jsval *vp, const char *name)
    {
        return JS_AddNamedValueRootRT(rt, vp, name);
    }
    DLLEXPORT bool JSh_AddNamedStringRoot(JSContext *cx, JSString **rp, const char *name)
    {
        return JS_AddNamedStringRoot(cx, rp, name);
    }
    DLLEXPORT bool JSh_AddNamedObjectRoot(JSContext *cx, JSObject **rp, const char *name)
    {
        return JS_AddNamedObjectRoot(cx, rp, name);
    }
    DLLEXPORT bool JSh_AddNamedScriptRoot(JSContext *cx, JSScript **rp, const char *name)
    {
        return JS_AddNamedScriptRoot(cx, rp, name);
    }
    DLLEXPORT void JSh_RemoveValueRoot(JSContext *cx, jsval *vp)
    {
        return JS_RemoveValueRoot(cx, vp);
    }
    DLLEXPORT void JSh_RemoveStringRoot(JSContext *cx, JSString **rp)
    {
        return JS_RemoveStringRoot(cx, rp);
    }
    DLLEXPORT void JSh_RemoveObjectRoot(JSContext *cx, JSObject **rp)
    {
        return JS_RemoveObjectRoot(cx, rp);
    }
    DLLEXPORT void JSh_RemoveScriptRoot(JSContext *cx, JSScript **rp)
    {
        return JS_RemoveScriptRoot(cx, rp);
    }
    DLLEXPORT void JSh_RemoveValueRootRT(JSRuntime *rt, jsval *vp)
    {
        return JS_RemoveValueRootRT(rt, vp);
    }
    DLLEXPORT void JSh_RemoveStringRootRT(JSRuntime *rt, JSString **rp)
    {
        return JS_RemoveStringRootRT(rt, rp);
    }
    DLLEXPORT void JSh_RemoveObjectRootRT(JSRuntime *rt, JSObject **rp)
    {
        return JS_RemoveObjectRootRT(rt, rp);
    }
    DLLEXPORT void JSh_RemoveScriptRootRT(JSRuntime *rt, JSScript **rp)
    {
        return JS_RemoveScriptRootRT(rt, rp);
    }
    DLLEXPORT  void JSh_SetNativeStackQuota(JSRuntime *cx, size_t systemCodeStackSize, size_t trustedScriptStackSize, size_t untrustedScriptStackSize)
    {
        JS_SetNativeStackQuota(cx, systemCodeStackSize, trustedScriptStackSize, untrustedScriptStackSize);
    }
};
