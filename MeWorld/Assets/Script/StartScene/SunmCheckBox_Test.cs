using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunmCheckBox_Test : MonoBehaviour
{
    /// <summary>
    /// 删除存档的ID
    /// </summary>
    public string RemoveSerial = "";
    /// <summary>
    /// 删除存档的bool
    /// </summary>
    public bool isLocal = true;
    void Start()
    {

    }
    public void IsActive()
    {
        if (UIToggle.current.value == true)
        {
            if (!isLocal)
            {
                SunmSSInit.DeleteContinueData.Add(RemoveSerial);
            }
        }
        else if (UIToggle.current.value == false)
        {
            CancelOperation();
        }
    }

    /// <summary>
    /// 取消删除 
    /// </summary>
    private void CancelOperation()
    {
        if (SunmSSInit.DeleteContinueData.Contains(RemoveSerial))
        {
            SunmSSInit.DeleteContinueData.Remove(RemoveSerial);
        }
    }
}
