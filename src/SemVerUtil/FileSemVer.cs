using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Semver;

namespace SlugEnt.SemVer
{

	public class FileSemVer : IComparable
	{
		/// <summary>
		/// Constructs a FileSemVer object
		/// </summary>
		/// <param name="fileName">FileName of the item that contains the semver2 string</param>
		/// <param name="prefix">Any prefix that occurs before the semver2 portion of the string, for instance this might be a filename, app fileName or some version prefix that you use.</param>
		public FileSemVer (string fileName, string prefix = "") {
			if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("PathName cannot be null, must specify the fileName of a fully qualified path.");

			string semVerPart;
			string nameOnly = Path.GetFileName(fileName);

			if ( prefix != string.Empty ) {
				if ( !nameOnly.StartsWith(prefix)) 
					throw new ArgumentException(string.Format("PathName [{0}] did not start with the specified prefix [{1}]",fileName,prefix));

				semVerPart = nameOnly.Substring(prefix.Length);
			}
			else
				semVerPart = nameOnly;


			FileName = fileName;
			Prefix = prefix;
			SemVerPart = semVerPart;
			SemVersionProper = SemVersion.Parse(semVerPart);
		}


		/// <summary>
		/// The actual file / folder name with prefix and version information in it.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		/// The prefix that the version folders start with
		/// </summary>
		public string Prefix { get; private set; }

		/// <summary>
		/// What the function determined was the SemVer part of the filename
		/// </summary>
		public string SemVerPart { get; private set; }


		/// <summary>
		/// Returns the actual SemVersion object representation of the Version
		/// </summary>
		public SemVersion SemVersionProper {
			get; set;
		}


		/// <summary>
		/// Returns the SemVersion in its string form, ie:  1.3.7-alpha.13
		/// </summary>
		public string Version {
			get { return SemVersionProper.ToString(); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param fileName="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			return ((IComparable)SemVersionProper).CompareTo(((FileSemVer)obj).SemVersionProper);
		}


		public override string ToString () { return Version; }
	}
}
