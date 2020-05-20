using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvDbSharper;

namespace RenamerPro
{
    public partial class frmMain : Form
    {
        string[] FileList;
        tvDbHelper tvhelper = new tvDbHelper();
        public frmMain()
        {
            InitializeComponent();
            gBFiles.AllowDrop = true;
            lbTvShows.SelectedValueChanged += new System.EventHandler(this.lbTvShows_SelectedValueChanged);
            lbTvShows.SelectedValueChanged -= new System.EventHandler(this.lbTvShows_SelectedValueChanged);

            Data.ApiKey = "4BAEAD7CF74B4493";
            Data.UserName = "tvfreak77";
            Data.UserKey = "2D1M2J52PQC6VWNK";
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            string seriesName = "Shameless";
            var helper = new tvDbHelper();
            var result = await helper.GetTvShowsByNameAsync(seriesName);
            popListBox(result, 0);


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (helper.IsNumeric(tbTvdbId.Text))
            {
                var helper = new tvDbHelper();
                var result = helper.GetTvShowsByTvDbIdAsync(Convert.ToInt32(tbTvdbId));
                lbTvShows.DataSource = result;
            }
            
        }

        private void lbTvShows_SelectedValueChanged(object sender, EventArgs e)
        {
            tvShow selectedShow = (tvShow)lbTvShows.SelectedItem;
            tbTvdbId.Text = selectedShow.tvdbID.ToString();
            tvhelper.GetTvShowData(FileList, selectedShow.tvdbID);
            tvhelper.TvShows.FindIndex(a => a.tvdbID.Equals(tvhelper.actualTvDbID));
        }
        private void popListBox(tvShows show, int selIndex)
        {
            if (tvhelper.TvShows != null && tvhelper.TvShows.Count > 0)
            {
                lbTvShows.SelectedValueChanged -= new System.EventHandler(lbTvShows_SelectedValueChanged);
                lbTvShows.DataSource = show;
                lbTvShows.DisplayMember = "name";
                //lbTvShows.SelectedIndex = selIndex;
                lbTvShows.SelectedValueChanged += new System.EventHandler(lbTvShows_SelectedValueChanged);
                tvShow selectedShow = (tvShow)lbTvShows.SelectedItem;
                //tbTvdbId.Text = selectedShow.tvdbID.ToString();
                var test = tvhelper.TvShows;
                MessageBox.Show("Finish");
            }
        }

        private void gBFiles_DragDrop(object sender, DragEventArgs e)
        {
            FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            tvhelper.GetTvShowData(FileList, -1);
            popListBox(tvhelper.TvShows, 0);
            var test = tvhelper.TvShows;
            //MessageBox.Show("Finish");
        }
        private void gBFiles_DragEnter(object sender, DragEventArgs e)
        {
            gBFiles.Font = new Font(gBFiles.Font, FontStyle.Bold);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void gBFiles_DragLeave(object sender, EventArgs e)
        {
            gBFiles.Font = new Font(gBFiles.Font, FontStyle.Regular);
        }
    }
}
