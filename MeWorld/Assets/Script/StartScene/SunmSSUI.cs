using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;

public class SunmSSUI : MonoBehaviour
{
    public GameObject modelPanel;
    /// <summary>
    /// 点击退出按钮弹出的两个按钮父物体
    /// </summary>
    public GameObject exitButton;                   
    /// <summary>
    /// 点击继续按钮的弹框父物体，存档弹窗
    /// </summary>
    public GameObject backPackPanel;          
    public GameObject mainCamera;
    public START_BUTTONID buttonID = START_BUTTONID.NOTHING;

    /// <summary>
    /// 进度条
    /// </summary>
    public GameObject TransparenBackground;

    /// <summary>
    /// 存档模式
    /// </summary>
    public static SunmContinueBean currentArchiveBean =  null;

    /// <summary>
    /// 找到Grid重置 UIGrid状态【存档返回时使用】
    /// </summary>
    private GameObject ResetGrid = null;
    /// <summary>
    /// 存档删除时提示【本地/云端】
    /// </summary>
    private bool PrintPrompt = true;

    void Start() 
    {
        mainCamera = GameObject.Find("Main Camera");
        ResetGrid = GameObject.Find("Grid");
    }

    void Update()
    {
     
    }
    /// <summary>
    /// 开始游戏按钮，继续游戏按钮，退出按钮，点击退出按钮后弹出的确认按钮和取消按钮，简单模式按钮，普通模式按钮，存档返回按钮，存档确认按钮，
    /// </summary>
    public enum START_BUTTONID
    {
        NOTHING = 0, STARTBTN, CONTINUEBTN, EXITBTN, EXIT_OK, EXIT_CANCEL, EASYBTN, NORMALBTN, SAVEBACK, SAVEDELETE, SAVESURE
    };

    void OnClick()
    {
        switch (buttonID)
        {
            case START_BUTTONID.STARTBTN:
                modelPanel.SetActive(true);
                break;
            case START_BUTTONID.CONTINUEBTN:
                SunmCallAndroid.UnityToAndroid("getArchiver", "invalid");                
                break;
            case START_BUTTONID.EXITBTN:
                exitButton.SetActive(true); 
                break;
            case START_BUTTONID.EXIT_OK:
                Application.Quit();
                Process.GetCurrentProcess().Kill();
                break;
            case START_BUTTONID.EXIT_CANCEL:

                break;
            case START_BUTTONID.EASYBTN:
                SunmConstant.level = SunmConstant.GAME_LEVEL.ROTATE_SCENE;
                Application.LoadLevel("LoadScene");
                break;
            case START_BUTTONID.NORMALBTN:
                SunmConstant.level = SunmConstant.GAME_LEVEL.ROAM_SCENE;
                Application.LoadLevel("LoadScene");
                break;
            case START_BUTTONID.SAVEBACK:
                ResetData();              
                transform.parent.parent.parent.gameObject.SetActive(false);
                //mainCamera.GetComponent<SunmSSInit>().Destroy();
                break;
            case START_BUTTONID.SAVEDELETE:
                if (false ==SunmSSInit.deleteFlag)
                {
                    SunmSSInit.deleteFlag = true;
                    if (PrintPrompt)
                    {
                        SunmCallAndroid.UnityToAndroid("toast", "温馨提示：本地存档只有同步后才能删除哦！");
                        PrintPrompt = false;
                    }
                   
                 //   print("开");
                }
                else if (true == SunmSSInit.deleteFlag)
                {
                    SunmSSInit.deleteFlag = false;
                    if (SunmSSInit.DeleteContinueData.Count > 0) 
                    {
                        TransparenBackground.SetActive(true);
                        DeleteArchive();
                    }

                    ArchivedItemsState();
                 //   print("关");
                }
               
                //SunmSSInit.deleteClick++;
                break;
            case START_BUTTONID.SAVESURE:
                if (null == SunmSSUI.currentArchiveBean)
                {
                    SunmCallAndroid.UnityToAndroid("toast","请选择要加载的存档");
                    return;
                }

                if (currentArchiveBean.isLocal)
                {
                    string LocalArchivePath = currentArchiveBean.parentPath + currentArchiveBean.fileName + ".xml";
                    if (File.Exists(LocalArchivePath))
                    {
                        ScenJumpReadFile(LocalArchivePath);
                    }
                    else 
                    {
                        SunmCallAndroid.UnityToAndroid("toast","本地文件损坏!");
                    }
                }
                else 
                {
                    string ArchivePathRoot = SunmSSInit.ArchivePathRoot + currentArchiveBean.model+"/"+currentArchiveBean.fileName + "/" + currentArchiveBean.fileName + ".xml";
                    ///< 解压时路径
                    string ArchivePathZip = SunmSSInit.ArchivePathRoot + currentArchiveBean.model + "/" + currentArchiveBean.fileName + "/" + currentArchiveBean.fileName;
                    if (File.Exists(ArchivePathRoot))
                    {
                        ScenJumpReadFile(ArchivePathRoot);
                    }
                    else 
                    {
                        StartCoroutine(downfile(currentArchiveBean.fileUrl, ArchivePathZip));
                    }
                }
                break;
        }
    }

    public void ContinueDataParsing(string Info)
    {
        if (null == SunmSSInit.ContinueDataArchive) 
        {
            SunmSSInit.ContinueDataArchive = new List<SunmContinueBean>();
        }
        if (null == SunmSSInit.DeleteContinueData) 
        {
            SunmSSInit.DeleteContinueData = new List<string>();
        }
        if (1 == SunmContinueData.ArchiveDataParsing(Info, SunmSSInit.ContinueDataArchive))
        {
            backPackPanel.SetActive(true);
            backPackPanel.transform.FindChild("BackPackBG").FindChild("backpack").FindChild("Scroll View").FindChild("Grid").GetComponent<UIGrid>().repositionNow = true;
        }
        else if (-1 == SunmContinueData.ArchiveDataParsing(Info, SunmSSInit.ContinueDataArchive)) 
        {
            SunmCallAndroid.UnityToAndroid("toast","获取存档失败，请再次尝试！");
        }
        else if (0 == SunmContinueData.ArchiveDataParsing(Info, SunmSSInit.ContinueDataArchive)) 
        {
            SunmCallAndroid.UnityToAndroid("toast", "获取存档失败，请检查网络连接！");
        }
    }

    /// <summary>
    /// 停止协程的方法
    /// </summary>
    private void StopCoroutines()
    {
        StopAllCoroutines();
        SunmCallAndroid.UnityToAndroid("toast", "由于网络原因存档加载失败！");
    }
    //------------------------------------------开启协程序下载存档----------------------------------------//
    IEnumerator downfile(string url, string LocalPath)
    {
        Uri u = new Uri(url);
        HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(u);
        mRequest.Method = "GET";
        mRequest.ContentType = "application/x-www-form-urlencoded";

        ///< 文件夹是否存档判断
        string[] str = LocalPath.Split('/');
        string dir = LocalPath.Substring(0, LocalPath.Length - str[str.Length - 1].Length);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        HttpWebResponse wr = null;
        int flag = 0;
        try
        {
            wr = (HttpWebResponse)mRequest.GetResponse();
        }
        catch (Exception)
        {
            flag = -1;
        }
        if (-1 == flag)
        {
            yield return -1;
            getResponse("" + -1, LocalPath);
        }
        else if (!wr.StatusCode.Equals(HttpStatusCode.OK))
        {
            yield return -1;
            getResponse("NotOK:" + wr.StatusCode, LocalPath);
        }
        else
        {
            Stream sIn = wr.GetResponseStream();
            FileStream fs = new FileStream(LocalPath, FileMode.Create, FileAccess.Write);
            long length = wr.ContentLength;
            long i = 0;
            decimal j = 0;

            while (i < length)
            {
                byte[] buffer = new byte[1024];
                int resLen = sIn.Read(buffer, 0, buffer.Length);
                i += resLen;
                fs.Write(buffer, 0, resLen);

                if ((i % 1024) == 0)
                {
                    j = Math.Round(Convert.ToDecimal((Convert.ToDouble(i) / Convert.ToDouble(length)) * 100), 4);
                    //t = "当前下载文件大小:" + length.ToString() + "字节   当前下载大小:" + i + "字节 下载进度" + j.ToString() + "%";
                }
                else
                {
                    //t = "当前下载文件大小:" + length.ToString() + "字节   当前下载大小:" + i + "字节";
                }
                yield return 1;
            }

            sIn.Close();
            wr.Close();
            fs.Close();

            if (!File.Exists(LocalPath))
            {
                getResponse("" + -1, LocalPath);
            }
            else
            {
                ZipHelper.UnZip(LocalPath, dir);                
                yield return 2;

                getResponse("" + 2, LocalPath + ".xml");
            }      
        }
    }

    public void getResponse(String _rsp, String _filePath)
    {
        if (_rsp.Equals("2"))
        {
            ScenJumpReadFile(_filePath);
        }
        else if (_rsp.Equals("-1"))
        {
            SunmCallAndroid.UnityToAndroid("toast", "网络请求失败！");
        }
        else
        {
            SunmCallAndroid.UnityToAndroid("toast", _rsp);
        }
    }

    private void ScenJumpReadFile(string _path)
    {
        SunmConstant.ExArchivePath = _path;
        SunmConstant.gameModel = SunmConstant.GAME_LEVEL.CONTINUE_MODEL;
        if (currentArchiveBean.model.Equals("ROTATE_SCENE"))
        {
            SunmConstant.level = SunmConstant.GAME_LEVEL.ROTATE_SCENE;
        }
        else if (currentArchiveBean.model.Equals("ROAM_SCENE"))
        {
            SunmConstant.level = SunmConstant.GAME_LEVEL.ROAM_SCENE;
        }

        Application.LoadLevel("LoadScene");
    }

    private void ResetData() 
    {
        SunmSSInit.DestroyArchiveData();
        ResetGrid.GetComponent<UIGrid>().Reposition();
        SunmConstant.gameModel = SunmConstant.GAME_LEVEL.START_SCENE;
    }

    /// <summary>
    /// 删除存档的方法
    /// </summary>
    private void DeleteArchive() 
    {
        ///< 删除存档条目ID
        string DeleteID = "";
        int count = SunmSSInit.DeleteContinueData.Count;
        for (int i = 0; i < count; ++i) 
        {
            if (i == (count - 1))
            {
                DeleteID += SunmSSInit.DeleteContinueData[i];
            }
            else
            {
                DeleteID += SunmSSInit.DeleteContinueData[i] + ",";
            }
            
        }

         SunmCallAndroid.UnityToAndroid("deleteCloudArchive", DeleteID);  
    }

    /// <summary>
    /// 删除存档取消进度条 - android调用
    /// </summary>
    public void CancelProgressBar(string str) 
    {
        if ("1" == str)
        {
            for (int i = 0; i < SunmSSInit.DeleteContinueData.Count; ++i)
            {
                string RemoveID = SunmSSInit.DeleteContinueData[i];
                for (int j = 0; j < SunmSSInit.ContinueDataArchive.Count; ++j)
                {
                    if (SunmSSInit.ContinueDataArchive[j].serial == RemoveID)
                    {
                        SunmSSInit.ContinueDataArchive.Remove(SunmSSInit.ContinueDataArchive[j]);
                    }
                }
            }
          
            SunmSSInit.DeleteContinueData.Clear();
            DeleteArchivePrefab();
            GameObject gameOBJ = GameObject.Find("Scroll View");
            gameOBJ.GetComponent<ScrollViewControl>().UIStart();
            ///< 取消进度条
            CancelLoding();
        }
        else
        {
            SunmCallAndroid.UnityToAndroid("toast", "删除存档失败！");

            CancelLoding(); 

            ///< 取消进度条
            CancelLoding(); 
        }
    }

    /// <summary>
    /// 删除存档预设体【删除云端成功后】
    /// </summary>
    public void DeleteArchivePrefab()
    {
        GameObject instenPer = GameObject.Find("Grid");
        if (null != instenPer)
        {
            int count = instenPer.transform.childCount;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    Destroy(instenPer.transform.GetChild(i).gameObject);
                    //if (instenPer.transform.GetChild(i).name == ID) 
                    //{
                    //    Destroy(instenPer.transform.GetChild(i).gameObject);
                    //    SunmCallAndroid.UnityToAndroid("systemout","删除成功！");
                    //}                  
                }
            }
        }
    }

    /// <summary>
    /// 按钮状态重置【删除框取消时】
    /// </summary>
    public static void ArchivedItemsState()
    {
        GameObject instenPer = GameObject.Find("Grid");
        if (null != instenPer)
        {
            int count = instenPer.transform.childCount;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    instenPer.transform.GetChild(i).gameObject.GetComponent<UISprite>().spriteName = "save_bar";
                }
            }
        }
    }

    /// <summary>
    /// 取消进度条
    /// </summary>
    private void CancelProgressBar() 
    {
        ///< 取消进度条
        if (null != TransparenBackground)
        {
            TransparenBackground.SetActive(false);
        }
        else
        {
            TransparenBackground = GameObject.Find("TransparenBackground");
            TransparenBackground.SetActive(false);
        }
    }
    /// <summary>
    /// 取消进度条
    /// </summary>
    //private void CancelProgressBar()
    //{
    //    ///< 取消进度条
    //    if (null != TransparenBackground)
    //    {
    //        TransparenBackground.SetActive(false);
    //    }
    //    else
    //    {
    //        TransparenBackground = GameObject.Find("TransparenBackground");
    //        TransparenBackground.SetActive(false);
    //    }
    //}

    /// <summary>
    /// 取消进度条
    /// </summary>
    private void CancelLoding() 
    {
        ///< 取消进度条
        if (null != TransparenBackground)
        {
            TransparenBackground.SetActive(false);
        }
        else
        {
            TransparenBackground = GameObject.Find("TransparenBackground");
            TransparenBackground.SetActive(false);
        }
    }
}
