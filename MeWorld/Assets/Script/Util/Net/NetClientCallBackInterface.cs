using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface NetClientCallBackInterface
{
    /// <summary>
    /// 没网
    /// </summary>
    /// <param name="response"></param>
    void onNet(string response);
    /// <summary>
    /// 失败
    /// </summary>
    /// <param name="response"></param>
    void onFailer(string response);
    /// <summary>
    /// 成功
    /// </summary>
    /// <param name="response"></param>
    void onSucces(StatusCode.NETSTATE nst, object response);
}
