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

namespace Screen_Shooter.Classes
{
    public static class ProcessHelper
    {


        public static List<CustomProcessData> GetDropdowndata()
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

        public static ImageSize getCaptureSize(List<IntPtr> captureprocess)
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
            return new ImageSize()
            {
                width = sumWidth,
                height = maxHeight,
            };
        }
        public static void CaptureApplications(List<IntPtr> captureprocess, string fileLocation, string FileName)
        {
            var imagesize = getCaptureSize(captureprocess);
            var MainbmpImage = new Bitmap(imagesize.width, imagesize.height, PixelFormat.Format32bppArgb);
            Graphics Maingraphics = Graphics.FromImage(MainbmpImage);
            int CurrentMotionRight = 0;


            foreach (IntPtr item in captureprocess)
            {
                var rect = new User32.Rect();
                User32.GetWindowRect(item, ref rect);
                int currentWidth = rect.right - rect.left;
                int currentHeight = rect.bottom - rect.top;
                var bmp = new Bitmap(imagesize.width, imagesize.height, PixelFormat.Format32bppArgb);
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
        public static List<IntPtr> getProcessHandlesFromName(List<string> ProcessNameToCapture)
        {
            List<IntPtr> ProcessHandlesToCapture = new List<IntPtr>();
            List<CustomProcessData> processdata = ProcessHelper.GetDropdowndata();

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
            return ProcessHandlesToCapture;
        }

    }
}
