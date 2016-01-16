using UnityEngine;
using System.Collections;

public class SunmGMUITrueOrFalse : MonoBehaviour 
{
    /// <summary>
    /// 控制显示隐藏按钮的精灵图替换
    /// </summary>
	void Update () 
    {
        if(transform.localPosition.x == 251f)
        {
            transform.GetComponent<UIButton>().normalSprite = "off";
            transform.GetComponent<UIButton>().pressedSprite = "off_pressed";
        }
        if (transform.localPosition.x == -143f)
        {
            transform.GetComponent<UIButton>().normalSprite = "on";
            transform.GetComponent<UIButton>().pressedSprite = "on_pressed"; 
        }      
    }
}
