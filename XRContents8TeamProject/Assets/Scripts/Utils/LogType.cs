using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Log/LogSetting", fileName = "LogSetting", order = 3)]
public class LogType : ScriptableObject
{
    [SerializeField] [EnumFlags]
    private ELogType logType;
    private static  LogType inst;

    public static  LogType GetLogTypeInstance()
    {
        if(!inst)
            inst = Resources.Load<LogType>("Log/LogSetting");
        return inst;
    }

    public ELogType GetLogType()
    {
        return logType;
    }
}
