using Duccsoft.Formats.Usd.Ascii;
using Duccsoft.Mounting;
using Sandbox.Mounting;

namespace HalfLife2Rtx;

public class ModelLoader( MountAssetPath modelPath ) : ResourceLoader<HalfLife2RtxMount>
{
	protected override object Load()
	{
		var usdStage = new UsdaReader().ReadFromPath( modelPath.Absolute );
		Log.Info( $"Loaded USDA file {modelPath.Relative} with {usdStage.Prims.Count} prims:" );
		// foreach ( var prim in usdStage.Prims )
		// {
		// 	Log.Info( $"{prim.Type} \"{prim.Name}\" " + '{' );
		// 	foreach ( var (_, attribute) in prim.Attributes )
		// 	{
		// 		Log.Info( $"\t{attribute.Type}{(attribute.IsArray ? "[]" : string.Empty)} {attribute.Name}" );
		// 	}
		// 	Log.Info( '}' );
		// }
		return usdStage.LoadModel();
	}
}
