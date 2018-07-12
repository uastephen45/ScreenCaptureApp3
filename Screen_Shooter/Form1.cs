using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using Screen_Shooter.Classes;
using Screen_Shooter.Models;

namespace Screen_Shooter
{
    public partial class ScreenShooterForm : Form
    {

        private CustomProcessData CurrentViewingProcess = new CustomProcessData();

        private int CaptureCounter = 0;


        public ScreenShooterForm()
        {
            InitializeComponent();
        }
        private void ScreenShooterForm_Load(object sender, EventArgs e){ }
        
        private void button2_Click_1(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            var processdata = ProcessHelper.GetDropdowndata();
            foreach (var item in processdata)
            {
                checkedListBox1.Items.Add(item.processName, false);
            }
        }
      
        private void button1_Click_1(object sender, EventArgs e)
        {
            List<string> ProcessNameToCapture = new List<string>();
            foreach (object item in checkedListBox1.CheckedItems)
            {
                ProcessNameToCapture.Add(item.ToString());
            }

            var phandles = ProcessHelper.getProcessHandlesFromName(ProcessNameToCapture);
            ProcessHelper.CaptureApplications(phandles, textBox1.Text, textBox2.Text);

            updateCounter();
        }
        public void updateCounter()
        {
            CaptureCounter = CaptureCounter + 1;
            textBox2.Text = CaptureCounter.ToString();
        }




       


       

       
    }
}
