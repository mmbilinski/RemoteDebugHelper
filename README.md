# RemoteDebugHelper

## Usage

    RemoteDebugHelper <-s dev|env> [-m start|finish|interactive]

* ```-s``` (required) indicates a side where app is run:
  * ```dev``` - developers machine
  * ```env``` - production/test (target) environment
* ```-m``` chooses run mode for ```env``` side:
  * ```start``` - starts debugging session
  * ```finish``` - ends debugging session
  * ```interactive``` - starts interactive debugging session (```start``` with ```finish``` after Ctrl+C)
 
## Sample configuration

Since v0.2.0, configuration is loaded from three locations in order:
* LocalConfiguration.config, located next to executable file,
* RemoteDebugHelperConfig.json, located next to executable file,
* RemoteDebugHelperConfig.json, located in user's home directory.

Each location is optional, also settings from further locations will complement settings from previous ones.
It means, that each location could store some part of configuration (it could be intersecting sets).

### Settings keys

| Key | Description |
| --- | --- |
| LocalWebsiteBinDirectory | Directory with locally built binaries |
| RemoteWebsiteDirectory | Directory with remote binaries (ie. with IIS running) |
| IntermediateZipDirectory | Directory for storing temporary ZIP file |
| TransferredExtensions | Extensions of copied files |
| BinDirectoryNameForProductionBinaries | Name of directory with original remote binaries |
| TryRestoreDebugBinariesWhenZipNotFound | Recovery action when intermediate ZIP is not present. Not implemented at the moment |
| IisAppPoolName | Name of IIS AppPool |
| UserInitials | User initials for creating directories |
| KeepRemoteBinWithInitials | Keeping debug binaries after debugging session |
| RemoteDebuggerPath | Path of remote debugging monitor |
| RemoteDebuggerParameters | Parameters passed to remote debugging monitor |
| RemoteDebuggerMinimized | Run remote debugging monitor minimized |
| IncludeOnlyFilesModifiedInLastNDays | Include only files modified in last N days. Set -1 to include all files |
| AutoCloseApp | Do not wait for user confirmation after completing actions |

### Sample content of LocalConfiguration.config:

```
<appSettings>
  <add key="LocalWebsiteBinDirectory" value="c:\inetpub\wwwroot\AppPoolName\Website\bin\" />
  <add key="RemoteWebsiteDirectory" value="c:\inetpub\wwwroot\AppPoolName\Website\" />
  <add key="IntermediateZipDirectory" value="x:\MyNetworkFolder" />
  <add key="TransferredExtensions" value=".dll|.pdb" />
  <add key="BinDirectoryNameForProductionBinaries" value="binProd" />
  <add key="TryRestoreDebugBinariesWhenZipNotFound" value="False" />
  <add key="IisAppPoolName" value="AppPoolName" />
  <add key="UserInitials" value="XXX" />
  <add key="KeepRemoteBinWithInitials" value="False" />
  <add key="RemoteDebuggerPath" value="C:\Program Files\Microsoft Visual Studio 15.0\Common7\IDE\Remote Debugger\x64\msvsmon.exe" />
  <add key="RemoteDebuggerParameters" value="-port 12345" />
  <add key="RemoteDebuggerMinimized" value="True" />
  <add key="IncludeOnlyFilesModifiedInLastNDays" value="7" />
  <add key="AutoCloseApp" value="true" />
</appSettings>
```

### Sample content of RemoteDebugHelperConfig.json:

```
{
	"LocalWebsiteBinDirectory": "c:\\inetpub\\wwwroot\\AppPoolName\\Website\\bin\\",
	"RemoteWebsiteDirectory": "c:\\inetpub\\wwwroot\\AppPoolName\\Website\\",
	"IntermediateZipDirectory": "x:\\MyNetworkFolder",
	"TransferredExtensions": ".dll|.pdb",
	"BinDirectoryNameForProductionBinaries": "binProd",
	"TryRestoreDebugBinariesWhenZipNotFound": false,
	"IisAppPoolName": "AppPoolName",
	"UserInitials": "XXX",
	"KeepRemoteBinWithInitials": false,
	"RemoteDebuggerPath": "C:\\Program Files\\Microsoft Visual Studio 15.0\\Common7\\IDE\\Remote Debugger\\x64\\msvsmon.exe",
	"RemoteDebuggerParameters": "-port 12345",
	"RemoteDebuggerMinimized": true,
	"IncludeOnlyFilesModifiedInLastNDays": -2,
	"AutoCloseApp": true
}
```