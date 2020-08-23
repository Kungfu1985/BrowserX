using MiniBlinkPinvoke;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MiniBlink
{
    public delegate object JsFunc(params object[] param);

    public class JsFuncWapper
    {
        private string _name;
        private BlinkBrowser _miniblink;

        internal JsFuncWapper(BlinkBrowser control, long jsvalue, IntPtr es)
        {
            _miniblink = control;
            _name = "func" + Guid.NewGuid().ToString().Replace("-", "");
            BlinkBrowserPInvoke.jsSetGlobal(es, _name, jsvalue);
        }

        public object Call(params object[] param)
        {
            object result = null;

            _miniblink.SafeInvoke(s =>
			{
				var es = BlinkBrowserPInvoke.wkeGlobalExec(_miniblink.Handle);
				var value = BlinkBrowserPInvoke.jsGetGlobal(es, _name);
                var jsps = param.Select(i => i.ToJsValue(_miniblink, es)).ToArray();
                result = BlinkBrowserPInvoke.jsCall(es, value, BlinkBrowserPInvoke.jsUndefined(), jsps, jsps.Length).ToValue(_miniblink, es);
                BlinkBrowserPInvoke.jsSetGlobal(es, _name, BlinkBrowserPInvoke.jsUndefined());
			});
			
            return result;
        }
    }
}
