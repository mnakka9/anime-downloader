﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using anime_downloader.Services;
using GalaSoft.MvvmLight;

namespace anime_downloader.ViewModels.Components
{
    public class DownloadLogViewModel: ViewModelBase
    {
        public ISettingsService Settings { get; set; }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { Set(() => Text, ref _text, value); }
        }

        public DownloadLogViewModel(ISettingsService settings)
        {
            Settings = settings;
            Logger();
        }

        private async void Logger()
        {
            Text = ">> No downloads have been logged so far.\n";

            if (File.Exists(Settings.PathConfig.Logging))
            {
                using (var reader = new StreamReader(Settings.PathConfig.Logging))
                {
                    var data = await reader.ReadToEndAsync();
                    Text = await Task.Run(() => string.Join("\n", data.Split('\n').Reverse().Skip(1)));
                }
            }
        }
    }
}
