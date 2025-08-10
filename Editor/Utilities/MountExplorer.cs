using System.IO;

namespace Duccsoft.Mounting;

public class MountExplorer( SteamGameMount gameMount )
{
	public MountAssetPath AbsolutePathToAssetRef( string absolutePath, string customExtension )
	{
		var relativePath = Path.GetRelativePath( gameMount.AppDirectory, absolutePath );
		return RelativePathToAssetRef( relativePath, customExtension );
	}
	
	public MountAssetPath RelativePathToAssetRef( string relativePath, string customExtension )
	{
		return new MountAssetPath( 
			gameMount, 
			relativePath, 
			customExtension 
		);
	}
	
	public IEnumerable<MountAssetPath> FindFilesRecursive( string directory, string pattern )
	{
		return Directory
			.EnumerateFiles( directory, pattern, SearchOption.AllDirectories )
			.Select( absPath => AbsolutePathToAssetRef( absPath, string.Empty ) );
	}
}
