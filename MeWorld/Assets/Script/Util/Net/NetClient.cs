using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 网络请求模块
 * author: hl
 * func: 
 *  ///< 获取存档信息
 *  ///< 参数说明：
            param1: Dictionary<string, string> get = new Dictionary<string, string>();
                    get.Add("userid", "196");
                    get.Add("appid", "3");
                    get.Add("status", "1");
                    get.Add("pageNum", "1");
                    get.Add("pageSize", "2");
 *          param2: NetClientCallBackInterface 需要自己内部继承实现
 *          
 *  public static IEnumerator GET_ArchiveInf(Dictionary<string, string> get, NetClientCallBackInterface ncbi)
 */

public class NetClient : MonoBehaviour
{
    ///// <summary>
    ///// 测试 - 模版
    ///// </summary>
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 100, 100), "进入游戏"))
    //    {
    //       // UnityHttpClient uhttp = 
    //        //    UnityCommunicationManager.CreateInstance().GetHttpClient();
    //        //uhttp.BeginPost()
    //        //uhttp.BeginPost()
    //        //uhttp.BeginGetHttpContent("", )


    //        ///< 上面不好用，参数都不知道怎么传递， 还是www搞搞....
    //        Dictionary<string, string> dic = new Dictionary<string,string>();
    //        dic.Add("userid", "196");
    //        dic.Add("appid", "3");
    //        dic.Add("status", "1");
    //        dic.Add("pageNum", "1");
    //        dic.Add("pageSize", "2");
    //        StartCoroutine(GET("http://114.215.155.177:8080/MoredoShell/modelSave/listPage", dic, new RBack()));
    //    }
    //}


    ///// <summary>
    ///// 回调处理函数 -  模版
    ///// </summary>
    //public class RBack : NetCallBackInterface
    //{
    //    public void onNet(string response)
    //    {
    //        GameObject.Find("out").GetComponentInChildren<UILabel>().text = response;
    //    }

    //    public void onFailer(string response)
    //    {
    //        GameObject.Find("out").GetComponentInChildren<UILabel>().text = response;
    //        //label.text = response; ///< 内部类静态成员无法直接访问
    //    }

    //    public void onSucces(string response)
    //    {
    //        GameObject.Find("out").GetComponentInChildren<UILabel>().text = response;
    //    }
    //}

    ///< 正式服务器IP
    private static string request_IP = "http://114.215.155.177";
    ///< 测试服务器IP
    //private static string request_IP = "http://115.28.172.248";
    private static string request_IP_FUN = request_IP + ":8080/MoredoShell";
    private static string requestArchiveInf_URL = request_IP_FUN + "/modelSave/listPage";
    private static string submitDeleteArchive_URL = request_IP_FUN + "/modelSave/batch";


    /// <summary>
    /// GET请求_获取存档信息（url?传值、效率高、不安全 ） 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="get"></param>
    /// <param name="ncbi"></param>
    /// <returns></returns>
    public static IEnumerator GET_ArchiveInf(Dictionary<string, string> get, NetClientCallBackInterface ncbi)
    {
        ///< 网络不可用
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ncbi.onNet("网络不可用!");
        }
        else
        {
            string parameters;
            bool first;
            if (get.Count > 0)
            {
                first = true;
                parameters = "?";
                ///< 从集合中取出所有参数，设置表单参数（AddField()). 
                foreach (KeyValuePair<string, string> post_arg in get)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        parameters += "&";
                    }

                    parameters += post_arg.Key + "=" + post_arg.Value;
                }
            }
            else
            {
                parameters = "";
            }

            ///< 直接URL传值就是get 
            WWW www = new WWW(requestArchiveInf_URL + parameters);
            yield return www;

            if (null != www.error)
            {
                ///< GET请求失败  
                ncbi.onFailer("error:" + www.error);
            }
            else
            {
                ///< GET请求成功  
                ncbi.onSucces(StatusCode.NETSTATE.STRING_RESPONSE, www.text);
            }
        }
    }

    /// <summary>
    /// GET请求_获取图片（url?传值、效率高、不安全 ） 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="get"></param>
    /// <param name="ncbi"></param>
    /// <returns></returns>
    public static IEnumerator GET_Pic(string picURL, NetClientCallBackInterface ncbi)
    {
        ///< 网络不可用
        if (picURL.Contains("http://") && Application.internetReachability == NetworkReachability.NotReachable)
        {
            ncbi.onNet("网络不可用!");
        }
        else
        {
            ///< 直接URL传值就是get 
            WWW www = new WWW(picURL);
            yield return www;

            if (null != www.error)
            {
                ///< GET请求失败  
                ncbi.onFailer("error:" + www.error);
            }
            else
            {
                ///< GET请求成功  
                ncbi.onSucces(StatusCode.NETSTATE.TEXTURE_RESPONSE, www.texture);
            }
        }
    }

    /// <summary>
    /// POST请求(Form表单传值、效率低、安全 ，)  
    /// </summary>
    /// <param name="url"></param>
    /// <param name="post"></param>
    /// <returns></returns>
    IEnumerator POST(string url, Dictionary<string, string> post, NetClientCallBackInterface ncbi)
    {
        ///< 网络不可用
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ncbi.onNet("网络不可用!");
        }
        else
        {
            ///< 表单   
            WWWForm form = new WWWForm();
            ///< 从集合中取出所有参数，设置表单参数（AddField()).  
            foreach (KeyValuePair<string, string> post_arg in post)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }
            ///< 表单传值，就是post   
            WWW www = new WWW(url, form);

            yield return www;

            if (www.error != null)
            {
                ///< POST请求失败  
                ncbi.onFailer("error:" + www.error);
            }
            else
            {
                ///< POST请求成功  
                ncbi.onSucces(StatusCode.NETSTATE.STRING_RESPONSE, www.text);
            }
        }
    }  
}
