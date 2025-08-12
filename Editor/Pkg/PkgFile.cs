using System.IO;
using Duccsoft.Mounting;
using Sandbox.Diagnostics;

namespace HalfLife2Rtx;

public class PkgFile
{
	private const uint Magic = 0xBAADD00D;
	private const uint Version = 1u;
	
	public PkgFile( MountAssetPath path, AssetDesc[] assets, BlobDesc[] blobs, string[] assetPaths )
	{
		Path = path;
		_assets = assets;
		_blobs = blobs;
		_assetPaths = assetPaths;
	}
	
	public MountAssetPath Path { get; }
	public IReadOnlyList<AssetDesc> Assets => _assets;
	private readonly AssetDesc[] _assets;

	public IReadOnlyList<string> AssetPaths => _assetPaths;
	private readonly string[] _assetPaths;
	
	public IReadOnlyList<BlobDesc> Blobs => _blobs;
	private readonly BlobDesc[] _blobs;
	
	/// <summary>
	/// Returns whether the given file path refers to a valid .pkg file.
	/// </summary>
	public static bool IsValid( MountAssetPath pkgPath )
	{
		if ( !File.Exists( pkgPath.SystemPath ) )
			return false;
		
		using var reader = new BinaryReader( File.OpenRead( pkgPath.SystemPath ) );

		if ( reader.BaseStream.Length < 0x10 )
			return false;
		
		return reader.ReadUInt32() == Magic;
	}

	public static PkgFile Load( MountAssetPath pkgPath )
	{
		using var reader = new BinaryReader( File.OpenRead( pkgPath.SystemPath ) );

		// Verify the file header.
		Assert.AreEqual( reader.ReadUInt32(), Magic, $"Invalid magic in: {pkgPath.DisplayPath}" );
		Assert.AreEqual( reader.ReadUInt32(), Version, "Unknown version number" );
		
		// Read the dict offset, then jump to it and read the asset and blob counts.
		var dictOffset = reader.ReadInt64();
		reader.BaseStream.Seek( dictOffset, SeekOrigin.Begin );
		var assetCount = reader.ReadUInt16();
		var blobCount = reader.ReadUInt16();
		
		var assets = new AssetDesc[assetCount];
		for ( int i = 0; i < assetCount; i++ )
		{
			assets[i] = ReadAssetDesc( reader );
		}

		var blobs = new BlobDesc[blobCount];
		for ( int i = 0; i < blobCount; i++ )
		{
			blobs[i] = ReadBlobDesc( reader );
		}

		var assetPaths = new string[assetCount];
		for ( int i = 0; i < assetCount; i++ )
		{
			assetPaths[i] = reader.BaseStream.ReadNullTerminatedString( reader.BaseStream.Position );
		}
		
		return new PkgFile( pkgPath, assets, blobs, assetPaths );
	}

	private static AssetDesc ReadAssetDesc( BinaryReader reader )
	{
		return new AssetDesc()
		{
			NameIndex = reader.ReadUInt16(),
			AssetType = (AssetType)reader.ReadByte(),
			Format = (VkFormat)reader.ReadByte(),
			Size = reader.ReadUInt32(),
			Depth = reader.ReadUInt16(),
			NumMips = reader.ReadUInt16(),
			NumTailMips = reader.ReadUInt16(),
			ArraySize = reader.ReadUInt16(),
			BaseBlobIdx = reader.ReadUInt16(),
			TailBlobIdx = reader.ReadUInt16()
		};
	}

	private static BlobDesc ReadBlobDesc( BinaryReader reader )
	{
		// Get two parts of the blob offset...
		var offset32 = reader.ReadUInt32();
		var offset8 = (ulong)reader.ReadByte();
		
		// ...and combine them to make a 40-bit integer.
		var offset40 = (offset8 << 32) & offset32;
		
		var compression = reader.ReadByte();
		// There's a mysterious empty byte here, so skip it.
		reader.ReadByte();
		
		// The rest of the fields can read in series without stopping.
		return new BlobDesc
		{
			Offset = offset40,
			Compression = (CompressionType)compression,
			Flags = reader.ReadByte(),
			Size = reader.ReadUInt32(),
			Crc32 = reader.ReadUInt32()
		};
	}
}
