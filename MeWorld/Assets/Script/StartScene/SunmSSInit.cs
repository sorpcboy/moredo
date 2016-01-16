using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunmSSInit : MonoBehaviour 
{
    /// <summary>
    /// 模式选择界面
    /// </summary>
    public GameObject modelPanel;             
    /// <summary>
    /// 删除bool判断
    /// </summary>
    public static bool deleteFlag = false;    
    /// <summary>
    /// 找到删除按钮
    /// </summary>
    public GameObject saveDelete;
    /// <summary>
    /// 继续游戏存档链表
    /// </summary>
    public static List<SunmContinueBean> ContinueDataArchive = null;
    /// <summary>
    /// 存档删除链表
    /// </summary>
    public static List<string> DeleteContinueData = null;
    /// <summary>
    /// 记录上次点击条目
    /// </summary>
    public static GameObject LastArchiveItem = null;
    /// <summary>
    /// 存档根路径
    /// </summary>
    public static string ArchivePathRoot = "";
    /// <summary>
    /// 【本地存档】同步失败提示
    /// </summary>
    public static bool Synfailer = true;
    /// <summary>
    /// 【本地存档同步】 没有数据提示bool
    /// </summary>
    public static bool SynNoData = true;
    
	// Use this for initialization
	void Start () 
    {
        ArchivaListInit();
        if (Application.platform == RuntimePlatform.Android)
        {
            ArchivePathRoot = SunmCallAndroid.UnityToAndroidStr("getArchiveRoot", "invilid");
        }
	}

    /// <summary>
    /// 链表初始化
    /// </summary>
    void ArchivaListInit() 
    {
        if (null == ContinueDataArchive) 
        {
            ContinueDataArchive = new List<SunmContinueBean>();
        }
        ContinueDataArchive.Clear();

        if (null == DeleteContinueData) 
        {
            DeleteContinueData = new List<string>();
        }
        DeleteContinueData.Clear();
    }

	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            modelPanel.SetActive(false);
        }
	}

    void OnDestroy() 
    {
        LastArchiveItem = null;
    }

    /// <summary>
    /// 清空链表及数据
    /// </summary>
    public static void DestroyArchiveData()
    {
       GameObject grid = GameObject.Find("Grid");
        if (null != grid && grid.transform.childCount > 0)
        {
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (null != ContinueDataArchive)
        {
            ContinueDataArchive.Clear();
            ContinueDataArchive = null;
        }

        if (null != LastArchiveItem)
        {
            LastArchiveItem = null;
        }

        if (null != SunmSSUI.currentArchiveBean) 
        {
            SunmSSUI.currentArchiveBean = null;
        }

        if (null != DeleteContinueData) 
        {
            DeleteContinueData.Clear();
            DeleteContinueData = null;
        }

        ScrollViewControl.ResetParameter();

        deleteFlag = false;
        Synfailer = true;
        SynNoData = true;
    }

    #region
    //public void DeleteChange() 
  //{
  //    saveDelete.GetComponent<UISprite>().color = new Color(0.5f,0.5f,0.5f,1f);
  //    saveDelete.GetComponent<BoxCollider>().enabled = false;
  //}
  //public void DeleteRes() 
  //{
  //    foreach (Transform child in grid.transform)
  //    {
  //        if (child.GetComponent<UISprite>().spriteName == "save_bar_select")
  //      {
  //          SunmSSInit.deleteFlag = true;
  //          saveDelete.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
  //          saveDelete.GetComponent<BoxCollider>().enabled = true;
  //      }
  //    }
    //}
    #endregion

}
