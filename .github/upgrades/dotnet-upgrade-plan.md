# .NET 8.0-windows Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade LawlerBallisticsDesk\LawlerBallisticsDesk.csproj to net8.0-windows

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

| Project name                                   | Description                                        |
|:-----------------------------------------------|:---------------------------------------------------|
| MVVMtools.csproj                               | External dependency (located outside solution)     |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                        | Current Version | New Version | Description                                                    |
|:----------------------------------------------------|:---------------:|:-----------:|:---------------------------------------------------------------|
| CommunityToolkit.Mvvm                               |                 | 8.4.0       | Replacement for MvvmLight and MvvmLightLibs                    |
| Microsoft.Bcl.AsyncInterfaces                       | 10.0.4          | 8.0.0       | Recommended for .NET 8.0                                       |
| Microsoft.Maps.MapControl.WPF                       | 1.0.0.3         |             | No supported version found - needs removal or alternative      |
| Microsoft.NETFramework.ReferenceAssemblies          | 1.0.3           |             | Not needed for .NET 8.0                                        |
| Microsoft.NETFramework.ReferenceAssemblies.net472   | 1.0.3           |             | Not needed for .NET 8.0                                        |
| MvvmLight                                           | 5.4.1.1         |             | Deprecated - replace with CommunityToolkit.Mvvm                |
| MvvmLightLibs                                       | 5.4.1.1         |             | Deprecated - replace with CommunityToolkit.Mvvm                |
| OxyPlot.Wpf                                         | 2.2.0           | 2.1.2       | Incompatible - downgrade to compatible version                 |
| OxyPlot.Wpf.Shared                                  | 2.2.0           | 2.1.2       | Incompatible - downgrade to compatible version                 |
| System.Buffers                                      | 4.6.1           |             | Functionality included with .NET 8.0 framework                 |
| System.ComponentModel.Annotations                   | 5.0.0           |             | Functionality included with .NET 8.0 framework                 |
| System.Diagnostics.Debug                            | 4.3.0           |             | Functionality included with .NET 8.0 framework                 |
| System.Diagnostics.DiagnosticSource                 | 10.0.4          | 8.0.1       | Recommended for .NET 8.0                                       |
| System.Memory                                       | 4.6.3           |             | Functionality included with .NET 8.0 framework                 |
| System.Numerics.Vectors                             | 4.6.1           |             | Functionality included with .NET 8.0 framework                 |
| System.Reflection.Emit                              | 4.7.0           |             | Functionality included with .NET 8.0 framework                 |
| System.Runtime.InteropServices.RuntimeInformation   | 4.3.0           |             | Functionality included with .NET 8.0 framework                 |
| System.Threading.Channels                           | 10.0.4          | 8.0.0       | Recommended for .NET 8.0                                       |
| System.Threading.Tasks.Extensions                   | 4.6.3           |             | Functionality included with .NET 8.0 framework                 |
| System.ValueTuple                                   | 4.6.2           |             | Functionality included with .NET 8.0 framework                 |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### LawlerBallisticsDesk.csproj modifications

Project properties changes:
- Convert from legacy project format to SDK-style project
- Target framework should be changed from `net472` to `net8.0-windows`

NuGet packages changes:
- MvvmLight should be removed and replaced with `CommunityToolkit.Mvvm` version `8.4.0` (*deprecated*)
- MvvmLightLibs should be removed and replaced with `CommunityToolkit.Mvvm` version `8.4.0` (*deprecated*)
- Microsoft.Maps.MapControl.WPF `1.0.0.3` should be removed (*no supported version found*)
- Microsoft.NETFramework.ReferenceAssemblies `1.0.3` should be removed (*not needed for .NET 8.0*)
- Microsoft.NETFramework.ReferenceAssemblies.net472 `1.0.3` should be removed (*not needed for .NET 8.0*)
- OxyPlot.Wpf should be updated from `2.2.0` to `2.1.2` (*compatible version*)
- OxyPlot.Wpf.Shared should be updated from `2.2.0` to `2.1.2` (*compatible version*)
- Microsoft.Bcl.AsyncInterfaces should be updated from `10.0.4` to `8.0.0` (*recommended for .NET 8.0*)
- System.Diagnostics.DiagnosticSource should be updated from `10.0.4` to `8.0.1` (*recommended for .NET 8.0*)
- System.Threading.Channels should be updated from `10.0.4` to `8.0.0` (*recommended for .NET 8.0*)
- System.Buffers `4.6.1` should be removed (*included with framework*)
- System.ComponentModel.Annotations `5.0.0` should be removed (*included with framework*)
- System.Diagnostics.Debug `4.3.0` should be removed (*included with framework*)
- System.Memory `4.6.3` should be removed (*included with framework*)
- System.Numerics.Vectors `4.6.1` should be removed (*included with framework*)
- System.Reflection.Emit `4.7.0` should be removed (*included with framework*)
- System.Runtime.InteropServices.RuntimeInformation `4.3.0` should be removed (*included with framework*)
- System.Threading.Tasks.Extensions `4.6.3` should be removed (*included with framework*)
- System.ValueTuple `4.6.2` should be removed (*included with framework*)

Other changes:
- Code changes may be required to migrate from MvvmLight to CommunityToolkit.Mvvm
- Code changes may be required if Microsoft.Maps.MapControl.WPF functionality is used (need alternative mapping solution)
