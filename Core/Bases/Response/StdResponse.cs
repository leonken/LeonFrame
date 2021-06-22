using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Bases.Response
{
    public class StdResponse
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
