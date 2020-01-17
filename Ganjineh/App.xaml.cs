using Ganjineh.Data;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Net;
using System.Windows;

namespace Ganjineh
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GlobalData.Init();

            AppCenter.Start("e01ac72e-2238-4eea-a010-6192e35ea91a",
                   typeof(Analytics), typeof(Crashes));

            ConfigHelper.Instance.SetLang("fa");
            if (GlobalData.Config.Skin != SkinType.Default)
            {
                UpdateSkin(GlobalData.Config.Skin);
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        internal void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/HandyControl;component/Themes/Skin{skin.ToString()}.xaml")
            });
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
        }
    }
}
