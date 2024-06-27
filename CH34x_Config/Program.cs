// See https://aka.ms/new-console-template for more information
using CH34x_Config;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System.Text;

string target = args.Length > 0 ? args[0] : "COM7";
Console.WriteLine("Target port: " + target);

uint ver = CH343PT.CH343PT_GetVersion();
Console.WriteLine("DLL ver: " + ver/16 + "." + ver%16);

// No need to use Name
//bool isCH34x_Name = CH343PT.CH343PT_NameIsCH34x(target);
//Console.WriteLine("isCH34X: " + isCH34x_Name);

using (SafeFileHandle com_handle = File.OpenHandle(path: target, mode: FileMode.Open, access: FileAccess.ReadWrite))
{
    bool isCH34x_Handle = CH343PT.CH343PT_HandleIsCH34x(com_handle);
    Console.WriteLine("isCH34X: " + isCH34x_Handle);
    byte result = CH343PT.CH343PT_GetChipProperty(com_handle, out CH343PT.ChipPropertyS chipProperty);
    Console.WriteLine("res:" + result);
    Console.WriteLine("prop:" + JsonConvert.SerializeObject(chipProperty, Formatting.Indented));

    bool isEnterConfigMode = CH343PT.CH343PT_EnterConfigMode(com_handle);
    Console.WriteLine(isEnterConfigMode);
    uint blen = 256;
    byte[] buff = new byte[blen];
    CH343PT.CH343PT_ReadDevConfig(com_handle, ref blen, buff);

    byte bbuff = (byte)'\0';
    CH343PT.CH343PT_ReadCfgEeprom_Byte(com_handle, 0, ref bbuff);
    CH343PT.CH343PT_ExitConfigMode(com_handle);

    Console.WriteLine(buff.ToString());
}
