﻿using System.Xml.Linq;

namespace anime_downloader.Models.MyAnimeList
{
    public class UpdateShow
    {
        private readonly XDocument _document;

        public UpdateShow(Anime anime, int episode)
        {
            var details = new AnimeDetailGroup(anime, episode);

            _document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("entry",
                    new XElement("episode", details.Episode),
                    new XElement("status", details.Status),
                    new XElement("score", details.Score),
                    new XElement("storage_type", ""),
                    new XElement("storage_value", ""),
                    new XElement("times_rewatched", ""),
                    new XElement("rewatch_value", ""),
                    new XElement("date_start", ""),
                    new XElement("date_finish", ""),
                    new XElement("priority", ""),
                    new XElement("enable_discussion", ""),
                    new XElement("enable_rewatching", ""),
                    new XElement("comments", ""),
                    new XElement("fansub_group", ""),
                    new XElement("tags", anime.Notes)
                )
            );
        }

        public override string ToString() => _document.Declaration + "\r\n" + _document;
    }

    internal class AnimeDetailGroup
    {
        public AnimeDetailGroup(Anime anime, int episode)
        {
            Episode = episode;
            Status = GetStatus(anime);
            Score = anime.Rating;
        }

        public string Status { get; set; }

        public int Episode { get; set; }

        public string Score { get; set; }

        private static string GetStatus(Anime anime)
        {
            // 1/watching, 2/completed, 3/onhold, 4/dropped, 6/plantowatch

            string status;

            switch (anime.Status)
            {
                case Enums.Status.Finished:
                    status = "completed";
                    break;

                case Enums.Status.Watching:
                    status = "watching";
                    break;

                case Enums.Status.OnHold:
                    status = "onhold";
                    break;

                case Enums.Status.Dropped:
                    status = "dropped";
                    break;

                case Enums.Status.Considering:
                    status = "plantowatch";
                    break;

                default:
                    status = "plantowatch";
                    break;
            }

            return status;
        }
    }
}