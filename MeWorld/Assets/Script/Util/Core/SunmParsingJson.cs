using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;

public class SunmParsingJson : MonoBehaviour 
{
    public static IEnumerator getMateriaJson(string _fileName, 
                                             Dictionary<string, Dictionary<byte, string>> _MaterialData)
    {
        string filepath;
        if (Application.platform == RuntimePlatform.Android)
        {
            filepath = Application.streamingAssetsPath + "/" + _fileName;                           // 在Android中实例化WWW不能在路径前面加"file://"
        }
        else
        {
            filepath = "file://" + UnityEngine.Application.streamingAssetsPath + "/" + _fileName;   // 在Windows中实例化WWW必须要在路径前面加"file://"
        }

        WWW www = new WWW(filepath);
        while (!www.isDone)
        {
            yield return www;
            PardingMaterialName(www.text, _MaterialData);
        }

    }

    /// <summary>
    /// 解析json取到材质的名字
    /// </summary>
    private static void PardingMaterialName(string _jsonMaterial, 
                                            Dictionary<string, Dictionary<byte, string>> MaterialData)
    {
        JsonData jd = JsonMapper.ToObject(_jsonMaterial)["matarial"];
        byte KeyCount = 1;

        if (null == SunmConstant.MaterialData)
        {
            MaterialData = new Dictionary<string, Dictionary<byte, string>>();
        }
        MaterialData.Clear();

        SunmConstant.MaterialList.Add(KeyCount++, "box");
        for (int i = 0; i < jd.Count; ++i)
        {
            Dictionary<byte, string> MaterialName = new Dictionary<byte, string>();
            JsonData item = (JsonData)jd[i];
            string key = (string)item["name"];
            JsonData value = (JsonData)item["data"];
            for (int j = 0; j < value.Count; ++j)
            {
                string str = (string)value[j][j + ""];
                MaterialName.Add(KeyCount, str);
                SunmConstant.MaterialList.Add(KeyCount, str);
                KeyCount++;
            }
            ///< 向材质链表添加参数
            MaterialData.Add(key, MaterialName);
        }
    }
}
