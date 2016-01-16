using UnityEngine;
using System.Collections;

public class SunmCallAndroid : MonoBehaviour
{
    /// <summary>
    /// U3D调用Android方法 - 无返回值
    /// </summary>
    /// <param name="MethName"></param>
    /// <param name="param"></param>
    public static void UnityToAndroid(string MethName, string param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                jo.Call(MethName, param);
            }
        }
    }

    /// <summary>
    /// U3D调用Android方法 - string
    /// </summary>
    /// <param name="MethName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static string UnityToAndroidStr(string MethName, string param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<string>(MethName, param);
            }
        }
    }

    /// <summary>
    /// U3D调用Android方法 - bool
    /// </summary>
    /// <param name="MethName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static bool UnityToAndroidBl(string MethName, string param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<bool>(MethName, param);
            }
        }
    }

    /// <summary>
    /// U3D调用Android方法 - ArrayList
    /// </summary>
    /// <param name="MethName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static ArrayList UnityToAndroidArr(string MethName, string param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<ArrayList>(MethName, param);
            }
        }
    }
}
