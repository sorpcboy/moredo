using UnityEngine;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// 根据不同的模式挂载不同的脚本 - 实现场景共用
/// </summary>
public class SunmLoadScript : MonoBehaviour
{
    /// <summary>
    /// 找到调节灵敏度按钮 - 后续优化需要把这个功能放到合适的位置
    /// </summary>
    public GameObject SetControlButton;
	void Start () 
    {
        ///< 初始化方块链表
        SunmConstant.init();
        ///< 挂载初始化脚本
        gameObject.AddComponent<SunmGameInit>();
        SunmConstant.originCube = GameObject.Find("(100.0, 100.0, 100.0)");
        SunmConstant.rootCube = GameObject.Find("RootCube");
        SunmConstant.cubeList.Add(SunmConstant.originCube);

        ///< 不同模式挂载不同手势脚本
	    ///< 漫游模式
        if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
        {
            ///< 方向键显示
            gameObject.GetComponent<SunmRMKeyControl>().enabled = true;
            gameObject.GetComponent<SunmRTKeyControl>().enabled = false;
            gameObject.GetComponent<TBDragView>().enabled = true;
            SetControlButton.SetActive(true);
            gameObject.AddComponent<SunmRMGesture>();
        }
        ///< 旋转模式
        else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
        {
            ///< 方向键取消
            gameObject.GetComponent<SunmRTKeyControl>().enabled = true;
            gameObject.GetComponent<SunmRMKeyControl>().enabled = false;
            gameObject.GetComponent<TBDragView>().enabled = false;
            gameObject.AddComponent<SunmRTGesture>();         
        }

        ///< 处理完后可以销毁该脚本了
        Destroy(gameObject.GetComponent<SunmLoadScript>());
	}
}
