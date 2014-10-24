using System;
using UnityEngine;

public class SendMessageOptionsWrap
{
    static JSEnum[] enums = new JSEnum[]
    {
        new JSEnum("RequireReceiver", (int)SendMessageOptions.RequireReceiver),
        new JSEnum("DontRequireReceiver", (int)SendMessageOptions.DontRequireReceiver),
    };

    public static void Register()
    {
        JSMgr.RegisterEnum("SendMessageOptions", enums);
    }
}
