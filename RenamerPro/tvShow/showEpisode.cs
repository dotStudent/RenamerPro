using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerPro
{
    public class showEpisode:tvShow
    {
        //TVDB Data
        public int tvDbEpisodeId { get; set; }

        public string episodeName { get; set; }
        public string episodeDescription { get; set; }
        public DateTime episodeAirDate { get; set; }

        //FileData
        public string filePath { get; set; }
        public string path { get; private set; }
        public string[] splitPath { get; private set; }
        public string oldFileName { get; private set; }
        public string fileExtension { get; private set; }
        public int seasonNr { get; set; }
        public int episodeNr { get; set; }
        public string episodeNumbering { get; set; }
        public string newFilename { get; set; }
        public string subPath { get; private set; }

        public showEpisode(string _filePath)
        {
            filePath = _filePath;
            path = Path.GetDirectoryName(_filePath);
            splitPath = path.Split('\\');
            oldFileName = Path.GetFileNameWithoutExtension(_filePath);
            fileExtension = Path.GetExtension(_filePath);
            subPath = CheckForSubs(path);
        }
        private string CheckForSubs(string path)
        {
            string newFilePath = path + "\\Subs";
            if (Directory.Exists(newFilePath) == true)
            {
                if (fileHelper.IsDirectoryEmpty(newFilePath) == false)
                {
                    return newFilePath;
                }
            }
            return "";
        }
    }
}
