﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySoft.IoC.Messages;
using MySoft.IoC.Configuration;

namespace MySoft.IoC
{
    /// <summary>
    /// Invoke 值
    /// </summary>
    [Serializable]
    public class InvokeData
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }
    }

    /// <summary>
    /// 调用者
    /// </summary>
    public class InvokeCaller
    {
        private CastleFactoryConfiguration config;
        private IServiceContainer container;
        private IService service;
        private string hostName;
        private string ipAddress;

        /// <summary>
        /// 实例化InvokeCaller
        /// </summary>
        /// <param name="config"></param>
        /// <param name="container"></param>
        /// <param name="service"></param>
        public InvokeCaller(CastleFactoryConfiguration config, IServiceContainer container, IService service)
        {
            this.config = config;
            this.container = container;
            this.service = service;

            this.hostName = DnsHelper.GetHostName();
            this.ipAddress = DnsHelper.GetIPAddress();
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public object CallMethod(InvokeMessage message)
        {
            #region 设置请求信息

            RequestMessage reqMsg = new RequestMessage();
            reqMsg.InvokeMethod = true;
            reqMsg.AppName = config.AppName;                                //应用名称
            reqMsg.HostName = hostName;                                     //客户端名称
            reqMsg.IPAddress = ipAddress;                                   //客户端IP地址
            reqMsg.ServiceName = message.ServiceName;                       //服务名称
            reqMsg.MethodName = message.MethodName;                         //方法名称
            reqMsg.ReturnType = typeof(string);                             //返回类型
            reqMsg.TransactionId = Guid.NewGuid();                          //传输ID号
            reqMsg.Timeout = config.Timeout;                                //设置超时时间
            reqMsg.CacheTime = -1;                                          //设置缓存时间
            //reqMsg.Expiration = DateTime.Now.AddSeconds(config.Timeout)                   //设置过期时间

            #endregion

            //给参数赋值
            reqMsg.Parameters["InvokeParameter"] = message.Parameter;

            //调用服务
            var resMsg = service.CallService(reqMsg);

            //如果数据为null,则返回null
            if (resMsg == null) return null;

            //如果有异常，向外抛出
            if (resMsg.IsError) throw resMsg.Error;

            //返回数据
            return resMsg.Value;
        }
    }
}