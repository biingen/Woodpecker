using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using Microsoft.Win32.SafeHandles;           //support SafeFileHandle
using System.Runtime.InteropServices;      //support DIIImport
using System.Reflection;                                //support BindingFlags
using jini;
using OPTT;

namespace ModuleLayer
{
    public class OperationalStates
    {
        //private int iPortNmuber = 0;
        private SerialPort _serialPort = new SerialPort();
        private Stream _internalSerialStream;

        
        //Read data Into buffer
        public int ReadTerm(Byte[] ResultDataBuf, ref int Count, char TermByte)
        {
            int DataLen = _serialPort.BytesToRead;
            Count = 0;

            if (DataLen >= 1)
            {
                for (int i = 0; i <= (DataLen - 1); i++)
                {
                    
                    ResultDataBuf[i] = (Byte)_serialPort.ReadByte();
                    Count++;

                    if (ResultDataBuf[i] == TermByte)
                    {
                        Array.Resize(ref ResultDataBuf, (i+1));
                        return 1;
                    }
                }

                return -1;
            }
            return -1;

        }
           

    }
}
