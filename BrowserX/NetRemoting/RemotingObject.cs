using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICommand;

namespace NetRemoting
{
    public class RemotingObject : MarshalByRefObject, IRemotingObject
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        public event SendHandler ClientToServer
        {
            add { _send += value; }
            remove { _send -= value; }
        }
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event ReceiveHandler ServerToClient;
        /// <summary>
        /// 发送事件
        /// </summary>
        public event UserChangedHandler Login
        {
            add { _login += value; }
            remove { _login -= value; }
        }
        /// <summary>
        /// 发送事件
        /// </summary>
        public event UserChangedHandler Exit
        {
            add { _exit += value; }
            remove { _exit -= value; }
        }
        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="x1">参数1</param>
        /// <param name="x2">参数2</param>
        /// <returns></returns>
        public string SUM(int x1, int x2)
        {
            return x1 + "+" + x2 + "=" + (x1 + x2);
        }
        /// <summary>
        /// 绑定服务端向客户端发送消息的事件方法
        /// </summary>
        /// <param name="receive">接收事件</param>
        public Delegate[] GetServerEventList()
        {
            return this.ServerToClient.GetInvocationList();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        public void ToServer(object info, string toName)
        {
            if (_send != null)
                _send(info, toName);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        public void ToClient(object info, string toName)
        {
            if (_receive != null)
                _receive(info, toName);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name">用户名</param>
        public void ToLogin(string name)
        {
            if (!_nameHash.Contains(name))
            {
                _nameHash.Add(name);
                if (_login != null)
                    _login(name);
            }
            else
            { throw new Exception("用户已存在"); }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="name">用户名</param>
        public void ToExit(string name)
        {
            if (_nameHash.Contains(name))
            {
                _nameHash.Remove(name);
                if (_exit != null)
                    _exit(name);
            }
        }

        //fix bug 202/7/30 9:49,不加此处代码，服务器端会一段时间断开
        //无限生命周期  
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private SendHandler _send;
        private ReceiveHandler _receive;
        private UserChangedHandler _login;
        private UserChangedHandler _exit;
        private HashSet<string> _nameHash = new HashSet<string>();
    }
}
