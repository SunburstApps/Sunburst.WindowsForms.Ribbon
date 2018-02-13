# Sunburst.WindowsForms.Ribbon

This repository contains a Windows Forms-based library for interoperating with the UIRibbon feature in Windows 7 and later.
The client API is a minimally modified copy of [RibbonLib](https://github.com/ennerperez/RibbonLib), as well as a set of
custom-written MSBuild tasks that compile the ribbon XML into the binary DLL required by the native framework. Unlike with
upstream RibbonLib, this library is fully compatible with the Windows Desktop Bridge (Project Centennial).

## Using Sunburst.WindowsForms.Ribbon

To use this framework, add a NuGet package reference to `Sunburst.WindowsForms.Ribbon`. Set the ribbon XML files to have
a "Build Action" of `RibbonDefinition`; the MSBuild tasks will take care of compiling it as required. Note that the Visual C++
tools must be installed locally for these tasks to succeed.

## License

As with RibbonLib, this library is licensed [under the terms of the Ms-PL](./LICENSE.txt).
