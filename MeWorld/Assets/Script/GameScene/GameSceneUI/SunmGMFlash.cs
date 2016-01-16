using UnityEngine;
using System.Collections;

/// <summary>
/// 材质选择框高亮提示处理
/// </summary>
public class SunmGMFlash : MonoBehaviour {
    public GameObject[] flash = new GameObject[4];
    private float flashTime = 0f;
    private bool flag = false ;
    private int btnid;
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
       if(flag)
       {
           flashTime += Time.deltaTime;
           if(flashTime >=0.2f)
           {
               flash[btnid].SetActive(false);
               flashTime = 0f;
               flag = false;
           }
       }
	}
    public void FlashTextue(int id) 
    {
        flash[id].SetActive(true);
        flag = true;
        btnid = id;
    }
   
}
