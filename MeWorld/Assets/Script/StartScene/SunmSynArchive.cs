using UnityEngine;
using System.Collections;

/// <summary>
/// 同步按钮处理【本地存档】
/// </summary>
public class SunmSynArchive : MonoBehaviour 
{
    /// <summary>
    /// 预设体父物
    /// </summary>
    public GameObject ObjetPrefab;

	// Use this for initialization
	void Start () 
    {
	    
	}

    void OnClick()
    {
        if (gameObject.name.Equals("SynButton")) 
        {
            ObjetPrefab.SendMessage("LocalArchiveSynchronous");
        }
    }
	// Update is called once per frame
	void Update () 
    {
        if (gameObject.name.Equals("SynLoding")) 
        {
            transform.Rotate(0, 0, -500 * Time.deltaTime);
        }    
	}
}
