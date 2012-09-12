﻿using System;
using System.Net.Sockets;
using MySoft.IoC.Communication.Scs.Client;

namespace MySoft.IoC
{
    /// <summary>
    /// Tcp connection
    /// </summary>
    public interface ITcpConnection
    {
        /// <summary>
        /// Connected
        /// </summary>
        event EventHandler<ConnectEventArgs> OnConnected;

        /// <summary>
        /// Disconnected
        /// </summary>
        event EventHandler<ConnectEventArgs> OnDisconnected;
    }

    /// <summary>
    /// Client event args
    /// </summary>
    [Serializable]
    public class ConnectEventArgs : EventArgs
    {
        /// <summary>
        /// Get connection channel
        /// </summary>
        public IScsClient Channel { get; private set; }

        /// <summary>
        /// Get socket error
        /// </summary>
        public SocketException Error { get; set; }

        /// <summary>
        /// Get is callback
        /// </summary>
        public bool IsCallback { get; set; }

        public ConnectEventArgs(IScsClient channel)
        {
            this.Channel = channel;
        }
    }
}