using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SDL3.SDL;

namespace Foster.Framework;

/// <summary>
/// Shows OS File Dialogs if the platform supports it.
/// </summary>
public static class FileDialog
{
	public enum Result
	{
		/// <summary>
		/// The user successfully selected files.
		/// </summary>
		Success,

		/// <summary>
		/// The user cancelled the selection.
		/// </summary>
		Cancelled,

		/// <summary>
		/// The user tried to select files but there was a system error.
		/// </summary>
		Failed
	}

	/// <summary>
	/// Callback with resulting file paths the user selected
	/// </summary>
	public delegate void Callback(string[] paths, Result result);

	/// <summary>
	/// Callback with the resulting file path the user selected
	/// </summary>
	public delegate void CallbackSingleFile(string path, Result result);
	
	/// <summary>
	/// File Filter
	/// </summary>
	public readonly record struct Filter(string Name, string Pattern);

	/// <summary>
	/// Shows an "Open File" Dialog
	/// </summary>
	public static void OpenFile(Callback callback, bool allowMany = false)
		=> OpenFile(callback, [], null, allowMany);

	/// <summary>
	/// Shows an "Open File" Dialog
	/// </summary>
	public static unsafe void OpenFile(Callback callback, Filter[] filters, string? defaultLocation = null, bool allowMany = false)
		=> ShowFileDialog(new(Modes.OpenFile, callback, filters, defaultLocation, allowMany));

	/// <summary>
	/// Shows an "Open File" Dialog
	/// </summary>
	public static void OpenFolder(Callback callback, bool allowMany = false)
		=> OpenFolder(callback, null, allowMany);

	/// <summary>
	/// Shows an "Open Folder" Dialog
	/// </summary>
	public static void OpenFolder(Callback callback, string? defaultLocation = null, bool allowMany = false)
		=> ShowFileDialog(new(Modes.OpenFolder, callback, [], defaultLocation, allowMany));

	/// <summary>
	/// Shows a "Save File" Dialog
	/// </summary>
	public static void SaveFile(CallbackSingleFile callback)
		=> SaveFile(callback, [], null);

	/// <summary>
	/// Shows a "Save File" Dialog
	/// </summary>
	public static unsafe void SaveFile(CallbackSingleFile callback, Filter[] filters, string? defaultLocation = null)
	{
		void Singular(string[] files, Result result)
			=> callback(files.FirstOrDefault() ?? string.Empty, result);
		
		ShowFileDialog(new(Modes.SaveFile, Singular, filters, defaultLocation, false));
	}

	private enum Modes
	{
		OpenFile,
		OpenFolder,
		SaveFile
	}

	private readonly record struct ShowProperties(
		Modes Mode,
		Callback Callback,
		Filter[] Filters,
		string? DefaultLocation,
		bool AllowMany
	);

	private static unsafe void ShowFileDialog(ShowProperties properties)
	{
		[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
		static void CallbackFromSDL(nint userdata, nint files, int filter)
		{
			// get actual callback, release held handle
			var handle = GCHandle.FromIntPtr(userdata);
			var callback = handle.Target as Callback;
			handle.Free();

			string[] paths;
			Result result;

			// get list of files (list of utf8)
			var ptr = (byte**)files;

			// null means there was a system error
			if (ptr == null)
			{
				Log.Error(SDL_GetError());
				paths = [];
				result = Result.Failed;
			}
			// empty list means the action was cancelled by the user
			else if (ptr[0] == null)
			{
				paths = [];
				result = Result.Cancelled;
			}
			// otherwise return the files
			else
			{
				var list = new List<string>();
				for (int i = 0; ptr[i] != null; i ++)
					list.Add(Platform.ParseUTF8(new nint(ptr[i])));
				paths = [..list];
				result = Result.Success;
			}

			// the callback can be invoked from any thread as per the SDL docs
			// but for ease-of-use we want it to always be called from the Main thread
			App.RunOnMainThread(() => callback?.Invoke(paths, result));
		}

		static void Show(ShowProperties properties)
		{
			// fallback to where we think the application is running from
			string? defaultLocation = properties.DefaultLocation;
			if (string.IsNullOrEmpty(defaultLocation))
				defaultLocation = Directory.GetCurrentDirectory();

			// get UTF8 string data for SDL
			Span<SDL_DialogFileFilter> filtersUtf8 = stackalloc SDL_DialogFileFilter[properties.Filters.Length];
			for (int i = 0; i < properties.Filters.Length; i ++)
			{
				filtersUtf8[i].name = (byte*)Platform.AllocateUTF8(properties.Filters[i].Name);
				filtersUtf8[i].pattern = (byte*)Platform.AllocateUTF8(properties.Filters[i].Pattern);
			}

			// create a pointer to our user callback so that SDL can pass it around
			var handle = GCHandle.Alloc(properties.Callback);
			var userdata = GCHandle.ToIntPtr(handle);

			// open file dialog
			switch (properties.Mode)
			{
				case Modes.OpenFile:
					SDL_ShowOpenFileDialog(
						&CallbackFromSDL, userdata, App.Window, filtersUtf8, 
						properties.Filters.Length, defaultLocation, properties.AllowMany);
					break;
				case Modes.SaveFile:
					SDL_ShowSaveFileDialog(
						&CallbackFromSDL, userdata, App.Window, filtersUtf8,
						properties.Filters.Length, defaultLocation);
					break;
				case Modes.OpenFolder:
					SDL_ShowOpenFolderDialog(
						&CallbackFromSDL, userdata, App.Window, 
						defaultLocation, properties.AllowMany);
					break;
			}

			// clear UTF8 string memory
			foreach (var it in filtersUtf8)
			{
				Platform.FreeUTF8(new nint(it.name));
				Platform.FreeUTF8(new nint(it.pattern));
			}
		}

		// Application must be running for these methods to work
		// as per SDL docs, some platforms require the events to be polled
		if (!App.Running)
			throw new Exception("Showing File Dialogs is only supported while the Application is running");
		
		// SDL docs say that showing file dialogs must be invoked from the Main Thread
		App.RunOnMainThread(() => Show(properties));
	}
}