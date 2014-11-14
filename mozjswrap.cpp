// mozjswrap.cpp
// author: answerwinner
// desc: wraps for SpiderMonkey api

#include "jsapi.h"

#ifdef _WINDOWS
#define DLLEXPORT __declspec( dllexport ) 
#else
#define DLLEXPORT
#endif
extern "C"
{
    void sc_finalize(JSFreeOp* freeOp, JSObject* obj)
    {

    }

    static JSClass global_class = {
        "global", JSCLASS_GLOBAL_FLAGS,
        JS_PropertyStub, JS_DeletePropertyStub, JS_PropertyStub, JS_StrictPropertyStub,
        JS_EnumerateStub, JS_ResolveStub, JS_ConvertStub, sc_finalize,
        JSCLASS_NO_OPTIONAL_MEMBERS
    };

    static JSClass qicuw_class = 
    {
        "qicuw",0,
        JS_PropertyStub, JS_DeletePropertyStub, JS_PropertyStub, JS_StrictPropertyStub,
        JS_EnumerateStub, JS_ResolveStub, JS_ConvertStub, NULL,
        JSCLASS_NO_OPTIONAL_MEMBERS
    };

    DLLEXPORT bool JSh_Init(void) { return JS_Init(); }
    DLLEXPORT void JSh_ShutDown(void) { JS_ShutDown(); }
    DLLEXPORT JSRuntime* JSh_NewRuntime(uint32_t maxbytes, int useHelperThreads) { return JS_NewRuntime(maxbytes, (JSUseHelperThreads)useHelperThreads); }
    DLLEXPORT void JSh_DestroyRuntime(JSRuntime *rt) { JS_DestroyRuntime(rt); }
    DLLEXPORT void JSh_SetGCParameter(JSRuntime *rt, int key, uint32_t value) { return JS_SetGCParameter(rt, (JSGCParamKey)key, value); }

    DLLEXPORT JSContext* JSh_NewContext(JSRuntime *rt, size_t stackChunkSize) { return JS_NewContext(rt, stackChunkSize); }
    DLLEXPORT void JSh_DestroyContext(JSContext *cx) { JS_DestroyContext(cx); }
    DLLEXPORT void JSh_DestroyContextNoGC(JSContext *cx) { JS_DestroyContextNoGC(cx); }

    DLLEXPORT JSErrorReporter JSh_SetErrorReporter(JSContext *cx, JSErrorReporter er) { return JS_SetErrorReporter(cx, er); }
    //void* JSh_GetContextPrivate(JSContext *cx);
    //void JSh_SetContextPrivate(JSContext *cx, void *data);

    DLLEXPORT JSObject* JSh_NewGlobalObject(JSContext *cx, int hookOption)
    {
        JS::CompartmentOptions options;
        options.setVersion(JSVERSION_LATEST);

        return JS_NewGlobalObject(cx, &global_class, 0/*principals*/, (JS::OnNewGlobalHookOption)hookOption, options);
    }

    DLLEXPORT bool JSh_InitStandardClasses(JSContext *cx, JSObject *obj){ 
        return JS_InitStandardClasses(cx, obj); }
    DLLEXPORT JSObject* JSh_InitReflect(JSContext *cx, JSObject *global) { return JS_InitReflect(cx, global); }
    DLLEXPORT JSFunction* JSh_DefineFunction(JSContext *cx, JSObject *obj, const char *name, JSNative call, unsigned nargs, unsigned attrs)
    {
        return JS_DefineFunction(cx, obj ,name, call, nargs, attrs);
    }
    DLLEXPORT int JSh_GetErroReportLineNo(JSErrorReport* report) { return report->lineno; }
    DLLEXPORT const char* JSh_GetErroReportFileName(JSErrorReport* report) { if (!report->filename) return ""; else return report->filename; }
    DLLEXPORT JSObject* JSh_NewArrayObject(JSContext *cx, int length, jsval *vector) { return JS_NewArrayObject(cx, length, vector); }
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

    DLLEXPORT void JSh_GC(JSRuntime *rt) { JS_GC(rt); }
    DLLEXPORT void JSh_MaybeGC(JSContext *cx) { JS_MaybeGC(cx); }
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

        JSClass* jsClass = &qicuw_class;
        jsClass->finalize = finalizeOp;
        JSObject* obj = JS_NewObject(cx, jsClass, proto, 0/* parentProto */);        
        return obj;
    }

    DLLEXPORT JSObject* JSh_NewObject(JSContext *cx, const JSClass *clasp, JSObject *proto, JSObject *parent)
    {
        return JS_NewObject(cx, clasp, proto, parent);
    }

    DLLEXPORT bool JSh_ArgvIsUndefined(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isUndefined(); }
    DLLEXPORT bool JSh_ArgvIsNull(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNull(); }
    DLLEXPORT bool JSh_ArgvIsNullOrUndefined(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNullOrUndefined(); }
    DLLEXPORT bool JSh_ArgvIsInt32(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isInt32(); }
    DLLEXPORT bool JSh_ArgvIsDouble(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isDouble(); }
    DLLEXPORT bool JSh_ArgvIsBool(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isBoolean(); }
    DLLEXPORT bool JSh_ArgvIsString(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isString(); }
    DLLEXPORT bool JSh_ArgvIsNumber(JSContext* cx, jsval* vp, int i) { return (JS_ARGV(cx, vp)[i]).isNumber(); }
    

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

    DLLEXPORT void JSh_SetJsvalBool(jsval* vp, bool value) { *vp = BOOLEAN_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalDouble(jsval* vp, double value) { *vp = DOUBLE_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalInt(jsval* vp, int value) { 
        *vp = INT_TO_JSVAL(value); 
    }
    DLLEXPORT void JSh_SetJsvalUInt(jsval* vp, unsigned int value) { *vp = UINT_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalString(JSContext* cx, jsval* vp, const char* value) { JSString* jsString = JS_NewStringCopyZ(cx, value); *vp = STRING_TO_JSVAL(jsString); }
    DLLEXPORT void JSh_SetJsvalObject(jsval* vp, JSObject* value) { *vp = OBJECT_TO_JSVAL(value); }
    DLLEXPORT void JSh_SetJsvalUndefined(jsval* vp) { vp->setUndefined(); }



    DLLEXPORT JSCompartment* JSh_EnterCompartment(JSContext *cx, JSObject *target) { return JS_EnterCompartment(cx, target); }
    DLLEXPORT void JSh_LeaveCompartment(JSContext *cx, JSCompartment *oldCompartment) { JS_LeaveCompartment(cx, oldCompartment); }




    ////////////////////////////////////////////////////////////////////////////////////

    DLLEXPORT void  JSh_SetTrustedPrincipals(JSRuntime *rt, const JSPrincipals *prin) { JS_SetTrustedPrincipals(rt, prin); }
    DLLEXPORT JSScript* JSh_CompileScript(JSContext *cx, JSObject* global, const char *ascii, size_t length) {
        return JS_CompileScript(cx, JS::RootedObject(cx, global), ascii, length, JS::CompileOptions(cx));
    }
    DLLEXPORT bool JSh_ExecuteScript(JSContext *cx, JSObject *obj, JSScript *script, jsval *rval) {
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

    DLLEXPORT bool JSh_CallFunction(JSContext *cx, JSObject *obj, JSFunction *fun, unsigned argc,
        jsval *argv, jsval *rval)
    {
        return JS_CallFunction(cx, obj, fun, argc, argv, rval);
    }
};