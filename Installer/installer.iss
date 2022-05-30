#define MyAppName "C Double Flat"
#define MyAppVersion "2.3"
#define MyAppPublisher "Hababisoft Corporation"
#define MyAppURL "https://githababi.github.io/"
#define MyAppExeName "cbb.exe"
#define MyAppAssocName "Cbb Script"
#define MyAppAssocExt ".cbb"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{1ED8E767-7440-4A0D-A4D2-BCDF0BD4F8EF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
ChangesAssociations=yes
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=..\Installer
OutputBaseFilename=cbb-2.3.0-installer
SetupIconFile=..\Assets\cbb_logo.ico
Compression=lzma
SolidCompression=yes                
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\C Double Flat.exe"; DestName: "cbb.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\lib\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\lib\StandardLibrary.dll*"; DestDir: "{app}\lib"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\clrcompression.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\clrjit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\coreclr.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\mscordaccore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\C-Double-Flat\bin\Release\net5.0\publish\aspnetcorev2_inprocess.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".myp"; ValueData: ""
; My attempt at making an app execution alias
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\App Paths\cbb.exe"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName}";
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\App Paths\cbb.exe"; ValueType: string; ValueName: "Path"; ValueData: "{app}";
Root: HKCU; Subkey: "Environment"; ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\cbb.exe"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName}"; Flags: noerror 
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\cbb.exe"; ValueType: string; ValueName: "Path"; ValueData: "{app}"; Flags: noerror 
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; Flags: noerror

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

