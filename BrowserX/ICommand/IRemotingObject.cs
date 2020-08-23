using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICommand
{
    public interface IRemotingObject
    {
        event SendHandler ClientToServer;
        event ReceiveHandler ServerToClient;
        event UserChangedHandler Login;
        event UserChangedHandler Exit;
        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="x1">参数1</param>
        /// <param name="x2">参数2</param>
        /// <returns></returns>
        string SUM(int x1, int x2);
        /// <summary>
        /// 获取服务端事件列表
        /// </summary>
        Delegate[] GetServerEventList();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        void ToServer(object info, string toName);
        /// <summary>
        /// 接受信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        void ToClient(object info, string toName);
        void ToLogin(string name);
        void ToExit(string name);
    }
    /// <summary>
    /// 客户端发送消息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="toName">发送给谁，""表示所有人，null表示没有接收服务器自己接收，其他表示指定某人</param>
    public delegate void SendHandler(object info, string toName);
    /// <summary>
    /// 客户端接收消息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="toName">发送给谁，""表示所有人，null表示没有接收服务器自己接收，其他表示指定某人</param>
    public delegate void ReceiveHandler(object info, string toName);
    /// <summary>
    /// 用户信息事件
    /// </summary>
    /// <param name="name">用户名</param>
    public delegate void UserChangedHandler(string name);
}
