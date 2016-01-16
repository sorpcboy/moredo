using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class SunmGameInit : MonoBehaviour
{
    /// <summary>
    /// 空间标志位 - 漫游模式会用到
    /// </summary>
    public static byte[, ,] cubeFlag = null;

    /// <summary>
    /// 临时变量，用于跳转场景时使用
    /// </summary>
    private GameObject gameTemp = null;

    /// <summary>
    /// 生成存档初始化
    /// </summary>
    private SunmArchive aArchive;

    /// <summary>
    /// 存档路径和名字
    /// </summary>
    public static String archiveingPath;
    public static String archiveingName;

    /// <summary>
    /// 初始化数据空间
    /// </summary>
    void Start()
    {
        ///< 初始化标志位
        initThreeArray();
        StartCoroutine(SunmParsingJson.getMateriaJson("json.txt", SunmConstant.MaterialData));
        setThreeArray(new Vector3(100.0f, 100.0f, 100.0f), 1);
        gameTemp = GameObject.Find("ExitButton");
        if (Application.platform == RuntimePlatform.Android)
        {
            initArchive();

            if (SunmConstant.gameModel == SunmConstant.GAME_LEVEL.CONTINUE_MODEL)
            {
                loadArchiveL();
            }
        }    
    }


    /// <summary>
    /// 测试
    /// </summary>
    void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 100, 100, 100), "重新开始"))
        //{
        //SunmConstant.reset();
        //}

        //if (GUI.Button(new Rect(10, 200, 100, 100), "输出STL"))
        //{
        //    //SunmCallAndroid.UnityToAndroid("getParentPath", "workname");

        //    ///< 生成模型data文件
        //    MDStl.OutToDataFromRMModel(
        //        "C:/Users/SorPCboy/Desktop/TEST/",
        //        "work.data", SunmConstant.cubeList);

        //    try
        //    {
        //        GameObject gameOBJ = GameObject.Find("(100.0, 100.0, 100.0)");
        //        //gameOBJ.SetActive(false);
        //        ///< 然后截屏
        //        ScreenShot.CaptureCamera("C:/Users/SorPCboy/Desktop/TEST/", "work.jpg");
        //        gameOBJ.SetActive(true);
        //    }
        //    catch (Exception)
        //    {
        //        ///< 失败也不要判断，并不是很重要....
        //    }
        //}
        //if (GUI.Button(new Rect(0, 0, 100, 100), "读档")) 
        //{
        //    loadArchiveL(SunmArchiveInit.getArchArchiveMessageProcessingiveInfo().localPath, SunmArchiveInit.getArchiveInfo().ArchiveName, ReadArchiveTemp);           
        //}
    }

    void Update()
    {
      
    }
    /// <summary>
    /// 销毁数据空间
    /// </summary>
    void OnDestroy()
    {
        cubeFlag = null;
        SunmGMMaterialChange.destroy();
        SunmConstant.destroy();
        SunmSSInit.DestroyArchiveData();
    }

    /**************************STL模型数据输出操作****************************/

    /// <summary>
    /// Android调用: 传递 parentPath#filename
    /// </summary>
    /// <param name="fileInfo"></param>
    public void toParentPath(string fileInfo)
    {
        string[] splitStr = fileInfo.Split('#');

        if (!splitStr[0].EndsWith("/"))
        {
            splitStr[0] = splitStr[0] + "/";
        }

        ///< 开启线程生成模型
        StartCoroutine(generateModel(splitStr[0], splitStr[1]));
    }

    /// <summary>
    /// 开启协程，生成模型。。。
    /// </summary>
    /// <param name="_parentPath"></param>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    private IEnumerator generateModel(string _parentPath, string _fileName)
    {
        StatusCode.NETSTATE reSt;

        ///< 简单判断下目录是否存在
        if (!Directory.Exists(_parentPath))
        {
            Directory.CreateDirectory(_parentPath);
        }

        ///< 生成模型data文件
        reSt = MDStl.OutToDataFromRMModel(
            _parentPath,
            _fileName + ".data", SunmConstant.cubeList);

        try
        {
            ///< 然后截屏
            ScreenShot.CaptureCamera(_parentPath, _fileName + ".jpg");
        }
        catch (Exception)
        {
            ///< 失败也不要判断，并不是很重要....
        }

        yield return 0;

        if (reSt == StatusCode.NETSTATE.NO_DATA)
        {
            GameObject.Find("TransparenBackground").SetActive(false);
            SunmCallAndroid.UnityToAndroid("toast", "没有数据可以打印！");
        }
        else if (reSt == StatusCode.NETSTATE.WRITE_FAILD)
        {
            GameObject.Find("TransparenBackground").SetActive(false);
            SunmCallAndroid.UnityToAndroid("toast", "生成模型失败，请重新尝试!");
        }
        else
        {

            ///< 运营部参数没有用到...
            SunmCallAndroid.UnityToAndroid("uploadMywork",
                _parentPath + "#" +
                _fileName + ".data" + "#" +
                _fileName + ".jpg");
        }
    }

    /// <summary>
    /// Android调用: 上传成功失败回调
    /// </summary>
    /// <param name="_flag"></param>
    public void uploadCallback(string _flag)
    {
        GameObject.Find("TransparenBackground").SetActive(false);

        if (_flag.Equals("success"))
        {
            //            Application.LoadLevel("StartScene");
            if (null != gameTemp.GetComponent<SunmGMUIBtn>())
            {
                gameTemp.GetComponent<SunmGMUIBtn>().DestoyDataAndSwitchScene();
            }
        }
        else if (_flag.Equals("failer"))
        {
            //SunmCallAndroid.UnityToAndroid("toast", "上传失败，关闭弹窗!");
        }
    }

    /**************************存档操作****************************/
    /// <summary>
    /// 接收自己发送的消息 - 存档
    /// </summary>
    /// <param name="_cout"></param>
    void ArchiveMessageProcessing(object[] _cout)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (_cout[0] is Vector3)
            {
                StartCoroutine(DeleteArchiveInfo((Vector3)_cout[0]));
            }
            else
            {
                StartCoroutine(WriteArchiveInfo((byte)_cout[0], (Vector3)_cout[1]));
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            ///< PC 测试
        }
    }

    /// <summary>
    /// 存档信息写入XML
    /// </summary>
    /// <param name="_martId"></param>
    /// <param name="_position"></param>
    /// <returns></returns>
    private IEnumerator WriteArchiveInfo(byte _martId, Vector3 _position)
    {
        SunmArchiveBean item = new SunmArchiveBean(_martId, _position.x, _position.y, _position.z, SunmConstant.level + "");
        aArchive.AddRole(item);

        yield return 0;
    }

    /// <summary>
    /// 从xml删除一个信息
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    private IEnumerator DeleteArchiveInfo(Vector3 _position)
    {
        aArchive.RemoveItemById(_position + "");
        yield return 0;
    }

    /// <summary>
    /// 加载存档 
    /// </summary>
    private void loadArchiveL()
    {
       StartCoroutine(loaddingArchiveL(SunmConstant.ExArchivePath, 0.01f));
    }

    /// <summary>
    /// 读档
    /// </summary>
    /// <param name="_fileName"></param>
    /// <param name="_ArchiveList"></param>
    /// <returns></returns>
    private IEnumerator loaddingArchiveL(string _ArchivePath, float waitTime)
    {
        string filepath = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            filepath = _ArchivePath;                           // 在Android中实例化WWW不能在路径前面加"file://"
        }
        else
        {
            //filepath = "file://" + UnityEngine.Application.streamingAssetsPath + "/" + _fileName;   // 在Windows中实例化WWW必须要在路径前面加"file://
        }

        yield return new WaitForSeconds(waitTime);
        SunmArchive loadArchiveed = new SunmArchive(filepath);
        List<SunmArchiveBean> _ArchiveList = loadArchiveed.GetDataFromXml();
   
        for (int i = 0; i < _ArchiveList.Count; ++i)
        {
            SunmArchiveBean ob = _ArchiveList[i];

            ///< 设置占据位置
            Vector3 position = new Vector3(ob.x, ob.y, ob.z);
            SunmGameInit.setThreeArray(position, ob.matarialId);         
            SunmCubeOpt.AddCube(position, SunmConstant.rootCube.transform.rotation, ob.matarialId);
//            ArchiveMessageProcessing(new object[] { ob.matarialId, position });
            /////< 加入新的存档
            SunmArchiveBean item = new SunmArchiveBean(ob.matarialId, position.x, position.y, position.z, SunmConstant.level + "");
            aArchive.AddRole(item);
        }

        _ArchiveList.Clear();
        _ArchiveList = null;
    }

    /**************************数据辅助操作****************************/

    public static void initThreeArray()
    {
        if (null == cubeFlag)
        {
            cubeFlag = new byte[200, 200, 200];
        }
        Array.Clear(cubeFlag, 0, cubeFlag.Length);
    }

    public void initArchive()
    {
        string ArchiveInfo = SunmCallAndroid.UnityToAndroidStr("getArchivePath", SunmConstant.level + "#.xml");

        string[] dataArray = ArchiveInfo.Split(new char[] { '#' });

        aArchive = new SunmArchive(dataArray[0], dataArray[1] + ".xml");
        ///< Add origin cube to archive.
//        SunmArchiveBean item = new SunmArchiveBean(1, 100, 100, 100, SunmConstant.level + "");
//        aArchive.AddRole(item);
        archiveingPath = dataArray[0];
        archiveingName = dataArray[1];
    }
    public static void clearThreeArray()
    {
        Array.Clear(cubeFlag, 0, cubeFlag.Length);
    }

    public static void setThreeArray(Vector3 _point,
                                     byte _flag)
    {
        cubeFlag[(int)_point.x, (int)_point.y, (int)_point.z] = _flag;
    }

    public static bool bOcupperThreeArray(Vector3 _point)
    {
        return (cubeFlag[(int)_point.x, (int)_point.y, (int)_point.z] != 0) ? true : false;
    }

    /// <summary>
    /// 返回标志位对应的材质名称
    /// </summary>
    /// <param name="_point"></param>
    /// <returns></returns>
    public static byte getMaterialId(Vector3 _point)
    {
        return cubeFlag[(int)_point.x, (int)_point.y, (int)_point.z];
    }
}
