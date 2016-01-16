using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class SunmGMUIBtn : MonoBehaviour
{
    /// <summary>
    ///  退出弹框
    /// </summary>
    public GameObject exitBoxTexture;
    /// <summary>
    /// 保存弹窗
    /// </summary>
    public GameObject savePopup;
    /// <summary>
    /// 材质弹窗         
    /// </summary>
    public GameObject modelPopupWindow;
    /// <summary>
    /// 上传进度条弹窗
    /// </summary>
    public GameObject transparenBackground;
    /// <summary>
    ///   玩家输入框
    /// </summary>
    public GameObject winText;
    /// <summary>
    /// 按钮枚举
    /// </summary>                      
    public ROAM_BUTTON buttonID = ROAM_BUTTON.NOTHING;
    /// <summary>
    /// 挂预设体
    /// </summary>
    public GameObject MaterialChooseFourButton;
    /// <summary>
    /// 预设体父物体
    /// </summary>
    public GameObject DetialTrdMenu;
    /// <summary>
    /// 实例化
    /// </summary>
    public GameObject menuchild;
    /// <summary>
    /// 初始化材质弹窗【点击Other时】
    /// </summary>
    public static string demo = "building";
    /// <summary>
    /// 灵敏度弹窗
    /// </summary>
    public GameObject setControl;
    /// <summary>
    /// 找到perspectiveLabel - 显示/隐藏
    /// </summary>
    public GameObject perspectiveLabel;
    /// <summary>
    /// 找到SumnRTLabel - 显示/隐藏
    /// </summary>
    public GameObject SumnRTLabel;
    /// <summary>
    /// 场景切换动画所用变量
    /// </summary>
    private SceneLoad sceneload;
    /// <summary>
    /// 找到主摄像机
    /// </summary>
    private GameObject MainCamera;

    /// <summary>
    /// < 空，右上角返回主界面按钮，主界面弹窗关闭按钮，主界面弹窗确定按钮，右下角保存模型按钮，保存框确认和取消按钮， 弹出模型种类弹窗的按钮，其他模型弹窗关闭按钮，
    /// </summary>
    public enum ROAM_BUTTON
    {
        NOTHING = 0, EXIT_BTN, EXITBOXCANCEL, EXITBOXOK, COMPLETEBTN, SAVEYES, SAVENO, OTHERCHOBTN, CANCLEBTN, ROLESBTN, FIGUREBTN, OTHERBTN, RESETCAMERA, SETCONTROL,
        SETCONTROLCLOSED,
    };

    void Start()
    {
        ///< 获取场景切换脚本
        sceneload = GameObject.Find("Main Camera").GetComponent<SceneLoad>();

        MainCamera = GameObject.Find("Main Camera");
    }

    void OnClick()
    {
        switch (buttonID)
        {
            case ROAM_BUTTON.EXIT_BTN:
                ReSpace();
                exitBoxTexture.SetActive(true);

                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = false;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = false;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = false;
                }
                break;
            case ROAM_BUTTON.EXITBOXCANCEL:
                transform.parent.parent.gameObject.SetActive(false);
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = true;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = true;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = true;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = true;
                }
                break;
            case ROAM_BUTTON.EXITBOXOK:
                ///< 判断有没有垒方块, 没有则直接返回开始场景
                if (SunmConstant.cubeList.Count > 1)
                {
                    uploadArchive(".jpg");
                }
                else
                {
                    uploadArchive("NoData");
                }
                break;
            case ROAM_BUTTON.COMPLETEBTN:
                if (SunmConstant.cubeList.Count <= 1)
                {
                    SunmCallAndroid.UnityToAndroid("toast", "没有数据可以打印！");
                    return;
                }
                ReSpace();
                winText.GetComponent<UILabel>().text = "" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                savePopup.SetActive(true);
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = false;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = false;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = false;
                }
                break;
            case ROAM_BUTTON.SAVEYES:
                if (null == winText.GetComponent<UILabel>().text ||
                    winText.GetComponent<UILabel>().text.Equals(""))
                {
                    SunmCallAndroid.UnityToAndroid("toast", "请输入作品名称!");
                    return;
                }

                transform.parent.gameObject.SetActive(false);
                transparenBackground.SetActive(true);
                ///< 调用Android:获取文件路径用于生成模型数据
                SunmCallAndroid.UnityToAndroid("getFilePathByGrade", winText.GetComponent<UILabel>().text);
                break;
            case ROAM_BUTTON.SAVENO:
                transform.parent.gameObject.SetActive(false);
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = true;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = true;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = true;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = true;
                }
                break;
            case ROAM_BUTTON.OTHERCHOBTN:
                ReSpace();
                modelPopupWindow.SetActive(true);
                AddMaterial(demo);
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = false;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = false;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = false;
                }
                //                   Debug.Log(demo);
                break;
            case ROAM_BUTTON.CANCLEBTN:
                transform.parent.gameObject.SetActive(false);
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = true;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = true;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = true;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = true;
                }
                break;
            case ROAM_BUTTON.ROLESBTN:
                AddMaterial("building");
                demo = "building";
                transform.GetComponent<UIButton>().normalSprite = "jianzhu_pressed";
                transform.parent.FindChild("FigureButton").GetComponent<UIButton>().normalSprite = "renwu";
                transform.parent.FindChild("OtherButton").GetComponent<UIButton>().normalSprite = "qita";
                //Debug.Log(transform.parent.FindChild("FigureButton").GetComponent<UIButton>().hoverSprite);
                break;
            case ROAM_BUTTON.FIGUREBTN:
                AddMaterial("figure");
                demo = "figure";
                transform.GetComponent<UIButton>().normalSprite = "renwu_pressed";
                transform.parent.FindChild("RolesButton").GetComponent<UIButton>().normalSprite = "jianzhu";
                transform.parent.FindChild("OtherButton").GetComponent<UIButton>().normalSprite = "qita";
                //Debug.Log(transform.parent.FindChild("RolesButton").GetComponent<UIButton>().hoverSprite);
                break;
            case ROAM_BUTTON.OTHERBTN:
                AddMaterial("other");
                demo = "other";
                transform.GetComponent<UIButton>().normalSprite = "qita_pressed";
                transform.parent.FindChild("RolesButton").GetComponent<UIButton>().normalSprite = "jianzhu";
                transform.parent.FindChild("FigureButton").GetComponent<UIButton>().normalSprite = "renwu";
                break;
            case ROAM_BUTTON.RESETCAMERA:
                SunmConstant.resetCamera(new Vector3(100f, 100f, 95f));
                MainCamera.AddComponent<SunmCameraResect>();
                break;
            case ROAM_BUTTON.SETCONTROL:
                ReSpace();
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = false;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    perspectiveLabel.SetActive(false);
                    SumnRTLabel.SetActive(true);
                    //                        MainCamera.GetComponent<TBDragView>().enabled = false;
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = false;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = false;
                }

                setControl.SetActive(true);
                break;
            case ROAM_BUTTON.SETCONTROLCLOSED:
                gameObject.SendMessage("ControlValue");
                if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE)
                {
                    MainCamera.GetComponent<TBDragView>().enabled = true;
                    MainCamera.GetComponent<SunmRMKeyControl>().enabled = true;
                }
                else if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE)
                {
                    //                        MainCamera.GetComponent<TBDragView>().enabled = true;
                    MainCamera.GetComponent<SunmRTKeyControl>().enabled = true;
                    MainCamera.GetComponent<SunmRTGesture>().enabled = true;
                }
                transform.parent.gameObject.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// 读取XML材质实例化预设体
    /// </summary>
    /// <param name="id"></param>
    void AddMaterial(string id)
    {
        if (DetialTrdMenu.transform.childCount != 0)
        {
            foreach (Transform go in DetialTrdMenu.transform)
            {
                Destroy(go.gameObject);
            }
        }

        foreach (KeyValuePair<byte, string> kvc in SunmConstant.MaterialData[id])
        {
            menuchild = NGUITools.AddChild(DetialTrdMenu, MaterialChooseFourButton);
            menuchild.transform.FindChild("Material").GetComponent<UISprite>().spriteName = kvc.Value;

            menuchild.GetComponent<SunmGMMaterialButton>().prefabID = kvc.Key;
            DetialTrdMenu.transform.GetComponent<UIGrid>().repositionNow = true;
        }
    }

    /// <summary>
    /// 切换至开始场景
    /// </summary>
    public void DestoyDataAndSwitchScene()
    {
        ///< 存档变量重置
        SunmConstant.gameModel = SunmConstant.GAME_LEVEL.START_SCENE;
        ///< 给场景切换脚本赋值
        sceneload.levelToLoad = "StartScene";
        ///< 跳转至开始场景
        sceneload.StartSplash();
    }

    private void uploadArchive(string str)
    {
        if (str.Equals("NoData"))
        {
            ///< 上传存档
            SunmCallAndroid.UnityToAndroid("uploadArchive", str);
            ///< 跳转至开始场景
            DestoyDataAndSwitchScene();
        }
        else
        {
            ///< 显示进度条
            transparenBackground.SetActive(true);
            try
            {
                if (null != ScreenShot.CaptureCamera(SunmGameInit.archiveingPath, SunmGameInit.archiveingName + str))
                {
                    ///< 上传存档
                    SunmCallAndroid.UnityToAndroid("uploadArchive", str);
                    ///< 消失进度条
                    transparenBackground.SetActive(false);
                    ///< 跳转至开始场景
                    DestoyDataAndSwitchScene();
                }

            }
            catch (Exception)
            {
                ///< 消失进度条
                transparenBackground.SetActive(false);
                ///< 跳转至开始场景
                DestoyDataAndSwitchScene();
            }
        }

        exitBoxTexture.SetActive(false);
    }

    /// <summary>
    /// 恢复弹窗状态
    /// </summary>
    void ReSpace()
    {
        modelPopupWindow.SetActive(false);
        exitBoxTexture.SetActive(false);
        savePopup.SetActive(false);
        setControl.SetActive(false);
    }
}
