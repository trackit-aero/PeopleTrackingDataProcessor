Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("EnTrackxDataProcessor")>
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyCompany("")>
<Assembly: AssemblyProduct("EnTrackxDataProcessor")>
<Assembly: AssemblyCopyright("Copyright ©  2024")>
<Assembly: AssemblyTrademark("")> 

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("eedb6410-3757-4310-8d4c-ea61c753f208")>

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers 
' by using the '*' as shown below:
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion("1.0.0.0")> 
<Assembly: AssemblyFileVersion("1.0.0.0")>
<Assembly: log4net.Config.XmlConfiguratorAttribute(ConfigFile:="DataProcessor.xml", Watch:=True)>