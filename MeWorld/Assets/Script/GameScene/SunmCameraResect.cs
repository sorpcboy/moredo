using UnityEngine;
using System.Collections;

public class SunmCameraResect : MonoBehaviour 
{
    /// <summary>
    /// 射线检测
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// 设定摄像机距离当前方块的初始值
    /// </summary>
    private float parameter = 5.0f;
	// Use this for initialization
	void Start () 
    {
        ///< 目标位置【这里是摄像机的前面】
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, 10))
        {
           // Debug.LogError("检测:" + hit.collider.transform.name);
            //Debug.DrawLine(transform.position, hit.point, Color.red);
            Vector3 ResectPosition = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - parameter);
            
            SunmConstant.resetCamera(ResectPosition);
        }

        Destroy(gameObject.GetComponent<SunmCameraResect>());
	}
	
	// Update is called once per frame
	void Update () 
    {
     
	}
}
