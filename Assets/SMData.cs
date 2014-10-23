using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SMData
{
    class JS_Native_Relation
    {
        public IntPtr jsObj;
        public object nativeObj;
        public JS_Native_Relation(IntPtr a, object b)
        {
            jsObj = a;
            nativeObj = b;
        }
    }

    public class GlobalType
    {
        public IntPtr jsClass; // JSClass*
        public IntPtr proto;   // JSObject* returned by JS_InitClass
        public IntPtr parentProto; // JSObject* parent
    }

    public static void addNativeJSRelation(IntPtr jsObj, object nativeObj)
    {
        mDict1.Add(jsObj.GetHashCode(), new JS_Native_Relation(jsObj, nativeObj));
        mDict2.Add(nativeObj.GetHashCode(), new JS_Native_Relation(jsObj, nativeObj));
    }
    public static object getNativeObj(IntPtr jsObj)
    {
        JS_Native_Relation obj;
        if (mDict1.TryGetValue(jsObj.GetHashCode(), out obj))
            return obj.nativeObj;
        return null;
    }
    public static IntPtr getJSObj(object nativeObj)
    {
        JS_Native_Relation obj;
        if (mDict2.TryGetValue(nativeObj.GetHashCode(), out obj))
            return obj.jsObj;
        return IntPtr.Zero;
    }
    static Dictionary<int, JS_Native_Relation> mDict1 = new Dictionary<int, JS_Native_Relation>(); // key = jsObj.hashCode()
    static Dictionary<int, JS_Native_Relation> mDict2 = new Dictionary<int, JS_Native_Relation>(); // key = nativeObj.hashCode()


    public static void addGlobalType(Type type, IntPtr jsClass, IntPtr proto, IntPtr parentProto)
    {
        int hash = type.GetHashCode();
        GlobalType gt = new GlobalType();
        gt.jsClass = jsClass;
        gt.proto = proto;
        gt.parentProto = parentProto;
        mGlobalType.Add(hash, gt);
    }
    public static GlobalType getGlobalType(Type type)
    {
        GlobalType gt;
        if (mGlobalType.TryGetValue(type.GetHashCode(), out gt))
        {
            return gt;
        }
        return null;
    }
    static Dictionary<int, GlobalType> mGlobalType = new Dictionary<int, GlobalType>();
}
