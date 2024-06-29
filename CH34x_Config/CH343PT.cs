using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CH34x_Config
{
    internal class CH34x
    {
        public ChipPropertyS property { get; private set; }
        public USERCFG_340 configuration { get; private set; }
        public bool configReadSuccess { get; private set; } = false;

        private SafeFileHandle com_handle;
        private readonly string PortName;

        public CH34x(string portName)
        {
            PortName = portName;

            InitializeCOMPortHandle();
            if(com_handle == null)
            {
                throw new Exception("Failed to initialize COM Port Handle.");
            }
            
            property = GetChipProperty();
            if(property.ChipType != (byte)CH343PT.CHIP_TYPE.CH340)
            {
                throw new Exception("Not supported device detected:" + property.ChipTypeStr);
            }
            configuration = GetChipConfiguration();
        }

        private void InitializeCOMPortHandle()
        {
            if (com_handle == null || com_handle.IsClosed)
            {
                com_handle = File.OpenHandle(path: @"\\.\" + PortName, mode: FileMode.Open, access: FileAccess.ReadWrite);

                if (!CH343PT.CH343PT_HandleIsCH34x(com_handle))
                {
                    throw new Exception("Specified COM port is not CH34x");
                }
            }
        }

        public static string GetLibraryVersion()
        {
            var ver = CH343PT.CH343PT_GetVersion();
            return (ver / 16 + "." + ver % 16);
        }

        public void EnableSerialNumber()
        {
            configuration.CFG |= 0x20;
            configuration.CFG ^= 0x20;
        }
        public void DisableSerialNumber()
        {
            configuration.CFG |= 0x20;
        }

        public void SetMaxPower(int power)
        {
            if(power < 0 || power > 512)
            {
                throw new InvalidDataException("Max Power should be in range 1-511");
            }

            configuration.Power = (byte)(power / 2);
        }
        public void SetVid(UInt16 vid)
        {
            configuration.Vid[1] = (byte)(vid >> 8);
            configuration.Vid[0] = (byte)(vid % 0x100);
        }

        public void SetPid(UInt16 pid)
        {
            configuration.Pid[1] = (byte)(pid >> 8);
            configuration.Pid[0] = (byte)(pid % 0x100);
        }

        public void SetSerialNumber(string sn)
        {
            if(sn.Length > 8)
            {
                throw new InvalidDataException("Serial Number is max 8 bytes.");
            }

            char[] buf = new char[8];
            Array.Copy(sn.ToCharArray(), buf, sn.Length);
            configuration.SN = buf;
        }

        public void SetProductString(string product)
        {
            if(product.Length > 18)
            {
                throw new InvalidDataException("Product is max 18 characters.");
            }

            byte[] buf = new byte[36];
            var prodbytes = System.Text.Encoding.Unicode.GetBytes(product);
            Array.Copy(prodbytes, buf, prodbytes.Length);
            
            configuration.PROD_LEN = (byte)(prodbytes.Length + 2);
            configuration.PROD_HDR = 0x03;
            configuration.PROD = buf;
        }

        public void ClearProductString()
        {
            configuration.PROD_LEN = 0;
            configuration.PROD_HDR = 0;
            configuration.PROD = new byte[36];
        }

        public void ReadChipConfiguration()
        {
            InitializeCOMPortHandle();
            configuration = GetChipConfiguration();
        }

        public void SaveChipConfiguration()
        {
            InitializeCOMPortHandle();
            if (!CH343PT.CH343PT_EnterConfigMode(com_handle))
            {
                throw new Exception("Unknown error for entering configuration mode!!!");
            }

            try
            {
                var cfg = configuration.ToBytes();
            
                CH343PT.CH343PT_WriteDevConfig(com_handle, (uint)cfg.Length, cfg);
            }
            finally
            {
                CH343PT.CH343PT_ExitConfigMode(com_handle);
            }
        }

        private ChipPropertyS GetChipProperty()
        {
            InitializeCOMPortHandle();
            ChipPropertyS _chipProperty = new ChipPropertyS();
            CH343PT.CH343PT_GetChipProperty(com_handle, _chipProperty);

            return _chipProperty;
        }

        private USERCFG_340 GetChipConfiguration()
        {
            InitializeCOMPortHandle();
            if (!CH343PT.CH343PT_EnterConfigMode(com_handle))
            {
                throw new Exception("Unknown error for entering configuration mode!!!");
            }

            uint blen = 256;
            byte[] buff = new byte[blen];
            try
            {
                CH343PT.CH343PT_ReadDevConfig(com_handle, ref blen, buff);
            }
            finally
            {
                CH343PT.CH343PT_ExitConfigMode(com_handle);
            }

            try
            {
                configReadSuccess = true;
                return USERCFG_340.ParseBytes(buff);
            }
            catch
            {
                configReadSuccess = false;
                return new USERCFG_340();
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ChipPropertyS
    {
        public byte ChipType = 0xFF;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string ChipTypeStr = "";
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FwVerStr = "";
        public byte GpioCount = 0;
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsEmbbedEeprom = false;
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsSupportMcuBootCtrl = false;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ManufacturerString = "";
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ProductString = "";
        public ushort bcdDevice = 0;
        public byte PortIndex = 0;
        public bool IsSupportGPIOInit = false;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string PortName = "";
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] ResvD = new uint[8];
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public class USERCFG_340
    {
        public byte SIG = 0x5B;
        public byte MODE = 0x23;
        public byte CFG = 0xFE;
        public byte WP = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Vid = new byte [2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Pid = new byte[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reserve1 = new byte[2];
        public byte Power = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserve2 = new byte[5];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] SN = new char[8];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reserve3 = new byte[2];
        public byte PROD_LEN = 0;
        public byte PROD_HDR = 0x03;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public byte[] PROD = new byte[36];

        public static USERCFG_340 ParseBytes(byte[] data)
        {
            if(data.Length < 256)
            {
                throw new InvalidDataException("length is less than 256");
            }
            if (data[0] != 0x5B)
            {
                throw new InvalidDataException("Signature is invalid.");
            }

            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            USERCFG_340? result;
            try
            {
                result = (USERCFG_340)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(USERCFG_340));
            }
            finally
            {
                gch.Free();
            }

            return result;
        }

        public byte[] ToBytes()
        {
            int size = Marshal.SizeOf(typeof(USERCFG_340));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);

            byte[] bytes = new byte[size];
            try
            {
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return bytes;
        }
    }

    internal class CH343PT
    {
        public const string DLL_NAME = "CH343PT.DLL";

        [Flags]
        internal enum CHIP_TYPE : byte
        {
            Unknown = 0xFF,
            CH341 = 0x10,
            CH340 = 0x20,
            CH340K = 0x21,
            CH330 = 0x22,
            CH9340 = 0x23,
            CH9340K = 0x24,
            CH9340C = 0x25,
            CH34E = 0x26,
            CH34X = 0x27,
            CH343K = 0x30,
            CH343J = 0x31,
            CH343G = 0x32, //CH343G/P
            CH343P = 0x33,
            CH9101U = 0x40,
            CH9101H = 0x40,
            CH9101R = 0x41,
            CH9101Y = 0x41,
            CH9101N = 0x42,
            CH9102F = 0x50,
            CH9102X = 0x51,
            CH9103M = 0x60,
            CH342F = 0xA0,
            CH342K = 0xA1,
            CH342J = 0xA2,
            CH342G = 0xA3,
            CH347T = 0xA4,
            CH347F = 0xA5,
            CH9344 = 0xD0,
            CH344L = 0xD1,
            CH344Q = 0xD2,
            CH9104L = 0xD3,
            CH348L = 0xE0,
            CH348Q = 0xE1
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
        public static extern byte CH343PT_GetChipProperty(SafeFileHandle iPortHandle, [MarshalAs(UnmanagedType.LPStruct), Out] ChipPropertyS chipPropertyS);

        [DllImport (DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_ReadDevConfig(SafeFileHandle iPortHandle, ref uint DataLen, [MarshalAs(UnmanagedType.LPArray), Out] byte[] DataBuf);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_WriteDevConfig(SafeFileHandle iPortHandle, uint BufferSize, [MarshalAs(UnmanagedType.LPArray), In] byte[] DataBuf);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_ReadCfgEeprom_Byte(SafeFileHandle iPortHandle, uint iAddr, ref byte DataBuf);

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CH343PT_WriteCfgEeprom_Byte(SafeFileHandle iPortHandle, uint iAddr, byte Data);
    }
}
