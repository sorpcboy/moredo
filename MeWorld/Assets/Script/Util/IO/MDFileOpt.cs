using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class MDFileOpt  
{
    /// <summary>
    /// 写STL原始数据文件顶点数分段
    /// </summary>
    public const int sectionCounts = 10000;   

    /****************************************文件操作相关类**************************************/

    /// <summary>
    /// 二进制文件读写内部类【静态方法】
    /// 使用规则：
    /// 1. OpenFileBinary(string _binFileNameWithPath)
    /// 2. WriteFileBinary(StringBuilder strB)
    /// 3. CloseBianaryStream()
    /// 1.2返回值： ErrorFlag.ERROR_LOG
    /// </summary>
    public class FileWriter
    {
        /// <summary>
        /// 文本文件读写
        /// </summary>
        private static FileStream fsT = null;      // 声明文件流的对象  
        private static StreamWriter swT = null;    // 声明写入器的对象

        public static bool AppendFileText(string _binFileNameWithPath, StringBuilder strB)
        {
            try
            {
                // 注意第2个参数：
                // FileMode.Append 指定打开文件并追加
                fsT = new FileStream(_binFileNameWithPath, FileMode.Append, FileAccess.Write);
                swT = new StreamWriter(fsT, new UTF8Encoding(false));   ///< Delete BOM
                swT.Write(strB.ToString());

                // 关闭文件
                swT.Close();
                fsT.Close();

                swT = null;
                fsT = null;
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                if (swT != null)
                {
                    try
                    {
                        swT.Close();
                        swT = null;
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
                if (fsT != null)
                {
                    try
                    {
                        fsT.Close();
                        fsT = null;
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }

            return true;
        }
    }
}
