﻿using MySoft.IoC.Configuration;
using MySoft.IoC.Messages;
using System;

namespace MySoft.IoC
{
    /// <summary>
    /// 调用者
    /// </summary>
    internal class InvokeCaller : BaseServiceHandler
    {
        private CastleFactoryConfiguration config;
        private string hostName;
        private string ipAddress;

        /// <summary>
        /// 实例化InvokeCaller
        /// </summary>
        /// <param name="config"></param>
        /// <param name="container"></param>
        /// <param name="call"></param>
        /// <param name="service"></param>
        public InvokeCaller(CastleFactoryConfiguration config, IServiceContainer container, IServiceCall call, IService service)
            : base(container, call, service)
        {
            this.config = config;
            this.hostName = DnsHelper.GetHostName();
            this.ipAddress = DnsHelper.GetIPAddress();
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public InvokeData InvokeResponse(InvokeMessage message)
        {
            #region 设置请求信息

            var reqMsg = new RequestMessage
            {
                InvokeMethod = true,
                AppVersion = ServiceConfig.CURRENT_FRAMEWORK_VERSION,       //版本号
                AppName = config.AppName,                                   //应用名称
                AppPath = AppDomain.CurrentDomain.BaseDirectory,            //应用路径
                HostName = hostName,                                        //客户端名称
                IPAddress = ipAddress,                                      //客户端IP地址
                ServiceName = message.ServiceName,                          //服务名称
                MethodName = message.MethodName,                            //方法名称
                EnableCache = config.EnableCache,                           //是否缓存
                CacheTime = message.CacheTime,                              //缓存时间
                TransactionId = Guid.NewGuid(),                             //Json字符串
                RespType = ResponseType.Json                                //数据类型
            };

            #endregion

            //给参数赋值
            reqMsg.Parameters["InvokeParameter"] = message.Parameters;

            var resMsg = CallService(reqMsg);

            if (resMsg != null)
            {
                return resMsg.Value as InvokeData;
            }

            return null;
        }
    }
}
