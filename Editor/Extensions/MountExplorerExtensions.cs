using Duccsoft.Mounting;

namespace HalfLife2Rtx;

public static class MountExplorerExtensions
{
	public static MountAssetPath GetTextureRef( this MountExplorer explorer, string relativePath )
		=> explorer.RelativePathToAssetRef( relativePath, ".vtex" );
	
	public static MountAssetPath GetModelRef( this MountExplorer explorer, string relativePath )
		=> explorer.RelativePathToAssetRef( relativePath, ".vmdl" );
}
