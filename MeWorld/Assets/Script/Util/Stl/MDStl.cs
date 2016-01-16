using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class MDStl : MonoBehaviour 
{
    /// <summary>
    /// 漫游模式STL->data文件输出
    /// </summary>
    /// <param name="cubeList"></param>
    /// <returns></returns>
    public static StatusCode.NETSTATE OutToDataFromRMModel
                                           (string _fileParent,
                                            string _fileName, 
                                            ArrayList _cubeList)
    {
        string parentPath = _fileParent;
        if (!parentPath.EndsWith("/"))
        {
            parentPath = parentPath + "/";
        }

        ///< 根据方块链表进行输出->每个方块链表的Gamobje都有不同数量的面
        if (null == _cubeList || _cubeList.Count <= 1)
        {
            ///< 累点东西吧....
            return StatusCode.NETSTATE.NO_DATA;
        }

        ///< 上下左右前后四个面
        Vector3[] vertex = new Vector3[24];
        //x=-0.5// 左
        vertex[0] = new Vector3(-0.5f, -0.5f, -0.5f);
        vertex[1] = new Vector3(-0.5f, -0.5f, 0.5f);
        vertex[2] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertex[3] = new Vector3(-0.5f, 0.5f, -0.5f);
        //x=0.5// 右
        vertex[4] = new Vector3(0.5f, -0.5f, -0.5f);
        vertex[5] = new Vector3(0.5f, 0.5f, -0.5f);
        vertex[6] = new Vector3(0.5f, 0.5f, 0.5f);
        vertex[7] = new Vector3(0.5f, -0.5f, 0.5f);
        //y=-0.5// 下
        vertex[8] = new Vector3(-0.5f, -0.5f, -0.5f);
        vertex[9] = new Vector3(0.5f, -0.5f, -0.5f);
        vertex[10] = new Vector3(0.5f, -0.5f, 0.5f);
        vertex[11] = new Vector3(-0.5f, -0.5f, 0.5f);
        //y=0.5//  上
        vertex[12] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertex[13] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertex[14] = new Vector3(0.5f, 0.5f, 0.5f);
        vertex[15] = new Vector3(0.5f, 0.5f, -0.5f);
        //z=-0.5// 前
        vertex[16] = new Vector3(-0.5f, -0.5f, -0.5f);
        vertex[17] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertex[18] = new Vector3(0.5f, 0.5f, -0.5f);
        vertex[19] = new Vector3(0.5f, -0.5f, -0.5f);
        //z=0.5// 后
        vertex[20] = new Vector3(-0.5f, -0.5f, 0.5f);
        vertex[21] = new Vector3(0.5f, -0.5f, 0.5f);
        vertex[22] = new Vector3(0.5f, 0.5f, 0.5f);
        vertex[23] = new Vector3(-0.5f, 0.5f, 0.5f);

        Transform gameObjTr;
        ArrayList vertexList = new ArrayList();
        ArrayList normalList = new ArrayList();
        for (int i = 0; i < _cubeList.Count; ++i)
        {
            gameObjTr = ((GameObject)_cubeList[i]).transform;
            Vector3 t = gameObjTr.position;
            t.z = -t.z;
            for (int j = 0; j < gameObjTr.childCount; ++j)
            {              
                switch (gameObjTr.GetChild(j).name)
                {
                    case "left":
                        vertexList.Add(vertex[0] + t);
                        vertexList.Add(vertex[1] + t);
                        vertexList.Add(vertex[2] + t);
                        normalList.Add(new Vector3(-1.0f, 0.0f, 0.0f));

                        vertexList.Add(vertex[0] + t);
                        vertexList.Add(vertex[2] + t);
                        vertexList.Add(vertex[3] + t);
                        normalList.Add(new Vector3(-1.0f, 0.0f, 0.0f));
                        break;

                    case "right":
                        vertexList.Add(vertex[4] + t);
                        vertexList.Add(vertex[5] + t);
                        vertexList.Add(vertex[6] + t);
                        normalList.Add(new Vector3(1.0f, 0.0f, 0.0f));

                        vertexList.Add(vertex[4] + t);
                        vertexList.Add(vertex[6] + t);
                        vertexList.Add(vertex[7] + t);
                        normalList.Add(new Vector3(1.0f, 0.0f, 0.0f));
                        break;

                    case "down":
                        vertexList.Add(vertex[8] + t);
                        vertexList.Add(vertex[9] + t);
                        vertexList.Add(vertex[10] + t);
                        normalList.Add(new Vector3(0.0f, -1.0f, 0.0f));

                        vertexList.Add(vertex[8] + t);
                        vertexList.Add(vertex[10] + t);
                        vertexList.Add(vertex[11] + t);
                        normalList.Add(new Vector3(0.0f, -1.0f, 0.0f));
                        break;

                    case "up":
                        vertexList.Add(vertex[12] + t);
                        vertexList.Add(vertex[13] + t);
                        vertexList.Add(vertex[14] + t);
                        normalList.Add(new Vector3(0.0f, 1.0f, 0.0f));

                        vertexList.Add(vertex[12] + t);
                        vertexList.Add(vertex[14] + t);
                        vertexList.Add(vertex[15] + t);
                        normalList.Add(new Vector3(0.0f, 1.0f, 0.0f));
                        break;

                    case "back"://front
                        vertexList.Add(vertex[16] + t);
                        vertexList.Add(vertex[17] + t);
                        vertexList.Add(vertex[18] + t);
                        normalList.Add(new Vector3(0.0f, 0.0f, -1.0f));

                        vertexList.Add(vertex[16] + t);
                        vertexList.Add(vertex[18] + t);
                        vertexList.Add(vertex[19] + t);
                        normalList.Add(new Vector3(0.0f, 0.0f, -1.0f));
                        break;

                    case "front"://back
                        vertexList.Add(vertex[20] + t);
                        vertexList.Add(vertex[21] + t);
                        vertexList.Add(vertex[22] + t);
                        normalList.Add(new Vector3(0.0f, 0.0f, 1.0f));

                        vertexList.Add(vertex[20] + t);
                        vertexList.Add(vertex[22] + t);
                        vertexList.Add(vertex[23] + t);
                        normalList.Add(new Vector3(0.0f, 0.0f, 1.0f));
                        break;
                }
            }
        }

        ///< 写入顶点和法向量到data文件
        if (!WriteSTLOriginDataWithVN(_fileParent + _fileName, vertexList, normalList))
        {
            return StatusCode.NETSTATE.WRITE_FAILD;
        }

        ///< 手动释放内存
        vertex = null;
        vertexList.Clear();
        vertexList = null;
        normalList.Clear();
        normalList = null;

        return StatusCode.NETSTATE.SUCCESS;
    }


    ///*************************************辅助接口************************///

    /// <summary>
    /// 模型顶点和法向量以文本文件的形式写入指定文件
    /// </summary>
    /// <param name="_filePath"></param>
    /// <param name="vertexs"></param>
    /// <param name="normals"></param>
    /// <returns></returns>
    private static bool WriteSTLOriginDataWithVN(string _filePath, ArrayList vertexs, ArrayList normals)
    {
        ///< 没有数据
        if (null == vertexs || null == normals)
        {
            return false;
        }

        StringBuilder strBd = new StringBuilder();
        ///< 分段写文件，避免StringBuilder容量超载
        int quotient = (int)(normals.Count / MDFileOpt.sectionCounts);
        int remainder = (int)(normals.Count % MDFileOpt.sectionCounts);

        bool erroFlag;
        ///< 分段写入
        for (int i = 0; i < quotient; ++i)
        {
            for (int k = (i * MDFileOpt.sectionCounts); k < ((i * MDFileOpt.sectionCounts) + MDFileOpt.sectionCounts); ++k)
            {
                strBd.AppendLine(((Vector3)normals[k]).x + " " + ((Vector3)normals[k]).y + " " + ((Vector3)normals[k]).z);
                strBd.AppendLine(((Vector3)vertexs[k * 3]).x + " " + ((Vector3)vertexs[k * 3]).y + " " + ((Vector3)vertexs[k * 3]).z);
                strBd.AppendLine(((Vector3)vertexs[k * 3 + 1]).x + " " + ((Vector3)vertexs[k * 3 + 1]).y + " " + ((Vector3)vertexs[k * 3 + 1]).z);
                strBd.AppendLine(((Vector3)vertexs[k * 3 + 2]).x + " " + ((Vector3)vertexs[k * 3 + 2]).y + " " + ((Vector3)vertexs[k * 3 + 2]).z);
            }
            erroFlag = MDFileOpt.FileWriter.AppendFileText(_filePath, strBd);
            ///< 写入后清空容器，下次再装载
            strBd.Length = 0;
            if (!erroFlag)
            {
                return erroFlag;
            }
        }

        ///< 最后一段
        for (int j = quotient * MDFileOpt.sectionCounts; j < normals.Count; ++j)
        {
            strBd.AppendLine(((Vector3)normals[j]).x + " " + ((Vector3)normals[j]).y + " " + ((Vector3)normals[j]).z);
            strBd.AppendLine(((Vector3)vertexs[j * 3]).x + " " + ((Vector3)vertexs[j * 3]).y + " " + ((Vector3)vertexs[j * 3]).z);
            strBd.AppendLine(((Vector3)vertexs[j * 3 + 1]).x + " " + ((Vector3)vertexs[j * 3 + 1]).y + " " + ((Vector3)vertexs[j * 3 + 1]).z);
            strBd.AppendLine(((Vector3)vertexs[j * 3 + 2]).x + " " + ((Vector3)vertexs[j * 3 + 2]).y + " " + ((Vector3)vertexs[j * 3 + 2]).z);
        }
        erroFlag = MDFileOpt.FileWriter.AppendFileText(_filePath, strBd);
        strBd.Length = 0;
        strBd = null;

        return erroFlag;
    }
}
