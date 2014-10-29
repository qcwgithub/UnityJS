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


public class Test
{
    public static string testScript = @"

GameObject = function() {}
/* static GameObject */
GameObject.Find = function(arg0/* String */) { return CS.Call(4, 0, 70, true, arg0); }
/* Camera */
Object.defineProperty(GameObject.prototype, 'camera', 
{
    get: function() { return CS.Call(2, 0, 4, false, this); },
    set: function(v) { return CS.Call(3, 0, 4, false, this, v); }
});


Camera = function() {}
/* String */
Object.defineProperty(Camera.prototype, 'tag', 
{
    get: function() { return CS.Call(2, 1, 57, false, this); },
    set: function(v) { return CS.Call(3, 1, 57, false, this, v); }
});



var go = GameObject.Find('Main Camera')
printString(go.camera.tag);
";
}
