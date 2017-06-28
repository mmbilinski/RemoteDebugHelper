# RemoteDebugHelper

## Usage

    RemoteDebugHelper <-s dev|env> [-m start|finish]

* ```-s``` (required) indicates a side where app is run:
  * ```dev``` - developers machine
  * ```env``` - production/test (target) environment
* ```-m``` chooses run mode for ```env``` side:
  * ```start``` - starts debugging session
  * ```finish``` - ends debugging session
 
## Sample configuration

For running RemoteDebugHelper there is needed ```LocalConfiguration.config``` file present in application's folder.
Sample content:

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
</appSettings>
```
