using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vxlapi_NET;

namespace USB_VN1630A
{
    class USB_VECTOR_Lib
    {
        // -----------------------------------------------------------------------------------------------
        // Global variables
        // -----------------------------------------------------------------------------------------------
        // Driver access through XLDriver (wrapper)
        public static XLDriver CANDrive = new XLDriver();
        public XLDefine.XL_Status XL_status = new XLDefine.XL_Status();
        private static String appName = "Woodpecker";

        // Driver configuration
        public static XLClass.xl_driver_config driverConfig = new XLClass.xl_driver_config();

        // Variables required by XLDriver
        private static XLDefine.XL_HardwareType hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
        private static uint hwIndex = 0;
        private static uint hwChannel = 0;
        private static int portHandle = -1;
        private static UInt64 accessMask = 0;
        private static UInt64 permissionMask = 0;
        private static UInt64 txMask = 0;
        private static UInt64 rxMask = 0;
        private static int txCi = -1;
        private static int rxCi = -1;
        private static EventWaitHandle xlEvWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, null);



        // RX thread
        private static Thread rxThread;
        private static bool blockRxThread = false;
        // -----------------------------------------------------------------------------------------------

        public uint Connect()
        {
            uint connection_status = ~(1U);
            XLDefine.XL_Status status;

            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("                     xlCANdemo.NET C# V11.0                        ");
            Console.WriteLine("Copyright (c) 2019 by Vector Informatik GmbH.  All rights reserved.");
            Console.WriteLine("-------------------------------------------------------------------\n");

            // print .NET wrapper version
            Console.WriteLine("vxlapi_NET        : " + typeof(XLDriver).Assembly.GetName().Version);

            // Open XL Driver
            status = CANDrive.XL_OpenDriver();
            Console.WriteLine("Open Driver       : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            // Get XL Driver configuration
            status = CANDrive.XL_GetDriverConfig(ref driverConfig);
            Console.WriteLine("Get Driver Config : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;
            
            // Convert the dll version number into a readable string
            Console.WriteLine("DLL Version       : " + CANDrive.VersionToString(driverConfig.dllVersion));


            // Display channel count
            Console.WriteLine("Channels found    : " + driverConfig.channelCount);


            // Display all found channels
            for (int i = 0; i < driverConfig.channelCount; i++)
            {
                Console.WriteLine("\n                   [{0}] " + driverConfig.channel[i].name, i);
                Console.WriteLine("                    - Channel Mask    : " + driverConfig.channel[i].channelMask);
                Console.WriteLine("                    - Transceiver Name: " + driverConfig.channel[i].transceiverName);
                Console.WriteLine("                    - Serial Number   : " + driverConfig.channel[i].serialNumber);
            }

            // If the application name cannot be found in VCANCONF...
            if ((CANDrive.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDrive.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
            {
                //...create the item with two CAN channels
                CANDrive.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDrive.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDrive.XL_PopupHwConfig();
                connection_status = 0x03;
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            if (!GetAppChannelAndTestIsOk(0, ref txMask, ref txCi) || !GetAppChannelAndTestIsOk(1, ref rxMask, ref rxCi))
            {
                CANDrive.XL_PopupHwConfig();
                connection_status = 0x03;
            }

            return connection_status;
        }

        public uint StartCAN()
        {
            uint connection_status = ~(1U);
            XLDefine.XL_Status status;

            PrintConfig();

            accessMask = txMask | rxMask;
            permissionMask = accessMask;

            // Open port
            status = CANDrive.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            Console.WriteLine("\n\nOpen Port             : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            // Check port
            status = CANDrive.XL_CanRequestChipState(portHandle, accessMask);
            Console.WriteLine("Can Request Chip State: " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            // Activate channel
            status = CANDrive.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            Console.WriteLine("Activate Channel      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            // Initialize EventWaitHandle object with RX event handle provided by DLL
            int tempInt = -1;
            status = CANDrive.XL_SetNotification(portHandle, ref tempInt, 1);
            xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

            Console.WriteLine("Set Notification      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            // Reset time stamp clock
            status = CANDrive.XL_ResetClock(portHandle);
            Console.WriteLine("Reset Clock           : " + status + "\n\n");
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            else connection_status = 0x01;

            return connection_status;
        }

        private Queue<USB_CAN2C.CAN_Data> CAN_Write_Data_Queue = new Queue<USB_CAN2C.CAN_Data>();

        public void CAN_Write_Queue_Clear()
        {
            CAN_Write_Data_Queue.Clear();
        }

        public void CAN_Write_Queue_Add(USB_CAN2C.CAN_Data can_data)
        {
            CAN_Write_Data_Queue.Enqueue(can_data);
        }

        public void CAN_Write_Queue_SendData()
        {
            while ((CAN_Write_Data_Queue.Count > 0))
            {
                XLDefine.XL_Status txStatus;

                // Create an event collection with 1 messages (events)
                XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection((uint)CAN_Write_Data_Queue.Count);

                // event
                for(int i = 0; i< CAN_Write_Data_Queue.Count; i++)
                {
                    USB_CAN2C.CAN_Data this_can_ctl = CAN_Write_Data_Queue.Dequeue();
                    xlEventCollection.xlEvent[i].tagData.can_Msg.id = this_can_ctl.ID;
                    xlEventCollection.xlEvent[i].tagData.can_Msg.dlc = this_can_ctl.DataLen;
                    for (int j = 0; j < this_can_ctl.DataLen; j++)
                    {
                        xlEventCollection.xlEvent[i].tagData.can_Msg.data[j] = this_can_ctl.Data[j];
                    }
                    xlEventCollection.xlEvent[i].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
                }

                // Transmit events
                txStatus = CANDrive.XL_CanTransmit(portHandle, txMask, xlEventCollection);
                Console.WriteLine("Transmit Message      : " + txStatus);
            }
        }

        public void CANTransmit(uint ID, uint timeRate, byte[] Data)
        {
            XLDefine.XL_Status txStatus;

            txStatus = CANDrive.XL_SetTimerRate(portHandle, timeRate*100);
            Console.WriteLine("Transmit Message      : " + txStatus);
            // Create an event collection with 1 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);

            // event 1
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = ID;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = (ushort)Data.Length;
            
            for (int orginal_index = 0; orginal_index < Data.Length; orginal_index++)
            {
                xlEventCollection.xlEvent[0].tagData.can_Msg.data[orginal_index] = Data[orginal_index];
            }

            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;

            // Transmit events
            txStatus = CANDrive.XL_CanTransmit(portHandle, txMask, xlEventCollection);
            Console.WriteLine("Transmit Message      : " + txStatus);
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// MAIN
        /// 
        /// Sends and receives CAN messages using main methods of the "XLDriver" class.
        /// This demo requires two connected CAN channels (Vector network interface). 
        /// The configuration is read from Vector Hardware Config (vcanconf.exe).
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        static int Init(string[] args)
        {
            XLDefine.XL_Status status;

            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("                     xlCANdemo.NET C# V11.0                        ");
            Console.WriteLine("Copyright (c) 2019 by Vector Informatik GmbH.  All rights reserved.");
            Console.WriteLine("-------------------------------------------------------------------\n");

            // print .NET wrapper version
            Console.WriteLine("vxlapi_NET        : " + typeof(XLDriver).Assembly.GetName().Version);

            // Open XL Driver
            status = CANDrive.XL_OpenDriver();
            Console.WriteLine("Open Driver       : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();


            // Get XL Driver configuration
            status = CANDrive.XL_GetDriverConfig(ref driverConfig);
            Console.WriteLine("Get Driver Config : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();


            // Convert the dll version number into a readable string
            Console.WriteLine("DLL Version       : " + CANDrive.VersionToString(driverConfig.dllVersion));


            // Display channel count
            Console.WriteLine("Channels found    : " + driverConfig.channelCount);


            // Display all found channels
            for (int i = 0; i < driverConfig.channelCount; i++)
            {
                Console.WriteLine("\n                   [{0}] " + driverConfig.channel[i].name, i);
                Console.WriteLine("                    - Channel Mask    : " + driverConfig.channel[i].channelMask);
                Console.WriteLine("                    - Transceiver Name: " + driverConfig.channel[i].transceiverName);
                Console.WriteLine("                    - Serial Number   : " + driverConfig.channel[i].serialNumber);
            }

            // If the application name cannot be found in VCANCONF...
            if ((CANDrive.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDrive.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
            {
                //...create the item with two CAN channels
                CANDrive.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDrive.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                PrintAssignErrorAndPopupHwConf();
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            while (!GetAppChannelAndTestIsOk(0, ref txMask, ref txCi) || !GetAppChannelAndTestIsOk(1, ref rxMask, ref rxCi))
            {
                PrintAssignErrorAndPopupHwConf();
            }

            PrintConfig();

            accessMask = txMask | rxMask;
            permissionMask = accessMask;

            // Open port
            status = CANDrive.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            Console.WriteLine("\n\nOpen Port             : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Check port
            status = CANDrive.XL_CanRequestChipState(portHandle, accessMask);
            Console.WriteLine("Can Request Chip State: " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Activate channel
            status = CANDrive.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            Console.WriteLine("Activate Channel      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Initialize EventWaitHandle object with RX event handle provided by DLL
            int tempInt = -1;
            status = CANDrive.XL_SetNotification(portHandle, ref tempInt, 1);
            xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

            Console.WriteLine("Set Notification      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Reset time stamp clock
            status = CANDrive.XL_ResetClock(portHandle);
            Console.WriteLine("Reset Clock           : " + status + "\n\n");
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Run Rx Thread
            Console.WriteLine("Start Rx thread...");
            rxThread = new Thread(new ThreadStart(RXThread));
            rxThread.Start();

            // User information
            Console.WriteLine("Press <ENTER> to transmit CAN messages \n  <b>, <ENTER> to block Rx thread for rx-overrun-test \n  <B>, <ENTER> burst of CAN TX messages \n  <x>, <ENTER> to exit");

            // Transmit CAN data
            while (true)
            {
                if (blockRxThread) Console.WriteLine("Rx thread blocked.");


                // Read user input
                string str = Console.ReadLine();
                if (str == "b") blockRxThread = !blockRxThread;
                else if (str == "B")
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        // Burst of CAN frames
                        CANTransmitDemo();
                    }
                }
                else if (str == "x") break;
                else
                {
                    // Send CAN frames
                    CANTransmitDemo();
                }
            }

            // Kill Rx thread
            rxThread.Abort();
            Console.WriteLine("Close Port                     : " + CANDrive.XL_ClosePort(portHandle));
            Console.WriteLine("Close Driver                   : " + CANDrive.XL_CloseDriver());

            return 0;
        }
        // -----------------------------------------------------------------------------------------------


        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message/exit in case of a functional call does not return XL_SUCCESS
        /// </summary>
            // -----------------------------------------------------------------------------------------------
        private static int PrintFunctionError()
        {
            Console.WriteLine("\nERROR: Function call failed!\nPress any key to continue...");
            Console.ReadKey();
            return -1;
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays the Vector Hardware Configuration.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private static void PrintConfig()
        {
            Console.WriteLine("\n\nAPPLICATION CONFIGURATION");

            foreach (int channelIndex in new int[] { txCi, rxCi })
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Configured Hardware Channel : " + driverConfig.channel[channelIndex].name);
                Console.WriteLine("Hardware Driver Version     : " + CANDrive.VersionToString(driverConfig.channel[channelIndex].driverVersion));
                Console.WriteLine("Used Transceiver            : " + driverConfig.channel[channelIndex].transceiverName);
            }

            Console.WriteLine("-------------------------------------------------------------------\n");
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message if channel assignment is not valid and popup VHwConfig, so the user can correct the assignment
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private static void PrintAssignErrorAndPopupHwConf()
        {
            Console.WriteLine("\nPlease check application settings of \"" + appName + " CAN1/CAN2\",\nassign them to available hardware channels and press enter.");
            CANDrive.XL_PopupHwConfig();
            Console.ReadKey();
        }
        // -----------------------------------------------------------------------------------------------

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieve the application channel assignment and test if this channel can be opened
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private static bool GetAppChannelAndTestIsOk(uint appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XLDefine.XL_Status status = CANDrive.XL_GetApplConfig(appName, appChIdx, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                Console.WriteLine("XL_GetApplConfig      : " + status);
                PrintFunctionError();
            }

            chMask = CANDrive.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel);
            chIdx = CANDrive.XL_GetChannelIndex(hwType, (int)hwIndex, (int)hwChannel);
            if (chIdx < 0 || chIdx >= driverConfig.channelCount)
            {
                // the (hwType, hwIndex, hwChannel) triplet stored in the application configuration does not refer to any available channel.
                return false;
            }

            // test if CAN is available on this channel
            return (driverConfig.channel[chIdx].channelBusCapabilities & XLDefine.XL_BusCapabilities.XL_BUS_ACTIVE_CAP_CAN) != 0;
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Sends some CAN messages.
        /// </summary>
        // ----------------------------------------------------------------------------------------------- 
        public static void CANTransmitDemo()
        {
            XLDefine.XL_Status txStatus;

            // Create an event collection with 2 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(2);

            // event 1
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = 0x100;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = 8;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[0] = 1;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[1] = 2;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[2] = 3;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[3] = 4;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[4] = 5;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[5] = 6;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[6] = 7;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[7] = 8;
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;

            // event 2
            xlEventCollection.xlEvent[1].tagData.can_Msg.id = 0x200;
            xlEventCollection.xlEvent[1].tagData.can_Msg.dlc = 8;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[0] = 9;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[1] = 10;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[2] = 11;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[3] = 12;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[4] = 13;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[5] = 14;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[6] = 15;
            xlEventCollection.xlEvent[1].tagData.can_Msg.data[7] = 16;
            xlEventCollection.xlEvent[1].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;


            // Transmit events
            txStatus = CANDrive.XL_CanTransmit(portHandle, txMask, xlEventCollection);
            Console.WriteLine("Transmit Message      : " + txStatus);
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// EVENT THREAD (RX)
        /// 
        /// RX thread waits for Vector interface events and displays filtered CAN messages.
        /// </summary>
        // ----------------------------------------------------------------------------------------------- 
        public static void RXThread()
        {
            // Create new object containing received data 
            XLClass.xl_event receivedEvent = new XLClass.xl_event();

            // Result of XL Driver function calls
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;


            // Note: this thread will be destroyed by MAIN
            while (true)
            {
                // Wait for hardware events
                if (xlEvWaitHandle.WaitOne(1000))
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...block RX thread to generate RX-Queue overflows
                        while (blockRxThread) { Thread.Sleep(1000); }

                        // ...receive data from hardware.
                        xlStatus = CANDrive.XL_Receive(portHandle, ref receivedEvent);

                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                        {
                            if ((receivedEvent.flags & XLDefine.XL_MessageFlags.XL_EVENT_FLAG_OVERRUN) != 0)
                            {
                                Console.WriteLine("-- XL_EVENT_FLAG_OVERRUN --");
                            }

                            // ...and data is a Rx msg...
                            if (receivedEvent.tag == XLDefine.XL_EventTags.XL_RECEIVE_MSG)
                            {
                                if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_OVERRUN) != 0)
                                {
                                    Console.WriteLine("-- XL_CAN_MSG_FLAG_OVERRUN --");
                                }

                                // ...check various flags
                                if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                    == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                {
                                    Console.WriteLine("ERROR FRAME");
                                }

                                else if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                                    == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                                {
                                    Console.WriteLine("REMOTE FRAME");
                                }

                                else
                                {
                                    Console.WriteLine(CANDrive.XL_GetEventString(receivedEvent));
                                }
                            }
                        }
                    }
                }
                // No event occurred
            }
        }
        // -----------------------------------------------------------------------------------------------
    }
}
