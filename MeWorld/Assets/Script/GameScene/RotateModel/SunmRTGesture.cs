using UnityEngine;
using System.Collections;

public class SunmRTGesture : MonoBehaviour
{
    /// <summary>
    /// 标记是否是长按，与点击一次区分开
    /// </summary>
    private bool bIsLongPress = false;
    /// <summary>
    /// 记录手指的数量，与旋转区分开
    /// </summary>
    private int fingerNumber;
    /// <summary>
    /// 标记是否是滑动，与累加方块区分开
    /// </summary>
    private bool bIsMovePress = false;
    /// <summary>
    /// 开始拖动的位置
    /// </summary>
    private Vector3 startPoint3;
    /// <summary>
    /// 拖动的位置
    /// </summary>
    private Vector3 dragPoint3;
    /// <summary>
    /// 当前四元数
    /// </summary>
    private Quaternion currentRotation;
    /// <summary>
    /// 相机初始位置
    /// </summary>
    public static Vector3 initPosition;
    /// <summary>
    /// 相机绕这个点旋转
    /// </summary>
    public static Vector3 rotateCenter;
    /// <summary>
    /// 鼠标按下时的位置
    /// </summary>
    private Vector2 downPosition;
    /// <summary>
    /// 鼠标拖动的位置
    /// </summary>
    private Vector2 dragPosition;
    ///< BLOCK-DESTRUCT
    private Vector3 blockTemp;
    private Vector3 blockCreateTemp;
    private Vector3 blockNormalTemp;
    private Vector3 ppTemp;
    private Vector3 allTemp;
    private Vector3 finalTemp;
    private RaycastHit hit;
    private Ray ray;

    /// <summary>
    /// target用于绑定游戏体作为摄像机的参照物
    /// </summary>
    //private Transform target;
    //public static Transform targetEx = null;
    /// <summary>
    /// 缩放系数
    /// </summary>
    private float distance = 15.0f;
    private float z = 0;
    /// <summary>
    /// 缩放限制数
    /// </summary>
    private float yMinLimit = 3f;
    private float yMaxLimit = 30f;
    /// <summary>
    /// 记录上次触摸位置判断是在做放大还是缩小的手势
    /// </summary>
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;
    /// <summary>
    /// 如果是操作方向键，则不能旋转
    /// </summary>
    public bool bIsKeying = false;
    /// <summary>
    /// 方块累加的bool值 ， 用于方向键判断
    /// </summary>
    public bool bISAddCube = true;
    /// <summary>
    /// 旋转速度控制变量 
    /// </summary>
    public float CNCRotation = 1.0f;

    void Update()
    {
        if (Input.touchCount > 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                // 计算出当前两点触摸的位置
                Vector2 tempPosition1 = Input.GetTouch(0).position;
                Vector2 tempPosition2 = Input.GetTouch(1).position;
                // 函数返回值真为放大，假为缩小
                if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                {
                    // 放大系数超过3以后不允许继续放大
                    if (distance > yMinLimit)
                    {
                        //    distance -= 0.5f;
                        //initPosition.z = initPosition.z + 10f * Time.deltaTime;
                        //z += 10.0f*Time.deltaTime;
                        moveinitPositionZ(10.0f * Time.deltaTime);
                        //transform.position = currentRotation * (initPosition - rotateCenter) + rotateCenter;//当前相机所在位置//
                    }
                }
                else
                {
                    // 缩小系数返回18.5后不允许继续缩小
                    if (distance < yMaxLimit)
                    {
                        //z += 10.0f*Time.deltaTime;
                        moveinitPositionZ(-10.0f * Time.deltaTime);
                        // distance += 0.5f;
                        //initPosition.z = initPosition.z - 10f * Time.deltaTime;
                        //transform.position = currentRotation * (initPosition - rotateCenter) + rotateCenter;//当前相机所在位置//
                    }
                }
                // 备份上一次触摸点的位置，用于对比
                oldPosition1 = tempPosition1;
                oldPosition2 = tempPosition2;
            }
        }
    }

    /// <summary>
    /// 拖拽时调用
    /// </summary>
    /// <param name="gesture"></param>
    void Start()
    {
        rotateCenter = new Vector3(100, 100, 100); //< 旋转中心点//
        //transform.LookAt (rotateCenter);
        initPosition = transform.position;
        currentRotation = transform.rotation;
    }

    /// <summary>
    /// 外部调用【重新开始、重置摄像机】
    /// </summary>
    public void resetPosition()
    {
        initPosition = transform.position;
        currentRotation = transform.rotation;
        rotateCenter = new Vector3(100f, 100f, 100f); //< 旋转中心点//
    }

    public void moveinitPositionZ(float z)
    {
        initPosition.z += z;
        //rotateCenter += _mov;
        transform.position = transform.rotation * (initPosition - rotateCenter) + rotateCenter;//当前相机所在位置//
    }
    //public static void moveCenter( Vector3 _mov )
    //{

    //}
    /// <summary>
    /// 重新设置旋转中心点
    /// </summary>
    /// <param name="_center"></param>
    /// 
    //public static void changeCenter(Vector3 _center)
    //{
    //    rotateCenter = rotateCenter + _center;
    //}

    void OnDrag(DragGesture gesture)
    {
        ///< UI层不做处理
        if (null != UICamera.hoveredObject)
        {
            return;
        }

        ///< 判断手指的数量与旋转区分开，如果大于两个手指则不旋转
        if (fingerNumber >= 1)
        {
            return;
        }

        //< 如果正在操作按键，则不处理//
        if (SunmRTKeyControl.bIsMoving || bIsKeying)
        {
            return;
        }

        //< 拖动的位置//
        dragPosition = gesture.Position;
        dragPoint3 = map2Sphere(dragPosition);
        float theta = Mathf.Acos(Mathf.Min(1.0f, Vector3.Dot(startPoint3, dragPoint3))) * CNCRotation;
        Vector3 axis = Vector3.Cross(startPoint3, dragPoint3).normalized;
        Quaternion rot = Quaternion.Inverse(Quaternion.AngleAxis(theta * 180.0f / Mathf.PI, axis));//当前拖动得到的四元数//
        currentRotation = currentRotation * rot;//当前相机的四元数//
        transform.position = currentRotation * (initPosition - rotateCenter) + rotateCenter;//当前相机所在位置//
        transform.rotation = currentRotation;
        //transform.LookAt (rotateCenter);
        if ((downPosition.x - dragPosition.x) > SunmConstant.offDistance)
        {
            //           Debug.LogError("left");
            bIsMovePress = true;
        }

        if ((dragPosition.x - downPosition.x) > SunmConstant.offDistance)
        {
            //            Debug.Log("right");
            bIsMovePress = true;
        }

        if ((dragPosition.y - downPosition.y) > SunmConstant.offDistance)
        {
            //           Debug.Log("up");
            bIsMovePress = true;
        }

        if ((downPosition.y - dragPosition.y) > SunmConstant.offDistance)
        {
            //            Debug.LogError("down");
            bIsMovePress = true;
        }

        downPosition = dragPosition;
        startPoint3 = dragPoint3;
    }

    /// <summary>
    /// 屏幕上的点映射到圆球上
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <returns></returns>
    Vector3 map2Sphere(Vector2 screenPoint)
    {
        float x = 1.0f - 2.0f * screenPoint.x / Screen.width;
        float y = 1.0f - 2.0f * screenPoint.y / Screen.height;
        float length = x * x + y * y;
        if (length <= 1.0f)
        {
            return new Vector3(x, y, Mathf.Sqrt(1.0f - length));
        }
        else
        {
            return (new Vector3(x, y, 0.0f)).normalized;
        }
    }

    /// <summary>
    /// 计算双指放大还是缩放
    /// </summary>
    /// <param name="oP1"></param>
    /// <param name="oP2"></param>
    /// <param name="nP1"></param>
    /// <param name="nP2"></param>
    /// <returns></returns>
    private static bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        // 函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            // 放大手势
            return true;
        }
        else
        {
            // 缩小手势
            return false;
        }
    }

    /// <summary>
    /// 按下时调用 
    /// </summary>
    /// <param name="gesture"></param>
    void OnFingerDown(FingerDownEvent gesture)
    {
        fingerNumber = gesture.Finger.Index;
        //Debug.Log("鼠标按下的点："+gesture.Finger.StartPosition);
        downPosition = gesture.Finger.StartPosition;
        startPoint3 = map2Sphere(downPosition);
		
		///< 判断是否是键盘区域，键盘区域不能垒方块
        if (false == SunmRTKeyControl.outsideButtonRec(gesture.Position.x, Screen.height - gesture.Position.y)
            || true == SunmRTKeyControl.insideButtonFunctionRec(gesture.Position)) 
        {
            bISAddCube = false;
        }
        
    }

    /// <summary>
    /// 抬起时调用 - 累方块和长按消方块需要区分、估计还有其它
    /// </summary>
    /// <param name="fingerIndex"></param>
    /// <param name="fingerPos"></param>
    /// <param name="timeHeldDown"></param>
    void OnFingerUp(FingerUpEvent gesture)
    {
        ///< UI层不做处理
        if (null != UICamera.hoveredObject)
        {
            return;
        }

        ///< 判断手指的数量与旋转区分开,当旋转的时候不做方块的累加
        if (fingerNumber >= 1)
        {
            return;
        }

        ///< 表示不是长按，表示点击，累加方块
        if (!bIsLongPress && !bIsMovePress && bISAddCube)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Vector3.Distance(hit.transform.position, transform.position) < SunmConstant.RayMaxLimit &&
                    Vector3.Distance(hit.transform.position, transform.position) > SunmConstant.RayMinLimit)
                {
                    ///< 计算将要累的方块的位置
                    blockTemp = hit.collider.transform.position;
                    finalTemp = blockTemp + hit.normal.normalized;

                    ///< 如果一旦占据，则直接返回
                    if (SunmGameInit.bOcupperThreeArray(finalTemp))
                    {
                        return;
                    }

                    ///< 设置占据位置
                    SunmGameInit.setThreeArray(finalTemp, SunmConstant.setMaterialID);
                    SunmCubeOpt.AddCube(finalTemp, SunmConstant.rootCube.transform.rotation, SunmConstant.setMaterialID);
                    MusicPlayer.Play(MusicPlayer.WHICH_SOUND.PUTCUBE);
                    gameObject.SendMessage("ArchiveMessageProcessing", new object[] { SunmConstant.setMaterialID, finalTemp });   ///< 给自己发送消息
                }
            }
        }

        ///< 抬手，标志位复位
        bIsLongPress = false;
        bIsMovePress = false;
    }

    /// <summary>
    /// 长按事件
    /// </summary>
    /// <param name="fingerIndex"></param>
    /// <param name="fingerPos"></param>
    void OnLongPress(LongPressGesture gesture)
    {
        bIsLongPress = true;

        ///< UI层不做处理
        if (null != UICamera.hoveredObject)
        {
            return;
        }

        ///< 滑动过程中也不能删除
        if (bIsMovePress)
        {
            return;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.transform.position, transform.position) < SunmConstant.RayMaxLimit &&
                Vector3.Distance(hit.transform.position, transform.position) > SunmConstant.RayMinLimit)
            {
                if (hit.collider.gameObject.name.Equals("(100.0, 100.0, 100.0)"))
                {
                    return;
                }
                if (!bISAddCube)
                {
                    return;
                }

                SunmGameInit.setThreeArray(hit.collider.transform.position, 0);
                SunmCubeOpt.RemoveCube(hit.collider.gameObject);
                ///< 销毁放开，播放粒子方块
                Instantiate(Resources.Load("Prefab/Model/Effect/CoinCollectFlash"), hit.transform.position, SunmConstant.rootCube.transform.rotation);
                MusicPlayer.Play(MusicPlayer.WHICH_SOUND.BREAKCUBE);
                gameObject.SendMessage("ArchiveMessageProcessing", new object[] { hit.collider.transform.position });   ///< 给自己发送消息
            }
        }
    }
}
