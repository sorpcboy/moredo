using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

public class NetClientTemplate : MonoBehaviour
{
    public string HostName = "http://www.webxml.com.cn";
    //城市天气预报服务   
    public string URLPath = "/WebServices/WeatherWebService.asmx/getWeatherbyCityName";
    //获得验证码服务（直接获得图片）  
    private string PictureName = "/WebServices/ValidateCodeWebService.asmx/cnValidateImage?byString='Picture'";
    //获得验证码服务（获得图片字节流）  
    private string PictureByteName = "/WebServices/ValidateCodeWebService.asmx/cnValidateByte?byString='picByte'";

    private Texture2D mPicture;
    private Texture2D mPictureByte;
    private Texture2D mConvertPNG;

    //public string[] Parameters = new string[] { "theCityName" };

    //private string XMLContent = "null";

    //public string testC = "null";

    //下载图片   
    public void downloadPicture(string picName)
    {
        //testC = "picurl = " + picName;

        StartCoroutine(GETTexture(HostName + picName));
    }

    //下载图片（字节流）  
    public void downloadPictureByte(string picName)
    {
        StartCoroutine(GETTextureByte(HostName + picName));
    }

    /*----------------------------------------------------Helper----------------------------------------------------------------------------*/

    private float mJindu = 0;
    private string mContent;

    public float getJindu()
    {
        return mJindu;
    }

    //POST请求(Form表单传值、效率低、安全 ，)  
    IEnumerator POST(string url, Dictionary<string, string> post)
    {
        //表单   
        WWWForm form = new WWWForm();
        //从集合中取出所有参数，设置表单参数（AddField()).  
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        //表单传值，就是post   
        WWW www = new WWW(url, form);

        yield return www;
        mJindu = www.progress;

        if (www.error != null)
        {
            //POST请求失败  
            mContent = "error :" + www.error;
        }
        else
        {
            //POST请求成功  
            mContent = www.text;
        }
    }

    //GET请求（url?传值、效率高、不安全 ）  
    IEnumerator GET(string url, Dictionary<string, string> get)
    {
        string Parameters;
        bool first;
        if (get.Count > 0)
        {
            first = true;
            Parameters = "?";
            //从集合中取出所有参数，设置表单参数（AddField()).  
            foreach (KeyValuePair<string, string> post_arg in get)
            {
                if (first)
                    first = false;
                else
                    Parameters += "&";

                Parameters += post_arg.Key + "=" + post_arg.Value;
            }
        }
        else
        {
            Parameters = "";
        }

        //testC = "getURL :" + Parameters;

        //直接URL传值就是get  
        WWW www = new WWW(url + Parameters);
        yield return www;
        mJindu = www.progress;

        if (www.error != null)
        {
            //GET请求失败  
            mContent = "error :" + www.error;
        }
        else
        {
            //GET请求成功  
            mContent = www.text;
        }
    }

    IEnumerator GETTexture(string picURL)
    {
        WWW wwwTexture = new WWW(picURL);

        yield return wwwTexture;

        if (wwwTexture.error != null)
        {
            //GET请求失败  
            Debug.Log("error :" + wwwTexture.error);
        }
        else
        {
            //GET请求成功  
            mPicture = wwwTexture.texture;
        }
    }

    IEnumerator GETTextureByte(string picURL)
    {
        WWW www = new WWW(picURL);

        yield return www;

        if (www.error != null)
        {
            //GET请求失败  
            Debug.Log("error :" + www.error);
        }
        else
        {
            //GET请求成功  
            Debug.Log("PicBytes text = " + www.text);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(www.text));

            //通过索引查找子节点   
            string PicByte = xmlDoc.GetElementsByTagName("base64Binary").Item(0).InnerText;
            //testC = PicByte;

            mPictureByte = BttetoPic(PicByte);
        }
    }

    //解析XML   
    string AnalysisXML()
    {
        string str = "";

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(mContent));

        //得到文档根节点的所有子节点集合   
        //XmlNodeList nodes = xmlDoc.DocumentElement.ChildNodes;  
        //通过节点名得到节点集合  
        XmlNodeList nodes = xmlDoc.GetElementsByTagName("string");

        //通过索引查找子节点   
        str += "item[1] = " + xmlDoc.GetElementsByTagName("string").Item(1).InnerText + "\n\n";

        //遍历所有子节点  
        foreach (XmlElement element in nodes)
        {
            if (element.Name == "string")
            {
                str += element.InnerText + "\n";
            }
        }

        return str;
    }

    //图片与byte[]互转  
    public void convertPNG(Texture2D pic)
    {
        byte[] data = pic.EncodeToPNG();
        Debug.Log("data = " + data.Length + "|" + data[0]);
        mConvertPNG = new Texture2D(200, 200);
        mConvertPNG.LoadImage(data);
    }

    //byte[]与base64互转   
    Texture2D BttetoPic(string base64)
    {
        Texture2D pic = new Texture2D(200, 200);
        //将base64转码为byte[]   
        byte[] data = System.Convert.FromBase64String(base64);
        //加载byte[]图片  
        pic.LoadImage(data);

        string base64str = System.Convert.ToBase64String(data);
        Debug.Log("base64str = " + base64str);

        return pic;
    }  
}