using UnityEngine;
using System.Collections;

public class SunmCubeOpt : MonoBehaviour 
{
    /// <summary>
    /// 添加方块
    /// </summary>
    /// <param name="_finalTemp"></param>
    /// <param name="_quaternion"></param>
    public static void AddCube(Vector3 _finalTemp, Quaternion _quaternion, byte _Material)
    {
        ///< 根据位置实例化方块、带旋转
        GameObject LoadModel = Resources.Load("Prefab/CubePice/PiceCub") as GameObject;
        ///< 实例化并继承父亲 - 这边先不贴材质，处理完后再贴材质
        GameObject tmp = Instantiate(LoadModel, _finalTemp, _quaternion) as GameObject;
        tmp.name = _finalTemp + "";
        tmp.transform.parent = SunmConstant.rootCube.transform;

        ///< 漫游模式、旋转模式
        if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE ||
            SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE ||
             SunmConstant.level == SunmConstant.GAME_LEVEL.CONTINUE_MODEL)
        {
            Vector3 up = _finalTemp + new Vector3(0, 1, 0);
            Vector3 down = _finalTemp + new Vector3(0, -1, 0);
            Vector3 left = _finalTemp + new Vector3(-1, 0, 0);
            Vector3 right = _finalTemp + new Vector3(1, 0, 0);
            Vector3 back = _finalTemp + new Vector3(0, 0, 1);
            Vector3 front = _finalTemp + new Vector3(0, 0, -1);


            ///< **************** 去片处理*******start**********/
            if (SunmGameInit.bOcupperThreeArray(up))
            {
                /////< 上面的方块去掉下面的片
                //Transform childT = GameObject.Find(up + "").
                //                    GetComponentInChildren<Transform>().
                //                    FindChild("down");
                ////Destroy(childT.GetComponent<MeshRenderer>());
                ////Destroy(childT.GetComponent<MeshFilter>());
                //Destroy(childT.gameObject);

                /////< 该方块去掉上面的片
                //Transform childM = tmp.
                //                    GetComponentInChildren<Transform>().
                //                    FindChild("up");
                //Destroy(childM.gameObject);
                ////Debug.LogError("up");
                DeletePlane(up, _finalTemp, "up");
            }
            if (SunmGameInit.bOcupperThreeArray(down))
            {
                DeletePlane(down, _finalTemp, "down");
            }
            if (SunmGameInit.bOcupperThreeArray(left))
            {
                DeletePlane(left, _finalTemp, "left");
            }
            if (SunmGameInit.bOcupperThreeArray(right))
            {
                DeletePlane(right, _finalTemp, "right");
            }
            if (SunmGameInit.bOcupperThreeArray(back))
            {
                DeletePlane(back, _finalTemp, "back");
            }
            if (SunmGameInit.bOcupperThreeArray(front))
            {
                DeletePlane(front, _finalTemp, "front");
            }
            ///< **************** 去片处理*******end**********/
        }

        ///< 贴材质
        setPiceMaterial(tmp.transform, _Material); 

        ///< 加入链表
        SunmConstant.cubeList.Add(tmp);
    }

    /// <summary>
    /// 删除方块
    /// </summary>
    /// <param name="_gameObject"></param>
    public static void RemoveCube(GameObject _gameObject)
    {
        ///< 如果包含则删除
        if (SunmConstant.cubeList.Contains(_gameObject))
        {
            ///< 漫游模式、旋转模式
            if (SunmConstant.level == SunmConstant.GAME_LEVEL.ROAM_SCENE ||
                SunmConstant.level == SunmConstant.GAME_LEVEL.ROTATE_SCENE ||
             SunmConstant.level == SunmConstant.GAME_LEVEL.CONTINUE_MODEL)
            {
                Vector3 _finalTemp = _gameObject.transform.position;

                ///< 填补周围方块
                Vector3 up = _finalTemp + new Vector3(0, 1, 0);
                Vector3 down = _finalTemp + new Vector3(0, -1, 0);
                Vector3 left = _finalTemp + new Vector3(-1, 0, 0);
                Vector3 right = _finalTemp + new Vector3(1, 0, 0);
                Vector3 back = _finalTemp + new Vector3(0, 0, 1);
                Vector3 front = _finalTemp + new Vector3(0, 0, -1);

                ///< **************** 去片处理*******start**********/
                if (SunmGameInit.bOcupperThreeArray(up))
                {
                    AddPlane(up, "down");
                }
                if (SunmGameInit.bOcupperThreeArray(down))
                {
                    AddPlane(down, "up");
                }
                if (SunmGameInit.bOcupperThreeArray(left))
                {
                    AddPlane(left, "right");
                }
                if (SunmGameInit.bOcupperThreeArray(right))
                {
                    AddPlane(right, "left");
                }
                if (SunmGameInit.bOcupperThreeArray(back))
                {
                    AddPlane(back, "front");
                }
                if (SunmGameInit.bOcupperThreeArray(front))
                {
                    AddPlane(front, "back");
                }
                ///< **************** 加片处理*******end**********/

                SunmConstant.cubeList.Remove(_gameObject);
                Destroy(_gameObject);
                SunmGameInit.setThreeArray(_finalTemp, 0);
            }
        }
    }


    /*******************************辅助功能***************************/

    /// <summary>
    /// 设置方块的材质 - 各个片
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="_materialName"></param>
    private static void setPiceMaterial(Transform parent, byte _materialName)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            parent.GetChild(i).gameObject.GetComponent<Renderer>().material =
                (Material)Resources.Load("Materials/" +SunmConstant.getMaterialName(_materialName));
            ///< 之所以GetComponent不是直接点出来据说是因为U5不支持直接点为了统一性
            //           parent.GetChild(i).gameObject.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Textures/MaterialTexture/" + SunmConstant.getMaterialName(_materialName));
        }
    }

    /// <summary>
    /// 垒方块时去片处理
    /// </summary>
    private static void DeletePlane(Vector3 AdjacentSquaresPOS, Vector3 OneselfPOS, string DirectionName)
    {
        ///< 临时GameObject
        GameObject operationGameOBJ = null;
        ///< 临时变量用于存传进来的名字
        string OneselfDirection = "";

        ///< 剔除自身的面
        operationGameOBJ = GameObject.Find(OneselfPOS + "").transform.Find(DirectionName).gameObject;
        GameObject.Destroy(operationGameOBJ);

        ///< 方向判断 ， 用于剔除相邻方块的面
        if (DirectionName.Equals("up"))
        {
            OneselfDirection = "down";
        }
        else if (DirectionName.Equals("down"))
        {
            OneselfDirection = "up";
        }
        else if (DirectionName.Equals("left"))
        {
            OneselfDirection = "right";
        }
        else if (DirectionName.Equals("right"))
        {
            OneselfDirection = "left";
        }
        else if (DirectionName.Equals("back"))
        {
            OneselfDirection = "front";
        }
        else if (DirectionName.Equals("front"))
        {
            OneselfDirection = "back";
        }

        ///< 剔除相邻方块的面
        operationGameOBJ = GameObject.Find(AdjacentSquaresPOS + "").transform.Find(OneselfDirection).gameObject;
        GameObject.Destroy(operationGameOBJ);
    }

    /// <summary>
    /// 删方块时增片处理
    /// </summary>
    private static void AddPlane(Vector3 AdjacentSquaresPOS, string DirectionName)
    {
        ///< 材质球的名字
        string MaterialsName = "";
        ///< 先找到需要实例化的父物体
        GameObject gameOBJParent = GameObject.Find(AdjacentSquaresPOS + "");
        ///< 实例化相应的面
        GameObject gameObjChild = (GameObject)Instantiate(Resources.Load("Prefab/CubePice/" + DirectionName));
            
        MaterialsName = SunmConstant.getMaterialName((byte)SunmGameInit.getMaterialId(AdjacentSquaresPOS));
        gameObjChild.GetComponent<Renderer>().material = (Material)Resources.Load("Materials/" + MaterialsName);
        gameObjChild.transform.parent = gameOBJParent.transform;
        gameObjChild.name = DirectionName;

        if (DirectionName.Equals("left"))
        {
            gameObjChild.transform.localPosition = new Vector3(-0.5f, 0, 0);
        }
        else if (DirectionName.Equals("right"))
        {
            gameObjChild.transform.localPosition = new Vector3(0.5f, 0 , 0);
        }
        else if (DirectionName.Equals("up"))
        {
            gameObjChild.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        else if (DirectionName.Equals("down"))
        {
            gameObjChild.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        else if (DirectionName.Equals("front"))
        {
            gameObjChild.transform.localPosition = new Vector3(0, 0, -0.5f);
        }
        else if (DirectionName.Equals("back"))
        {
            gameObjChild.transform.localPosition = new Vector3(0, 0, 0.5f);
        }
    }
}
