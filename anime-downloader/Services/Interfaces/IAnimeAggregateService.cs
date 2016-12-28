﻿namespace anime_downloader.Services.Interfaces
{
    public interface IAnimeAggregateService
    {
        IAnimeService Animes { get; set; }
        IAnimeFileService Files { get; set; }
        IAnimeDownloaderService Downloader { get; set; }
        IMyAnimeListService Mal { get; set; }
        IPlaylistService Playlist { get; set; }
    }
}