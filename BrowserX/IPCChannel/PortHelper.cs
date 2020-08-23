using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX.IPCChannel
{
    class PortHelperOld
    {

        #region 指定类型的端口是否已经被使用了
        /// <summary>
        /// 指定类型的端口是否已经被使用了
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="type">端口类型</param>
        /// <returns></returns>
        public static bool portInUse(int port, PortType type)
        {
            bool flag = false;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipendpoints = null;
            if (type == PortType.TCP)
            {
                ipendpoints = properties.GetActiveTcpListeners();
            }
            else
            {
                ipendpoints = properties.GetActiveUdpListeners();
            }
            foreach (IPEndPoint ipendpoint in ipendpoints)
            {
                if (ipendpoint.Port == port)
                {
                    flag = true;
                    break;
                }
            }
            ipendpoints = null;
            properties = null;
            return flag;
        }
        #endregion

        #region "获取一个没有使用的端口号，从9527开始"
        public static int portReport(int portnum = 9527)
        {
            int intPortNo = portnum;
            while (!portInUse(intPortNo, PortType.TCP) )
            {
                intPortNo++;
                Application.DoEvents();
            }
            return intPortNo;

        }
        #endregion

    }

    public class PortHelperOld2
    {
        #region 成员字段

        /// <summary>
        /// 同步锁
        /// 用来在获得端口的时候同步两个线程
        /// </summary>
        private static object inner_asyncObject = new object();

        /// <summary>
        /// 开始的端口号
        /// </summary>
        private static int inner_startPort = 8022;

        #endregion

        #region 获得本机所使用的端口

        /// <summary>
        /// 使用 IPGlobalProperties 对象获得本机使用的端口
        /// </summary>
        /// <returns>本机使用的端口列表</returns>
        private static List<int> GetPortIsInOccupiedState()
        {
            List<int> retList = new List<int>();
            //遍历所有使用的端口，是不是与当前的端口有匹配
            try
            {
                //获取本地计算机的网络连接和通信统计数据的信息
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                //返回本地计算机上的所有Tcp监听程序
                IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
                //返回本地计算机上的所有UDP监听程序
                IPEndPoint[] ipsUDP = ipProperties.GetActiveUdpListeners();
                //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息
                TcpConnectionInformation[] tcpConnInfoArray = ipProperties.GetActiveTcpConnections();

                //将使用的端口加入
                retList.AddRange(ipEndPoints.Select(m => m.Port));
                retList.AddRange(ipsUDP.Select(m => m.Port));
                retList.AddRange(tcpConnInfoArray.Select(m => m.LocalEndPoint.Port));
                retList.Distinct();//去重
            }
            catch (Exception ex)//直接抛出异常
            {
                throw ex;
            }

            return retList;
        }

        /// <summary>
        /// 使用 NetStat 命令获得端口的字符串
        /// </summary>
        /// <returns>端口的字符串</returns>
        private static string GetPortIsInOccupiedStateByNetStat()
        {
            string output = string.Empty;
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo("netstat", "-an");
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    output = process.StandardOutput.ReadToEnd().ToLower();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }

        #endregion

        #region 获得一个当前没有被使用过的端口号

        /// <summary>
        /// 获得一个当前没有被使用过的端口号
        /// </summary>
        /// <returns>当前没有被使用过的端口号</returns>
        public static int GetUnusedPort()
        {
            /*
             * 在端口获取的时候防止两个进程同时获得一个一样的端口号
             * 在一个线程获得一个端口号的时候，下一个线程获取会从上一个线程获取的端口号+1开始查询
             */
            lock (inner_asyncObject)//线程安全
            {
                List<int> portList = GetPortIsInOccupiedState();
                string portString = GetPortIsInOccupiedStateByNetStat();

                for (int i = inner_startPort; i < 60000; i++)
                {
                    if (portString.IndexOf(":" + inner_startPort) < 0 &&
                        !portList.Contains(inner_startPort))
                    {
                        //记录一下 下次的端口查询从 inner_startPort+1 开始
                        inner_startPort = i + 1;
                        return i;
                    }
                }

                //如果获取不到
                return -1;
            }
        }

        #endregion
    }

    public class PortHelper
    {
        private const string PortReleaseGuid = "8875BD8E-8022-11DE-B2F4-691756D89593";

        /// <summary> 
        /// Check if startPort is available, incrementing and 
        /// checking again if it's in use until a free port is found 
        /// </summary> 
        /// <param name="startPort">The first port to check</param> 
        /// <returns>The first available port</returns> 
        public static int FindNextAvailableTCPPort(int startPort)
        {
            int port = startPort;
            bool isAvailable = true;

            var mutex = new Mutex(false, string.Concat("Global/", PortReleaseGuid));
            mutex.WaitOne();
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endPoints = ipGlobalProperties.GetActiveTcpListeners();

                do
                {
                    if (!isAvailable)
                    {
                        port++;
                        isAvailable = true;
                    }

                    foreach (IPEndPoint endPoint in endPoints)
                    {
                        if (endPoint.Port != port) continue;
                        isAvailable = false;
                        break;
                    }

                } while (!isAvailable && port < IPEndPoint.MaxPort);

                if (!isAvailable)
                    throw new ApplicationException("Not able to find a free TCP port.");

                return port;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        /// <summary> 
        /// Check if startPort is available, incrementing and 
        /// checking again if it's in use until a free port is found 
        /// </summary> 
        /// <param name="startPort">The first port to check</param> 
        /// <returns>The first available port</returns> 
        public static int FindNextAvailableUDPPort(int startPort)
        {
            int port = startPort;
            bool isAvailable = true;

            var mutex = new Mutex(false, string.Concat("Global/", PortReleaseGuid));
            mutex.WaitOne();
            try
            {
                IPGlobalProperties ipGlobalProperties =
                    IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endPoints =
                    ipGlobalProperties.GetActiveUdpListeners();

                do
                {
                    if (!isAvailable)
                    {
                        port++;
                        isAvailable = true;
                    }

                    foreach (IPEndPoint endPoint in endPoints)
                    {
                        if (endPoint.Port != port)
                            continue;
                        isAvailable = false;
                        break;
                    }

                } while (!isAvailable && port < IPEndPoint.MaxPort);

                if (!isAvailable)
                    throw new ApplicationException("Not able to find a free UDP port.");

                return port;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }


    #region 端口枚举类型
    /// <summary>
    /// 端口类型
    /// </summary>
    enum PortType
    {
        /// <summary>
        /// TCP类型
        /// </summary>
        TCP,
        /// <summary>
        /// UDP类型
        /// </summary>
        UDP
    }
    #endregion
}
