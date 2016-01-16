using UnityEngine;
using System.Collections;
using System;
public class SunmItemControl : MonoBehaviour
{
    /// <summary>
    /// 条目唯一标识符
    /// </summary>
    [HideInInspector]
    public string serial = "";
    /// <summary>
    /// 文件名
    /// </summary>
    [HideInInspector]
    public string fileName = "";
    /// <summary>
    /// 存档模式
    /// </summary>
    [HideInInspector]
    public string saveType = "";
    /// <summary>
    /// 存档路径
    /// </summary>
    [HideInInspector]
    public string modelUrl = "";
    /// <summary>
    /// 存档图片路径
    [HideInInspector]
    public string picUrl = "";
    /// <summary>
    /// 本地路径 - 父路径 parentPath+fileName+".png" +".xml"
    /// </summary>
    [HideInInspector]
    public string parentPath = "";
    /// <summary>
    /// 存档ID
    /// </summary>
    [HideInInspector]
    public int id = 0;
    /// <summary>
    /// 判断存档类型的bool
    /// </summary>
    [HideInInspector]
    public bool isLocal = true;
    /// <summary>
    /// 预设体名字
    /// </summary>
     [HideInInspector]
    public string entryName;
    /// <summary>
    /// 找到删除选择框
    /// </summary>
    public GameObject ColoredCheckbox;
    /// <summary>
    /// 云同步按钮
    /// </summary>
    public GameObject SynButton;
    /// <summary>
    /// 云同步进度条
    /// </summary>
    public GameObject SynLoding;
    /// <summary>
    /// 找到Grid做点击状态
    /// </summary>
    private GameObject Grid = null;
    /// <summary>
    /// 找到显示图片的GameOBJ
    /// </summary>
    private GameObject ShowImage = null;

    void Start()
    {
        try
        {
            long lt = long.Parse(fileName);
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度                         
            long time_tricks = tricks_1970 + lt * 10000;//日志日期刻度
            DateTime dt = new DateTime(time_tricks).AddHours(8);//转化为DateTime
            String getWTime = dt.ToString("yyyy/MM/dd/ HH:mm:ss");
            gameObject.transform.GetComponentInChildren<UILabel>().text = getWTime;
        }
        catch (Exception)
        {
            gameObject.transform.GetComponentInChildren<UILabel>().text = fileName;
        }

        ShowImage = GameObject.Find("ShowImage");

        ///< 如果是本地存档则显示同步按钮
        if (isLocal && null != SynButton)
        {
            SynButton.SetActive(true);
        }
    }

    void Update()
    {
        if (!isLocal)
        {
            ControlSelectBox();
        }
    }
    void OnClick()
    {
        if (null != SunmSSInit.LastArchiveItem && false == SunmSSInit.deleteFlag)
        {
            SunmSSInit.LastArchiveItem.GetComponent<UISprite>().spriteName = "save_bar";
        }
        SunmSSInit.LastArchiveItem = gameObject;
        gameObject.GetComponent<UISprite>().spriteName = "save_bar_select";

        if (false == SunmSSInit.deleteFlag)
        {
            if (isLocal)
            {
                ShowImage.SendMessage("showImage", "file:///" + parentPath + fileName + ".jpg");
            }
            else
            {
                ShowImage.SendMessage("showImage", picUrl);
            }
        }

        if (null == SunmSSUI.currentArchiveBean)
        {
            SunmSSUI.currentArchiveBean = new SunmContinueBean();
        }

        SunmSSUI.currentArchiveBean.parentPath = parentPath;
        SunmSSUI.currentArchiveBean.fileUrl = modelUrl;
        SunmSSUI.currentArchiveBean.fileName = fileName;
        SunmSSUI.currentArchiveBean.model = saveType;
        SunmSSUI.currentArchiveBean.isLocal = isLocal;
        SunmSSUI.currentArchiveBean.id = id;
        SunmSSUI.currentArchiveBean.serial = serial;

        ///< 删除存档时传值
        ColoredCheckbox.transform.FindChild("Checkmark").GetComponent<SunmCheckBox_Test>().RemoveSerial = serial;
        ColoredCheckbox.transform.FindChild("Checkmark").GetComponent<SunmCheckBox_Test>().isLocal = isLocal;
    }

    /// <summary>
    /// 删除选择框开关
    /// </summary>
    private void ControlSelectBox()
    {
        if (true == SunmSSInit.deleteFlag)
        {
            ColoredCheckbox.SetActive(true);
        }
        else if (false == SunmSSInit.deleteFlag)
        {
            ColoredCheckbox.SetActive(false);
        }
    }

    /// <summary>
    ///本地存档同步 
    /// </summary>
    private void LocalArchiveSynchronous()
    {
        if (isLocal)
        {
            cancelSynButton();
            ///< 此处调用进度条
            if (null != SynLoding)
            {
                SynLoding.SetActive(true);
            }
            ///< 同步用到的参数
            String localInfo = entryName + "#"+ serial + "#" + parentPath + "#" + fileName + ".xml" + "#" + fileName + ".jpg" + "#" + saveType;
            ///< 调用android同步方法
            SunmCallAndroid.UnityToAndroid("asycnArchive", localInfo);
        }
    }

    /// <summary>
    /// 安卓调用返回【本地存档】同步状态并取消进度条
    /// </summary>
    public void ArchiveSynRegime(string _str)
    {
        ///< 此处判断同步是否成功 if成功 : 则需要把同步按钮隐藏 把进度条取消 把isLocal变为false 把picUrl变为云端连接 最后再修改总链表的值
        ///< else失败:把进度条取消 并且toast提示
        string[] dataArray = _str.Split(new char[] { '#' });

        if (dataArray[0].Equals("success"))
        {
            ///< 成功处理

            cancelSynButton();
            cancelArchiveLoding();
            //            isLocal = false;
            picUrl = dataArray[1];
            SunmSSUI.ArchivedItemsState();
            int count = SunmSSInit.ContinueDataArchive.Count;
            for (int i = 0; i < count; ++i)
            {
                if (SunmSSInit.ContinueDataArchive[i].serial == serial)
                {
                    SunmSSInit.ContinueDataArchive[i].isLocal = false;
                    SunmSSInit.ContinueDataArchive[i].imageUrl = dataArray[1];
                }
            }
        }
        else if (dataArray[0].Equals("failer"))
        {
            ///< 失败处理

            cancelArchiveLoding();
            SynButton.SetActive(true);
            if (SunmSSInit.Synfailer)
            {
                SunmCallAndroid.UnityToAndroid("toast", "存档同步失败了");
                SunmSSInit.Synfailer = false;
                SunmSSUI.ArchivedItemsState();
            }
        }
        else if (dataArray[0].Equals("nodata"))
        {
            ///< 本地存档已经删除或者不存在的时候做判断  

            cancelArchiveLoding();
            SynButton.SetActive(true);
            SunmCallAndroid.UnityToAndroid("toast", "本地没有存档数据了 :(");
            SunmSSInit.SynNoData = false;
            SunmSSUI.ArchivedItemsState();
        }
    }

    /// <summary>
    /// 隐藏进度条
    /// </summary>
    private void cancelArchiveLoding()
    {
        if (null != SynLoding)
        {
            SynLoding.SetActive(false);
        }
        else
        {
            GameObject _SynLoding = GameObject.Find("SynLoding");
            _SynLoding.SetActive(false);
        }
    }

    /// <summary>
    /// 隐藏同步按钮
    /// </summary>
    private void cancelSynButton()
    {
        if (null != SynButton)
        {
            SynButton.SetActive(false);
        }
        else
        {
            GameObject _SynButton = GameObject.Find("SynButton");
            _SynButton.SetActive(false);
        }
    }
}

