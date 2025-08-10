using Duccsoft.Mounting;
using Sandbox.Mounting;

namespace HalfLife2Rtx;

public class ModelLoader( MountAssetPath modelPath ) : ResourceLoader<HalfLife2RtxMount>
{
	protected override object Load() => Sandbox.Model.Cube;
}
