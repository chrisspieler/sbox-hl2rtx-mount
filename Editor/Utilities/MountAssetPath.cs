using System.IO;

namespace Duccsoft.Mounting;

public readonly struct MountAssetPath
{
	public MountAssetPath( SteamGameMount mount, string baseDir, string relativeFilePath, string customExtension )
	{
		_sourceMount = mount;

		BaseDirectory = baseDir;
		RelativePath = relativeFilePath;
		CustomExtension = customExtension;
	}
	
	public string BaseDirectory { get; init; }
	public string RelativePath { get; init; }
	public string CustomExtension { get; init; }
	
	/// <summary>
	/// The original path of a file, which typically represents either an actual file on desk, or a unique identifier
	/// within the file table of an archive.
	/// </summary>
	public string SystemPath => Path.Combine( BaseDirectory, RelativePath );
	
	/// <summary>
	/// A path relative to the game mount directory, using a custom extension. This is the path that will be used
	/// when adding a resource to a <see cref="Sandbox.Mounting.MountContext"/>
	/// </summary>
	public string DisplayPath => string.IsNullOrWhiteSpace( CustomExtension ) 
		? RelativePath.ToLowerInvariant() 
		: Path.ChangeExtension( RelativePath, CustomExtension )?.ToLowerInvariant();
	
	/// <summary>
	/// A path that may be used by s&amp;box to load the resource. For example, via <see cref="Sandbox.Texture.Load(string,bool)"/>
	/// </summary>
	public string ResourcePath => Path.Combine( $"mount://{_sourceMount.Ident}/", DisplayPath );
	
	private readonly SteamGameMount _sourceMount;

	public MountAssetPath WithBaseDirectory( string newBaseDir )
		=> this with { BaseDirectory = newBaseDir, RelativePath = Path.GetRelativePath( newBaseDir, SystemPath ) };

	public MountAssetPath WithExtension( string newExtension )
		=> this with { CustomExtension = newExtension };

	public MountAssetPath WithRelativePath( string newRelativePath )
		=> this with { RelativePath = newRelativePath };
}
