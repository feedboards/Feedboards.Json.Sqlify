namespace Feedboards.Json.Sqlify;

internal class Utils
{
	internal static FileOrFolderChecker CheckPath(string path)
	{
		//if (Directory.Exists(path))
		//{
		//	return FileOrFolderChecker.Folder;
		//}
		//else if (File.Exists(path))
		//{
		//	return FileOrFolderChecker.File;
		//}
		//else
		//{
		//	throw new Exception(nameof(path));
		//}
		if (Directory.Exists(path))
		{
			return FileOrFolderChecker.Folder;
		}
		else if (File.Exists(path))
		{
			return FileOrFolderChecker.File;
		}
		else if (Path.HasExtension(path))
		{
			// Path has an extension, so assume it's meant to be a file.
			return FileOrFolderChecker.File;
		}
		else
		{
			// If it doesn't have an extension, assume it's meant to be a folder.
			return FileOrFolderChecker.Folder;
		}
	}
}
