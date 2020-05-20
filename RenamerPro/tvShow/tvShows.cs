using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvDbSharper.Dto;

namespace RenamerPro
{
    class tvShows:List<tvShow>
    {
        public tvShows(TvDbResponse<SeriesSearchResult[]> results, string searchString)
        {
            //lstTvShow = new List<tvShow>();
            
            foreach (var series in results.Data)
            {
                tvShow show = ConvertTvDbSeriesToTvShow(series.Id, series.SeriesName, series.Overview, series.FirstAired, searchString);
                base.Add(show);
            }
            base.Sort((x, y) => x.distance.CompareTo(y.distance));
        }
        public tvShows (TvDbResponse<TvDbSharper.Dto.Series> tvshow)
        {
            tvShow show = ConvertTvDbSeriesToTvShow(Convert.ToInt32(tvshow.Data.SeriesId), tvshow.Data.SeriesName, tvshow.Data.Overview, tvshow.Data.FirstAired, null);
            base.Add(show);
        }
        #region Private Methods
        private tvShow ConvertTvDbSeriesToTvShow(int showId, string showName, string showOverview, string showFirstAired, string searchString)
        {
            tvShow show = new tvShow();
            show.tvdbID = showId;
            show.orgName = showName;
            show.description = showOverview;
            var ret = getNameAndYear(showName);
            show.year = ret.Item1;
            show.name = ret.Item2;
            if (show.year == 0)
            {
                int yearFromAirDate = getYearFromAirDate(showFirstAired);
                if (yearFromAirDate != -1)
                {
                    show.year = yearFromAirDate;
                }
            }
            if (searchString != null || searchString == "")
            {
                show.distance = LevenshteinDistance(searchString.ToLower(), show.name.ToLower());
            }
            return show;
        }
        private (int, string) getNameAndYear(string str)
        {
            Regex regex = new Regex(@"\(([^)]*)\)$");
            Match match = regex.Match(str);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int value) == true)
            {
                int extractYear = Convert.ToInt32(match.Groups[1].Value);
                string substring = Regex.Match(str, @"\(([^)]*)\)$").Groups[0].Value;
                string newname = "";
                if (substring.Length > 0)
                {
                    newname = str.Replace(" " + substring, "");
                }
                return (extractYear, newname);
            }
            else
            {
                return (0, str);
            }
        }
        private int getYearFromAirDate(string str)
        {
            DateTime FirstAired;
            try
            {
                FirstAired = Convert.ToDateTime(str);
                return FirstAired.Year;
            }
            catch
            {

            }
            return -1;
        }
        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        #endregion
    }
}
