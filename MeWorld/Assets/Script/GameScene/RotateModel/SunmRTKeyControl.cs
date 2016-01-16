using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class SunmRTKeyControl : MonoBehaviour
{
    private Texture2D down;
    private Texture2D left;
    private Texture2D middle;
    private Texture2D right;
    private Texture2D up;

    public Texture2D downEx;
    public Texture2D leftEx;
    public Texture2D middleEx;
    public Texture2D rightEx;
    public Texture2D upEx;
    public Texture2D caozuoEx;

    public Texture2D downPressed;
    public Texture2D leftPressed;
    public Texture2D middlePressed;
    public Texture2D rightPressed;
    public Texture2D upPressed;

    ///// <summary>
    ///// 记录是何种运动方式，这样直接决定了运动过程中小人的动作
    ///// </summary>
    //public enum MOVETYPE
    //{
    //    MVLEFT, MVRIGHT
    //};
    //public MOVETYPE whichMoveType;

    private static Rect downRec = new Rect();
    private static Rect leftRec = new Rect();
    private static Rect middleRec = new Rect();
    private static Rect rightRec = new Rect();
    private static Rect upRec = new Rect();
    private static Rect caozuoRec = new Rect();
    private static Rect keyRec = new Rect();

    //private float scaleX = Screen.width/1280;
    //private float scaleY = Screen.height/800;
    private float scaleX = 1f;
    private float scaleY = 1f;
    private float keyWidth;
    private float keyHeight;

    public float speed = 0.02f;

    /// <summary>
    /// 将要移动的物体【获得控制器】
    /// </summary>
    //private CharacterController charMotorControl = null;
    //private CharacterMotor charMotor = null;
    private Vector3 directionVector = Vector3.zero;
    private float directionLength = 0;
    /// <summary>
    /// 是否是移动状态，提供给人物动作使用，如果是移动状态则开始做摆手摆脚动作.
    /// </summary>
    public static bool bIsMoving = false;

    ///< 当前哪个矩形区域
    private enum Orientation
    {
        FRONT_RC, BACK_RC, LEFT_RC, RIGHT_RC, CENTER_RC
    }

    /// <summary>
    /// 运动种类
    /// </summary>
    private enum MoveWhere
    {
        STOP, Forward, Backward, TurnLeft, TurnRight
    }

    /// <summary>
    /// 抬手和处理
    /// </summary>
    private bool bIsKeepOn = true;
    private MoveWhere DERECTION;
    private Vector2 m_screenpos;
    private Thread thread = null;

    private int id = 0;

    ///// <summary>
    ///// 计算左右、上下滑动事件
    ///// </summary>
    private Vector2 touchfirst = Vector2.zero;  // 手指开始按下的位置
    private Vector2 touchsecond = Vector2.zero; // 手指拖动的位置
    private float offDistance = 1.5f;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;

    /// <summary>
    /// GUI适配
    /// </summary>
    private Vector2 NativeResolution = new Vector2(480, 320);
    private float _guiScaleFactor = -1.0f;
    private Vector3 _offset = Vector3.zero;
    private bool _didResizeUI;
    private List<Matrix4x4> stack = new List<Matrix4x4>();

    /// <summary>
    /// 供场景切换的动画使用
    /// </summary>
    public static bool DrawTextureBool = true;

    public void BeginUIResizing()
    {
        Vector2 nativeSize = NativeResolution;

        _didResizeUI = true;

        stack.Add(GUI.matrix);
        Matrix4x4 m = new Matrix4x4();
        var w = (float)Screen.width;
        var h = (float)Screen.height;
        var aspect = w / h;
        var offset = Vector3.zero;
        if (aspect < (nativeSize.x / nativeSize.y))
        {
            //screen is taller
            _guiScaleFactor = (Screen.width / nativeSize.x);
            offset.y += (Screen.height - (nativeSize.y * _guiScaleFactor)) * 0.5f;
        }
        else
        {
            // screen is wider
            _guiScaleFactor = (Screen.height / nativeSize.y);
            offset.x += (Screen.width - (nativeSize.x * _guiScaleFactor)) * 0.5f;
        }

        m.SetTRS(offset, Quaternion.identity, Vector3.one * _guiScaleFactor);
        GUI.matrix *= m;
    }

    public void EndUIResizing()
    {
        GUI.matrix = stack[stack.Count - 1];
        stack.RemoveAt(stack.Count - 1);
        _didResizeUI = false;
    }

    // Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

        }
        if (Application.platform == RuntimePlatform.Android)
        {
            keyWidth = Screen.height * 16 / 100;
            keyHeight = Screen.height * 16 / 100;
        }
        else
        {
            keyWidth = 128f;
            keyHeight = 128f;
        }

        ///< 保存按钮恢复状态纹理
        up = upEx;
        down = downEx;
        left = leftEx;
        right = rightEx;
        middle = middleEx;

        ///< 初始化绘制区域【根据屏幕宽高动态计算】
        initKeyRec();
        ///< 动作初始化
        DERECTION = MoveWhere.STOP;
    }

    // Update is called once per frame
    void Update()
    {
        // 判断当前触摸屏幕的手指个数 该函数可自定义 然后在Update()里面调用 也可以直接在Update()里面写
        if (Input.touchCount <= 0) { return; }

        int pointCount = Input.touchCount;

        //ti4p = "" + Input.touches[0].phase + " id " + id;

        m_screenpos = Input.touches[0].position;

        if (Input.touchCount == 1) //一个手指触摸屏幕
        {
            // 自定义的二维坐标向量 记录初始触屏位置
            
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:  ///< 开始按钮
                    {
                        bIsKeepOn = true;
                        if (null != thread && thread.IsAlive)
                        {
                            thread.Abort();
                            thread = null;
                        }
                        thread = new Thread(KeyWorker);
                        thread.Start();     
                        ///< 如果前进方向键被选中
                        if (true == insideRect(m_screenpos.x, Screen.height - m_screenpos.y, upRec))
                        {
                            ///< 向前
                            DERECTION = MoveWhere.Forward;
                            up = upPressed;

                            bIsMoving = true;
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;                          
                        }
                        ///< 如果后退方向键被选中
                        else if (true == insideRect(m_screenpos.x, Screen.height - m_screenpos.y, downRec))
                        {
                            ///< 向后
                            DERECTION = MoveWhere.Backward;
                            down = downPressed;

                            bIsMoving = true;
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;
                        }
                        ///< 如果向左方向键被选中
                        else if (true == insideRect(m_screenpos.x, Screen.height - m_screenpos.y, leftRec))
                        {
                            ///< 向左
                            DERECTION = MoveWhere.TurnLeft;
                            left = leftPressed;
                            //GUI.DrawTexture(leftRec, leftPressed);
                            //ti2p = ti2p + "向左键被选中";

                            // 摆手
                            bIsMoving = true;
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;

                            //transform.Translate(Vector3.left * speed * Time.deltaTime);
                        }
                        ///< 如果向右方向键被选中
                        else if (true == insideRect(m_screenpos.x, Screen.height - m_screenpos.y, rightRec))
                        {
                            ///< 向右
                            DERECTION = MoveWhere.TurnRight;
                            right = rightPressed;
                            //GUI.DrawTexture(rightRec, rightPressed);
                            //ti2p = ti2p + "向右键被选中";

                            // 摆手
                            bIsMoving = true;
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;

                            //transform.Translate(Vector3.right * speed * Time.deltaTime);
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        ///< 如果已经滑动到按钮区域外，则恢复状态
                        if (outsideKeyRec(m_screenpos.x, Screen.height - m_screenpos.y))
                        {
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = false;
                            ///< 屏蔽累方块恢复
                            gameObject.GetComponent<SunmRTGesture>().bISAddCube = true;
                            // ti3p = "划出屏幕外"+
                        }
                        else
                        {
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;
                            gameObject.GetComponent<SunmRTGesture>().bISAddCube = false;
                        }
                   
                        ///< 按钮释放，滑动继续，后面单独做处理
                        if ((false == bIsKeepOn && 1 == pointCount))
                        {
                            break;
                        }
              
                        ///< 如果已经滑动到按钮区域外，则恢复状态
                        if (true == outsideButtonRec(m_screenpos.x, Screen.height - m_screenpos.y))
                        {
                            /////< 手抬起后中间键功能取消
                            //bIsUp = false;
                            ///< 手抬起停止方向动作
                            bIsKeepOn = false;
                            ///< 恢复初始状态
                            DERECTION = MoveWhere.STOP;
                            ///< 按钮均恢复背景
                            up = upEx;
                            down = downEx;
                            left = leftEx;
                            right = rightEx;
                            middle = middleEx;
                            //ti2p = "划出屏幕外面";
                            //charMotor.inputJump = false;
                            bIsMoving = false;
                            gameObject.GetComponent<SunmRTGesture>().bIsKeying = false;
                            gameObject.GetComponent<SunmRTGesture>().bISAddCube = true;
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    {    
                        //// 手抬起后中间键功能取消
                        //bIsUp = false;
                        // 恢复初始状态
                        DERECTION = MoveWhere.STOP;
                        // 手抬起停止方向动作
                        bIsKeepOn = false;

                        up = upEx;
                        down = downEx;
                        left = leftEx;
                        right = rightEx;
                        middle = middleEx;
                       
                        bIsMoving = false;
                        gameObject.GetComponent<SunmRTGesture>().bIsKeying = false;
                        gameObject.GetComponent<SunmRTGesture>().bISAddCube = true;   
                    }
                    break;
            }
        }

        ///< 按钮持续事件消失，只剩下触屏事件
        if ((false == bIsKeepOn && 1 == pointCount))
        {
            ///< 操作圈矩形区域不做处理
            if (true == insideButtonFunctionRec(Input.touches[0].position))
            {
                gameObject.GetComponent<SunmRTGesture>().bIsKeying = true;
                gameObject.GetComponent<SunmRTGesture>().bISAddCube = false; 
                return;
            }

            gameObject.GetComponent<SunmRTGesture>().bIsKeying = false;
            gameObject.GetComponent<SunmRTGesture>().bISAddCube = true;
        }
    }

    /// <summary>
    /// 处理移动事件【做了个计算】？？？
    /// </summary>
    /// <param name="_directionVector"></param>
    /// <returns></returns>
    private Vector3 handleMove(Vector3 _directionVector)
    {
        if (_directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            var directionLength = _directionVector.magnitude;
            _directionVector = _directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            _directionVector = _directionVector * directionLength;

            return _directionVector;
        }

        return Vector3.zero;
    }

    void OnGUI()
    {
        //BeginUIResizing();                  //call this in the beginning of method
        if (DrawTextureBool == true)
        {
            //GUI.DrawTexture(caozuoRec, caozuoEx);
            GUI.DrawTexture(upRec, up);
            GUI.DrawTexture(downRec, down);
            GUI.DrawTexture(leftRec, left);
            GUI.DrawTexture(rightRec, right);
            //GUI.DrawTexture(middleRec, middle);
        }
    }

    /// <summary>
    /// 初始化方向键的位置和大小
    /// </summary>
    private void initKeyRec()
    {


        // down
        downRec.Set((keyWidth / scaleX)*0.8f, (Screen.height - keyHeight / scaleY), keyWidth / scaleX, keyHeight / scaleY);
        // up
        upRec.Set((keyWidth / scaleX)*0.8f, (Screen.height - (keyHeight / scaleY) * 2.4f), keyWidth / scaleX, keyHeight / scaleY);
        // left
        leftRec.Set(0f / scaleX, (Screen.height - (keyHeight / scaleY) * 1.7f), keyWidth / scaleX, keyHeight / scaleY);
        // right
        rightRec.Set((keyWidth / scaleX) * 1.6f, (Screen.height - (keyHeight / scaleY) * 1.7f), keyWidth / scaleX, keyHeight / scaleY);
        // middle
        //       middleRec.Set(keyWidth / scaleX, (Screen.height - (keyHeight / scaleY) * 2), keyWidth / scaleX, keyHeight / scaleY);
       // middleRec.Set((keyWidth / scaleX) * 0.95f, (Screen.height - (keyHeight / scaleY) * 2.05f), keyWidth / scaleX * 1.1f, keyHeight / scaleY * 1.1f);
        // caozuo
        //       caozuoRec.Set(0f / scaleX, (Screen.height - (keyHeight / scaleY) * 3), (keyWidth / scaleX) * 3, (keyHeight / scaleY) * 3);
      //  caozuoRec.Set((keyWidth / scaleX) * 0.25f, (Screen.height - (keyHeight / scaleY) * 2.75f), (keyWidth / scaleX) * 2.5f, (keyHeight / scaleY) * 2.5f);
        // keyrec
        keyRec.Set(0, (Screen.height / 2), (keyWidth / scaleX) * 2.6f, Screen.height / 2);


    }

    /// <summary>
    /// 判断一个点是否在指定矩形范围内
    /// </summary>
    /// <param name="pointX"></param>
    /// <param name="pointY"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    private static bool insideRect(float pointX, float pointY, Rect rect)
    {
        if (pointX > rect.xMin && pointX < rect.xMax &&
            pointY > rect.yMin && pointY < rect.yMax)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否属于按钮区域
    /// </summary>
    /// <param name="_m_screenpos"></param>
    /// <returns></returns>
    public static bool insideButtonRec(Vector2 _m_screenpos)
    {
        return !outsideButtonRec(_m_screenpos.x, Screen.height - _m_screenpos.y);
    }

    /// <summary>
    /// 判断是否属于按钮及左下区域部分
    /// </summary>
    /// <param name="_m_screenpos"></param>
    /// <returns></returns>
    public static bool insideButtonFunctionRec(Vector2 _m_screenpos)
    {
        return insideRect(_m_screenpos.x, Screen.height - _m_screenpos.y, keyRec);
    }

    /// <summary>
    /// 判断是否属于操作圈区域
    /// </summary>
    /// <param name="_m_screenpos"></param>
    /// <returns></returns>
    public static bool insideCaozuoRec(Vector2 _m_screenpos)
    {
        ///< 操作圈
        //return insideRect(_m_screenpos.x, Screen.height - _m_screenpos.y, caozuoRec);
        return insideRect(_m_screenpos.x, Screen.height - _m_screenpos.y, keyRec);
    }

    /**
     * 是否在按钮区域【十字架区域】，滑出该区域，则不再响应前后左右等方向事件
     * @param pointX
     * @param pointY
     * @return
     */
    public static bool outsideButtonRec(float pointX, float pointY)
    {
        ///< 属于向前退按钮区域
        if (true == insideRect(pointX, pointY, upRec))
        {
            return false;
        }
        ///< 属于向后退按钮区域
        else if (true == insideRect(pointX, pointY, downRec))
        {
            return false;
        }
        ///< 属于向左按钮区域
        else if (true == insideRect(pointX, pointY, leftRec))
        {
            return false;
        }
        ///< 属于向右按钮区域
        else if (true == insideRect(pointX, pointY, rightRec))
        {
            return false;
        }
        ///< 属于功能键区域
        else if (true == insideRect(pointX, pointY, middleRec))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 是否超出按钮区域
    /// </summary>
    /// <param name="pointX"></param>
    /// <param name="pointY"></param>
    /// <returns></returns>
    private static bool outsideKeyRec(float pointX, float pointY)
    {
        return !insideRect(pointX, pointY, keyRec);
    }

    /// <summary>
    /// 方向键线程
    /// </summary>
    //public class KeyWorker
    //{
    // This method will be called when the thread is started.
    public void KeyWorker()
    {
        // TODO Auto-generated method stub
        while (bIsKeepOn)
        {
            switch (DERECTION)
            {
                case MoveWhere.Backward:
                    {
                        SunmRTGesture.rotateCenter -= transform.rotation * (Vector3.up * speed * Time.deltaTime);
                        SunmRTGesture.initPosition -= transform.rotation * (Vector3.up * speed * Time.deltaTime);
                        transform.position -= transform.rotation * (Vector3.up * speed * Time.deltaTime);
                    }
                    break;
                case MoveWhere.Forward:
                    {                     
                        SunmRTGesture.rotateCenter -= transform.rotation * (Vector3.down * speed * Time.deltaTime);
                        SunmRTGesture.initPosition -= transform.rotation * (Vector3.down * speed * Time.deltaTime);
                        transform.position -= transform.rotation * (Vector3.down * speed * Time.deltaTime);
                    }
                    break;
                case MoveWhere.TurnRight:
                    {
                        SunmRTGesture.rotateCenter -= transform.rotation * (Vector3.left * speed * Time.deltaTime);
                        SunmRTGesture.initPosition -= transform.rotation * (Vector3.left * speed * Time.deltaTime);
                        transform.position -= transform.rotation * (Vector3.left * speed * Time.deltaTime);
                    }
                    break;
                case MoveWhere.TurnLeft:
                    {
                        SunmRTGesture.rotateCenter -= transform.rotation * (Vector3.right * speed * Time.deltaTime);
                        SunmRTGesture.initPosition -= transform.rotation * (Vector3.right * speed * Time.deltaTime);
                        transform.position -= transform.rotation * (Vector3.right * speed * Time.deltaTime);
                    }
                    break;           
                default:
                    break;
            }
            Thread.Sleep(1);
        }
    }
}

