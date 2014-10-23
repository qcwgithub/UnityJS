using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AnalyzeDll : MonoBehaviour 
{
	void Start () 
    {
        //Assembly asm = Assembly.GetExecutingAssembly();
        //Debug.Log(asm.FullName);

        if (!Application.isPlaying)
        {
            return;
        }

        Type type = typeof(GameObject);
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | /*BindingFlags.Static*//* | BindingFlags.IgnoreCase | */BindingFlags.DeclaredOnly | BindingFlags.Instance);
        string s = "";
        for (int i = 0; i < methods.Length; i++)
        {
            s += " " + methods[i].Name;
            
        }
        Debug.Log(s);
        Debug.Log(methods.Length);
        
        PropertyInfo[] pros = type.GetProperties();
        Debug.Log(pros.Length);
	}
	
	void Update () 
    {
	
	}
}
