using UnityEngine;
using System.Collections;
using System;
public class ScrollViewControl : MonoBehaviour
{
    /// <summary>
    /// 找到物体UIScrollView来进行计算上一页下一页相关操作
    /// </summary>
    UIScrollView scrollView;
    /// <summary>
    /// 找到预设体进行实例化
    /// </summary>
    private GameObject item = null;
    /// <summary>
    /// 找到grid， 作为父物体进行实例化等相关操作
    /// </summary>
    public GameObject grid;

    /// <summary>
    /// 总页数
    /// </summary>
    private static int pagesNumberAll = 1;
    /// <summary>
    /// 页数初始化
    /// </summary>
    private static int NumberPages = 1;
    /// <summary>
    /// 每页显示的个数初始化
    /// </summary>
    private static int PagesInfoNumber = 4;

    void Start()
    {

    }
    void initControl()
    {
        scrollView = transform.GetComponent<UIScrollView>();
        pagesNumberAll = (SunmSSInit.ContinueDataArchive.Count / PagesInfoNumber) + 1;
    }

    ///// <summary>
    ///// 外部调用刷新列表【删除存档时】
    ///// </summary>
    public void UIStart()
    {
        pagesNumberAll = 1;
        NumberPages = 1;
        PagesInfoNumber = 4;
        initControl();
        getInfo();
        scrollView.onDragFinished += OnDragFinished;
        flushRank();
    }
    void OnEnable()
    {
        initControl();
        getInfo();
        scrollView.onDragFinished += OnDragFinished;
    }
    void OnDisable()
    {
       scrollView.onDragFinished -= OnDragFinished;
    }
    void OnDragFinished()
    {
        scrollView.UpdateScrollbars(true);
        Vector3 constraint = scrollView.panel.CalculateConstrainOffset(scrollView.bounds.min, scrollView.bounds.min);
        if (constraint.y <= 1f)
        {
            if (NumberPages < pagesNumberAll)
            {
                NumberPages++;
                getInfo();
            }

        }

        flushRank();
    }
    /// <summary>
    /// 生成存档条目的方法
    /// </summary>
    void getInfo()
    {
        for (int i = (NumberPages - 1) * PagesInfoNumber; i < ((NumberPages - 1) * PagesInfoNumber + PagesInfoNumber) && i < SunmSSInit.ContinueDataArchive.Count; ++i)
        {
            item = (GameObject)Resources.Load("Prefab/ContinuePrefab/ObjetPrefab");
            GameObject newItem = NGUITools.AddChild(grid, item);
            newItem.name = i+"items";
            newItem.GetComponent<SunmItemControl>().serial = SunmSSInit.ContinueDataArchive[i].serial;
            newItem.GetComponent<SunmItemControl>().fileName = SunmSSInit.ContinueDataArchive[i].fileName;
            newItem.GetComponent<SunmItemControl>().modelUrl = SunmSSInit.ContinueDataArchive[i].fileUrl;
            newItem.GetComponent<SunmItemControl>().picUrl = SunmSSInit.ContinueDataArchive[i].imageUrl;
            newItem.GetComponent<SunmItemControl>().saveType = SunmSSInit.ContinueDataArchive[i].model;
            newItem.GetComponent<SunmItemControl>().parentPath = SunmSSInit.ContinueDataArchive[i].parentPath;
            newItem.GetComponent<SunmItemControl>().id = SunmSSInit.ContinueDataArchive[i].id;
            newItem.GetComponent<SunmItemControl>().isLocal = SunmSSInit.ContinueDataArchive[i].isLocal;
            newItem.GetComponent<SunmItemControl>().entryName = i + "items";
        }
    }

    /// <summary>
    /// 初始化UIGrid的方法
    /// </summary>
    void flushRank()
    {
        grid.GetComponent<UIGrid>().Reposition();
        grid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(grid);
    }

    /// <summary>
    /// 重置分页变量
    /// </summary>
    public static void ResetParameter()
    {
        pagesNumberAll = 1;
        NumberPages = 1;
        PagesInfoNumber = 4;
    }
}
