﻿using BlueRatLibrary;
using DirectX.Capture;
using jini;
using Microsoft.Win32.SafeHandles;      //support SafeFileHandle
using RedRat.IR;
using RedRat.RedRat3;
using RedRat.RedRat3.USB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;                            //support BindingFlags
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using USBClassLibrary;
using System.Net.Sockets;
using System.Net;
using BlockMessageLibrary;
using DTC_ABS;
using DTC_OBD;
using MySerialLibrary;
using KWP_2000;
using USB_CAN2C;
//using MaterialSkin.Controls;
//using MaterialSkin;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using USB_VN1630A;
using DriverLayer;
//using NationalInstruments.DAQmx;

namespace Woodpecker
{
	public class LogDumpping
	{
		private serialPort = new DrvRS232();
		const int byteMessage_max_Hex = 16;
		const int byteMessage_max_Ascii = 256;
		
		public void LogDataReceiving(DrvRS232 serialPort)
        {
            while (serialPort.IsOpen())
            {
                int data_to_read = serialPort.GetRxBufLen();
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    
                    serialPort.ReadDataIn(dataset, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        //logA_recorder(input_ch);
						/*
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
						*/
                    }
                    //else
                    //    logA_recorder(0x00,true); // tell log_recorder no more data for now.
                }
                //else
                //    logA_recorder(0x00,true); // tell log_recorder no more data for now.
            }
        }
		
		
		byte[] byteMessage_A = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
		int byteMessage_length_A = 0;

		private void LogRecording(byte ch, bool SaveToLog = false)
		{
			if (ini12.INIRead(MainSettingPath, "Record", "Displayhex", "") == "1")
			{
				// if (SaveToLog == false)
				{
					byteMessage_A[byteMessage_length_A] = ch;
					byteMessage_length_A++;
				}
				if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_A >= byteMessage_max_Hex))
				{
					string dataValue = BitConverter.ToString(byteMessage_A).Replace("-", "").Substring(0, byteMessage_length_A * 2);
					if (ini12.INIRead(MainSettingPath, "Record", "Timestamp", "") == "1")
					{
						DateTime dt = DateTime.Now;
						dataValue = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
					}
					log_process("A", dataValue);
					log_process("All", dataValue);
					byteMessage_length_A = 0;
				}
			}
			else
			{
				if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_A >= byteMessage_max_Ascii))
				{
					string dataValue = Encoding.ASCII.GetString(byteMessage_A).Substring(0, byteMessage_length_A);
					if (ini12.INIRead(MainSettingPath, "Record", "Timestamp", "") == "1")
					{
						DateTime dt = DateTime.Now;
						dataValue = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
					}
					log_process("A", dataValue);
					log_process("All", dataValue);
					byteMessage_length_A = 0;
				}
				else
				{
					byteMessage_A[byteMessage_length_A] = ch;
					byteMessage_length_A++;
				}
			}
		}
		
		byte[] byteTemperature = new byte[byteTemperature_max];
		int byteTemperature_length = 0;
		private void log_temperature(byte ch)
		{
			const int packet_len = 16;
			const int header_offset_1 = -16;
			const int header_offset_2 = -15;
			const int temp_ch_offset = -14;
			const int temp_unit_02 = -13;
			const int temp_unit_01 = -12;
			const int temp_polarity_offset = -11;
			const int temp_dp_offset = -10;
			const int temp_data8_offset = -9;
			const int temp_data7_offset = -8;
			const int temp_data6_offset = -7;
			const int temp_data5_offset = -6;
			const int temp_data4_offset = -5;
			const int temp_data3_offset = -4;
			const int temp_data2_offset = -3;
			const int temp_data1_offset = -2;
			const double temp_abs_value = 0.05;

			// If data_buffer is too long, cut off data not needed
			if (byteTemperature_length >= byteTemperature_max)
			{
				int destinationIndex = 0;
				for (int i = (byteTemperature_max - packet_len); i < byteTemperature_max; i++)
				{
					byteTemperature[destinationIndex++] = byteTemperature[i];
				}
				byteTemperature_length = destinationIndex;
			}

			byteTemperature[byteTemperature_length] = ch;
			byteTemperature_length++;

			if (ch == 0x0D)
			{
				if (((byteTemperature_length + header_offset_1) >= 0) &&
					 (byteTemperature[byteTemperature_length + header_offset_1] == 0x02) &&
					 (byteTemperature[byteTemperature_length + header_offset_2] == '4'))
				{
					// Packet is valid here
					if (byteTemperature[byteTemperature_length + temp_ch_offset] == Temperature_Data.temperatureChannel)
					{
						// Channel number is checked and ok here
						if ((byteTemperature[byteTemperature_length + temp_unit_02] == '0'))
						{
							if ((byteTemperature[byteTemperature_length + temp_unit_01] == '1')
								|| (byteTemperature[byteTemperature_length + temp_unit_01] == '2'))
							{
								if ((byteTemperature[byteTemperature_length + temp_data1_offset] != 0x18))
								{
									// data is valid
									int DP_convert = '0';
									int byteArray_position = 0;
									byte[] byteArray = new byte[8];
									for (int pos = byteTemperature_length + temp_data8_offset;
												pos <= (byteTemperature_length + temp_data1_offset);
												pos++)
									{
										byteArray[byteArray_position] = byteTemperature[pos];
										byteArray_position++;
									}

									string tempSubstring = System.Text.Encoding.Default.GetString(byteArray);
									double digit = Math.Pow(10, Convert.ToInt64(byteTemperature[byteTemperature_length + temp_dp_offset] - DP_convert));
									double currentTemperature = Convert.ToDouble(Convert.ToInt32(tempSubstring) / digit);

									// is value negative?
									if (byteTemperature[byteTemperature_length + temp_polarity_offset] == '1')
									{
										currentTemperature = -currentTemperature;
									}

									// is value Fahrenheit?
									if (byteTemperature[byteTemperature_length + temp_unit_01] == '2')
									{ 
										currentTemperature = (currentTemperature - 32) / 1.8;
										currentTemperature = Math.Round((currentTemperature),2,MidpointRounding.AwayFromZero);
									}

									// check whether 2 temperatures are close enough
									if (Math.Abs(previousTemperature-currentTemperature) >= temp_abs_value)
									{
										previousTemperature = currentTemperature;
										foreach (Temperature_Data item in temperatureList)
										{
											if (item.temperatureList == currentTemperature &&
												item.temperatureShot == true)
											{
												Console.WriteLine("~~~ targetTemperature ~~~ " + previousTemperature + " ~~~ currentTemperature ~~~ " + currentTemperature);
												temperatureDouble.Enqueue(currentTemperature);
												Console.WriteLine("~~~ Enqueue temperature ~~~ " + currentTemperature);
											}

											if (item.temperatureList == currentTemperature &&
												item.temperaturePause == true)
											{
												label_Command.Text = "Condition: " + item.temperatureList + ", PAUSE: " + currentTemperature;
												button_Pause.PerformClick();
												Console.WriteLine("Temperature: " + currentTemperature + "~~~~~~~~~Temperature matched. Pause the schedule.~~~~~~~~~");
											}

											if (item.temperatureList == currentTemperature &&
													 item.temperaturePort != "" &&
													 item.temperatureLog != "" &&
													 item.temperatureNewline != "")
											{
												label_Command.Text = "Condition: " + item.temperatureList + ", Log: " + currentTemperature;
												if (item.temperatureLog.Contains('|'))
												{
													string[] logArray = item.temperatureLog.Split('|');
													switch (item.temperaturePort)
													{
														case "A":
															for (int i = 0; i < logArray.Length; i++)
																ReplaceNewLine(PortA, logArray[i], item.temperatureNewline);
															break;
														case "B":
															for (int i = 0; i < logArray.Length; i++)
																ReplaceNewLine(PortB, logArray[i], item.temperatureNewline);
															break;
														case "C":
															for (int i = 0; i < logArray.Length; i++)
																ReplaceNewLine(PortC, logArray[i], item.temperatureNewline);
															break;
														case "D":
															for (int i = 0; i < logArray.Length; i++)
																ReplaceNewLine(PortD, logArray[i], item.temperatureNewline);
															break;
														case "E":
															for (int i = 0; i < logArray.Length; i++)
																ReplaceNewLine(PortE, logArray[i], item.temperatureNewline);
															break;
													}
												}
												else
												{
													switch (item.temperaturePort)
													{
														case "A":
															ReplaceNewLine(PortA, item.temperatureLog, item.temperatureNewline);
															break;
														case "B":
															ReplaceNewLine(PortB, item.temperatureLog, item.temperatureNewline);
															break;
														case "C":
															ReplaceNewLine(PortC, item.temperatureLog, item.temperatureNewline);
															break;
														case "D":
															ReplaceNewLine(PortD, item.temperatureLog, item.temperatureNewline);
															break;
														case "E":
															ReplaceNewLine(PortE, item.temperatureLog, item.temperatureNewline);
															break;
													}
												}
												Console.WriteLine("Temperature: " + currentTemperature + "~~~~~~~~~Temperature matched. Send the log to device.~~~~~~~~~");
											}
										}
									}
								}
							}
						}
					}
				}
				byteTemperature_length = 0;
			}
		}
		
	}
	
	
	
	
	/*
    private void initComboboxSaveLog()
	{
		List<string> portList = new List<string> { "Port A", "Port B", "Port C", "Port D", "Port E", "Kline", "Canbus" };

		foreach (string port in portList)
		{
			if (ini12.INIRead(MainSettingPath, port, "Checked", "") == "1")
			{
				comboBox_savelog.Items.Add(port);
			}
			else if (ini12.INIRead(MainSettingPath, port, "Checked", "") == "0" || ini12.INIRead(MainSettingPath, port, "Checked", "") == "")
			{
				comboBox_savelog.Items.Remove(port);
			}
		}

		if (ini12.INIRead(MainSettingPath, "Device", "CA310Exist", "") == "1")
			comboBox_savelog.Items.Add("CA310");

		if (ini12.INIRead(MainSettingPath, "Canbus", "Log", "") == "1")
			comboBox_savelog.Items.Add("Canbus");

		if (comboBox_savelog.Items.Count > 1)
			comboBox_savelog.Items.Add("Port All");

		if (comboBox_savelog.Items.Count == 0)
		{
			button_savelog.Enabled = false;
			comboBox_savelog.Enabled = false;
		}
		else
		{
			button_savelog.Enabled = true;
			comboBox_savelog.Enabled = true;
			comboBox_savelog.SelectedIndex = 0;
		}
	}
	*/
	
	
	// Woodpecker debug function 
	/*
	private void debug_process(string log)
	{
		try
		{
			debug_text = string.Concat(debug_text, "[Debug] [" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + log + "\r\n");
		}
		catch (Exception ex)
		{
			Console.Write(ex.Message.ToString());
			Serialportsave("Debug");
		}
	}
	*/

	// Log record function
	/*
	private void log_process(string port, string log)
	{
		try
		{
			switch (port)
			{
				case "A":
					logA_text = string.Concat(logA_text, log);
					break;
				case "B":
					logB_text = string.Concat(logB_text, log);
					break;
				case "C":
					logC_text = string.Concat(logC_text, log);
					break;
				case "D":
					logD_text = string.Concat(logD_text, log);
					break;
				case "E":
					logE_text = string.Concat(logE_text, log);
					break;
				case "CA310":
					ca310_text = string.Concat(ca310_text, log);
					break;
				case "Canbus":
					canbus_text = string.Concat(canbus_text, log);
					break;
				case "KlinePort":
					kline_text = string.Concat(kline_text, log);
					break;
				case "All":
					logAll_text = string.Concat(logAll_text, log);
					break;
			}
		}
		catch (Exception ex)
		{
			Console.Write(ex.Message.ToString());
			Serialportsave("All");
		}
	}
	*/

	/*
	private void Log(string msg)
	{
		textBox_serial.Invoke(new EventHandler(delegate
		{
			textBox_serial.Text = msg.Trim();
			PortA.WriteLine(msg.Trim());
			//PortA.We
			//myPortA .WriteLine(msg.Trim());
		}));
	}
	

	/*
	const int byteChamber_max = 64;
	byte[] byteChamber = new byte[byteChamber_max];
	int byteChamber_length = 0;

	private void logA_chamber(byte ch)
	{
		const int header_data1_offset = -9;
		const int header_data2_offset = -8;
		const int length_data_offset = -7;
		const int data_actual2_offset = -6;
		const int data_actual1_offset = -5;
		const int data_target2_offset = -4;
		const int data_target1_offset = -3;
		const int crc16_highbit_offset = -2;
		const int crc16_lowbit_offset = -1;

		byteChamber[byteChamber_length] = ch;
		byteChamber_length++;

		if ((byteChamber[byteChamber_length + header_data1_offset] == 0x01) &&
			(byteChamber[byteChamber_length + header_data2_offset] == 0x03) &&
			(byteChamber[byteChamber_length + length_data_offset] == 0x04))
		{
			byte[] byteActual = new byte[2];
			byte[] byteTarget = new byte[2];
			byteActual[0] = byteChamber[byteChamber_length + data_actual2_offset];
			byteActual[1] = byteChamber[byteChamber_length + data_actual1_offset];
			byteTarget[0] = byteChamber[byteChamber_length + data_target2_offset];
			byteTarget[1] = byteChamber[byteChamber_length + data_target1_offset];
			string stringActual = System.Text.Encoding.Default.GetString(byteActual);
			string stringTarget = System.Text.Encoding.Default.GetString(byteTarget);
			int intActual = Convert.ToInt32(stringActual, 16);
			int intTarget = Convert.ToInt32(stringTarget, 16);
		}
		byteChamber_length = 0;
	}
	#endregion
	*/

/*
	private void logB_analysis()
	{
		while (PortB.IsOpen == true)
		{
			int data_to_read = PortB.BytesToRead;
			if (data_to_read > 0)
			{
				byte[] dataset = new byte[data_to_read];
				PortB.Read(dataset, 0, data_to_read);

				for (int index = 0; index < data_to_read; index++)
				{
					byte input_ch = dataset[index];
					logB_recorder(input_ch);
					if (TemperatureIsFound == true)
					{
						log_temperature(input_ch);
					}
				}
			}
			//else
			//{
			//    logB_recorder(0x00,true); // tell log_recorder no more data for now.
			//}
		}
	}
*/
	#region -- 儲存SerialPort的log --
	/*
	private void Serialportsave(string Port)
	{
		string fName = "";

		// 讀取ini中的路徑
		fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
		switch (Port)
		{
			case "A":
				string t = fName + "\\_PortA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logA_text);
				MYFILE.Close();
				Txtbox1("", textBox_serial);
				logA_text = String.Empty;
				break;
			case "B":
				t = fName + "\\_PortB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logB_text);
				MYFILE.Close();
				logB_text = String.Empty;
				break;
			case "C":
				t = fName + "\\_PortC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logC_text);
				MYFILE.Close();
				logC_text = String.Empty;
				break;
			case "D":
				t = fName + "\\_PortD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logD_text);
				MYFILE.Close();
				logD_text = String.Empty;
				break;
			case "E":
				t = fName + "\\_PortE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logE_text);
				MYFILE.Close();
				logE_text = String.Empty;
				break;
			case "CA310":
				t = fName + "\\_CA310_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(ca310_text);
				MYFILE.Close();
				ca310_text = String.Empty;
				break;
			case "Canbus":
				t = fName + "\\_Canbus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(canbus_text);
				MYFILE.Close();
				canbus_text = String.Empty;
				break;
			case "KlinePort":
				t = fName + "\\_Kline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(kline_text);
				MYFILE.Close();
				kline_text = String.Empty;
				break;
			case "All":
				t = fName + "\\_AllPort_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(logAll_text);
				MYFILE.Close();
				logAll_text = String.Empty;
				break;
			case "Debug":
				t = fName + "\\_Debug_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
				MYFILE = new StreamWriter(t, false, Encoding.ASCII);
				MYFILE.Write(debug_text);
				MYFILE.Close();
				debug_text = String.Empty;
				break;
		}
	}
	#endregion
	*/

	/*
	private void MyLog1Camd()
	{
		string csvFile = ini12.INIRead(MainSettingPath, "Record", "LogPath", "") + "\\PortA_keyword.csv";
		int[] compare_number = new int[10];
		bool[] send_status = new bool[10];
		int compare_paremeter = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", ""));
		byte myByteList;
		const int byteMessage_max = 1024;
		byte[] byteMessage = new byte[byteMessage_max];
		int byteMessage_length = 0;

		while (StartButtonPressed == true)
		{
			while (SearchLogQueue_A.Count > 0)
			{
				myByteList = SearchLogQueue_A.Dequeue();

				if (Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "")) == 1 && Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "")) > 0)
				{
					#region 0x0A || 0x0D
					if ((myByteList == 0x0A) || (myByteList == 0x0D) || (byteMessage_length >= byteMessage_max))
					{
						for (int i = 0; i < compare_paremeter; i++)
						{
							string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
							int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
							string compare_words = Encoding.ASCII.GetString(byteMessage);
							if (Convert.ToInt32(compare_words.Length - 1) >= 1 && compare_words.Contains(compare_string) == true)
							{
								compare_number[i] = compare_number[i] + (compare_words.Length - 1);
								//Console.WriteLine(compare_string + ": " + compare_number[i]);
								if (System.IO.File.Exists(csvFile) == false)
								{
									StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
									sw1.WriteLine("Key words, Setting times, Search times, Time");
									sw1.Dispose();
								}
								StreamWriter sw2 = new StreamWriter(csvFile, true);
								sw2.Write(compare_string + ",");
								sw2.Write(compare_num + ",");
								sw2.Write(compare_number[i] + ",");
								sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
								sw2.Close();

								////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
								if (compare_number[i] > compare_num && send_status[i] == false)
								{
									ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
									ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
									if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
										&& ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
										&& ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
									{
										FormMail FormMail = new FormMail();
										FormMail.logsend();
										send_status[i] = true;
									}
								}
								////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
								if (compare_number[i] % compare_num == 0
									&& ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
									&& ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
								{
									byte[] val1;
									val1 = new byte[2];
									val1[0] = 0;

									bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
									if (!jSuccess)
									{
										Log("GP0 output enable FAILED.");
									}
									else
									{
										uint val;
										val = (uint)int.Parse("0");
										bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
										if (bSuccess)
										{
											{
												PowerState = false;
												pictureBox_AcPower.Image = Properties.Resources.OFF;
											}
										}
									}

									System.Threading.Thread.Sleep(5000);

									jSuccess = PL2303_GP0_Enable(hCOM, 1);
									if (!jSuccess)
									{
										Log("GP0 output enable FAILED.");
									}
									else
									{
										uint val;
										val = (uint)int.Parse("1");
										bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
										if (bSuccess)
										{
											{
												PowerState = true;
												pictureBox_AcPower.Image = Properties.Resources.ON;
											}
										}
									}
								}
								////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
								if (compare_number[i] % compare_num == 0
									&& ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
									&& ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
								{
									byte[] val1 = new byte[2];
									val1[0] = 0;
									uint val = (uint)int.Parse("0");

									bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
									bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

									bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
									bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

									PowerState = false;

									pictureBox_AcPower.Image = Properties.Resources.OFF;
								}
								////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
								if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
								{
									string fName = "";

									// 讀取ini中的路徑
									fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
									string t = fName + "\\_SaveLog1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

									StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
									MYFILE.Write(textBox_serial.Text);
									MYFILE.Close();
									Txtbox1("", textBox_serial);
								}
								////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
								if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
								{
									button_Start.PerformClick();
								}
								////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
								if (compare_number[i] % compare_num == 0)
								{
									int keyword_numer = i + 1;
									switch (keyword_numer)
									{
										case 1:
											GlobalData.keyword_1 = "true";
											break;

										case 2:
											GlobalData.keyword_2 = "true";
											break;

										case 3:
											GlobalData.keyword_3 = "true";
											break;

										case 4:
											GlobalData.keyword_4 = "true";
											break;

										case 5:
											GlobalData.keyword_5 = "true";
											break;

										case 6:
											GlobalData.keyword_6 = "true";
											break;

										case 7:
											GlobalData.keyword_7 = "true";
											break;

										case 8:
											GlobalData.keyword_8 = "true";
											break;

										case 9:
											GlobalData.keyword_9 = "true";
											break;

										case 10:
											GlobalData.keyword_10 = "true";
											break;
									}
								}
							}
						}
						//textBox1.AppendText(my_string + '\n');
						byteMessage_length = 0;
					}
					#endregion
					else
					{
						byteMessage[byteMessage_length] = myByteList;
						byteMessage_length++;
					}
				}
				else
				{
					if ((myByteList == 0x0A) || (myByteList == 0x0D))
					{
						byteMessage_length = 0;
					}
					else
					{
						byteMessage[byteMessage_length] = myByteList;
						byteMessage_length++;
					}
				}
			}
			Thread.Sleep(500);
		}
	}
	#endregion
	#region -- 換行符號置換 --
	private void ReplaceNewLine(SerialPort port, string columns_serial, string columns_switch)
	{
		List<string> originLineList = new List<string> {"\\r\\n", "\\n\\r", "\\r", "\\n"};
		List<string> newLineList = new List<string> {"\r\n", "\n\r", "\r", "\n"};
		var originAndNewLine = originLineList.Zip(newLineList, (o, n) => new { origin = o, newLine = n });
		foreach (var line in originAndNewLine)
		{
			if (columns_switch.Contains(line.origin))
			{
				port.Write(columns_serial + columns_switch.Replace(line.origin, line.newLine)); //發送數據 Rs232
				return;
			}
		}
	}
	#endregion
	*/
		
	/*
	private void button_savelog_Click(object sender, EventArgs e)
	{
		string save_option = comboBox_savelog.Text;
		switch (save_option)
		{
			case "Port A":
				Serialportsave("A");
				MessageBox.Show("Port A is saved.", "Reminder");
				break;
			case "Port B":
				Serialportsave("B");
				MessageBox.Show("Port B is saved.", "Reminder");
				break;
			case "Port C":
				Serialportsave("C");
				MessageBox.Show("Port C is saved.", "Reminder");
				break;
			case "Port D":
				Serialportsave("D");
				MessageBox.Show("Port D is saved.", "Reminder");
				break;
			case "Port E":
				Serialportsave("E");
				MessageBox.Show("Port E is saved.", "Reminder");
				break;
			case "CA310":
				Serialportsave("CA310");
				MessageBox.Show("CA310 is saved.", "Reminder");
				break;
			case "Canbus":
				Serialportsave("Canbus");
				MessageBox.Show("Canbus is saved.", "Reminder");
				break;
			case "Kline":
				Serialportsave("KlinePort");
				MessageBox.Show("Kline Port is saved.", "Reminder");
				break;
			case "Port All":
				Serialportsave("All");
				MessageBox.Show("All Port is saved.", "Reminder");
				break;
			default:
				break;
		}
	}
	*/
}