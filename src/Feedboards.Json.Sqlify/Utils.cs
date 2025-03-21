namespace Feedboards.Json.Sqlify;

internal class Utils
{
	internal static FileOrFolderChecker CheckPath(string path)
	{
		if (Directory.Exists(path))
		{
			return FileOrFolderChecker.Folder;
		}
		else if (File.Exists(path))
		{
			return FileOrFolderChecker.File;
		}
		else
		{
			// For non-existent paths, check if the parent directory exists
			var parentDir = Path.GetDirectoryName(path);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				throw new DirectoryNotFoundException($"Directory '{parentDir}' does not exist.");
			}

			// If parent directory exists, use extension to determine type
			if (Path.HasExtension(path))
			{
				return FileOrFolderChecker.File;
			}
			else
			{
				return FileOrFolderChecker.Folder;
			}
		}
	}
}
