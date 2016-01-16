using UnityEngine;
using System.Collections;

public class ShowImage : MonoBehaviour 
{
    private static GameObject gameShowImg;

    void Start()
    {
        gameShowImg = gameObject;
    }

    /// <summary>
    /// 显示图片的方法
    /// </summary>
    /// <param name="_url"></param>
    public void showImage(string _url)
    {
        StartCoroutine(NetClient.GET_Pic(_url, new ShowCallBack()));
    }

    /// <summary>
    /// 启动协程后的回调
    /// </summary>
    public class ShowCallBack : NetClientCallBackInterface
    {
        public void onNet(string response)
        {       
            SunmCallAndroid.UnityToAndroid("toast", "请检查网络！");
        }

        public void onFailer(string response)
        {
            SunmCallAndroid.UnityToAndroid("toast", "加载预览图失败！");
        }

        public void onSucces(StatusCode.NETSTATE nst, object response)
        {
            if (nst == StatusCode.NETSTATE.STRING_RESPONSE)
            {
                
            }
            else if (nst == StatusCode.NETSTATE.TEXTURE_RESPONSE)
            {
                gameShowImg.GetComponentInChildren<UITexture>().mainTexture = (Texture2D)response;
            }
        }
    }
}
