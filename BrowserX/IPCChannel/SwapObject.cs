using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICommand
{
    public class SwapObject : MarshalByRefObject
    {

        public event ReceiveHandler SwapServerToClient 
        {
            add { _receive += value; }
            remove { _receive -= value; }
        }
        /// <summary>
        /// 接受信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        public void ToClient(object info, string toName)
        {
            if (_receive != null)
                _receive(info, toName);
        }
        //无限生命周期  
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private ReceiveHandler _receive;
    }  
}
