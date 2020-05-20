using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvDbSharper;
using TvDbSharper.Dto;

namespace RenamerPro
{
    public class tvDbHelper
    {
        internal bool seasonFolderFound { get; set; } = false;
        internal tvShows TvShows;
        internal int actualTvDbID;

        internal async void  GetTvShowData(string[] fileList, int tvDbId)
        {
            if (tvDbId == -1)
            {
                string tvShowName = null;
                if (fileList.Length > 0)
                {

                    tvShowName = GetTvShowFromFilePath(fileList.First());
                    tvShows shows = await GetTvShowsByNameAsync(tvShowName);
                    foreach (string filePath in fileList)
                    {
                        showEpisode ep = GetEpisodeInfoFromFile(filePath);
                        shows[0].AddEpisode(ep);

                    }
                }
                if (tvShowName != null)
                {
                    TvShows = await GetTvShowsByNameAsync(tvShowName);
                    actualTvDbID = TvShows[0].tvdbID;
                }
            }
            else
            {
                actualTvDbID = tvDbId;
                TvShows = await GetTvShowsByTvDbIdAsync(tvDbId);
            }
            GetEpisodes();

        }

        internal async Task<tvShows> GetTvShowsByNameAsync (string tvShowName)
        {
            var client = new TvDbClient();
            client.AcceptedLanguage = "de";
            await client.Authentication.AuthenticateAsync(Data.ApiKey, Data.UserName, Data.UserKey);
            var response = await client.Search.SearchSeriesByNameAsync(tvShowName);
            tvShows shows = new tvShows(response, tvShowName);
            return shows;
        }
        internal async Task<tvShows> GetTvShowsByTvDbIdAsync (int id)
        {
            var client = new TvDbClient();
            client.AcceptedLanguage = "de";
            await client.Authentication.AuthenticateAsync(Data.ApiKey, Data.UserName, Data.UserKey);
            var response = await client.Series.GetAsync(id);
            tvShows shows = new tvShows(response);
            return shows;
        }
        private string GetTvShowFromFilePath(string filePath)
        {
            int seriesPosition = -1;
            //Split Path to Foldernames
            string[] arrpath = filePath.Split('\\');
            for (int i = 0; i < arrpath.Count(); i++)
            {
                //Inspect Folders if they have "Season" or anything else in it...
                for (int j = 0; j < Data.seasonPathNames.Count(); j++)
                {
                    if (arrpath[i].ToLower().Contains(Data.seasonPathNames[j].ToLower()))
                    {
                        //Series Name is always a Level higher than the Season Number
                        seriesPosition = i - 1;
                        seasonFolderFound = true;
                        break;
                    }
                }
            }
            if (seriesPosition >= 0 && seasonFolderFound == true)
            {
                //We have a TV Show
                return arrpath[seriesPosition].Replace(".", " ");
            }
            else
            {
                //We have a Movie
                return null;
            }
        }

        private async void GetEpisodes()
        {
            if (TvShows != null)
            {
                //Load Episode Data from TvShows and write back to TvShows;
                var client = new TvDbClient();
                await client.Authentication.AuthenticateAsync(Data.ApiKey, Data.UserName, Data.UserKey);
                client.AcceptedLanguage = Data.prefLang;

                var tasks = new List<Task<TvDbResponse<TvDbSharper.Dto.EpisodeRecord[]>>>();
                var response = new TvDbResponse<EpisodeRecord[]>();
                tvShow show = TvShows.Find(i => i.tvdbID == actualTvDbID);
                if (show.Count > 1)
                {
                    foreach (showEpisode ep in TvShows.Where(i => i.tvdbID == actualTvDbID))
                    {
                        response = await client.Series.GetEpisodesAsync(ep.tvdbID, 1);
                        for (int i = 2; i <= response.Links.Last; i++)
                        {
                            tasks.Add(client.Series.GetEpisodesAsync(ep.tvdbID, i));
                        }
                    }
                }
                else
                {
                    EpisodeQuery q = new EpisodeQuery();
                    q.AiredSeason = TvShows[0][0].seasonNr;
                    q.AiredEpisode = TvShows[0][0].episodeNr;
                    response = await client.Series.GetEpisodesAsync(show.tvdbID, 1, q);
                }

                var results = await Task.WhenAll(tasks);


                //var eps = response.Data.Concat(results.SelectMany(x => x.Data));
                foreach (EpisodeRecord er in response.Data.Concat(results.SelectMany(x => x.Data)))
                {
                    foreach (showEpisode ep in TvShows.Where(i => i.tvdbID == actualTvDbID))
                    {
                        if (er.AiredSeason == ep.seasonNr && er.AiredEpisodeNumber == ep.episodeNr)
                        {
                            //DateTime dt = DateTime.Parse(ep.FirstAired);

                            if (er.EpisodeName == null) //Backup if no German
                            {
                                client.AcceptedLanguage = "en";
                                TvDbResponse<EpisodeRecord> er1 = await client.Episodes.GetAsync(er.Id);

                                ep.episodeName = er1.Data.EpisodeName;
                                ep.description = er1.Data.Overview;
                            }
                            else
                            {
                                ep.episodeName = er.EpisodeName;
                                ep.description = er.Overview;
                            }
                            ep.tvDbEpisodeId = er.Id;
                            if (helper.isDateTime(er.FirstAired))
                            {
                                ep.episodeAirDate = DateTime.Parse(er.FirstAired);
                            }
                            if (ep.episodeName != null && ep.episodeNumbering != null)
                            {
                                ep.newFilename = GetCorrectedFileName(ep.episodeName, ep.episodeNumbering);
                            }
                        }
                    }
                }
                //Episode not found in Episode List

            }
        }
        private showEpisode GetEpisodeInfoFromFile(string filePath)
        {
            showEpisode ep = new showEpisode(filePath);
            
            string strRegex = @"(?i:S)\d{1,2}(?i:E)\d{0,1}\w+.*";
            string strRegex1 = @"(?i:)\d{1,2}x(?i:)\d{0,1}\w+";
            string strRegex2 = @"(?i:)\d{1,2}(?i:E)\d{0,1}\w+";

            if (Regex.IsMatch(filePath, strRegex)) //Check if s0(0)e0(0) is in Filename
            {
                ep.episodeName = Convert.ToString(Regex.Match(ep.filePath, strRegex)); //Cut Part before matching string
                ep = episodeImprove(ep, true); //Send to Function to improve Episode Information
                return ep; //Return Improved and Cutted Filename
            }
            else if (Regex.IsMatch(ep.oldFileName.ToLower(), strRegex1)) //Check for Schema (S)SxEE e.g. 01x01 or 1x01
            {
                string season = "";
                string episode = "";
                Regex regex = new Regex(strRegex1);
                Match match = regex.Match(ep.oldFileName);
                var epinfo = ExtractEpisodeInfosFromString(match.Value.ToLower().Replace("x", ""));
                season = epinfo.Item1;
                episode = epinfo.Item2;
                if (season.Length > 0 && episode.Length > 0)
                {
                    ep.seasonNr = Convert.ToInt32(season);
                    ep.episodeNr = Convert.ToInt32(episode);
                    ep.episodeNumbering = GetNiceEpisodeInfo(season, episode);
                    return ep;
                }
            }
            else if (Regex.IsMatch(ep.oldFileName.ToLower(), strRegex2)) //Check for Schema (S)SeEE e.g. 01e01 or 1e01
            {
                string season = "";
                string episode = "";
                Regex regex = new Regex(strRegex2);
                Match match = regex.Match(ep.oldFileName);
                var epinfo = ExtractEpisodeInfosFromString(match.Value.ToLower().Replace("x", ""));
                season = epinfo.Item1;
                episode = epinfo.Item2;
                if (season.Length > 0 && episode.Length > 0)
                {
                    ep.seasonNr = Convert.ToInt32(season);
                    ep.episodeNr = Convert.ToInt32(episode);
                    ep.episodeNumbering = GetNiceEpisodeInfo(season, episode);
                    return ep;
                }
            }
            else //We have to guess....
            {
                string[] filechunks = ep.oldFileName.Split('-');

                string season = "";
                string episode = "";

                foreach (string part in filechunks)
                {
                    part.Replace(" ", "");
                }
                for (int i = 0; i < filechunks.Length; i++)
                {
                    if (helper.IsNumeric(filechunks[i]) == true) //Season and Episode is only numerically
                    {
                        string suspect = filechunks[i];
                        var epinfo = ExtractEpisodeInfosFromString(suspect);
                        season = epinfo.Item1;
                        episode = epinfo.Item2;

                        if (season.Length > 0 && episode.Length > 0)
                        {
                            ep.seasonNr = Convert.ToInt32(season);
                            ep.episodeNr = Convert.ToInt32(episode);
                            ep.episodeNumbering = GetNiceEpisodeInfo(season, episode);
                        }
                    }
                }

            }
            return ep; // Nothing to cut...send back received filename
        }
        private (string, string) ExtractEpisodeInfosFromString(string epinfo)
        {
            string season = "";
            string episode = "";
            if (helper.IsNumeric(epinfo) == true) //Season and Episode is only numerically
            {
                string suspect = epinfo;
                if (suspect.Length == 4) //Propapbly Season 2 digit and Episode 2 digit
                {
                    season = suspect.Substring(0, 2);
                    episode = suspect.Substring(2, 2);
                }
                else if (suspect.Length == 3) //Propapbly Season 1 digit and Episode 2 digit
                {
                    season = suspect.Substring(0, 1);
                    episode = suspect.Substring(1, 2);
                }
            }
            else
            {
                if (epinfo.Contains("e"))
                {
                    string[] parts = epinfo.Split('e');
                    if (parts.Length == 2 && helper.IsNumeric(parts[0]) == true && helper.IsNumeric(parts[1]))
                    {
                        season = parts[0];
                        episode = parts[1];
                    }
                }
            }
            return (season, episode);
        }
        private string GetNiceEpisodeInfo(string season, string episode)
        {
            string episodeNumber = "";
            if (season.Length < 2) //Season is only 1 digit
            {
                season = "0" + season; //Add 0 before season number
            }
            if (episode.Length < 2) //Episode is only 1 digit
            {
                episode = "0" + episode; //Add 0 before episode number
            }
            episodeNumber = "S" + season + "E" + episode;

            return episodeNumber;
        }
        private showEpisode episodeImprove(showEpisode ep, bool prefix) //Take Care that Episode Information is Always in Upper and 2 digit
        {
            string regex = @"(?i:S)\d*(?i:E)\d*";
            string season = "";
            string episode = "";
            string episodename = "";
            string episodepart = Regex.Match(ep.oldFileName, regex).ToString(); //Extract Episode Part from Filename
            episodename = ep.episodeName.Replace(episodepart, ""); //Remove Episode Part from Filename
            if (Regex.IsMatch(episodepart, regex) == true)// && episodepart.Length == 6)
            {
                episodepart = episodepart.ToUpper(); //Bring characters to upper
                string[] seasonepisode = episodepart.Split('E'); //Split Season Part from Episode
                season = seasonepisode[0].Replace("S", ""); //Only give Season number
                ep.seasonNr = Convert.ToInt32(season);
                episode = seasonepisode[1]; //Only give Episode number
                ep.episodeNr = Convert.ToInt32(episode);
                if (season.Length < 2) //Season is only 1 digit
                {
                    season = "0" + season; //Add 0 before season number
                }
                if (episode.Length < 2) //Episode is only 1 digit
                {
                    episode = "0" + episode; //Add 0 before episode number
                }
                if (prefix == true)
                {
                    ep.episodeNumbering = "S" + season + "E" + episode;
                    ep.newFilename = "S" + season + "E" + episode + episodename; //Bulid and return full Episode Information String
                }
                else
                {
                    ep.episodeNumbering = "S" + season + "E" + episode;
                }
            }
            else
            {
                episodepart = episodepart.ToUpper(); //Bring characters to upper
                ep.episodeNumbering = episodepart;
                string[] seasonepisode = episodepart.Split('E'); //Split Season Part from Episode
                season = seasonepisode[0].Replace("S", ""); //Only give Season number
                ep.seasonNr = Convert.ToInt32(season);
                episode = seasonepisode[1]; //Only give Episode number
                ep.episodeNr = Convert.ToInt32(episode);

                if (prefix == true)
                {
                    ep.episodeName = episodepart.ToUpper() + episodename; //Take care, "S" and "E" are Upper Case
                }
                else
                {
                    ep.episodeName = episodepart.ToUpper();
                }
            }
            return ep;
        }
        private string GetCorrectedFileName(string _OrgFileName, string epNumber)
        {
            _OrgFileName = _OrgFileName.TrimEnd(' ');
            string filename = _OrgFileName;
            if (Data.ReplSpecial == true)
            {
                Dictionary<string, string> replacements = new Dictionary<string, string>()
            {
                {"ä","ae"},
                {"Ä","Ae"},
                {"ü","ue"},
                {"Ü","Ue"},
                {"ö","oe" },
                {"Ö","Oe" },
                {" ","." }, //Replace Whitespace with "."
                {"ß","ss" },
                {"\"","" },
                {":","" },
                {"^","" },
                {"~","" },
                {"<","" },
                {">","" },
                {",","" },
                {"'","" },
                {"!","" },
                {"?","" },
                {"*","" },
                {"+","" },
                {"§","" },
                {"$","" },
                {"%","" },
                {"&","" },
                {"/","" },
                {"(","" },
                {")","" },
                {"[","" },
                {"]","" },
                {"{","" },
                {"}","" },
            };
                filename = replacements.Aggregate(_OrgFileName, (current, value) =>
                                        current.Replace(value.Key, value.Value));
                filename = filename.TrimEnd('.');
                while (filename.Contains("..") == true)
                {
                    filename = filename.Replace("..", ".");
                }
            }
            return epNumber + "." + filename;
        }
    }
}
