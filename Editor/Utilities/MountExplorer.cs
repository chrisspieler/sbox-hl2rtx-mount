using System.IO;

namespace Duccsoft.Mounting;

public class MountExplorer( SteamGameMount gameMount )
{
	public MountAssetPath SystemPathToAssetRef( string systemPath, string customExtension )
	{
		var relativePath = Path.GetRelativePath( gameMount.AppDirectory, systemPath );
		return RelativePathToAssetRef( relativePath, customExtension );
	}
	
	public MountAssetPath RelativePathToAssetRef( string relativePath, string customExtension )
	{
		return new MountAssetPath( 
			gameMount,
			baseDir: gameMount.AppDirectory, 
			relativePath,
			customExtension 
		);
	}
	
	public IEnumerable<MountAssetPath> FindFilesRecursive( string directory, string pattern )
	{
		return Directory
			.EnumerateFiles( directory, pattern, SearchOption.AllDirectories )
			.Select( systemPath => SystemPathToAssetRef( systemPath, string.Empty ) );
	}
}
