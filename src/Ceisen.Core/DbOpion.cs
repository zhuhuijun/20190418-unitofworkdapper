/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：数据库连接选项                                                    
*│　作    者：zhuhj                                             
*│　版    本：1.0                                                 
*│　创建时间：2019/04/15                         
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间： Ceisen.Core                                   
*│　类    名： DbOpion                                      
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Ceisen.Core
{
    /// <summary>
    /// DbOpion
    /// </summary>
    public class DbOpion
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
    }
}
