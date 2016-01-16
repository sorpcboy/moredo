using UnityEngine;
using System.Collections;

public class SunmRMGesture : MonoBehaviour
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

	///< 鼠标按下时的位置
    private Vector2 downPosition;
    ///< 鼠标拖动的位置
    private Vector2 dragPosition;
    ///< BLOCK-DESTRUCT
    private Vector3 blockTemp;
	private Vector3 blockCreateTemp;
	private Vector3 allTemp;
	private Vector3 finalTemp;
	private RaycastHit hit;
	private Ray ray;
    /// <summary>
    /// 方块累加的bool值 ， 用于方向键判断
    /// </summary>
    public bool bISAddCube = true;
	/// <summary>
	/// 拖拽时调用
	/// </summary>
	/// <param name="gesture"></param>
    void OnDrag(DragGesture gesture)
	{
        ///< UI层不做处理
        if (null != UICamera.hoveredObject)
        {
            gameObject.GetComponent<TBDragView>().enabled = false;
            //return;
        }
        else
        {
            gameObject.GetComponent<TBDragView>().enabled = true;
        }

        ///<判断手指的数量与旋转区分开，如果大于两个手指则不旋转
        if (fingerNumber >= 1)
		{
			return;
		}

        ///<拖动的位置
        dragPosition = gesture.Position;
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
	}

	/// <summary>
	/// 按下时调用 
	/// </summary>
	/// <param name="gesture"></param>
    void OnFingerDown(FingerDownEvent gesture)
	{
		fingerNumber = gesture.Finger.Index;
		downPosition = gesture.Finger.StartPosition;
		
		///< 判断是否是键盘区域，键盘区域不能垒方块
        if (false == SunmRMKeyControl.outsideButtonRec(gesture.Position.x, Screen.height - gesture.Position.y)
         || true == SunmRMKeyControl.insideButtonFunctionRec(gesture.Position))
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
