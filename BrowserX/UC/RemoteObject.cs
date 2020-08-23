using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowserX.UC
{
    #region "ipc通信中远程使用的对象"
    public class RemoteObject : MarshalByRefObject
    {
        public RemoteObject()
        {
            Console.WriteLine("Constructor called");
        }
        public override object InitializeLifetimeService()
        {
            //base.InitializeLifetimeService();
            return null;
        }
        public void CreateNewTab(string url)
        {
            try
            {
                Win32API.OutputDebugStringA(string.Format("RemoteObject CreateNewTab called:{0}", url));

                PublicModule.NewTabUrl = url;


                Console.WriteLine("CreateNewTab called");
            }
            catch(Exception ex)
            {
                Win32API.OutputDebugStringA(string.Format("RemoteObject CreateNewTab called error:{0}", ex.Message));
            }
        }
    }
    #endregion
}
