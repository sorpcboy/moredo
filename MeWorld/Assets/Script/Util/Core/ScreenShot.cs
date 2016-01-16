using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour
{
    public static Texture2D screenShot;

    /// <summary>
    /// 对相机截图。 
    /// </summary>
    /// <returns>The screenshot2.</returns>
    /// <param name="camera">Camera.要被截屏的相机</param>
    /// <param name="rect">Rect.截屏的区域</param>
    public static Texture2D CaptureCamera(string _fileParentPath, string _fileName)
    {
        // 创建一个RenderTexture对象
        RenderTexture rt = new RenderTexture((int)Screen.width, (int)Screen.height, 24);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        Camera.main.targetTexture = rt;
        Camera.main.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //ps: camera2.targetTexture = rt;
        //ps: camera2.Render();
        //ps: -------------------------------------------------------------------

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        screenShot = new Texture2D((int)Screen.width * 3 / 4, (int)Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(Screen.width * 1 / 8, 0, Screen.width * 3 / 4, Screen.height), 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示
        Camera.main.targetTexture = null;
        //ps: camera2.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件
        //byte[] bytes = screenShot.EncodeToPNG();
        byte[] bytes = screenShot.EncodeToJPG();
        string filename = _fileParentPath + _fileName;
        System.IO.File.WriteAllBytes(filename, bytes);

        return screenShot;
    }
}
