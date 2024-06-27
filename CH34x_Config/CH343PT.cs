

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace CH34x_Config
{
    internal class CH343PT
    {
        public const string DLL_NAME = "CH343PT.DLL";

        [StructLayout(LayoutKind.Sequential)]
        public struct ChipPropertyS
        {
            public byte ChipType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ChipTypeStr;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FwVerStr;
            public byte GpioCount;
            [MarshalAs(UnmanagedType.Bool)]
            public bool IsEmbbedEeprom;
            [MarshalAs(UnmanagedType.Bool)]
            public bool IsSupportMcuBootCtrl;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string ManufacturerString;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string ProductString;
            public ushort bcdDevice;
            public byte PortIndex;
            public bool IsSupportGPIOInit;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string PortName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] ResvD;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class USERCFG_340
        {
            public byte SIG = 0x5b;
            public byte MODE = 0x23;
            public byte CFG = 0;
            public byte WP = 0;
            public ushort Vid = 0;
            public ushort Pid = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string Reserve1 = "";
            public byte Power = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string Reserve2 = "";
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string SN = "";
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string Reserve3 = "";
            public byte PROD_LEN = 0;
            public byte PROD_HDR_03H = 0x3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
            public string PROD = "";
        }

        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern uint CH343PT_GetVersion();

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_NameIsCH34x(string iPortName);

        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_HandleIsCH34x(SafeFileHandle iPortHandle);

        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_EnterConfigMode(SafeFileHandle iPortHandle);

        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_ExitConfigMode(SafeFileHandle iPortHandle);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern byte CH343PT_GetChipProperty(SafeFileHandle iPortHandle, out ChipPropertyS chipPropertyS);

        [DllImport (DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_ReadDevConfig(SafeFileHandle iPortHandle, [MarshalAs(UnmanagedType.U4)] ref uint DataLen, [MarshalAs(UnmanagedType.LPArray)] byte[] DataBuf);


        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_WriteDevConfig(SafeFileHandle iPortHandle, [MarshalAs(UnmanagedType.U4)] uint BufferSize, [MarshalAs(UnmanagedType.LPArray)] byte[] DataBuf);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_ReadCfgEeprom_Byte(SafeFileHandle iPortHandle, [MarshalAs(UnmanagedType.U4)] uint iAddr, ref byte DataBuf);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_WriteCfgEeprom_Byte(SafeFileHandle iPortHandle, [MarshalAs(UnmanagedType.U4)] uint iAddr, byte Data);
    }
}
