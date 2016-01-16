using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SunmConstant : MonoBehaviour 
{
    /// <summary>
    /// 当前游戏模式
    /// </summary>
    public static GAME_LEVEL level = GAME_LEVEL.ROTATE_SCENE;
    public static GAME_LEVEL gameModel = GAME_LEVEL.START_SCENE;
    /// <summary>
    /// 读档路径
    /// </summary>
    public static string ExArchivePath = "";

    public enum GAME_LEVEL
    {
        START_SCENE, ROTATE_SCENE, ROAM_SCENE , CONTINUE_MODEL
    }

    /// <summary>
    /// 【释放】原始魔方 - 各个场景的原始魔方对象
    /// </summary>
    public static GameObject originCube = null;
    public static GameObject rootCube = null;
    /// <summary>
    /// 【释放】方块链表
    /// </summary>
    public static ArrayList cubeList = null;
    /// <summary>
    /// 材质字典
    /// </summary>
    public static Dictionary<string, Dictionary<byte, string>> MaterialData = null;
    /// <summary>
    /// 记录材质使用，一个ID对应一个材质名字
    /// </summary>
    public static Dictionary<byte, string> MaterialList = null;
    ///< 选择的材质ID、同时也作为方块存在的标志位 1【原始方块】 - 255
    public static byte setMaterialID = 8;
    /// <summary>
    /// 手指滑动偏移量
    /// </summary>
    public static float offDistance = 2f;
    /// <summary>
    /// 限定射线检测位置的最大最小值
    /// </summary>
    public static float RayMinLimit = 1f;
    public static float RayMaxLimit = 50f;

    /// <summary>
    /// 初始化链表
    /// </summary>
    public static void init()
    {
        if (null == cubeList)
        {
            cubeList = new ArrayList();
        }
        cubeList.Clear();
        if (null == MaterialData)
        {
            MaterialData = new Dictionary<string, Dictionary<byte, string>>();
        }
        MaterialData.Clear();
        if (null == MaterialList)
        {
            MaterialList = new Dictionary<byte, string>();
        }
        MaterialList.Clear();
    }

    /// <summary>
    /// 重置链表
    /// </summary>
    public static void reset()
    {
        ///< 清空方块链表【已经包括原始方块】
        if (null != cubeList)
        {
            foreach (var obj in cubeList)
            {
                if (obj is GameObject)
                {
                    GameObject.Destroy((GameObject)obj);
                }
            }
            cubeList.Clear();
        }
        else
        {
            cubeList = new ArrayList();
        }

        ///< 旋转模式、漫游模式
        if (SunmConstant.level == GAME_LEVEL.ROTATE_SCENE ||
            SunmConstant.level == GAME_LEVEL.ROAM_SCENE)
        {
            ///< 清空标志位
            SunmGameInit.clearThreeArray();
            ///< 重新创建原始方块
            originCube = Instantiate(Resources.Load("Prefab/CubePice/PiceCub")) as GameObject;
            originCube.name = "(100.0, 100.0, 100.0)";
            originCube.transform.parent = rootCube.transform;

            ///< 设置标志位-加入链表
            SunmGameInit.setThreeArray(new Vector3(100.0f, 100.0f, 100.0f), 1);
            cubeList.Add(originCube);
        }

        ///< 重置摄像头
        GameObject.Find("Main Camera").transform.position = new Vector3(100, 100, 95f);
        GameObject.Find("Main Camera").transform.rotation = Quaternion.identity;
        if (SunmConstant.level == GAME_LEVEL.ROTATE_SCENE)
        {
            GameObject.Find("Main Camera").GetComponent<SunmRTGesture>().resetPosition();
        }
    }

    /// <summary>
    /// 摄像机重置
    /// </summary>
    public static void resetCamera(Vector3 CameraRe) 
    {
        ///< 重置摄像头
        GameObject CameraGameOBJ = GameObject.Find("Main Camera");
        CameraGameOBJ.transform.position = CameraRe;
        CameraGameOBJ.transform.rotation = Quaternion.identity;
        if (SunmConstant.level == GAME_LEVEL.ROTATE_SCENE)
        {
            GameObject.Find("Main Camera").GetComponent<SunmRTGesture>().resetPosition();
        }
    }

    /// <summary>
    /// 销毁链表和原始方块
    /// </summary>
    public static void destroy()
    {
        ///< 销毁方块链表【已经包括原始方块】
        if (null != cubeList)
        {
            foreach (var obj in cubeList)
            {
                if (obj is GameObject)
                {
                    GameObject.Destroy((GameObject)obj);
                }
            }
            cubeList.Clear();
            cubeList = null;
        }

        ///< 销毁方块的根
        if (null != rootCube)
        {
            GameObject.Destroy(rootCube);
            rootCube = null;
        }

        ///< 销毁材质字典
        if (null != MaterialData)
        {
            MaterialData.Clear();
            MaterialData = null;
        }
        ///< 销毁材质链表
        if (null != MaterialList)
        {
            MaterialList.Clear();
            MaterialList = null;
        }

        ///< 材质ID重置
        setMaterialID = 8;
    }

    /// <summary>
    /// 返回材质的名字
    /// </summary>
    /// <returns></returns>
    public static string getMaterialName(byte _materialName)
    {
        return SunmConstant.MaterialList[_materialName];
    }
}
