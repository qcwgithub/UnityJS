Thank you for downloading JavaScript Binding for Unity!

Any bugs, questions, advises, visit:
https://groups.google.com/forum/#!forum/js-binding-for-unity

or e-mail to:
answerwinner@gmail.com

===============================================
          Version
===============================================
1.0



===============================================
        IMPORTANT NOTES !!
===============================================

This package is currently focused on exporting C# interfaces to javascript.
It can export 99% C# interfaces to javascript.

This is an initial version and may contain bug. It will be updated very soon.

So, any advice, bugs, please let me know, I will improve it as soon as possible.

Generally, it is highly-efficient. P/Invokes and boxing/unboxing are reduced to minimal count.
It doesn't use C# reflection.

It exports classes on both C# side and js side, that is, if one class is configured to be exported,
one C# file and one js file will be generated.

===============================================
        How to use / run demo
===============================================
1 Import this plugin to your project
2 In the Project window, open example scene 'UnityJSTestScene'
3 Click Menu [JS for Unity] -> [Generate JS Bindings]
4 Click Menu [JS for Unity] -> [Generate CS Bindings]
5 Switch to another program, and go back to Unity. Wait for importing scripts
6 ! Open JSMgr.cs, uncomment 2 lines (around line 104, 105)
7 Click Play button

If everything goes right, you should see a rotating cube.
every click on big cube, a smaller cube will be generated!

In windows editor, one error might occur: "DllNotFoundException", try:
Copy "Assets/Plugins/x86/mozjs-28.dll" -> Unity Install Directory/Editor (e.g. "D:\Program Files\Unity\Editor")
If problem still exists, make sure you have "msvcr110.dll" in "C:\windows\system32". 
(the dll was built in visual studio 2012)

You must do 3,4,5 steps every time when you switch platforms !!!


===============================================
     How to build js engine by yourself
===============================================

This package uses Mozilla SpiderMonkey js engine, version 28.
Git: https://github.com/ricardoquesada/Spidermonkey
Official Documents: https://developer.mozilla.org/en-US/docs/Mozilla/Projects/SpiderMonkey
This package doesn't include SpiderMonkey source code.

A cpp file is used to wrap needed apis. (see mozjswrap.cpp)
If you want to modify mozjswrap.cpp and rebuild library, you should build SpiderMonkey yourself (for every platform needed), 
and link it to mozjswrap.
Why use a wrap? SpiderMonkey 28 is written in C++, its interfaces are also C++.
But Unity only supports C-interface plugin.

One thing you must know when you try to build library yourself:
Unity Editor is 32-bit(both Windows and Mac OS). It can't use 64-bit plugin.
On Mac, SpiderMonkey is default built to 64-bit. So, build it to 32-bit.



===============================================
        Supported platforms
===============================================
Windows Editor   yes
Windows Exe      yes
Android          yes
iOS              yes

Mac Editor       no           // will be supported next version
Mac              no           // will be supported next version



===============================================
        Terminology / Convention
===============================================
Use 'UnityScript' for Unity js.
Use 'js' for real JavaScript.
Use 'Primitive' for (Boolean, Char, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double)
Use 'Type1' for (string, enum, Primitive)
Use 'Type2' for (struct, class)




===============================================
        What does/can this plugin do
===============================================
1 export 99% C# classes/functions to js script
2 calling C# functions from js script
3 calling js functions from C#



==================================================================
         How to call cs functions from js / Supported list
         PhraseBook
==================================================================

new an Object from js
---------------------------------
    // js code
    var go = new GameObject();
    var vec = new Vector3();
---------------------------------

call overloaded function
---------------------------------------------------------------------------
    // js code
    var go = new GameObject();
    go.GetComponent('RotateObject'); // this is an overloaded function
---------------------------------------------------------------------------

passing ref/out 'Type2' (use as if it were normal parameter)
-----------------------------------------------
    // js code
    var hit = new RaycastHit();
    Physics.Raycast(ray, hit);
-----------------------------------------------

passing ref/out 'Type1' (by wrapping it)
-------------------------------------------------------
    // C# code
    public class Apple {
        public void refAdd(int a, int b, ref int sum) {
            sum = a + b;
        }
        public void outAdd(int a, int b, out int sum) {
            sum = a + b;
        }
    }
    // js code
    var Apple a = new Apple();
    var sum = CS.int32(0);
    a.refAdd(5, 6, sum)
    Debug.Log(sum.Value()) // getting value back
-------------------------------------------------------

call operator +-*/
-----------------------------------------------
    // js code
    var a = new Vector3(1,0,0)
    var b = new Vector3(0,1,1)
    var c = Vector3.op_Addition(a, b);

    // /  -> op_Division
    // *  -> op_Multiply
    // -  -> op_Subtraction
    // -  -> op_UnaryNegation
    // == -> op_Equality
    // != -> op_Inequality

    refer to: BuildSpecialFunctionCall function in CSGenerator.cs
-----------------------------------------------

call functions with optional parameters
-----------------------------------------------
    // C# code
    public class Apple {
        public void Add(int a, int b = 0) {
            sum = a + b;
        }
    }
    // js code
    var Apple a = new Apple();
    Debug.Log(a.Add(5));
-----------------------------------------------

set delegate
.set a js function to a C# delegate
.only support for fields. e.g. can't pass a jsfunction to a property or as method parameter
.the js function using for delegate will not have 'this'.
-----------------------------------------------
    // C# code
    public class Orange {
        public void delegate AddDel(int result);
        public AddDel del;

        public void Add(int a, int b) {
            int result = a + b;
            if (del != null)
                del(result);
        }
    }

    // js code
    function OnAdd(int result) {
        // 'this' is unavaiable here
        Debug.Log(result);
    }
    var o = new Orange();
    o.del = OnAdd;  // set C# delegate to a js function
    o.Add(6,7);
-----------------------------------------------


=========================================================
         Not-Supported list (call C# from js)
=========================================================
1 passing [] parameters to C#  (no)
2 return / passing [] parameters to js
3 generic function (e.g. GameObject.AddComponent<Type>(); ) (no)
4 indexers [] (e.g. Matrix4x4.[3]) (no)
5 function returning structure (no)


=========================================================
         About GC
=========================================================

GC is controlled by js.
js object collected -> c# GC callback, remove c# object from dictionary -> c# GC