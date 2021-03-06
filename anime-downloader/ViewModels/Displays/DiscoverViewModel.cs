﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using anime_downloader.Enums;
using anime_downloader.Models;
using anime_downloader.Models.AniList;
using anime_downloader.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace anime_downloader.ViewModels.Displays
{
    public class DiscoverViewModel : ViewModelBase
    {
        private readonly IFindSeasonAnimeService _findService;
        private readonly IAnimeService _animeService;
        private bool _visible;
        private int _selectedIndex;
        private int? _previousIndex;
        private AnimeSeason _season = AnimeSeason.Current;
        private ObservableCollection<AiringAnime> _airingShows;
        private ObservableCollection<AiringAnime> _leftoverShows;
        private List<AiringAnime> _airing;
        private List<AiringAnime> _leftover;
        private AiringAnime _selectedAiring;
        private AiringAnime _selectedLeftover;

        // 

        public DiscoverViewModel(IFindSeasonAnimeService findService, IAnimeService animeService)
        {
            _findService = findService;
            _animeService = animeService;
            AddCommand = new RelayCommand(Add);

            MessengerInstance.Register<ViewRequest>(this, HandleViewAction);
        }
        
        // 

        public AiringAnime SelectedAiring
        {
            get => _selectedAiring;
            set
            {
                Set(() => SelectedAiring, ref _selectedAiring, value);
                if (value != null && string.IsNullOrEmpty(SelectedAiring?.Description))
                    FillOutDetails();
            }
        }

        public AiringAnime SelectedLeftover
        {
            get => _selectedLeftover;
            set
            {
                Set(() => SelectedLeftover, ref _selectedLeftover, value);
                if (value != null && string.IsNullOrEmpty(SelectedLeftover?.Description))
                    FillOutDetails();
            }
        }

        public AnimeSeason Season
        {
            get => _season;
            set
            {
                Set(() => Season, ref _season, value);
                AiringShows.Clear();
                LeftoverShows.Clear();
                LoadPage();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                Set(() => SelectedIndex, ref _selectedIndex, value);
                LoadPage();
            }
        }

        public ObservableCollection<AiringAnime> AiringShows
        {
            get => _airingShows;
            set => Set(() => AiringShows, ref _airingShows, value);
        }

        public ObservableCollection<AiringAnime> LeftoverShows
        {
            get => _leftoverShows;
            set => Set(() => LeftoverShows, ref _leftoverShows, value);
        }

        public RelayCommand AddCommand { get; set; }

        // 

        /// <summary>
        ///     Only attempt to load properties from the view when it is visible.
        /// </summary>
        public void VisibilityChanged(bool visible)
        {
            _visible = visible;

            if (_visible)
                LoadPage();
        }

        private void HandleViewAction(ViewRequest va)
        {
            if (va == ViewRequest.Refresh)
                Refresh();
        }

        private void Refresh()
        {
            if (_airing != null)
                AiringShows = new ObservableCollection<AiringAnime>(_airing
                    .Where(anime =>    !_animeService.WatchingAndAiringContains(anime.TitleEnglish)
                                    && !_animeService.WatchingAndAiringContains(anime.TitleRomaji)));
            if (_leftover != null)
                LeftoverShows = new ObservableCollection<AiringAnime>(_leftover
                    .Where(anime =>    !_animeService.WatchingAndAiringContains(anime.TitleEnglish)
                                    && !_animeService.WatchingAndAiringContains(anime.TitleRomaji)));

            switch (SelectedIndex)
            {
                case 0:
                    if (AiringShows != null)
                        SelectedAiring = _previousIndex != null && _previousIndex.Value > 0 &&
                                         _previousIndex.Value < AiringShows.Count && AiringShows.Count > 0
                            ? AiringShows[_previousIndex.Value - 1]
                            : AiringShows.FirstOrDefault();
                    break;
                case 1:
                    if (LeftoverShows != null)
                        SelectedLeftover = _previousIndex != null && _previousIndex.Value > 0 &&
                                           _previousIndex.Value < LeftoverShows.Count && LeftoverShows.Count > 0
                            ? LeftoverShows[_previousIndex.Value - 1]
                            : LeftoverShows.FirstOrDefault();
                    break;
                default:
                    SelectedAiring = null;
                    SelectedLeftover = null;
                    break;
            }
        }

        private async void LoadPage()
        {
            switch (SelectedIndex)
            {
                // First tab: Airing Shows
                case 0:
                    if (AiringShows == null)
                    {
                        MessengerInstance.Send(ViewState.IsLoading);
                        _airing = await _findService.New(Season);
                        await Task.Run(() =>
                            AiringShows = new ObservableCollection<AiringAnime>(_airing
                                .Where(anime =>    !_animeService.WatchingAndAiringContains(anime.TitleEnglish)
                                                && !_animeService.WatchingAndAiringContains(anime.TitleRomaji))));
                        MessengerInstance.Send(ViewState.DoneLoading);
                    }
                    break;

                // Second tab: Leftover
                case 1:
                    if (LeftoverShows == null)
                    {
                        MessengerInstance.Send(ViewState.IsLoading);
                        _leftover = await _findService.Leftover(Season);
                        await Task.Run(() =>
                            LeftoverShows = new ObservableCollection<AiringAnime>(_leftover
                                .Where(anime =>    !_animeService.WatchingAndAiringContains(anime.TitleEnglish)
                                                && !_animeService.WatchingAndAiringContains(anime.TitleRomaji))));
                        MessengerInstance.Send(ViewState.DoneLoading);
                    }
                    break;
                default:
                    break;
            }
        }

        private async void FillOutDetails()
        {
            var anime = SelectedIndex == 0 ? SelectedAiring : SelectedLeftover;
            if (anime != null)
            {
                MessengerInstance.Send(ViewState.IsLoading);
                await _findService.FillInDetails(Season, SelectedIndex == 0, anime);
                MessengerInstance.Send(ViewState.DoneLoading);
            }
        }

        private void Add()
        {
            var anime = SelectedIndex == 0 ? SelectedAiring : SelectedLeftover;
            if (anime != null)
            {
                anime.AnimeSeason = Season;
                MessengerInstance.Send(Display.Anime);
                MessengerInstance.Send(anime);
                _previousIndex = SelectedIndex == 0 ? AiringShows.IndexOf(anime) : LeftoverShows.IndexOf(anime);
            }
        }
        
    }
}