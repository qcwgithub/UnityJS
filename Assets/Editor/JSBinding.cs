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



public class JSBinding
{
//     public class ExportEnumeration
//     {
//         public Type type;
//     }

//     [MenuItem("ToJS/Output All Types in UnityEngine")]
//     public static void OutputAllTypesInUnityEngine()
//     {
// 
//     }

    [MenuItem("ToJS/Output All Types in UnityEngine")]
    public static void OutputAllTypesInUnityEngine()
    {
        var asm = typeof(GameObject).Assembly;
        var tps = asm.GetTypes();
        var writer = new StreamWriter(Application.dataPath + "/StreamingAssets/JavaScript/alltypes.javascript", false, Encoding.UTF8);

        writer.WriteLine("// enum");
        writer.WriteLine("");

        for (int i = 0; i < tps.Length; i++)
        {
            if (tps[i].IsEnum)
            {
                if (tps[i].IsEnum)
                    writer.WriteLine(tps[i].ToString());
            }
        }

        writer.WriteLine("");
        writer.WriteLine("// interface");
        writer.WriteLine("");

        for (int i = 0; i < tps.Length; i++)
        {
            if (tps[i].IsInterface)
                writer.WriteLine(tps[i].ToString());
        }

        writer.WriteLine("");
        writer.WriteLine("// class");
        writer.WriteLine("");

        for (int i = 0; i < tps.Length; i++)
        {
            if ((!tps[i].IsEnum && !tps[i].IsInterface) &&
                tps[i].IsClass)
                writer.WriteLine(tps[i].ToString());
        }
        writer.Close();
        return;
    }

    public class BindType
    {
        public string name;
        public Type type;
        public string baseName = null;

        public BindType(string s, Type t, string bn)
        {
            name = s;
            type = t;
            baseName = bn;
        }
    }
	public class Qiucw
	{
		public static void AddIntOut(out int i)
		{
			i = 28;
		}
		
		public static void AddIntRef(ref int i)
		{
			i++;
		}
	}

    static BindType[] binds = new BindType[]
    {
        new BindType("GameObject", typeof(GameObject), ""),
        
        //new BindType("Camera", typeof(Camera), ""),
        //new BindType("Transform", typeof(Camera), ""),
        //new BindType("EventType", typeof(EventType), ""),
    };

    [MenuItem("ToJS/Generate Bindings")]
    public static void GenerateJSBindingFiles()
    {
//         if (!Application.isPlaying)
//         {
//             EditorApplication.isPlaying = true;
//         }




        ToJS.OnBegin();

//         for (int i = 0; i < ToExportEnums.lst.Length; i++)
//         {
//             ToJS.Clear();
//             ToJS.type = ToExportEnums.lst[i];
//             ToJS.Generate();
//         }

        for (int i = 0; i < binds.Length; i++)
        {
            ToJS.Clear();
            ToJS.className = binds[i].name;
            ToJS.type = binds[i].type;
            ToJS.baseClassName = binds[i].baseName;
            ToJS.Generate();
        }

        ToJS.OnEnd();
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
