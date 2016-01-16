using UnityEngine;
using System.Collections;
/// <summary>
/// 上传进度
/// </summary>
public class SunmGMRotate : MonoBehaviour
{
    public GameObject rotatingSchedule;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z - 50f * Time.deltaTime);
        rotatingSchedule.transform.localEulerAngles = new Vector3(0f, 0f, rotatingSchedule.transform.localEulerAngles.z + 50f * Time.deltaTime);
	}
}
