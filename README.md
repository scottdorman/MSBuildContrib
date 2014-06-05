# MSBuildContrib [![Build status](https://ci.appveyor.com/api/projects/status/lx7xgx0sx152177n)](https://ci.appveyor.com/project/scottdorman/msbuildcontrib)

## What is MSBuildContrib?
While MSBuild is a fairly complete platform, there are always tasks and tools that are available
in other build systems (like NAnt) that haven't made their way in to MSBuild. This project attempts
to solve that deficiency. The initial set of tasks and tools are rewritten versions of the equivalent
NAnt or NAntContrib tasks and tools that are missing. These are not simply ports of the code but rather
complete rewrites using newer language features and adhering to the MSBuild preferences for how
tasks behave.

## What tasks are available?

_Full task documentation will be coming soon._

<table><tr><th> Task </th><th> Summary </th><th> Notes </th></tr>
<tr><td> Attrib </td><td> Changes the file attributes of a file or set of files and directories. </td><td> </td></tr>
<tr><td> CheckDiskspace </td><td> Reports any local fixed disks that are less than the minimum available space. </td><td> The default minimum space available is 3GB. </td></tr>
<tr><td> Checksum </td><td> Calculates checksums for a set of files. Loosely based on Ant&#39;s Checksum task. </td><td> </td></tr>
<tr><td> CodeStats </td><td> Generates statistics from source code. </td><td> </td></tr>
<tr><td> Concat </td><td> A task that concatenates a set of files. Loosely based on Ant&#39;s Concat task. </td><td> </td></tr>
<tr><td> CreateItemRegex </td><td> Sets project properties based on the evaluatuion of a regular expression. </td><td> </td></tr>
<tr><td> FxCop </td><td> Analyzes managed code assemblies and reports information about the assemblies, such as possible design, localization, performance, and security improvements. </td><td> </td></tr>
<tr><td> GacUtil </td><td> Installs or uninstalls assemblies into the Global Assembly Cache (GAC) by using the gacutil SDK tool. </td><td> </td></tr>
<tr><td> GetEnvironment </td><td> Sets properties with system information. </td><td> </td></tr>
<tr><td> Grep </td><td> Searches files for a regular-expression and produces an XML report of the matches. </td><td> </td></tr>
<tr><td> Move </td><td> Moves a file or set of files to a new file or directory. </td><td> </td></tr>
<tr><td> UpdateItemMetadata </td><td> Adds or updates metadata to an item. </td><td> </td></tr></table>


### What is MSBuild?
The Microsoft Build Engine is a platform for building applications. This engine, which is also
known as MSBuild, provides an XML schema for a project file that controls how the build platform
processes and builds software. Visual Studio uses MSBuild, but it doesn't depend on Visual Studio. 
By invoking msbuild.exe on your project or solution file, you can orchestrate and build products
in environments where Visual Studio isn't installed.

Visual Studio uses MSBuild to load and build managed projects. The project files in Visual Studio
(.csproj,.vbproj, vcxproj, and others) contain MSBuild XML code that executes when you build a 
project by using the IDE. Visual Studio projects import all the necessary settings and build 
processes to do typical development work, but you can extend or modify them from within Visual
Studio or by using an XML editor.

You can get more information about MSBuild from the [Microsoft Developer Network](http://msdn.microsoft.com/en-us/library/dd393574.aspx).

## Contributing
If you don't see a task here, feel free to open an issue or fork this repository, add it, and then submit a pull request. _More detailed instructions on how to contribute will be coming soon._

## Bugs and feature requests

Have a bug or a feature request? [Please open a new issue](https://github.com/scottdorman/MSBuildContrib/issues). Before opening any issue, please search for existing issues and read the [Issue Guidelines](https://github.com/necolas/issue-guidelines), written by [Nicolas Gallagher](https://github.com/necolas/).


## Versioning

For transparency and insight into our release cycle, and for striving to maintain backward compatibility, MSBuildContrib will be maintained under the Semantic Versioning guidelines as much as possible.

Releases will be numbered with the following format:

`<major>.<minor>.<patch>`

And constructed with the following guidelines:

* Breaking backward compatibility bumps the major (and resets the minor and patch)
* New additions without breaking backward compatibility bumps the minor (and resets the patch)
* Bug fixes and misc changes bumps the patch

For more information on SemVer, please visit [http://semver.org/](http://semver.org/).



## Authors

**Scott Dorman**

+ [http://twitter.com/sdorman](http://twitter.com/sdorman)
+ [http://github.com/scottdorman](http://github.com/scottdorman)


## Copyright and license

Copyright 2013 Scott Dorman under [the GPL V3 license](LICENSE).
