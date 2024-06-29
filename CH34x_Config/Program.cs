// See https://aka.ms/new-console-template for more information
using CH34x_Config;
using Newtonsoft.Json;

class Program
{
    /// <summary>
    /// CH34x EEPROM Configuration App
    /// </summary>
    /// <param name="target">COM Port</param>
    /// <param name="v">Print verbose.</param>
    /// <param name="p">Print chip property. Then exit.</param>
    /// <param name="r">Print chip configuration. Then exit.</param>
    /// <param name="w">Write chip configuration.</param>
    /// <param name="vid">USB VID(Hex)</param>
    /// <param name="pid">USB PID(Hex)</param>
    /// <param name="useSn">Use EEPROM Serial</param>
    /// <param name="maxPower">Max Power (mA)</param>
    /// <param name="serial">Serial String(8)</param>
    /// <param name="product">Product String(18)</param>
    /// <param name="dryRun">Do not perform write action.</param>
    /// <returns></returns>
    public static int Main
        (string target = "COM7",
        bool v = false,
        bool p = false,
        bool r = false,
        bool w = false,
        string? vid = null,
        string? pid = null,
        bool? useSn = null,
        string? maxPower = null,
        string? serial = null,
        string? product = null,
        bool? dryRun = false) {

        if (v) { Console.WriteLine("Target port: " + target); };

        if (v) { Console.WriteLine("Library version: " + CH34x.GetLibraryVersion()); };

        CH34x ch34x;
        try
        {
            ch34x = new CH34x(target);
        }
        catch
        {
            Console.WriteLine("The specified device is not CH34x. Exit...");

            return -1;
        }

        // property
        if (p)
        {
            Console.WriteLine("Property:");
            pp(ch34x.property);
            return 0;
        }

        // read EEPROM
        if (r)
        {
            ch34x.ReadChipConfiguration();

            if (ch34x.configReadSuccess)
            {
                Console.WriteLine("Configuration read is success!");

                Console.WriteLine("\nChip Original Configuration:");
                pp(ch34x.configuration);
                Console.WriteLine(BitConverter.ToString(ch34x.configuration.ToBytes()).Replace("-", " "));
            }
            else
            {
                Console.WriteLine("Configuration read is failed!");
                return -1;
            }

            return 0;
        }

        // write EEPROM
        if (w)
        {
            ch34x.ReadChipConfiguration();
            if (ch34x.configReadSuccess)
            {
                Console.WriteLine("Configuration read is success!");
            }
            else
            {
                Console.WriteLine("Configuration read is failed!");
                //return -1;
            }

            if (vid != null)
                ch34x.SetVid(UInt16.Parse(vid, System.Globalization.NumberStyles.HexNumber));
            if (pid != null)
                ch34x.SetPid(UInt16.Parse(pid, System.Globalization.NumberStyles.HexNumber));
            if (useSn != null)
            {
                if (useSn == true)
                {
                    ch34x.EnableSerialNumber();
                }
                else
                {
                    ch34x.DisableSerialNumber();
                }
            }

            if (maxPower != null)
                ch34x.SetMaxPower(UInt16.Parse(maxPower));
            if (serial != null)
                ch34x.SetSerialNumber(serial);
            if (product != null)
                ch34x.SetProductString(product);


            Console.WriteLine("\nConfiguration to be written:");
            pp(ch34x.configuration);
            Console.WriteLine("\nConfiguration(In Hex) to be written:");
            pb(ch34x.configuration.ToBytes());
            var tbw = ch34x.configuration.ToBytes();

            if (dryRun == false)
            {
                ch34x.SaveChipConfiguration();

                ch34x.ReadChipConfiguration();
                if (ch34x.configReadSuccess)
                {
                    Console.WriteLine("Configuration read is success!");
                }
                else
                {
                    Console.WriteLine("Configuration read is failed!");
                    return -1;
                }

                Console.WriteLine("\nWrote configuration:");
                pp(ch34x.configuration);
                Console.WriteLine("\nWrote configuration(In Hex):");
                pb(ch34x.configuration.ToBytes());

                if(ch34x.configuration.ToBytes() != tbw)
                {
                    Console.WriteLine("Wrote configuration is different!!");
                    return -1;
                }else
                {
                    Console.WriteLine("Verification Success.");
                }
            }
            else
            {
                Console.WriteLine("Do not perform write action when --dry-run is set.");
            }

            return 0;
        }

        // print help when not executed anything
        System.CommandLine.DragonFruit.CommandLine.ExecuteAssembly(typeof(AutoGeneratedProgram).Assembly, new string[] { "-h" }, "");
        return 0;
    }

    private static void pp(Object obj)
    {
        Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
    }

    private static void pb(byte[] bytes)
    {
        Console.WriteLine(BitConverter.ToString(bytes).Replace("-", " "));
    }
}