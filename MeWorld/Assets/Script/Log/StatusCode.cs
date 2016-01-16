using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @des  错误类别
 * @author hl
 * @date 2015.07.20
 * @fun
 * @modify
 *  
 */

public class StatusCode 
{
	/**
	 * 网络错误码枚举值
	 * @author hl
	 *
	 */
	public enum NETSTATE
	{  
		///< 网络标准错误码
		REQUEST_SUCCESS=200,			INLINE_ERRROR=500,

		
		///< 自定义错误码 300 - 399
		NO_LOGIN=299,               NO_DATA=300,
        WRITE_FAILD = 301,
		TIME_OUT=306,	            ARCHIVEINFO_ERROR=311,
		ARCHIVEDOWN_ERROR=312, 		DELETE_ARCHIVE_ERROR=313,
		
		///< 正常状态 450 - 499
		SUCCESS=450,                    
		ARCHIVEINFO_SUCCESS=454,  		ARCHIVEITMEDOWN_SUCCESS=455,
		DELETE_ARCHIVE_SUCCESS=456, 
		
		///< 无效状态码
        NULL_STATUS = 550, 

        /// <summary>
        /// 响应标志
        /// </summary>
        TEXTURE_RESPONSE = 601, STRING_RESPONSE = 602
	}
}
