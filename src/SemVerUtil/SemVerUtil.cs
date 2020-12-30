using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;


namespace SlugEnt.SemVer
{
	/// <summary>
	/// Class that provides Semantic Versioning functionality to help manage Folders that represent a Semantic Version.
	/// <para> Typically this is when developed applications are deployed into Semantic Version folders that then need to be managed.</para>
	/// <para> This includes finding the latest version and purging older versions.</para>
	/// </summary>
	public  class SemVerUtil {
		private readonly IFileSystem _fileSystem;
		private List<FileSemVer> _versions;

		/// <summary>
		/// Constructs a SemVerUtil object with the given FileSystem.  Used for Unit Tests
		/// </summary>
		/// <param name="fileSystem"></param>
		public SemVerUtil (IFileSystem fileSystem) { _fileSystem = fileSystem; }



		/// <summary>
		/// Whether the object has been initialized.  Cannot do anything if IsInitialized = false;
		/// </summary>
		public bool IsInitialized { get; private set; }


		/// <summary>
		/// The number of Version Folders
		/// </summary>
		public int VersionCount { get; private set; }


		/// <summary>
		/// Initializes the object
		/// </summary>
		/// <param name="path">The Directory that contains the Versioned folders</param>
		/// <param name="prefix">The prefix that identifies a "semver" folder.  The part of the name that occurs before the Semantic Version part.  For instance if all the semver folders start with Ver then that is the prefix.  Ver1.0.2, Ver2.0.4-alpha.2</param>
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
		/// Returns the oldest N number of versions.
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
		/// Returns a list of FileSemVer objects that represent the oldest n versions after ensuring there is a numberOfNewest latest versions in the list.
		/// So, a list with 6 items, with a passed in numberOfNewest of 4 would return the 2 oldest.  
		/// </summary>
		/// <param name="numberOfNewest">This returns all the oldest items in the list after ensuring a minimum number of the latest versions are kept.</param>
		/// <returns>Empty list if the versions list contains no items or contains less than or equal to the numberOfNewest items </returns>
		public List<FileSemVer> OldestWithMin (int numberOfNewest) {
			if (!IsInitialized) throw new ApplicationException("The SemVerUtil object has not been initialized.");

			return OldestWithMin(numberOfNewest, _versions);
			/*
			List<FileSemVer> oldest = new List<FileSemVer>();

			if (_versions.Count == 0) return oldest;
			if ( _versions.Count <= numberOfNewest ) return oldest;

			int oldMax = _versions.Count - numberOfNewest;
			oldest.AddRange(_versions.GetRange(0, oldMax));
			return oldest;
			*/
		}


		/// <summary>
		/// Returns the oldest versions in the provided list, ensuring at least numberOfNewest versions are kept in the list.
		/// </summary>
		/// <param name="numberOfNewest">This returns all the oldest items in the list after ensuring a minimum number of the latest versions are kept.</param>
		/// <param name="versionList">The FileSemVer list of versions</param>
		/// <returns></returns>
		private List<FileSemVer> OldestWithMin (int numberOfNewest, List<FileSemVer> versionList) {
			List<FileSemVer> oldest = new List<FileSemVer>();

			if (versionList.Count == 0) return oldest;
			if (versionList.Count <= numberOfNewest) return oldest;

			int oldMax = versionList.Count - numberOfNewest;
			oldest.AddRange(versionList.GetRange(0, oldMax));
			return oldest;
		}


		/// <summary>
		/// Returns a list of the oldest Semantic versions, keeping a minimum number of the newest as well as keeping anything newer than the minAgeToKeep.  Basically keeps anything newer than MinAgeToKeep and then numberOfNewestToKeep after that.
		/// </summary>
		/// <param name="numberOfNewestToKeep">The minimum number of the newest versions that should be kept</param>
		/// <param name="minAgeToKeep">The minimum age from right now that forces us to keep all of these versions.  For instance 3d would ensure that any version that is newer than 3 days would be kept.</param>
		/// <returns></returns>
		public List<FileSemVer> OldestWithMinAge (int numberOfNewestToKeep, TimeUnit minAgeToKeep) {
			// A.  First just get the oldest that meet the minimum number to keep
			List<FileSemVer> oldest = OldestWithMin(numberOfNewestToKeep);
			int oldestCount = oldest.Count;

			long seconds = -1 * (minAgeToKeep.InSecondsLong);
			DateTime threshold = DateTime.Now.AddSeconds(seconds);

			// B.  Now go thru the list and remove any that are newer than the age requirement.
			for (int i= oldest.Count -1; i >= 0; i--) {
				FileSemVer fileSemVer = oldest [i];
				DateTime created =  _fileSystem.File.GetCreationTime(fileSemVer.FileName);

				// If the item does not meet the minimum age requirement we delete it from the list.
				if ( created > threshold ) { oldest.RemoveAt(i);}
			}

			// C.  Now remove any that do not meet the numberOfNewestToKeep.

			// We treat all the min age versions as though they were just 1 version.  
			if ( oldest.Count != oldestCount ) numberOfNewestToKeep--;
			return OldestWithMin(numberOfNewestToKeep, oldest);
		}
	}
}
