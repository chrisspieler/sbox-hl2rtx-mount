using System.IO;
using Duccsoft.Mounting;
using Sandbox.Diagnostics;

namespace HalfLife2Rtx;

public class UsdFile
{
	public const string UsdaHeader = "#usda 1.0";
	
	public UsdFile( MountAssetPath path )
	{
		Path = path;
	}

	public MountAssetPath Path { get; }

	/// <summary>
	/// Returns whether the given file path refers to a valid USD file.
	/// </summary>
	public static bool IsValid( MountAssetPath filePath )
	{
		if ( !File.Exists( filePath.Absolute ) )
			return false;
		
		using var reader = new StreamReader( File.OpenRead( filePath.Absolute ) );

		return reader.ReadLine() == UsdaHeader;
	}

	public static UsdFile Load( MountAssetPath filePath )
	{
		using var reader = new StreamReader( File.OpenRead( filePath.Absolute ) );

		Assert.True( System.IO.Path.GetExtension( filePath.Absolute ) == ".usda", "Only .usda files are supported!" );
		Assert.AreEqual( reader.ReadLine(), UsdaHeader, $"{filePath.Relative} does not start with \"{UsdaHeader}\"");
		
		// TODO: Actually parse the file.
		
		return new UsdFile( filePath );
	}
}
