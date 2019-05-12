using jini;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace AutoTest
{
    class Add_ons
    {
        String sPath = Application.StartupPath + "\\Config.ini";

        #region MonketTest指令
        public void MonkeyTest()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            string strOutput = null;

            p.Start();

            p.StandardInput.WriteLine("D:");
            p.StandardInput.WriteLine("cd \\adb");
            p.StandardInput.WriteLine("adb kill-server");
            p.StandardInput.WriteLine("adb devices");
            p.StandardInput.WriteLine("adb shell monkey -v -v -v 500 > D:\\logcat.txt");

            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine("exit");  //不加exit便不會輸出到console
            strOutput = p.StandardOutput.ReadToEnd();

            p.WaitForExit();
            p.Close();

            Console.WriteLine(strOutput);
        }
        #endregion

        private void SearchMonkeyLog()
        {
            string FileName = @"d:\5.5.5.101_5555_Log_18.txt";
            string[] filelist = File.ReadAllLines(FileName, Encoding.Default);
            List<string> StringLists = new List<string>();
            int gg = 0;
            
            for (int linenum = filelist.Length - 1; linenum >= 0; linenum--)
            {
                if (filelist[linenum].IndexOf("ANR") > -1)
                {
                    int first = filelist[linenum].IndexOf("ANR in ") + "ANR in ".Length;
                    int last = filelist[linenum].LastIndexOf(" (");
                    string str2 = filelist[linenum].Substring(first, last - first);
                    StringLists.Add(str2);
                }
            }
            
            System.IO.StreamWriter sw = new System.IO.StreamWriter(@"d:\ANR_LIST.txt"); // open the file for streamwriter
            foreach (string myStringList in StringLists)
            {
                sw.WriteLine(myStringList); // output the reslut to the file (WriteLine or Write)
            }
            sw.Close(); // close the file

            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string myStringList in StringLists)
            {
                if (dict.ContainsKey(myStringList))
                {
                    //如果Dictionary中存在这个关键词元素，则把这个Dictionary的key+1
                    dict[myStringList]++;
                }
                else
                {
                    //如果Dictionary中不存在这个关键词元素，则把它添加进Dictionary
                    dict.Add(myStringList, 1);
                }
            }

            foreach (KeyValuePair<string, int> item in dict)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }
        }

        #region 產生Monkey Test Excel報告
        public void CreateExcelFile()
        {
            string FileName = @"d:\5.5.5.101_5555_Log_18.txt";
            string[] filelist = File.ReadAllLines(FileName, Encoding.Default);
            List<string> StringLists = new List<string>();
            int gg = 0;

            for (int linenum = filelist.Length - 1; linenum >= 0; linenum--)
            {
                if (filelist[linenum].IndexOf("ANR") > -1)
                {
                    int first = filelist[linenum].IndexOf("ANR in ") + "ANR in ".Length;
                    int last = filelist[linenum].LastIndexOf(" (");
                    string str2 = filelist[linenum].Substring(first, last - first);
                    StringLists.Add(str2);
                }
            }
            
            ////建立Excel 2007檔案
            IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            //合併區
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(3, 3, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 4, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(5, 5, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(6, 6, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(7, 7, 1, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(9, 9, 0, 6));   //合併Summary行
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(10, 10, 0, 5)); //合併Error List行

            //背景色（藍色）
            ICellStyle cellStyle0 = workbook.CreateCellStyle();
            cellStyle0.FillPattern = FillPattern.SolidForeground;
            cellStyle0.FillForegroundColor = IndexedColors.PaleBlue.Index;
            cellStyle0.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle0.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle0.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle0.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            //背景色（綠色）
            ICellStyle cellStyle1 = workbook.CreateCellStyle();
            cellStyle1.FillPattern = FillPattern.SolidForeground;
            cellStyle1.FillForegroundColor = IndexedColors.Lime.Index;
            cellStyle1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            //背景色（粉色）
            ICellStyle cellStyle2 = workbook.CreateCellStyle();
            cellStyle2.FillPattern = FillPattern.SolidForeground;
            cellStyle2.FillForegroundColor = IndexedColors.Tan.Index;
            cellStyle2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            //背景色（灰色）
            ICellStyle cellStyle3 = workbook.CreateCellStyle();
            cellStyle3.FillPattern = FillPattern.SolidForeground;
            cellStyle3.FillForegroundColor = IndexedColors.Grey25Percent.Index;

            //背景色（白色）
            ICellStyle cellStyle4 = workbook.CreateCellStyle();
            cellStyle4.FillPattern = FillPattern.SolidForeground;
            cellStyle4.FillForegroundColor = IndexedColors.White.Index;

            //Summary儲存格格式
            ICellStyle summaryStyle = workbook.CreateCellStyle();
            IFont summaryFont = workbook.CreateFont();
            summaryFont.FontHeightInPoints = 18;
            summaryStyle.SetFont(summaryFont);
            summaryStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            summaryStyle.FillPattern = FillPattern.SolidForeground;
            summaryStyle.FillForegroundColor = IndexedColors.PaleBlue.Index;

            //A列
            sheet.CreateRow(0).CreateCell(0).SetCellValue("Project Name");
            sheet.CreateRow(1).CreateCell(0).SetCellValue("Model Name");
            sheet.CreateRow(2).CreateCell(0).SetCellValue("Start Time");
            sheet.CreateRow(3).CreateCell(0).SetCellValue("Renew Time");
            sheet.CreateRow(4).CreateCell(0).SetCellValue("SW Build Time");
            sheet.CreateRow(5).CreateCell(0).SetCellValue("Project No.");
            sheet.CreateRow(6).CreateCell(0).SetCellValue("Test Device");
            sheet.CreateRow(7).CreateCell(0).SetCellValue("Tester");
            for (int A = 0; A < 8; A++)
                sheet.GetRow(A).GetCell(0).CellStyle = cellStyle0;

            //E列
            sheet.GetRow(0).CreateCell(4).SetCellValue("Date");
            sheet.GetRow(1).CreateCell(4).SetCellValue("Period (H)");
            sheet.GetRow(2).CreateCell(4).SetCellValue("SW ISSUES");
            sheet.GetRow(3).CreateCell(4).SetCellValue("System Crash");
            sheet.GetRow(4).CreateCell(4).SetCellValue("Result");
            sheet.GetRow(5).CreateCell(4).SetCellValue("MTBF_SW");
            sheet.GetRow(6).CreateCell(4).SetCellValue("MTBF_Crash");
            for (int E = 0; E < 7; E++)
                sheet.GetRow(E).GetCell(4).CellStyle = cellStyle0;
            sheet.GetRow(4).GetCell(4).CellStyle = cellStyle4;

            //F列
            sheet.GetRow(0).CreateCell(5).SetCellValue("-----");
            sheet.GetRow(1).CreateCell(5).SetCellValue("-----");
            sheet.GetRow(2).CreateCell(5).SetCellValue("-----");
            sheet.GetRow(3).CreateCell(5).SetCellValue("-----");
            sheet.GetRow(4).CreateCell(5).SetCellValue("");
            sheet.GetRow(5).CreateCell(5).SetCellValue("-----");
            sheet.GetRow(6).CreateCell(5).SetCellValue("-----");
            for (int F = 0; F < 7; F++)
                sheet.GetRow(F).GetCell(5).CellStyle = cellStyle2;
            sheet.GetRow(4).GetCell(5).CellStyle = cellStyle4;

            //Summary
            sheet.CreateRow(9).CreateCell(0).SetCellValue("Summary");
            sheet.GetRow(9).GetCell(0).CellStyle = summaryStyle;

            //Error List
            sheet.CreateRow(10).CreateCell(0).SetCellValue("Error List");
            sheet.GetRow(10).GetCell(0).CellStyle = cellStyle3;

            //Total
            sheet.GetRow(10).CreateCell(6).SetCellValue("Total");
            sheet.GetRow(10).GetCell(6).CellStyle = cellStyle3;

            //搜尋相同字串並記次
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string myStringList in StringLists)
            {
                if (dict.ContainsKey(myStringList))
                {
                    //如果Dictionary中存在这个关键词元素，则把这个Dictionary的key+1
                    dict[myStringList]++;
                }
                else
                {
                    //如果Dictionary中不存在这个关键词元素，则把它添加进Dictionary
                    dict.Add(myStringList, 1);
                }
            }

            int rowcnt = dict.Count;
            while (rowcnt != 0)
            {
                foreach (KeyValuePair<string, int> item in dict)
                {
                    Console.WriteLine(item.Key);
                    Console.WriteLine(item.Value);

                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(10 + rowcnt, 10 + rowcnt, 0, 5)); //合併Error List行
                    sheet.CreateRow(10 + rowcnt).CreateCell(0).SetCellValue(item.Key);
                    sheet.GetRow(10 + rowcnt).CreateCell(6).SetCellValue(item.Value);
                    rowcnt--;
                }
            }

            for (int c = 0; c <= 25; c++)
                sheet.AutoSizeColumn(c);

            FileStream file = new FileStream(@"d:\npoi.xlsx", FileMode.Create);//產生檔案
            workbook.Write(file);
            file.Close();
        }
        #endregion

        #region 讀取Vid Pid
        public void ReadVidPid()
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                string deviceId = usbDevice.DeviceID.ToString();
                string deviceTp = usbDevice.Name.ToString();
                string deviecDescription = usbDevice.Description.ToString();

                if (deviecDescription.IndexOf("USB 視訊裝置", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    deviceTp.IndexOf("Webcam", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int vidIndex = deviceId.IndexOf("VID_");
                    string startingAtVid = deviceId.Substring(vidIndex + 4); // + 4 to remove "VID_"                    
                    string vid = startingAtVid.Substring(0, 4); // vid is four characters long
                    //Console.WriteLine("VID: " + vid);

                    Global.VID.Add(vid);

                    int pidIndex = deviceId.IndexOf("PID_");
                    string startingAtPid = deviceId.Substring(pidIndex + 4); // + 4 to remove "PID_"                    
                    string pid = startingAtPid.Substring(0, 4); // pid is four characters long
                    //Console.WriteLine("PID: " + pid);

                    Global.PID.Add(pid);
                    ini12.INIWrite(sPath, "Device", "Camera", "1");
                }
                //Console.WriteLine("DeviceID: {0}\nName: {1}\nDescription: {2}",
                //     usbDevice.DeviceID, usbDevice.Name, usbDevice.Description);
            }
            /*
                        foreach (System.Management.ManagementObject usb in USBCollection)
                        {
                            string deviceId = usb["deviceid"].ToString();
                            string deviceTp = usb["Name"].ToString();
                            //Console.WriteLine(deviceId);
                            //Console.WriteLine(deviceTp);

                            if (deviceTp.IndexOf("Webcam", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                int vidIndex = deviceId.IndexOf("VID_");
                                string startingAtVid = deviceId.Substring(vidIndex + 4); // + 4 to remove "VID_"                    
                                string vid = startingAtVid.Substring(0, 4); // vid is four characters long
                                Console.WriteLine("VID: " + vid);

                                Global.VID.Add(vid);
                                //Console.WriteLine(VID[0]);

                                int pidIndex = deviceId.IndexOf("PID_");
                                string startingAtPid = deviceId.Substring(pidIndex + 4); // + 4 to remove "PID_"                    
                                string pid = startingAtPid.Substring(0, 4); // pid is four characters long
                                Console.WriteLine("PID: " + pid);

                                Global.PID.Add(pid);
                                //Console.WriteLine(PID[0]);
                            }
                            else
                            {
                                Global.VID.Add("0000");
                                Global.PID.Add("0000");
                            }
                        }
             */
        }

        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("Name"),
                (string)device.GetPropertyValue("Description")
                ));
            }

            collection.Dispose();
            return devices;
        }

        class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceID, string name, string description)
            {
                DeviceID = deviceID;
                Name = name;
                Description = description;
            }
            public string DeviceID { get; private set; }
            public string Name { get; private set; }
            public string Description { get; private set; }
        }
        #endregion
    }
}
