﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using anime_downloader.Classes.Web;
using anime_downloader.Classes.Xml;
using HtmlAgilityPack;

namespace anime_downloader.Classes {

    public class Anime {
        
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        private static int LevenshteinDistance(string s, string t) {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];
            if (n == 0)
                return m;
            if (m == 0)
                return n;
            for (var i = 0; i <= n; d[i, 0] = i++) { }
            for (var j = 0; j <= m; d[0, j] = j++) { }
            for (var i = 1; i <= n; i++) {
                for (var j = 1; j <= m; j++) {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        /// <summary>
        ///     Gets the best guess to what the anime is based solely on name
        /// </summary>
        /// <param name="animes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Anime ClosestTo(IEnumerable<Anime> animes, string name) {
            return animes.Select(a => new { Anime = a, Distance = LevenshteinDistance(a.Name, name) })
                .OrderBy(ap => ap.Distance)
                .First()
                .Anime;
        }

        private readonly XmlController _xml;

        public XContainer Root { get; }

        /// <summary>
        ///     Create an empty anime xml node.
        /// </summary>
        /// <remarks>
        ///      Must be explicitly added to the schema with the XmlController.
        /// </remarks>
        public Anime() {
            Root = XmlCreate.AnimeNode();
        }

        /// <summary>
        ///     Create an anime object with explicit xml nodes.  
        /// </summary>
        /// <remarks>
        ///     Preferrably read from the already existing schema and 
        ///     instantiated from the XmlController.
        /// </remarks>
        /// <param name="root"></param>
        /// <param name="xml"></param>
        public Anime(XContainer root, XmlController xml) {
            Root = root;
            _xml = xml;
        }

        private void Save() {
            if (_xml == null || !_xml.AutoSave)
                return;
            _xml.SaveAnime();
        }
        
        /// <summary>
        ///     Main referenced title.
        /// </summary>
        public string Name {
            get { return Root.Element("name")?.Value; }
            set {
                if (value.Equals(Name))
                    return;
                Root.Element("name")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     User's current watched episode.
        /// </summary>
        public string Episode {
            get { return Root.Element("episode")?.Value; }
            set {
                if (value.Equals(Episode))
                    return;
                Root.Element("episode")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     User's status on watching the anime.
        /// </summary>
        public string Status {
            get { return Root.Element("status")?.Value; }
            set {
                if (value.Equals(Status))
                    return;
                Root.Element("status")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     The quality to be downloaded.
        /// </summary>
        public string Resolution {
            get { return Root.Element("resolution")?.Value; }
            set {
                if (value.Equals(Resolution))
                    return;
                Root.Element("resolution")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     If the anime is ongoing and currently airing.
        /// </summary>
        public bool Airing {
            get { return bool.Parse(Root.Element("airing")?.Value ?? bool.FalseString); }
            set {
                if (value == Airing)
                    return;
                Root.Element("airing")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     if searching for the anime should contain exclusively it's own name with no fragments.
        /// </summary>
        public bool NameStrict {
            get { return bool.Parse(Root.Element("name-strict")?.Value ?? bool.FalseString); }
            set {
                if (value == NameStrict)
                    return;
                Root.Element("name-strict")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     If searching for the anime should only download from a specific subgroup if chosen
        /// </summary>
        public string PreferredSubgroup {
            get { return Root.Element("preferredSubgroup")?.Value; }
            set {
                if (value.Equals(PreferredSubgroup))
                    return;
                Root.Element("preferredSubgroup")?.SetValue(value);
                Save();
            }
        }

        /// <summary>
        ///     Proper title name of anime.
        /// </summary>
        /// <returns>A title</returns>
        public string Title => new CultureInfo("en-US", false).TextInfo.ToTitleCase(Name);

        /// <summary>
        ///     A zero padded string of the number of the next episode.
        /// </summary>
        /// <returns>A padded string representation of the next episode in sequence.</returns>
        public string NextEpisode() => $"{int.Parse(Episode) + 1:D2}";

        /// <summary>
        ///     Joins properties of anime together to a string that can be read by an RSS query.
        /// </summary>
        /// <returns>A RSS parsable string.</returns>
        public string ToRss() {
            string[] seperators = {string.Join("+", Title.Split(' ')), NextEpisode(), Resolution};
            return string.Join("+", seperators);
        }

        /// <summary>
        ///     Seeks the next episode for the current anime on Nyaa.eu
        /// </summary>
        /// <returns>A Nyaa object containing information about the file download.</returns>
        public async Task<IEnumerable<Nyaa>> GetLinksToNextEpisode() {
            var url = new Uri("https://www.nyaa.se/?page=rss&cats=1_37&term=" + ToRss() + "&sort=2");
            var client = new WebClient();
            var document = new HtmlDocument();
            var html = await Task.Run(() => client.DownloadString(url));
            document.LoadHtml(html);
            var itemNodes = document.DocumentNode.SelectNodes("//item");

            var result = itemNodes?.Select(n => new Nyaa(n))
                .Where(n => n.Measurement.Equals("MiB") &&
                            n.Size > 10 &&
                            n.StrippedName().Contains(NextEpisode()) &&
                            n.Seeders > 0);

            return result;
        }
        
    }
    
}