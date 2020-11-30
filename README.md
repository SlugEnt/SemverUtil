# SemverUtil
App / Library for working with Semantic Version in relation to file system / directory version folders.

For instance,say you have a folder for an app that is used to store the latest version of an app, in fact it stores multiple versions of the app (maybe for deployement):  C:\apps\<appName>\
In that folder are folders named:
 Ver1.0.0
 Ver1.0.1-alpha.2
 Ver1.0.1-beta.4
 Ver1.0.1
 Ver1.2.0-rc.3

The SemVerUtil class allows you to work with those versioned folders as follows:

			FileSystem fileSystem = new System.IO.Abstractions.FileSystem();
			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\apps\appname","Ver")
			FileSemVer fileSemVer = semVerUtil.DirectoryMostRecentVersion();
			Console.Writeline("Most Recent: " + fileSemVer.Version);

Will output:   Most Recent: 1.2.0-rc.3

			List<FileSemVer> versions = semVerUtil.DirectoryNewestVersions(4);
Will return the 4 most recent versions in descending order:
 Ver1.2.0-rc.3
 Ver1.0.1
 Ver1.0.1-beta.4
 Ver1.0.1-alpha.2

## Check Out the Tests for more functional examples



