using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SemVerUtil
{
	public static class SemVerUtil
	{
		public static string DirectoryMostRecentVersion (string path, string prefix) {
			if (! Directory.Exists(path)) throw new ArgumentException("The directory [{0}] could not be found.", path);

			string searchPattern = prefix + "*";
			List<string> directories = Directory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly).ToList();

			if ( directories.Count == 0 ) return null;

			
			
		}
	}
}
