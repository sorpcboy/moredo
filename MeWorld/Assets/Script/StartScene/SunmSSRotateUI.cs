using UnityEngine;
using System.Collections;

/// <summary>
/// 模式选择UI旋转脚本
/// </summary>
public class SunmSSRotateUI : MonoBehaviour
{
    /// <summary>
    /// ModelPanel界面普通模式按钮
    /// </summary>
    public GameObject normalRotation;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.transform.localEulerAngles = new Vector3(0f, 0f, transform.transform.localEulerAngles.z - 50f * Time.deltaTime);
        normalRotation.transform.localEulerAngles = new Vector3(0f, 0f, normalRotation.transform.localEulerAngles.z -50f * Time.deltaTime);
	}
}
