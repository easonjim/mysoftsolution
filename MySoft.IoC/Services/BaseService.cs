using System;
using MySoft.IoC.Messages;

namespace MySoft.IoC.Services
{
    /// <summary>
    /// The base class of services.
    /// </summary>
    [Serializable]
    public abstract class BaseService : IService
    {
        /// <summary>
        ///  The service logger
        /// </summary>
        private IServiceContainer logger;
        private Type serviceType;

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName
        {
            get { return serviceType.FullName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        public BaseService(IServiceContainer logger, Type serviceType)
        {
            this.logger = logger;
            this.serviceType = serviceType;
        }

        /// <summary>
        /// Runs the specified MSG.
        /// </summary>
        /// <param name="reqMsg">The MSG.</param>
        /// <returns>The msg.</returns>
        protected abstract ResponseMessage Run(RequestMessage reqMsg);

        #region IService Members

        /// <summary>
        /// Calls the service.
        /// </summary>
        /// <param name="reqMsg">The MSG.</param>
        /// <returns>The msg.</returns>
        public ResponseMessage CallService(RequestMessage reqMsg)
        {
            //获取响应的消息
            var resMsg = Run(reqMsg);

            if (resMsg == null) return null;

            if (resMsg.IsError)
            {
                string body = string.Format("Remote client【{0}】call service ({1},{2}) error.\r\nParameters => {3}\r\nMessage => {4}",
                    reqMsg.Message, reqMsg.ServiceName, reqMsg.MethodName, reqMsg.Parameters.ToString(), resMsg.Message);

                //获取异常
                var error = IoCHelper.GetException(OperationContext.Current, reqMsg, body, resMsg.Error);

                logger.WriteError(error);
            }

            //返回结果数据
            if (reqMsg.InvokeMethod)
            {
                resMsg.Value = new InvokeData
                {
                    Value = SerializationManager.SerializeJson(resMsg.Value),
                    Count = resMsg.Count,
                    ElapsedTime = resMsg.ElapsedTime,
                    OutParameters = resMsg.Parameters.ToString()
                };

                //清除参数集合
                resMsg.Parameters.Clear();
            }

            return resMsg;
        }

        #endregion
    }
}
