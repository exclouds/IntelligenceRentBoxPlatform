// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : ITransactionLogProvider.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using Magicodes.Admin.LogInfos;

namespace Magicodes.Pay.Log
{
    /// <summary>
    ///     交易日志提供程序
    /// </summary>
    public interface ITransactionLogProvider
    {
        /// <summary>
        ///     填充交易信息
        /// </summary>
        /// <param name="transactionLog">交易日志</param>
        /// <param name="exception">交易异常</param>
        void Fill(TransactionLog transactionLog, Exception exception = null);
    }
}