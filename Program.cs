using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;


namespace GetNetFrameworkVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetOSandIISandDotNetVersion.GetSqlServerVersion("localhost");
            //GetOSandIISandDotNetVersion.Get45PlusFromRemoteRegistry("localhost");
            //GetOSandIISandDotNetVersion.GetIISFromRemoteRegistry("localhost");
            //GetOSandIISandDotNetVersion.GetOSFromRemoteRegistry("localhost");

            using (StreamReader sr = new StreamReader("AppServer.txt"))
            {
                string sline = sr.ReadLine();
                while(sline != null)
                {
                    Console.WriteLine("==========" + sline + "==========");
                    using (StreamWriter sw = new StreamWriter("AppServer_Info.txt", true))
                    {
                        try
                        {
                            sw.WriteLine("==========" + sline + "==========");
                            sw.WriteLine("操作系统版本： " + GetOSandIISandDotNetVersion.GetOSFromRemoteRegistry(sline));
                            sw.WriteLine(".Net Framework版本： " + GetOSandIISandDotNetVersion.Get45PlusFromRemoteRegistry(sline));
                            sw.WriteLine("IIS版本： " + GetOSandIISandDotNetVersion.GetIISFromRemoteRegistry(sline));
                            sw.WriteLine();
                        }
                        catch(Exception exc)
                        {
                            sw.WriteLine(exc.Message);
                            sw.WriteLine(exc.StackTrace);
                        }
                    }
                    Console.WriteLine();
                    sline = sr.ReadLine();
                }
            }

            using (StreamReader sr = new StreamReader("DBServer.txt"))
            {
                string sline = sr.ReadLine();
                while (sline != null)
                {
                    Console.WriteLine("==========" + sline + "==========");
                    string[] sinfo = sline.Split(',');
                    using (StreamWriter sw = new StreamWriter("DBServer_Info.txt", true))
                    {
                        try
                        {
                            sw.WriteLine("==========" + sline + "==========");
                            sw.WriteLine("操作系统版本： " + GetOSandIISandDotNetVersion.GetOSFromRemoteRegistry(sinfo[0]));
                            sw.WriteLine(".Net Framework版本： " + GetOSandIISandDotNetVersion.Get45PlusFromRemoteRegistry(sinfo[0]));
                            sw.WriteLine("数据库版本： " + GetOSandIISandDotNetVersion.GetSqlServerVersion(sinfo[1]));
                            sw.WriteLine();
                        }
                        catch (Exception exc)
                        {
                            sw.WriteLine(exc.Message);
                            sw.WriteLine(exc.StackTrace);
                        }
                    }
                    Console.WriteLine();
                    sline = sr.ReadLine();
                }
            }
            Console.ReadLine();
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp ProductString,VersionString
        }
    }

    public class ServerInfo
    {
        string osName;
        string osVersion;
        string osBuild;
        string iisName;
        string iisVersion;
        string dotNetVersion;
        string machineName;

        public string OsName
        {
            get
            {
                return osName;
            }

            set
            {
                osName = value;
            }
        }

        public string OsVersion
        {
            get
            {
                return osVersion;
            }

            set
            {
                osVersion = value;
            }
        }

        public string OsBuild
        {
            get
            {
                return osBuild;
            }

            set
            {
                osBuild = value;
            }
        }

        public string IisName
        {
            get
            {
                return iisName;
            }

            set
            {
                iisName = value;
            }
        }

        public string IisVersion
        {
            get
            {
                return iisVersion;
            }

            set
            {
                iisVersion = value;
            }
        }

        public string DotNetVersion
        {
            get
            {
                return dotNetVersion;
            }

            set
            {
                dotNetVersion = value;
            }
        }

        public string MachineName
        {
            get
            {
                return machineName;
            }

            set
            {
                machineName = value;
            }
        }
    }

    public class GetOSandIISandDotNetVersion
    {

        public static string GetSqlServerVersion(string instanceName)
        {
            string sversion = string.Empty;
            SqlConnectionStringBuilder scb = new SqlConnectionStringBuilder();
            //scb.UserInstance = instanceName;
            scb.DataSource = instanceName;
            scb.IntegratedSecurity = true;
            using (SqlConnection con = new SqlConnection(scb.ConnectionString))
            {
                con.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = con;
                command.CommandText = "select @@version";
                command.CommandType = CommandType.Text;

                try
                {
                    object oj = command.ExecuteScalar();
                    sversion = oj.ToString().Replace("\n\t", " ");
                    Console.WriteLine("Sql Server Version: " + sversion);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.StackTrace);
                }
                con.Close();
            }
            return sversion;

        }
        public static string GetIISFromRemoteRegistry(string machineName)
        {
            string sversion = string.Empty;
            const string subkey = @"SOFTWARE\Microsoft\InetStp\";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry32).OpenSubKey(subkey)) // RegistryKey.Op
            {
                //RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string MachineName, RegistryView.Registry32)
                if (ndpKey != null && ndpKey.GetValue("ProductString") != null)
                {
                    sversion += ndpKey.GetValue("ProductString").ToString();
                    sversion += " ";
                    sversion += ndpKey.GetValue("VersionString").ToString();
                    Console.WriteLine("IIS Name: " + ndpKey.GetValue("ProductString").ToString());
                    Console.WriteLine("IIS Version: " + ndpKey.GetValue("VersionString").ToString());
                }
                else
                {
                    Console.WriteLine("IIS is not detected.");
                }
            }
            return sversion;
        }
        public static string GetOSFromRemoteRegistry(string machineName)
        {
            string svervion = string.Empty;
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion
            const string subkey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry32).OpenSubKey(subkey)) // RegistryKey.Op
            {
                //RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string MachineName, RegistryView.Registry32)
                if (ndpKey != null && ndpKey.GetValue("ProductName") != null)
                {
                    svervion += ndpKey.GetValue("ProductName").ToString();
                    svervion += " ";
                    svervion += ndpKey.GetValue("CurrentVersion").ToString();
                    svervion += " ";
                    svervion += ndpKey.GetValue("CurrentBuild").ToString();
                    Console.WriteLine("OS Name: " + ndpKey.GetValue("ProductName").ToString());
                    Console.WriteLine("OS Version: " + ndpKey.GetValue("CurrentVersion").ToString());
                    Console.WriteLine("OS Build: " + ndpKey.GetValue("CurrentBuild").ToString());
                }
                else
                {
                    Console.WriteLine("OS is not detected.");
                }
            }
            return svervion;
        }

        public static string Get45PlusFromRegistry()
        {
            string sversion = string.Empty;
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)) // RegistryKey.Op
            {
                //RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string MachineName, RegistryView.Registry32)
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    sversion = CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
                    Console.WriteLine(".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                }
                else
                {
                    Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                }
            }
            return sversion;
        }

        public static string Get45PlusFromRemoteRegistry(string machineName)
        {
            string sversion = string.Empty;
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry32).OpenSubKey(subkey)) // RegistryKey.Op
            {
                //RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string MachineName, RegistryView.Registry32)
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    sversion = CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
                    Console.WriteLine(".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                }
                else
                {
                    Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                }
            }
            return sversion;
        }

        // Checking the version using >= will enable forward compatibility.
        private static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 460798)
            {
                return "4.7 or later";
            }
            if (releaseKey >= 394802)
            {
                return "4.6.2";
            }
            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            if (releaseKey >= 393295)
            {
                return "4.6";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5";
            }
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
        // Calling the GetDotNetVersion.Get45PlusFromRegistry method produces 
        // output like the following:
        //       .NET Framework Version: 4.6.1
}
