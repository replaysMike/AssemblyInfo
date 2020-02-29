# AssemblyInfo

Provides assembly information for dll's and exe's in the windows explorer context menu as well as command line.

# Installation

Download the [latest release](https://github.com/replaysMike/AssemblyInfo/releases) of Assembly Info and install using the MSI package.

# Usage

Right-click on any assembly (dll, exe) and choose "Open with AssemblyInfo" to retrieve it's metadata. You can also use it via command line, or launch the AssemblyInfo.exe to be prompted via the UI to load an assembly file to inspect.

# Screenshot

![AnyConsole](https://github.com/replaysMike/AssemblyInfo/wiki/screenshot.png)

## Command-line usage

```
> AssemblyInfo.exe --filename C:\myassembly.dll --print --pretty
```

### Getting help:
```
> AssemblyInfo.exe --help
```

### Output in CSV format:
```
> AssemblyInfo.exe --filename C:\myassembly.dll --print
```
