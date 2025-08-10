using System.IO;

namespace Duccsoft.Mounting;

public readonly struct MountAssetPath
{
	public MountAssetPath( SteamGameMount mount, string relativeAssetPath, string customExtension )
	{
		_sourceMount = mount;
		_customExtension = customExtension;
		
		Absolute = Path.Combine( mount.AppDirectory, relativeAssetPath );
		Relative = (relativeAssetPath + customExtension).ToLowerInvariant();
		Mount = Path.Combine( $"mount://{_sourceMount.Ident}/", Relative );
		
		HashCode = Mount.GetHashCode();
	}

	public MountAssetPath( SteamGameMount mount, string basePath, string relativeAssetPath, string customExtension )
	{
		_sourceMount = mount;
		_customExtension = customExtension;
		
		Absolute = Path.Combine( basePath, relativeAssetPath );
		Relative = (relativeAssetPath + customExtension).ToLowerInvariant();
		Mount = Path.Combine( $"mount://{_sourceMount.Ident}/", Relative );
		
		HashCode = Mount.GetHashCode();
	}
	
	public int HashCode { get; }
	public string Absolute { get; }
	public string Mount { get; }
	public string Relative { get; }
	
	private readonly SteamGameMount _sourceMount;
	public MountExplorer Explorer => _sourceMount.Explorer;

	private readonly string _customExtension;

	public override int GetHashCode() => HashCode;

	public MountAssetPath MakeRelativeTo( string newBasePath )
		=> new MountAssetPath( _sourceMount, newBasePath, Path.GetRelativePath( newBasePath, Absolute ), _customExtension );
}
