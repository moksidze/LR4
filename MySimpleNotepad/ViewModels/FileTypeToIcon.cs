using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Data.Converters;
using System;
using System.Globalization;
using MySimpleNotepad.Models;
using static MySimpleNotepad.Models.FileTypes;

namespace MySimpleNotepad.ViewModels
{
    internal class FileTypeToIcon : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is FileTypes @fileType)
            {
                string icon = @fileType switch
                {
                    BackFolder => "backFolder",
                    Document => "file",
                    Folder => "folder",
                    Hardrive => "hardrive",
                    SystemHardrive => "systemHardrive",
                    _ => ""
                };

                var app = Application.Current;
                if (app == null) return null;

                var resources = app.Resources;

                var image = (Image?) resources[icon];
                if (image == null) return null;

                return (Bitmap?)image.Source;
            }

            throw new NotSupportedException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
