using HandyControl.Data;
using System;

namespace Ganjineh.Data
{
    internal class AppConfig
    {
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";

        public string Location { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public SkinType Skin { get; set; }
    }
}