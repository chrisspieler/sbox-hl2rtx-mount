using System.IO;
using Duccsoft.Mounting;
using Sandbox.Diagnostics;
using Sandbox.Mounting;

namespace HalfLife2Rtx;

public class HalfLife2RtxMount : SteamGameMount
{
	public override string Ident => "hl2_rtx";
	public override string Title => "Half-Life 2 RTX";
	public override long AppId => 2477290;

	/// <summary>
	/// A directory containing assets specific to RTX Remix. 
	/// </summary>
	public override string DataDirectory => Path.Combine( AppDirectory, "rtx-remix\\mods\\hl2rtx" );
	
	public int TexturesFound { get; private set; }
	public int MaterialsFound { get; private set; }
	public int ModelsFound { get; private set; }

	protected override IEnumerable<AddMountResourceCommand> GetAllResources()
	{
		TexturesFound = 0;
		var pkgFiles = GetAllPkgFiles();
		foreach ( var pkgFile in pkgFiles )
		{
			var extensions = pkgFile.AssetPaths.Select( Path.GetExtension ).Distinct().ToArray();

			Assert.True( pkgFile.Assets.Count > 0, $".pkg file contains no assets: {pkgFile.Path.DisplayPath}" );
			Assert.True( pkgFile.Blobs.Count > 0, $".pkg file contains no blobs: {pkgFile.Path.DisplayPath}" );
			
			foreach ( var asset in pkgFile.AssetPaths )
			{
				TexturesFound++;
				var texRef = Explorer.GetTextureRef( asset );
				var loader = new TextureLoader( pkgFile, texRef );
				yield return new AddMountResourceCommand( ResourceType.Texture, texRef, loader );
			}
			
			var extString = string.Empty;
			for ( int i = 0; i < extensions.Length; i++ )
			{
				var ext = extensions[i];
				extString += ext;
				if ( i < extensions.Length - 1 )
				{
					extString += ", ";
				}
			}
			
			Assert.True( extensions.Length == 1 && extensions[0] == ".dds", $".pkg file {pkgFile.Path.DisplayPath} must contain only textures. Found extensions: {extString}" );
			
			Log.Trace( $"{pkgFile.Path.DisplayPath} has {pkgFile.Assets.Count} asset(s) and {pkgFile.Blobs.Count} blob(s), extensions: {extString}" );
		}

		MaterialsFound = 0;
		// TODO: Figure out where materials are defined, and create material loaders.

		ModelsFound = 0;
		foreach ( var usdFilePath in GetAllUsdFilePaths() )
		{
			ModelsFound++;
			yield return new AddMountResourceCommand( ResourceType.Model, usdFilePath, new ModelLoader( usdFilePath ) );
		}

		Log.Info( $"Found {TexturesFound} texture(s), {MaterialsFound} material(s), and {ModelsFound} model(s) in \"{Title}\"" );
	}
	
	public IEnumerable<PkgFile> GetAllPkgFiles()
	{
		var foundPkgPaths = Explorer.FindFilesRecursive( DataDirectory, "*.pkg" );
		foreach ( var pkgFilePath in foundPkgPaths )
		{
			if ( !PkgFile.IsValid( pkgFilePath ) )
				continue;

			yield return PkgFile.Load( pkgFilePath );
		}
	}

	public IEnumerable<MountAssetPath> GetAllUsdFilePaths()
	{
		var foundUsdaPaths = Explorer.FindFilesRecursive( DataDirectory, "*.usda" );
		foreach ( var usdaFilePath in foundUsdaPaths )
		{
			// All texture paths are relative to DataDirectory, so for the sake of consistency, do the same for models.
			yield return usdaFilePath
				.WithBaseDirectory( DataDirectory )
				.WithExtension( ".vmdl" );
		}
	}
}
