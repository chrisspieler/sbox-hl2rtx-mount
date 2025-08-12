using System.IO;
using System.Threading.Tasks;
using Sandbox.Mounting;

namespace Duccsoft.Mounting;

public abstract class SteamGameMount : BaseGameMount
{
	protected record AddMountResourceCommand( ResourceType Type, MountAssetPath Path, ResourceLoader Loader );
	
	/// <summary>
	/// The Steam AppID that uniquely identifies this application. 
	/// </summary>
	public abstract long AppId { get; }
	/// <summary>
	/// Returns a set of commands that will be used in <see cref="Mount"/> to add <see cref="ResourceLoader"/> instances
	/// via <see cref="MountContext.Add"/>.
	/// </summary>
	protected abstract IEnumerable<AddMountResourceCommand> GetAllResources();
	
	public string AppDirectory { get; protected set; }
	public virtual string DataDirectory => AppDirectory;
	public MountExplorer Explorer { get; protected set; }

	protected SteamGameMount()
	{
		Explorer = new MountExplorer( this );
	}
	
	protected override void Initialize( InitializeContext context )
	{
		if ( !context.IsAppInstalled( AppId ) )
			return;
		
		AppDirectory = context.GetAppDirectory( AppId );
		IsInstalled = Path.Exists( AppDirectory );
	}
	
	protected override Task Mount( MountContext context )
	{
		var commands = GetAllResources().ToArray();
		foreach ( var addCommand in commands )
		{
			context.Add( addCommand.Type, addCommand.Path.DisplayPath, addCommand.Loader );
		}
		
		IsMounted = commands.Length > 0;
		
		var logString = IsMounted
			? $"Mounted \"{Title}\" with {commands.Length} {nameof(ResourceLoader)} instances."
			: $"Failed to mount \"{Title}\" because no {nameof(ResourceLoader)} instances were given by {nameof(GetAllResources)}";
		Log.Info( logString );
		
		return Task.CompletedTask;
	}
}
