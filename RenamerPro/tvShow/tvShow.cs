using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerPro
{
    public class tvShow:List<showEpisode>
    {

        public void AddEpisode (showEpisode epi)
        {
            this.Add(epi);
            //base.Add(epi);
        }

        #region Getter/Setter
        public int tvdbID { get; set; }
        public string name { get; set; }
        public string orgName { get; set; }
        public string language { get; set; }
        public int year { get; set; }
        public string description { get; set; }
        public int distance { get; set; }
        #endregion
    }
}
