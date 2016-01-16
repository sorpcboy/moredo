using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SunmContinueBean : IComparable
{
    public string serial { set; get; }
    public string model { set; get; }
    public string parentPath { set; get; }
    public string fileName { set; get; }
    public string fileUrl { set; get; }
    public string imageUrl { set; get; }
    public bool isLocal { set; get; }
    public int id { set; get; }

    /// <summary>
    /// 实现IComparable接口，用存档时间戳做比较
    /// </summary>
    /// <param name="obj">比较对象</param>
    /// <returns>比较结果</returns>
    public int CompareTo(object obj)
    {
        if (obj is SunmContinueBean)
        {
            return long.Parse(((SunmContinueBean)obj).fileName).CompareTo(long.Parse(fileName));
        }

        return 1;
    }
}
