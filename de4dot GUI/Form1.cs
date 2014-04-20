using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

namespace de4dot_GUI
{
    public partial class Form1 : Form
    {
        de4dot.Class.de4dot de4dot = new de4dot.Class.de4dot();
        public Form1 (string param)
        {
            InitializeComponent();

            int contextstatus = de4dot.Contextual(true);

            if (contextstatus == 1 )
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }

            if (param.Length > 0 && param != "null")
            {
                string[] arr = new string[2];
                listView1.View = View.Details;
                listView1.ForeColor = Color.White;
                de4dot.TopMost(Handle);

                ListViewItem lvi;

                arr[0] = Path.GetFileName(param);
                arr[1] = de4dot.Detect(param);

                lvi = new ListViewItem(arr);
                listView1.Items.Add(lvi);
            }
            else
            {
                listView1.View = View.Details;
                listView1.ForeColor = Color.White;
                de4dot.TopMost(Handle);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            de4dot.DeObfuscate();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                string[] arr = new string[2];
                ListViewItem itm;

                foreach (string files in filePaths)
                {
                    arr[0] = Path.GetFileName(files);
                    arr[1] = de4dot.Detect(files);

                    if (arr[1] == "-1" || arr[1] == "-2" || arr[1] == "-3")
                    {
                        continue;
                    }
                    else
                    {
                        itm = new ListViewItem(arr);
                        listView1.Items.Add(itm);
                    }
                }
            }
        }
        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            de4dot.Clear();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(de4dot.Verbose() + "\n" + "de4dotUI v1.0.0" + "\n" + "\n" 
                + "Yashar/Mahmoudnia" + "\n" + "y.mahmoudnia@gmail.com", "de4dot GUI"
                , MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                de4dot.Contextual(false,true);
            }
            else
            {
                de4dot.Contextual(false, false);
            }
        }
    }
}
