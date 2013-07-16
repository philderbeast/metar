# Metar #

Parsing METAR using FParsec - in progress and not complete

## Errors ##

This project was developed a beta of Visual Studio 11, dotNet 4.5 and F# 3.0. When using F# interactive this error occurs ...

`error FS0193: Could not load file or assembly 'FSharp.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies. The system cannot find the file specified.`

It can be fixed by having the IDE use a local copy of F# interactive with an fsi.exe.config that uses an updated assembly binding ...

        <?xml version="1.0" encoding="utf-8"?>
        <configuration>
          <runtime>
            <legacyUnhandledExceptionPolicy enabled="true" />
            <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
              <dependentAssembly>
                <assemblyIdentity name="FSharp.Core" publicKeyToken="b03f5f7f11d50a3a"
                                  culture="neutral"/>
                <bindingRedirect oldVersion="4.0.0.0" newVersion="4.3.0.0"/>
              </dependentAssembly>
            </assemblyBinding>
          </runtime>
        </configuration>

On my system fsi.exe can be found at `C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0`.