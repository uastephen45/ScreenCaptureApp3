using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

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
        private void ScreenShooterForm_Load(object sender, EventArgs e)

        {



        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            var processdata = GetDropdowndata();
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
            List<IntPtr> ProcessHandlesToCapture = new List<IntPtr>();
            List<CustomProcessData> processdata = GetDropdowndata();
            foreach (string item in ProcessNameToCapture)
            {
                foreach (CustomProcessData processdataItem in processdata)
                {
                    if (item.Trim() == processdataItem.processName.Trim())
                    {
                        ProcessHandlesToCapture.Add(processdataItem.windowsProcesshandle);
                    }
                }
            }
            CaptureApplications(ProcessHandlesToCapture, textBox1.Text, textBox2.Text);
            CaptureCounter = CaptureCounter + 1;
            textBox2.Text = CaptureCounter.ToString();
        }




        public void CaptureApplications(List<IntPtr> captureprocess, string fileLocation, string FileName)
        {
            foreach (IntPtr item in captureprocess)
            {
                User32.ShowWindow(item, 3);
                System.Threading.Thread.Sleep(200);
            }

            int maxHeight = 0;
            int sumWidth = 0;
            foreach (IntPtr item in captureprocess)
            {
                var rect = new User32.Rect();
                User32.GetWindowRect(item, ref rect);
                sumWidth = sumWidth + rect.right - rect.left;
                if ((rect.bottom - rect.top) > maxHeight)
                {
                    maxHeight = (rect.bottom - rect.top);
                }
            }
            int width = sumWidth;
            int height = maxHeight;
            var MainbmpImage = new Bitmap(sumWidth, maxHeight, PixelFormat.Format32bppArgb);
            Graphics Maingraphics = Graphics.FromImage(MainbmpImage);
            int CurrentMotionRight = 0;


                foreach (IntPtr item in captureprocess)
            {
               var rect = new User32.Rect();
                User32.GetWindowRect(item, ref rect);
                int currentWidth = rect.right - rect.left;
                int currentHeight = rect.bottom - rect.top;
                var bmp = new Bitmap(sumWidth, maxHeight, PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(bmp);
                System.Threading.Thread.Sleep(200);
                User32.SetForegroundWindow(item);
                System.Threading.Thread.Sleep(100);
                graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(currentWidth, currentHeight), CopyPixelOperation.SourceCopy);
                Maingraphics.DrawImage(bmp, new Point(CurrentMotionRight, 0));
                CurrentMotionRight = CurrentMotionRight + currentWidth;
            }
            string directoryToCreate = string.Concat("c:\\users\\", Environment.UserName, "\\Screen Shooter Projects\\", fileLocation, "\\");
            Directory.CreateDirectory(directoryToCreate);
            MainbmpImage.Save(string.Concat(directoryToCreate, FileName, ".png"), ImageFormat.Png);

        }

        private List<CustomProcessData> GetDropdowndata()
        {
            Process[] processArray = Process.GetProcesses();
            List<CustomProcessData> processList = new List<CustomProcessData>();
            foreach (var item in processArray)
            {
                if (item.MainWindowTitle.Trim() != "")
                    {
                        var currentItem = new CustomProcessData();
                        currentItem.PID = item.Id;
                        currentItem.processHandle = "";
                        currentItem.processName = item.ProcessName;
                        currentItem.windowsProcesshandle = item.MainWindowHandle;
                        processList.Add(currentItem);
                    }
            }           
            return processList;

        }


        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
            [DllImport("USER32.DLL")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
           public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        }
        private class CustomProcessData
        {
            public int PID;
            public string processName;
            public string processHandle;
            public IntPtr windowsProcesshandle;
        }

       
    }
}
