using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SemVerUtil
{

	public class FileSemVer
	{
		public FileSemVer (string name, string prefix = "") {
			if (string.IsNullOrEmpty(name)) throw new ArgumentException("PathName cannot be null, must specify the name of a fully qualified path.");

			string semVerPart = "";

			if ( prefix != string.Empty ) {
				if ( !name.StartsWith(prefix) )
					throw new ArgumentException(string.Format("PathName [{name}] did not start with the specified prefix [{prefix}]"));

				semVerPart = name.Substring(prefix.Length - 1);
			}
			else
				semVerPart = name;


			Name = name;
			Prefix = prefix;
			SemVer = semVerPart;
		}



		public string Name { get; private set; }
		public string Prefix { get; private set; }
		public string SemVer { get; private set; }
	}
}
