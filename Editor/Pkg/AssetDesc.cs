namespace HalfLife2Rtx;

public class AssetDesc
{
	public ushort NameIndex;
	public AssetType AssetType;
	public VkFormat Format;
	public uint Size;
	public ushort Depth;
	public ushort NumMips;
	public ushort NumTailMips;
	public ushort ArraySize;
	public ushort BaseBlobIdx;
	public ushort TailBlobIdx;
}
