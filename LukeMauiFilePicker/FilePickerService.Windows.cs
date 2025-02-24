﻿using Windows.Storage.Pickers;

namespace LukeMauiFilePicker;

partial class FilePickerService
{
    
    public partial async Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple)
        => await DefaultPickFilesAsync(title, types, multiple);

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        var picker = CreatePicker();
        if (picker is null) { return false; }

        picker.FileTypeChoices.Add(options.WindowsFileTypes.FileTypeName, options.WindowsFileTypes.FileTypeExts);
        picker.SuggestedFileName = options.SuggestedFileName;

        var file = await picker.PickSaveFileAsync();
        if (file is null) { return false; }

        using var outStr = await file.OpenStreamForWriteAsync();
        await options.Content.CopyToAsync(outStr);

        return true;
    }

    static FileSavePicker? CreatePicker()
    {
        var diag = new FileSavePicker();

        // Need this for HWND fix
        // https://github.com/dotnet/maui/issues/11527

        var currWin = Application.Current?.Windows.FirstOrDefault();
        var hwnd = (currWin?.Handler.PlatformView as MauiWinUIWindow)?.WindowHandle;
        if (hwnd is null) { return null; }

        WinRT.Interop.InitializeWithWindow.Initialize(diag, hwnd.Value);

        return diag;
    }


}
