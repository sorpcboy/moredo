using UnityEngine;
using System.Collections;

public class SunmGMMaterialButton : MonoBehaviour
{
    /// <summary>
    /// 该值只需要底部材质按钮设置即可
    /// </summary>
    public SunmGMMaterialChange.MATARIA_BUTTON mCurrentId;

    /// <summary>
    /// 而该值只需要实例出来的材质弹窗设置即可
    /// </summary>
    public byte prefabID;
    
    void OnClick()
    {
        ///< 材质菜单点击事件
        if (mCurrentId != SunmGMMaterialChange.MATARIA_BUTTON.OTHER)
        {
           // Debug.LogError("c:" + mCurrentId);
            SunmGMMaterialChange.getInstance().setBGImg(mCurrentId);
            SunmConstant.setMaterialID = SunmGMMaterialChange.getInstance().getMaterialId();
        }
        ///< 其他材质点击事件
        else
        {
            int buttonId;
            if (-1 != (buttonId = SunmGMMaterialChange.getInstance().bHasExsitMaterial(prefabID)))
            {
                GameObject.Find("Flash").GetComponent<SunmGMFlash>().FlashTextue(buttonId);
            }
            else
            {
                SunmGMMaterialChange.getInstance().setCurSpritImg(prefabID);
                SunmConstant.setMaterialID = SunmGMMaterialChange.getInstance().getMaterialId();
            }
        }
    }
}
