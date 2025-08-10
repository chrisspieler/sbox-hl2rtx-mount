using Duccsoft.Mounting;
using Sandbox.Mounting;

namespace HalfLife2Rtx;

public class TextureLoader( PkgFile sourcePkg, MountAssetPath texPath ) : ResourceLoader<HalfLife2RtxMount>
{
	// TODO: Decompress the (presumably) GDeflate compressed .dds file from the source .pkg, and load its texture data.
	protected override object Load() => Texture.Invalid;
}
