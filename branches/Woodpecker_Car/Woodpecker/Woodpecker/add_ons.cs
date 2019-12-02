using jini;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Woodpecker
{
    class Add_ons
    {
        #region -- 讀取USB裝置 --
        public void USB_Read()
        {
            //預設AutoBox沒接上
            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxExist", "0");
            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxPort", "");
            ini12.INIWrite(Global.MainSettingPath, "Device", "CANbusExist", "0");
            ini12.INIWrite(Global.MainSettingPath, "Device", "KlineExist", "0");

            ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            ManagementObjectCollection collection = search.Get();
            var usbList = from u in collection.Cast<ManagementBaseObject>()
                          select new
                          {
                              id = u.GetPropertyValue("DeviceID"),
                              name = u.GetPropertyValue("Name"),
                              description = u.GetPropertyValue("Description"),
                              status = u.GetPropertyValue("Status"),
                              system = u.GetPropertyValue("SystemName"),
                              caption = u.GetPropertyValue("Caption"),
                              pnp = u.GetPropertyValue("PNPDeviceID"),
                          };

            foreach (var usbDevice in usbList)
            {
                string deviceId = (string)usbDevice.id;
                string deviceTp = (string)usbDevice.name;
                string deviecDescription = (string)usbDevice.description;

                string deviceStatus = (string)usbDevice.status;
                string deviceSystem = (string)usbDevice.system;
                string deviceCaption = (string)usbDevice.caption;
                string devicePnp = (string)usbDevice.pnp;

                if (deviecDescription != null)
                {
                    #region 偵測相機
                    if (deviecDescription.IndexOf("USB 視訊裝置", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviecDescription.IndexOf("USB 视频设备", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviceTp.IndexOf("Webcam", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviceTp.IndexOf("Camera", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (deviceId.IndexOf("VID_", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int vidIndex = deviceId.IndexOf("VID_");
                            string startingAtVid = deviceId.Substring(vidIndex + 4); // + 4 to remove "VID_"
                            string vid = startingAtVid.Substring(0, 4); // vid is four characters long
                            Global.VID.Add(vid);
                        }

                        if (deviceId.IndexOf("PID_", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int pidIndex = deviceId.IndexOf("PID_");
                            string startingAtPid = deviceId.Substring(pidIndex + 4); // + 4 to remove "PID_"
                            string pid = startingAtPid.Substring(0, 4); // pid is four characters long
                            Global.PID.Add(pid);
                        }

                        Console.WriteLine("-----------------Camera------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        //Camera存在
                        ini12.INIWrite(Global.MainSettingPath, "Device", "CameraExist", "1");
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Device", "CameraExist", "0");
                    }
                    #endregion

                    #region 偵測AutoBox1
                    if (deviceId.Substring(deviceId.Length - 2, 2) == "&3" &&
                        deviceId.IndexOf("USB\\VID_067B&PID_2303\\", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine("-----------------AutoBox1------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        int FirstIndex = deviceTp.IndexOf("(");
                        string AutoBoxPortSubstring = deviceTp.Substring(FirstIndex + 1);
                        string AutoBoxPort = AutoBoxPortSubstring.Substring(0);

                        int AutoBoxPortLengh = AutoBoxPort.Length;
                        string AutoBoxPortFinal = AutoBoxPort.Remove(AutoBoxPortLengh - 1);
                        
                        if (AutoBoxPortSubstring.Substring(0, 3) == "COM")
                        {
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxExist", "1");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxVerson", "1");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxPort", AutoBoxPortFinal);
                        }
                    }
                    #endregion

                    #region 偵測AutoBox2
                    if (deviceId.IndexOf("&0&5", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        deviceId.IndexOf("USB\\VID_067B&PID_2303\\", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine("-----------------AutoBox2------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        int FirstIndex = deviceTp.IndexOf("(");
                        string AutoBoxPortSubstring = deviceTp.Substring(FirstIndex + 1);
                        string AutoBoxPort = AutoBoxPortSubstring.Substring(0);

                        int AutoBoxPortLengh = AutoBoxPort.Length;
                        string AutoBoxPortFinal = AutoBoxPort.Remove(AutoBoxPortLengh - 1);
                        
                        if (AutoBoxPortSubstring.Substring(0, 3) == "COM")
                        {
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxExist", "1");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxVerson", "2");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxPort", AutoBoxPortFinal);
                        }
                    }
                    #endregion

                    #region 偵測CANbus
                    if (deviceId.IndexOf("USB\\VID_04D8&PID_0053\\", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine("-----------------Canbus------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        ini12.INIWrite(Global.MainSettingPath, "Device", "CANbusExist", "1");
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region -- 創建Config.ini --
        public void CreateConfig()
        {
            string[] Device = { "AutoboxExist", "AutoboxVerson", "AutoboxPort", "CameraExist", "RedRatExist", "DOS"};
            string[] Camera = { "VideoIndex", "VideoNumber", "VideoName", "AudioIndex", "AudioNumber", "AudioName" };
            string[] PortA = { "Checked", "PortName", "BaudRate", "DataBit", "StopBits" };
            string[] PortB = { "Checked", "PortName", "BaudRate", "DataBit", "StopBits" };
            string[] PortC = { "Checked", "PortName", "BaudRate", "DataBit", "StopBits" };
            string[] PortD = { "Checked", "PortName", "BaudRate", "DataBit", "StopBits" };
            string[] PortE = { "Checked", "PortName", "BaudRate", "DataBit", "StopBits" };
            string[] Record = { "VideoPath", "LogPath", "Generator", "EachVideo"};
            string[] Schedule1 = { "Exist", "Loop", "Path" };
            string[] Schedule2 = { "Exist", "Loop", "Path" };
            string[] Schedule3 = { "Exist", "Loop", "Path" };
            string[] Schedule4 = { "Exist", "Loop", "Path" };
            string[] Schedule5 = { "Exist", "Loop", "Path" };

            if (File.Exists(Global.MainSettingPath) == false)
            {
                for (int i = 0; i < Device.Length; i++)
                {
                    if (i == (Device.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Device", Device[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Device", Device[i], "");
                    }
                }

                for (int i = 0; i < Camera.Length; i++)
                {
                    if (i == (Camera.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Camera", Camera[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Camera", Camera[i], "");
                    }
                }

                for (int i = 0; i < PortA.Length; i++)
                {
                    if (i == (PortA.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port A", PortA[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port A", PortA[i], "");
                    }
                }

                for (int i = 0; i < PortB.Length; i++)
                {
                    if (i == (PortB.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port B", PortB[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port B", PortB[i], "");
                    }
                }

                for (int i = 0; i < PortC.Length; i++)
                {
                    if (i == (PortC.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port C", PortC[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port C", PortC[i], "");
                    }
                }

                for (int i = 0; i < PortD.Length; i++)
                {
                    if (i == (PortD.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port D", PortD[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port D", PortD[i], "");
                    }
                }

                for (int i = 0; i < PortE.Length; i++)
                {
                    if (i == (PortE.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port E", PortE[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Port E", PortE[i], "");
                    }
                }

                for (int i = 0; i < Record.Length; i++)
                {
                    if (i == (Record.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Record", Record[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Record", Record[i], "");
                    }
                }

                for (int i = 0; i < Schedule1.Length; i++)
                {
                    if (i == (Schedule1.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule1", Schedule1[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule1", Schedule1[i], "");
                    }
                }

                for (int i = 0; i < Schedule2.Length; i++)
                {
                    if (i == (Schedule2.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule2", Schedule2[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule2", Schedule2[i], "");
                    }
                }

                for (int i = 0; i < Schedule3.Length; i++)
                {
                    if (i == (Schedule3.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule3", Schedule3[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule3", Schedule3[i], "");
                    }
                }

                for (int i = 0; i < Schedule4.Length; i++)
                {
                    if (i == (Schedule4.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule4", Schedule4[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule4", Schedule4[i], "");
                    }
                }

                for (int i = 0; i < Schedule5.Length; i++)
                {
                    if (i == (Schedule5.Length - 1))
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule5", Schedule5[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MainSettingPath, "Schedule5", Schedule5[i], "");
                    }
                }

            }
        }
        #endregion

        #region -- 創建Mail.ini --
        public void CreateMailConfig()
        {
            string[] SendMail = { "value" };
            string[] DataInfo = { "TestCaseNumber", "Result", "NGfrequency", "CreateTime", "CloseTime", "ProjectNumber" };
            string[] TotalTestTime = { "value", "value1", "value2", "value3", "value4", "value5", "How Long" };
            string[] TestCase = { "TestCase1", "TestCase2", "TestCase3", "TestCase4", "TestCase5" };
            string[] MailInfo = { "From", "To", "ProjectName", "ModelName", "Version", "Tester", "TeamViewerID", "TeamViewerPassWord" };

            if (File.Exists(Global.MailSettingPath) == false)
            {
                for (int i = 0; i < SendMail.Length; i++)
                {
                    if (i == (SendMail.Length - 1))
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Send Mail", SendMail[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Send Mail", SendMail[i], "");
                    }
                }

                for (int i = 0; i < DataInfo.Length; i++)
                {
                    if (i == (DataInfo.Length - 1))
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Data Info", DataInfo[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Data Info", DataInfo[i], "");
                    }
                }

                for (int i = 0; i < TotalTestTime.Length; i++)
                {
                    if (i == (TotalTestTime.Length - 1))
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Total Test Time", TotalTestTime[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Total Test Time", TotalTestTime[i], "");
                    }
                }

                for (int i = 0; i < TestCase.Length; i++)
                {
                    if (i == (TestCase.Length - 1))
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Test Case", TestCase[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Test Case", TestCase[i], "");
                    }
                }

                for (int i = 0; i < MailInfo.Length; i++)
                {
                    if (i == (MailInfo.Length - 1))
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Mail Info", MailInfo[i], "" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        ini12.INIWrite(Global.MailSettingPath, "Mail Info", MailInfo[i], "");
                    }
                }
            }
        }
        #endregion
    }
}
