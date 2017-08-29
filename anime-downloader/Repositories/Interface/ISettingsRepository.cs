﻿using System.Collections.Generic;
using anime_downloader.Enums;
using anime_downloader.Models;
using anime_downloader.Models.Configurations;

namespace anime_downloader.Repositories.Interface
{
    public interface ISettingsRepository
    {
        PathConfiguration PathConfig { get; set; }
        FlagConfiguration FlagConfig { get; set; }
        VersionCheck Version { get; set; }
        
        DownloadProvider Provider { get; set; }
        string SortBy { get; set; }
        string FilterBy { get; set; }
        List<string> Subgroups { get; set; }

        bool CrucialDirectoriesExist();

        void Save();
    }
}