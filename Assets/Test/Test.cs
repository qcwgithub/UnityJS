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
/* static GameObject[] */
GameObject.FindGameObjectsWithTag = function(arg0/* String */) { return CS.Call(4, 0, 52, true, arg0); }

/*  String */
Object.defineProperty(GameObject.prototype, 'getname', 
{
    get: function() { return CS.Call(2, 0, 25, false, this); },
    set: function(v) { return CS.Call(3, 0, 25, false, this, v); }
});

var goList = GameObject.FindGameObjectsWithTag('CreditCard')
printInt(goList.length)
for (var i = 0; i < goList.length; i++)
    printString(goList[i].getname);
";
}
