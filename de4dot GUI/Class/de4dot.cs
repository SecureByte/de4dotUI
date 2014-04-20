using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/*
de4dotUI is a GUI version of de4dot .NET deobfuscator and unpacker written in C#.
Author : Yashar/Mahmoudnia
Contact : y.mahmoudnia@gmail.com
*/

namespace de4dot.Class
{
    class de4dot
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        const UInt32 SWP_NOZORDER = 0X4;

        static string output;
        static string version;
        public static ArrayList fpath = new ArrayList();

        ThreadStart StartDObfuscate;
        Thread DObfuscate;
        public string Detect(string file)
        {
            string de4dotPath = Application.StartupPath + "\\de4dot.exe";
            string cfile = "\"" + file + "\"";
            int sign;

            if (!File.Exists(de4dotPath))
            {
                return "de4dot engine not found";
            }
            else
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = de4dotPath;
                p.StartInfo.Arguments = " -d " + cfile;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();

                if (Path.GetExtension(file) == ".exe" || Path.GetExtension(file) == ".dll" || Path.GetExtension(file) == "null")
                {
                    if (output.IndexOf("a .NET PE file") > 0)
                    {
                        return "-1";
                    }
                    else
                    {
                        fpath.Add(cfile);
                        output = output.Remove(0, 146);
                        sign = output.IndexOf("(", 0);
                        output = output.Substring(0, sign);
                        output = output.Trim();
                        return output;
                    }
                }
                else
                {
                    return "-2";
                }
            }
            return "de4dot engine not found";
        }
        public void DeObfuscate()
        {
            StartDObfuscate = new ThreadStart(RunDObfuscate);
            DObfuscate = new Thread(StartDObfuscate);
            DObfuscate.Start();            
        }
        private void RunDObfuscate()
        {
            string de4dotPath = Application.StartupPath + "\\de4dot.exe";
            string cfile = "\"" + fpath + "\"";

            foreach (string opath in fpath)
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = de4dotPath;
                p.StartInfo.Arguments = opath;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
        }
        public string Verbose()
        {
            string de4dotPath = Application.StartupPath + "\\de4dot.exe";
            int sign;

            if (!File.Exists(de4dotPath))
            {
                return "de4dot engine not found";
            }
            else
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "de4dot.exe";
                p.StartInfo.Arguments = " -v";
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                version = p.StandardOutput.ReadToEnd();
                sign = version.IndexOf("Copyright", 0);
                version = version.Substring(0, sign);
                version = version.Trim();
                return version;
            }
            return "de4dot engine not found";
        }
        public void TopMost(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 422, 238, 0, 0, SWP_NOSIZE);
            
        }
        public void Clear()
        {
            fpath.Clear();
        }
        public int Contextual(bool status, bool? opt = null)
        {
            string appname = Process.GetCurrentProcess().MainModule.FileName;

            var dllkey = Registry.ClassesRoot.OpenSubKey("dllfile\\shell\\de4dot\\command");
            var exekey = Registry.ClassesRoot.OpenSubKey("exefile\\shell\\de4dot\\command");

            if (status == true)
            {
                if (dllkey == null && exekey == null)
                {
                    return 1;
                }
                else if (dllkey != null && exekey != null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (opt == false)
                {
                    Registry.ClassesRoot.DeleteSubKeyTree("dllfile\\shell\\de4dot");
                    Registry.ClassesRoot.DeleteSubKeyTree("exefile\\shell\\de4dot");
                }
                else if (opt == true)
                {
                    var rdllkey = Registry.ClassesRoot.CreateSubKey("dllfile\\shell\\de4dot\\command");
                    var rexekey = Registry.ClassesRoot.CreateSubKey("exefile\\shell\\de4dot\\command");
                    rdllkey.SetValue("", appname + " %1", RegistryValueKind.String);
                    rexekey.SetValue("", appname + " %1", RegistryValueKind.String);
                    rdllkey.Close();
                    rexekey.Close();
                }
            }
            return -2;
        }
    }
}
