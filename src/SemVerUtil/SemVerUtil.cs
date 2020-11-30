using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;


namespace SlugEnt.SemVer
{
	public  class SemVerUtil {
		private readonly IFileSystem _fileSystem;
		private List<FileSemVer> _versions;


		public SemVerUtil (IFileSystem fileSystem) { _fileSystem = fileSystem; }



		/// <summary>
		/// Whether the object has been initialized.  Cannot do anything if IsInitialized = false;
		/// </summary>
		public bool IsInitialized { get; private set; }

		public int VersionCount { get; private set; }


		/// <summary>
		/// Initializes the object
		/// </summary>
		/// <param name="path">The Directory that contains the Versioned folders</param>
		/// <param name="prefix"></param>
		public void Initialize (string path, string prefix) {
			// Reset list to empty.
			_versions =  new List<FileSemVer>();


			if (!_fileSystem.Directory.Exists(path)) throw new ArgumentException("The directory [{0}] could not be found.", path);

			string searchPattern = prefix + "*";
			string[] directories = _fileSystem.Directory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

			VersionCount = directories.Length;

			if ( directories.Length != 0 ) {
				// Now FileSemVer objects for each item in the array.
				foreach ( string dir in directories ) { _versions.Add(new FileSemVer(dir, prefix)); }

				// Sort the list.
				_versions.Sort();
			}

			IsInitialized = true;
		}


		/// <summary>
		/// Returns the latest FileSemVer Version #
		/// </summary>
		/// <returns>Null if no Versions matching criteria are found.</returns>
		public FileSemVer DirectoryMostRecentVersion () {
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			if ( _versions.Count == 0 ) return null;
			FileSemVer lastEntry = _versions.Last();
			return lastEntry;
		}


		/// <summary>
		/// Returns the oldest Versioned item
		/// </summary>
		/// <returns></returns>
		public FileSemVer DirectoryOldestVersion () {
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			if ( _versions.Count == 0 ) return null;
			FileSemVer oldestEntry = _versions.First();
			return oldestEntry;
		}



		/// <summary>
		/// Returns the newest N number of versions.  So 3, would return the newest, the one prior to and the one prior to that one. 
		/// </summary>
		/// <param name="numberOfNewest">The number of the latest n versions to return.</param>
		/// <returns>List<FileSemVer> of 3 most recent versions.  The newest one is listed first and then in descending order</FileSemVer></returns>
		public List<FileSemVer> DirectoryNewestVersions(int numberOfNewest)
		{
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			if (_versions.Count == 0) return null;
			if (_versions.Count < numberOfNewest) { numberOfNewest = _versions.Count; }

			List<FileSemVer> newest = new List<FileSemVer>();
			newest.AddRange(_versions.GetRange((_versions.Count - numberOfNewest), numberOfNewest));
			newest.Reverse();
			return newest;
		}




		/// <summary>
		/// Returns the oldest N number of versions.  So 3, would return the oldest versions.
		/// </summary>
		/// <param name="numberOfOldest">The number of oldest n versions to return.</param>
		/// <returns>List of FileSemVer objects of the n oldest versions.  They are listed in oldest to newest order.  Returns null if there are no versions.</returns>
		public List<FileSemVer> DirectoryOldestVersions(int numberOfOldest)
		{
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			if (_versions.Count == 0) return null;
			if (_versions.Count < numberOfOldest) { numberOfOldest = _versions.Count; }

			List<FileSemVer> oldest = new List<FileSemVer>();
			oldest.AddRange(_versions.GetRange(0, numberOfOldest));
			return oldest;
		}



		/// <summary>
		/// Returns a list of FileSemVer objects that represent the oldest n versions after ensuring there is a numberOfNewest latest versions in the list.  So, a list with 6 items, with a passed in numberOfNewest of 4 would return the 2 oldest.  
		/// </summary>
		/// <param name="numberOfNewest">This returns all the oldest items in the list after ensuring a minimum number of the latest versions are kept.</param>
		/// <returns>Empty list if the versions list contains no items or contains less than or equal to the numberOfNewest items </returns>
		public List<FileSemVer> OldestWithMin (int numberOfNewest) {
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			List<FileSemVer> oldest = new List<FileSemVer>();

			if (_versions.Count == 0) return oldest;
			if ( _versions.Count <= numberOfNewest ) return oldest;

			int oldMax = _versions.Count - numberOfNewest;
			oldest.AddRange(_versions.GetRange(0, oldMax));
			return oldest;
		}
	}
}
