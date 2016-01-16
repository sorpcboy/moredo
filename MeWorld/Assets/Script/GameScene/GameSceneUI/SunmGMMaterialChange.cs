using UnityEngine;
using System.Collections;

public class SunmGMMaterialChange 
{
    /// <summary>
    /// 底部材质按钮
    /// </summary>
    private GameObject[] aa = new GameObject[4];
    /// <summary>
    /// 分别标识四个材质按钮对应的材质id号
    /// </summary>
    private byte[] aaFlag = new byte[4];
    private MATARIA_BUTTON currentId = MATARIA_BUTTON.ONE;
    /// <summary>
    /// 按钮实例对象 - 单例模式
    /// </summary>
    private static SunmGMMaterialChange instance = null;

    public enum MATARIA_BUTTON
    {
        ONE = 0, TWO, THREE, FOUR, OTHER
    }

    public SunmGMMaterialChange()
    {
        aa[0] = GameObject.Find("MaterialChooseOneButtonOne").transform.FindChild("Material").gameObject;
        aa[1] = GameObject.Find("MaterialChooseTwoButtonTwo").transform.FindChild("Material").gameObject;
        aa[2] = GameObject.Find("MaterialChooseThreeButtonThree").transform.FindChild("Material").gameObject;
        aa[3] = GameObject.Find("MaterialChooseFourButtonFour").transform.FindChild("Material").gameObject;
        aaFlag[0] = 8;
        aaFlag[1] = 20;
        aaFlag[2] = 32;
        aaFlag[3] = 2;
    }

    public static SunmGMMaterialChange getInstance()
    {
        if (null == instance)
        {
            instance = new SunmGMMaterialChange();
        }

        return instance;
    }

    private void resetBGImg()
    {
        aa[0].transform.parent.GetComponent<UIButton>().normalSprite = "material";
        aa[1].transform.parent.GetComponent<UIButton>().normalSprite = "material";
        aa[2].transform.parent.GetComponent<UIButton>().normalSprite = "material";
        aa[3].transform.parent.GetComponent<UIButton>().normalSprite = "material";
    }

    /// <summary>
    /// 设置选中状态，同时设置ID号
    /// </summary>
    /// <param name="_Id"></param>
    public void setBGImg(MATARIA_BUTTON _Id)
    {
        resetBGImg();
        aa[(int)_Id].transform.parent.GetComponent<UIButton>().normalSprite = "material_pressed";
        currentId = _Id;
    }

    /// <summary>
    /// 根据当前选中的按钮返回对应的材质id号
    /// </summary>
    /// <returns></returns>
    public byte getMaterialId()
    {
        return aaFlag[(int)currentId];
    }

    /// <summary>
    /// 设置预览材质效果图
    /// </summary>
    /// <param name="_key"></param>
    public void setCurSpritImg(byte _key)
    {
       // Debug.LogError("currentId:" + currentId);
        aa[(int)currentId].GetComponent<UISprite>().spriteName = SunmConstant.MaterialList[_key];
        ///< 记录下新的材质ID【在点击其他材质的时候会用到： 用于设置其他三个按钮材质预览图是做区分，已经存在的不需要设置】
        aaFlag[(int)currentId] = _key;
    }

    /// <summary>
    /// 检查是否已经选择过该材质，如果选择过同时返回该按钮ID号； 否则返回-1
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public int bHasExsitMaterial(byte _key)
    {
        int flag = -1;
        for (int i = 0; i < 4; ++i)
        {
            ///<  当前选中的材质按钮框不需要考虑是否会重复选择
            if (i == (int)currentId)
            {
                continue;
            }

            if (aaFlag[i] == _key)
            {
                flag = i;
                break;
            }
        }

        return flag;
    }

    /// <summary>
    ///  销毁对象
    /// </summary>
    public static void destroy() 
    {
        instance = null;
    }
}
