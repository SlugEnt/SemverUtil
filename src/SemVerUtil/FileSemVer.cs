using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Semver;

namespace SemVerUtil
{

	public class FileSemVer : IComparable
	{
		/// <summary>
		/// Constructs a FileSemVer object
		/// </summary>
		/// <param fileName="fileName">the fileName of the item that contains the semver2 string</param>
		/// <param fileName="prefix">Any prefix that occurs before the semver2 portion of the string, for instance this might be a filename, app fileName or some version prefix that you use.</param>
		public FileSemVer (string fileName, string prefix = "") {
			if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("PathName cannot be null, must specify the fileName of a fully qualified path.");

			string semVerPart = "";
			string nameOnly = Path.GetFileName(fileName);

			if ( prefix != string.Empty ) {
				if ( !nameOnly.StartsWith(prefix)) 
					throw new ArgumentException(string.Format("PathName [{fileName}] did not start with the specified prefix [{prefix}]"));

				semVerPart = nameOnly.Substring(prefix.Length);
			}
			else
				semVerPart = nameOnly;


			FileName = fileName;
			Prefix = prefix;
			SemVerPart = semVerPart;
			SemVersion = SemVersion.Parse(semVerPart);

		}



		public string FileName { get; private set; }
		public string Prefix { get; private set; }
		public string SemVerPart { get; private set; }
		public SemVersion SemVersion { get; set; }


		/// <summary>
		/// 
		/// </summary>
		/// <param fileName="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			return ((IComparable)SemVersion).CompareTo(((FileSemVer)obj).SemVersion);
		}
	}
}
