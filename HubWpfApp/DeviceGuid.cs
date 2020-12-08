using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public class DeviceGuid
    {
        public static KeyValuePair<bool, string> Guid
        {
            //get
            //{
            //    var wmikey = GetMacByIPConfig();

            //    if (wmikey.Count == 0)
            //    {
            //        wmikey = GetMacByNetworkInterface();
            //    }

            //    if (wmikey.Count == 0)
            //    {
            //        return new KeyValuePair<bool, string>(false, null);
            //    }
            //    else
            //    {
            //        wmikey.Sort();
            //        StringBuilder result = new StringBuilder(String.Join("", wmikey.Distinct().ToList().SkipWhile(p => p.StartsWith("0000000000"))));

            //        var cpuinfo = GetCpuID();

            //        if (cpuinfo.Key)
            //        {
            //            result.Append(cpuinfo.Value);
            //        }

            //        var hardinfo = GetHardDiskID();

            //        if (hardinfo.Key)
            //        {
            //            result.Append(hardinfo.Value);
            //        }

            //        return new KeyValuePair<bool, string>(true, result.ToString());
            //    }
            //}

            get
            {
                StringBuilder result = new StringBuilder();

                var cpuinfo = GetCpuID();

                if (cpuinfo.Key)
                {
                    result.Append(cpuinfo.Value);
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Empty);
                }

                var biosinfo = GetBIOSSerialNumber();

                if (biosinfo.Key)
                {
                    result.Append(biosinfo.Value);
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Empty);
                }

                var hardinfo = GetHardDiskID();

                if (hardinfo.Key)
                {
                    result.Append(hardinfo.Value);
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Empty);
                }

                return new KeyValuePair<bool, string>(true, result.ToString());
            }
        }

        private static List<string> GetMacByWMI()
        {
            List<string> macs = new List<string>();
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        macs.Add(mo["MacAddress"].ToString());
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {

            }
            return macs;
        }

        private static List<string> GetMacByIPConfig()
        {
            List<string> macs = new List<string>();

            ProcessStartInfo startInfo = new ProcessStartInfo("ipconfig", "/all")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            Process p = Process.Start(startInfo);
            //截取输出流
            StreamReader reader = p.StandardOutput;
            string line = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    line = line.Trim();

                    if (line.StartsWith("Physical Address") || line.StartsWith("物理地址"))
                    {
                        macs.Add(line.Split(':')[1].Replace(":", "").Replace("-", "").Trim());
                    }
                }

                line = reader.ReadLine();
            }

            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();
            reader.Close();

            return macs;
        }

        /// <summary>  
        /// 获取主板序列号  
        /// </summary>  
        /// <returns></returns>  
        public static KeyValuePair<bool, string> GetBIOSSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BIOS");
                string sBIOSSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sBIOSSerialNumber = mo["SerialNumber"].ToString().Trim();
                }
                return new KeyValuePair<bool, string>(true, sBIOSSerialNumber);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }



        private static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }

        private static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        //取机器名  
        private static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        //取CPU编号 
        private static KeyValuePair<bool, string> GetCpuID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                String strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return new KeyValuePair<bool, string>(true, strCpuID);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        //取第一块硬盘编号 
        private static KeyValuePair<bool, string> GetHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                String strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return new KeyValuePair<bool, string>(true, strHardDiskID);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        private class HardwareInfo
        {
            public enum NCBCONST
            {
                NCBNAMSZ = 16,      /* absolute length of a net name         */
                MAX_LANA = 254,      /* lana's in range 0 to MAX_LANA inclusive   */
                NCBENUM = 0x37,      /* NCB ENUMERATE LANA NUMBERS            */
                NRC_GOODRET = 0x00,      /* good return                              */
                NCBRESET = 0x32,      /* NCB RESET                        */
                NCBASTAT = 0x33,      /* NCB ADAPTER STATUS                  */
                NUM_NAMEBUF = 30,      /* Number of NAME's BUFFER               */
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ADAPTER_STATUS
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                public byte[] adapter_address;
                public byte rev_major;
                public byte reserved0;
                public byte adapter_type;
                public byte rev_minor;
                public ushort duration;
                public ushort frmr_recv;
                public ushort frmr_xmit;
                public ushort iframe_recv_err;
                public ushort xmit_aborts;
                public uint xmit_success;
                public uint recv_success;
                public ushort iframe_xmit_err;
                public ushort recv_buff_unavail;
                public ushort t1_timeouts;
                public ushort ti_timeouts;
                public uint reserved1;
                public ushort free_ncbs;
                public ushort max_cfg_ncbs;
                public ushort max_ncbs;
                public ushort xmit_buf_unavail;
                public ushort max_dgram_size;
                public ushort pending_sess;
                public ushort max_cfg_sess;
                public ushort max_sess;
                public ushort max_sess_pkt_size;
                public ushort name_count;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NAME_BUFFER
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
                public byte[] name;
                public byte name_num;
                public byte name_flags;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NCB
            {
                public byte ncb_command;
                public byte ncb_retcode;
                public byte ncb_lsn;
                public byte ncb_num;
                public IntPtr ncb_buffer;
                public ushort ncb_length;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
                public byte[] ncb_callname;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
                public byte[] ncb_name;
                public byte ncb_rto;
                public byte ncb_sto;
                public IntPtr ncb_post;
                public byte ncb_lana_num;
                public byte ncb_cmd_cplt;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public byte[] ncb_reserve;
                public IntPtr ncb_event;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct LANA_ENUM
            {
                public byte length;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.MAX_LANA)]
                public byte[] lana;
            }

            [StructLayout(LayoutKind.Auto)]
            public struct ASTAT
            {
                public ADAPTER_STATUS adapt;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NUM_NAMEBUF)]
                public NAME_BUFFER[] NameBuff;
            }
            public class Win32API
            {
                [DllImport("NETAPI32.DLL")]
                public static extern char Netbios(ref NCB ncb);
            }

            public string GetMacAddress()
            {
                string addr = "";
                try
                {
                    int cb;
                    ASTAT adapter;
                    NCB Ncb = new NCB();
                    char uRetCode;
                    LANA_ENUM lenum;

                    Ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                    cb = Marshal.SizeOf(typeof(LANA_ENUM));
                    Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    Ncb.ncb_length = (ushort)cb;
                    uRetCode = Win32API.Netbios(ref Ncb);
                    lenum = (LANA_ENUM)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(LANA_ENUM));
                    Marshal.FreeHGlobal(Ncb.ncb_buffer);
                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";

                    for (int i = 0; i < lenum.length; i++)
                    {
                        Ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                        Ncb.ncb_lana_num = lenum.lana[i];
                        uRetCode = Win32API.Netbios(ref Ncb);
                        if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                            return "";

                        Ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                        Ncb.ncb_lana_num = lenum.lana[i];
                        Ncb.ncb_callname[0] = (byte)'*';
                        cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) + Marshal.SizeOf(typeof(NAME_BUFFER)) * (int)NCBCONST.NUM_NAMEBUF;
                        Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                        Ncb.ncb_length = (ushort)cb;
                        uRetCode = Win32API.Netbios(ref Ncb);
                        adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                        Marshal.FreeHGlobal(Ncb.ncb_buffer);

                        if (uRetCode == (short)NCBCONST.NRC_GOODRET)
                        {
                            if (i > 0)
                                addr += ":";
                            addr = string.Format("{0,2:X}{1,2:X}{2,2:X}{3,2:X}{4,2:X}{5,2:X}",
                             adapter.adapt.adapter_address[0],
                             adapter.adapt.adapter_address[1],
                             adapter.adapt.adapter_address[2],
                             adapter.adapt.adapter_address[3],
                             adapter.adapt.adapter_address[4],
                             adapter.adapt.adapter_address[5]);
                        }
                    }
                }
                catch
                { }
                return addr.Replace(' ', '0');
            }

        }
    }
}
