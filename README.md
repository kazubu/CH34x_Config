# CH34x_Config

* EEPROM configuration tool for CH340B

```
Description:
  CH34x EEPROM Configuration App

Usage:
  CH34x_Config [options]

Options:
  --target <target>        COM Port [default: COM7]
  -v                       Print verbose. [default: False]
  -p                       Print chip property. Then exit. [default: False]
  -r                       Print chip configuration. Then exit. [default: False]
  -w                       Write chip configuration. [default: False]
  --vid <vid>              USB VID(Hex) []
  --pid <pid>              USB PID(Hex) []
  --use-sn                 Use EEPROM Serial []
  --max-power <max-power>  Max Power (mA) []
  --serial <serial>        Serial String(8) []
  --product <product>      Product String(18) []
  --dry-run                Do not perform write action. [default: False]
  --version                Show version information
  -?, -h, --help           Show help and usage information
```

* License: MIT
