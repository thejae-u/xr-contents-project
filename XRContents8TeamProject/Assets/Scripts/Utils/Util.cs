using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class LogPrintSystem
{
    private static DateTime dateTime;

    private static void Print(ref Transform t, ref string str)
    {
        dateTime = DateTime.Now;
        Debug.Log($"[{dateTime.ToString()}] {t.name} : {str}");
    }

    public static void SystemLogPrint(Transform t, string str, ELogType type)
    {
        var inst = LogType.GetLogTypeInstance();
        
        if (inst == null)
            Debug.LogError("LogSetting이 존재하지 않습니다.");
        
        if (((int)inst.GetLogType() & (int)type) == 0) return;
        
        Print(ref t, ref str);
    }
}
