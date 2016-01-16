using UnityEngine;
using System.Collections;

public class SunmArchiveBean
{
    /// <summary>
    /// The identifier.id索引、名称、职业、等级、剩余经验、创建时间
    /// </summary>
    public string id;
    public byte matarialId;
    public string archiveModel;
    public float x;
    public float y;
    public float z;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataStructure"/> class.构造函数
    /// </summary>
    /// <param name="_matarialId"></param>
    /// <param name="_position"></param>
    /// <param name="_archiveModel"></param>
    public SunmArchiveBean( byte _matarialId,
                            float _x, float _y, float _z,
                            string _archiveModel)
    {
        matarialId = _matarialId;
        x = _x;
        y = _y;
        z = _z;
        archiveModel = _archiveModel;
    }
    public SunmArchiveBean()
    {

    }
}
