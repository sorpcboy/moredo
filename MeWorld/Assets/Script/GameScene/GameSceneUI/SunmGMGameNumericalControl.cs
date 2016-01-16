using UnityEngine;
using System.Collections;
/// <summary>
/// 方向键、摄像机旋转等灵敏度调节
/// </summary>
public class SunmGMGameNumericalControl : MonoBehaviour 
{
    public UISlider DirectionKey;
    public UISlider FieldOfVision;
    public GameObject GameCamera;
    public float DirectionValue;
    public float FieldOfVisionValue;

    private void ControlValue()
    {
        if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE) 
        {
            DirectionValue = DirectionKey.GetComponent<UISlider>().value;
            GameCamera.GetComponent<SunmRMKeyControl>().speed = DirectionValue;

            FieldOfVisionValue = FieldOfVision.GetComponent<UISlider>().value * 400.0f;
            GameCamera.GetComponent<TBDragView>().sensitivity = FieldOfVisionValue;
        }
        else if(SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE) 
        {
            DirectionValue = DirectionKey.GetComponent<UISlider>().value;
            GameCamera.GetComponent<SunmRTKeyControl>().speed = DirectionValue;

            FieldOfVisionValue = FieldOfVision.GetComponent<UISlider>().value * 3.0f;         
            GameCamera.GetComponent<SunmRTGesture>().CNCRotation = FieldOfVisionValue;
        }
        
    }
}
