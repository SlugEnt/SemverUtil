using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;
using NUnit.Framework.Api;
using SemVerUtil;

namespace Test_SemVer
{
	using XFS = MockUnixSupport;

	class SemVerUtilTest {
		private const string FIRST_VERSION = "0.0.4";
		private const string VERSION_2 = "1.2.5-alpha.14";
		private const string VERSION_3 = "1.2.5-alpha.a";
		private const string VERSION_4 = "1.2.5-beta.27";
		private const string VERSION_5 = "1.2.5-rc.2";
		private const string VERSION_6 = "1.2.5";
		private const string VERSION_7 = "1.2.6-fix.2";
		private const string VERSION_8 = "1.2.6-fix.3+somedata";
		private const string VERSION_9 = "1.2.6";
		private const string VERSION_10 = "1.2.15-beta.304";
		private const string VERSION_11= "1.2.15";
		private const string VERSION_12 = "1.3.0";
		private const string VERSION_13 = "2.6.104";
		private const string VERSION_14 = "3.0.0+somedata";
		private const string MAX_VERSION = "4.0.1";


		[SetUp]
		public void Setup()
		{ }



		/// <summary>
		/// Setup FileSystem
		/// </summary>
		/// <returns></returns>
		private MockFileSystem SetupFileSystem() {
			var fileSystem = new MockFileSystem();

			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + FIRST_VERSION);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_2);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_3);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_4);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_5);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_6);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_7);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_8);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_9);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_10);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_11);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_12);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_13);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_14);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + MAX_VERSION);
			
			fileSystem.AddFile(@"C:\Ver1.3.0\Ver.txt",new MockFileData("some data in a file"));
			return fileSystem;
		}


		private MockFileSystem SetupNoVersions () {
			var fileSystem = new MockFileSystem();
			return fileSystem;
		}



		[Test]
		public void MostRecentVersion () {
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryMostRecentVersion();
			Assert.AreEqual(MAX_VERSION,fileSemVer.SemVersion.ToString(),"A10:");
		}


		
		[Test]
		public void OldestVersion()
		{
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryOldestVersion();
			Assert.AreEqual(FIRST_VERSION, fileSemVer.SemVersion.ToString(), "A10:");
		}



		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[Test]
		public void NewestNVersions (int n) {
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			List<FileSemVer> newest =  semVerUtil.DirectoryNewestVersions(n);
			Assert.AreEqual(n, newest.Count,"A10:  Incorrect number of items");

			for ( int i = 0; i < n; i++ ) {
				switch ( i ) {
					case 0:
						Assert.AreEqual(MAX_VERSION, newest [i++].SemVersion.ToString(), "A20:  Newest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_14, newest[i++].SemVersion.ToString(), "A30:  2nd newest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_13, newest[i++].SemVersion.ToString(), "A40:  3rd newest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_12, newest[i++].SemVersion.ToString(), "A50:  4th newest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_11, newest[i++].SemVersion.ToString(), "A60:  5th newest item is incorrect");
						break;
				}
			}
		}


		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[Test]
		public void OldestNVersions(int n)
		{
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			List<FileSemVer> oldest = semVerUtil.DirectoryOldestVersions(n);
			Assert.AreEqual(n, oldest.Count, "A10:  Incorrect number of items");

			for (int i = 0; i < n; i++)
			{
				switch (i)
				{
					case 0:
						Assert.AreEqual(FIRST_VERSION, oldest[i++].SemVersion.ToString(), "A20:  Oldest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_2, oldest[i++].SemVersion.ToString(), "A30:  2nd Oldest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_3, oldest[i++].SemVersion.ToString(), "A40:  3rd oldest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_4, oldest[i++].SemVersion.ToString(), "A50:  4th oldest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_5, oldest[i++].SemVersion.ToString(), "A60:  5th oldest item is incorrect");
						break;
				}
			}
		}

		/// <summary>
		/// Tests that null is returned any time there are no versions in the internal list.
		/// </summary>
		[Test]
		public void NoVersions () {
			var fileSystem = SetupNoVersions();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryMostRecentVersion();
			Assert.IsNull(fileSemVer);

			fileSemVer = semVerUtil.DirectoryOldestVersion();
			Assert.IsNull(fileSemVer);

			List<FileSemVer> lists = semVerUtil.DirectoryNewestVersions(4);
			Assert.IsNull(lists);

			lists = semVerUtil.DirectoryOldestVersions(5);
			Assert.IsNull(lists);

			lists = semVerUtil.OldestWithMin(2);
			Assert.AreEqual(0,lists.Count);
		}



		[TestCase(12)]
		[TestCase(2)]
		[TestCase(5)]
		[Test]
		public void KeepNNewVersions (int n) {
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			int max = semVerUtil.VersionCount;

			List<FileSemVer> oldest = semVerUtil.OldestWithMin(n);
			Assert.AreEqual(max - n, oldest.Count, "A10:  Incorrect number of items");

			for (int i = 0; i < (max -n); i++)
			{
				switch ( i ) {
					case 0:
						Assert.AreEqual(FIRST_VERSION, oldest [i++].SemVersion.ToString(), "A20:  Oldest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_2, oldest [i++].SemVersion.ToString(), "A30:  2nd Oldest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_3, oldest [i++].SemVersion.ToString(), "A40:  3rd oldest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_4, oldest [i++].SemVersion.ToString(), "A50:  4th oldest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_5, oldest [i++].SemVersion.ToString(), "A60:  5th oldest item is incorrect");
						break;
					case 5:
						Assert.AreEqual(VERSION_6, oldest [i++].SemVersion.ToString(), "A60:  6th oldest item is incorrect");
						break;
					case 6:
						Assert.AreEqual(VERSION_7, oldest [i++].SemVersion.ToString(), "A60:  7th oldest item is incorrect");
						break;
					case 7:
						Assert.AreEqual(VERSION_8, oldest [i++].SemVersion.ToString(), "A60:  8th oldest item is incorrect");
						break;
					case 8:
						Assert.AreEqual(VERSION_9, oldest [i++].SemVersion.ToString(), "A60:  9th oldest item is incorrect");
						break;
					case 9:
						Assert.AreEqual(VERSION_10, oldest [i++].SemVersion.ToString(), "A60:  10th oldest item is incorrect");
						break;
					case 10:
						Assert.AreEqual(VERSION_11, oldest [i++].SemVersion.ToString(), "A60:  11th oldest item is incorrect");
						break;
					case 11:
						Assert.AreEqual(VERSION_12, oldest [i++].SemVersion.ToString(), "A60:  12th oldest item is incorrect");
						break;
					case 12:
						Assert.AreEqual(VERSION_13, oldest [i++].SemVersion.ToString(), "A60:  13th oldest item is incorrect");
						break;
					case 13:
						Assert.AreEqual(VERSION_14, oldest [i++].SemVersion.ToString(), "A60:  14th oldest item is incorrect");
						break;
					case 14:
						Assert.AreEqual(MAX_VERSION, oldest [i++].SemVersion.ToString(), "A60:  15th oldest item is incorrect");
						break;
					case 15:
						Assert.AreEqual(0, oldest.Count);
						break;
				}
			}
		}


		[TestCase(32)]
		[TestCase(20)]
		[Test]
		public void KeepNNewVersionsToMany (int n) {
			var fileSystem = SetupFileSystem();

			SemVerUtil.SemVerUtil semVerUtil = new SemVerUtil.SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			int max = semVerUtil.VersionCount;
			Assert.Greater(n,max,"A10:");
			List<FileSemVer> oldest = semVerUtil.OldestWithMin(n);
			Assert.AreEqual(0,oldest.Count,  "A20:  Incorrect number of items");
		}

	}
}
