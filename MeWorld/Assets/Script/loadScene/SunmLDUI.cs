using UnityEngine;
using System.Collections;
/// <summary>
/// 控制进度条显示
/// </summary>
public class SunmLDUI : MonoBehaviour
{
	void Update ()
    {
        if (transform.localPosition.x<900f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + 200f * Time.deltaTime, -173f, 0f);
        }
        else 
        {
            Application.LoadLevel("GameScene");
        }
	}
}
