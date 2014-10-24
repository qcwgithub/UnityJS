using UnityEngine;
using System.Collections;

public struct JSEnum
{
    public string name;
    public int val;

    public JSEnum(string str, int v)
    {
        name = str;
        val = v;
    }
}