using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * @des  日志类别
 * @author hl
 * @date 2015.11.04
 * @fun
 * public static void setOpenLog(bool _bOpen)
 * public static void Out(object _logMsg, TYPE _type)
 * @modify
 *  
 */
class Print
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum TYPE
    {
        NORMAL, ERROR, EXCEPTION, WARNING
    }

    /// <summary>
    /// 日志开关
    /// </summary>
    private static bool bOpen = false;

    /// <summary>
    /// 设置日志是否打开
    /// </summary>
    /// <param name="_bOpen"></param>
    public static void setOpenLog(bool _bOpen)
    {
        bOpen = _bOpen;
    }

    public static void Out(object _logMsg, TYPE _type)
    {
        if (bOpen)
        {
            switch (_type)
            {
                case TYPE.ERROR:
                    Debug.LogError((string)_logMsg);
                    break;
                case TYPE.WARNING:
                    Debug.LogWarning((string)_logMsg);
                    break;
                case TYPE.EXCEPTION:
                    Debug.LogException((Exception)_logMsg);
                    break;
                case TYPE.NORMAL:
                    Debug.Log((string)_logMsg);
                    break;
                default:
                    Debug.Log((string)_logMsg);
                    break;
            }
        }
    }
}