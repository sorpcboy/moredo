using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;
using System.Threading;

public class SunmContinueData
{
    /// <summary>
    /// 初始化对象数组
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="_ContinueDataArchive"></param>
    public static int ArchiveDataParsing(string Info, List<SunmContinueBean> _ContinueDataArchive)
    {
        try
        {
            ///< 转对象方法【直接解析至数组】       
            SunmContinueBean[] jarrBean = JsonMapper.ToObject<SunmContinueBean[]>(Info);
            int count = jarrBean.Length;
            if (count == 0)
            {
                ///< 没有数据
                return 0;
            }

            for (int i = 0; i < jarrBean.Length; ++i)
            {
                _ContinueDataArchive.Add(jarrBean[i]);
            }
            _ContinueDataArchive.Sort();
        }
        catch (Exception e)
        {
            return -1;
        }

        return 1;
    }
}
