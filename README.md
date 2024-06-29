# CH34x_Config

* EEPROM configuration tool for CH340B

```
Description:
  CH34x EEPROM Configuration App

Usage:
  CH34x_Config [options]

Options:
  --target <target>        COM Port
  -v                       Print verbose. [default: False]
  -p                       Print chip property. Then exit. [default: False]
  -r                       Print chip configuration. Then exit. [default: False]
  -w                       Write chip configuration. [default: False]
  --vid <vid>              USB VID(Hex) []
  --pid <pid>              USB PID(Hex) []
  --use-sn                 Use EEPROM Serial [True|False] []
  --max-power <max-power>  Max Power (mA) []
  --serial <serial>        Serial String(Length: 1-8) []
  --product <product>      Product String(Length: 1-18) []
  --clear-product          Clear Product String [default: False]
  --dry-run                Do not perform write action. [default: False]
  --version                Show version information
  -?, -h, --help           Show help and usage information
```

* License: MIT
* NOTE: [CH343PT.DLL](CH34x_Config/CH343PT.DLL) is copyrighted material owned by Nanjing Qinheng Microelectronics and is not covered by the license of this repository.
