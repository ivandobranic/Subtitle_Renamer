using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleRenamer
{

    /// <summary>
    // Program pronalazi video i subtitle extenzije, izdvaja naziv i sprema ih u array-e. U tim array-ima
    // uklanja najcesce separatore i razmak, i sprema u nove arraye. Ukoliko je u naslovu godina filma
    // (sto je i cest slucaj) uklanja sve u nazivu nakon godine i ostavlja sam naslov filma za usporedbu.
    // Ukoliko subtitle file sadrzi nazive iz video array-a, video file se rename-a sa subtitleov-im.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;

        }


        string[] videoPathss;
        string[] subtitlePathss;
        string selectedVideos;
        string selectedSubtitles;
        string regularExpression = "\\d\\d\\d\\d";
        Match Year;
        string resolution = "1080|720|2140|blue-ray|rip|cam|ts|web";
        Match videoResolution;
        string TvShowExpression = "\\w\\d\\d\\w\\d\\d";
        Match TvShow;
        string TvShowExpression2 = "\\d\\dx\\d\\d";
        Match TvShow2;
        string subtitle2;
        string video2;
        string TvShowValue;
        string TvShowValue2;
        int TvShowIndex;
        int realTvShowIndex;
        int searchYearIndex;
        int searchResolutionIndex;
        string resolutionValue;
        string searchValue;
        string subtitleName;
        string fullPath;
        string[] videoFiles;
        int succesfullyRenamed = 0;
        int count = 0;
        


        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedVideos = folderBrowserDialog1.SelectedPath;
                var videoPaths = Directory.EnumerateFiles(selectedVideos, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".mp4") || s.EndsWith(".mkv") || s.EndsWith(".avi")
                    || s.EndsWith(".mov") || s.EndsWith(".mpg") || s.EndsWith(".flv"))
                    .Select(Path.GetFullPath);

                videoPathss = videoPaths.ToArray();



            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            button3.Enabled = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedSubtitles = folderBrowserDialog1.SelectedPath;

                var subtitlePaths = Directory.EnumerateFiles(selectedSubtitles, "*.*", SearchOption.AllDirectories)
                   .Where(s => s.EndsWith(".srt") || s.EndsWith(".sub") || s.EndsWith(".txt"))
                   .Select(Path.GetFileNameWithoutExtension);

                subtitlePathss = subtitlePaths.ToArray();

            }



        }



        private void button3_Click(object sender, EventArgs e)
        {
          for (int i = 0; i<1; i++)
            {
               subtitle2 = subtitlePathss[i];
            }

            TvShow2 = Regex.Match(subtitle2, TvShowExpression2, RegexOptions.IgnoreCase);
            TvShowValue2 = TvShow2.Value;
            int TvShowIndex2 = subtitle2.IndexOf(TvShowValue2);
            int realTvShowIndex2 = TvShowIndex2 + TvShowValue2.Length;
            try
            {

                if (videoPathss.Length != 0)
                {
                    if (subtitlePathss.Length != 0)
                    {
                        foreach (string video in videoPathss)
                        {

                            
                            
                            Year = Regex.Match(video, regularExpression);
                            searchValue = Year.Value;
                            searchYearIndex = video.IndexOf(searchValue);

                            videoResolution = Regex.Match(video, resolution, RegexOptions.IgnoreCase);
                            resolutionValue = videoResolution.Value;
                            searchResolutionIndex = video.IndexOf(resolutionValue);

                            TvShow = Regex.Match(video, TvShowExpression, RegexOptions.IgnoreCase);
                            TvShowValue = TvShow.Value;
                            TvShowIndex = video.IndexOf(TvShowValue);
                            realTvShowIndex = TvShowIndex + TvShowValue.Length;
                            
                           if (TvShowIndex2 != 0 && subtitle2.Contains(TvShowValue2))
                            {
                                
                                video2 = video.Remove(realTvShowIndex);
                                video2 = video2.Remove(TvShowIndex, 1);
                                video2 = video2.Insert(TvShowIndex + 2, "x");
                                video2 = video2.Remove(TvShowIndex + 3, 1);
                               

                            }

                           else if (TvShowIndex != 0 && video.Contains(TvShowValue))
                            {

                              video2 = video.Remove(realTvShowIndex);

                            }

                           else if (searchYearIndex != 0 && video.Contains(searchValue))
                            {

                               video2 = video.Remove(searchYearIndex);

                            }

                            else if (searchResolutionIndex != 0 && video.Contains(resolutionValue))

                            {
                               video2 = video.Remove(searchResolutionIndex);
                            }

                            else
                            {
                                video2 = video.Remove(video.LastIndexOf("."), video.Length - video.LastIndexOf("."));
                            }


                            videoFiles = GetFileNameToRename2(video2).Split(new Char[] { '.', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);







                            foreach (string subtitle in subtitlePathss)
                            {
                                count = 0;
                               
                               


                                foreach (string videos in videoFiles)
                                {



                                    if (subtitle.IndexOf(videos, StringComparison.OrdinalIgnoreCase) >= 0)

                                    {
                                        count++;
                                        subtitleName = subtitle;
                                    }
                                    else

                                    {
                                        break;
                                    }

                                }
                                if (videoFiles.Length != 0 && videoFiles.Length == count)
                                {

                                    fullPath = ReplaceMethod(video, GetFileNameToRename(video), subtitleName);

                                    try
                                    {
                                        File.Move(video, fullPath);
                                        succesfullyRenamed++;
                                    }

                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                }

                            }
                          

                        }
                   
                    }
                    else
                    {
                        MessageBox.Show("No subtitle files selected, please try again");
                    }


                }



                else
                {
                    MessageBox.Show("No video files selected, please try again.");
                }
            }
            catch
            {
                MessageBox.Show("Please select a folder.");
            }

            if (succesfullyRenamed != 0)
            {
                MessageBox.Show(succesfullyRenamed + " files successfully renamed!");
                button3.Enabled = false;
            }

            else
            {
                MessageBox.Show(" 0 files renamed!");
            }

        }


        // Metoda izdvajanja file name-a iz full path-a
        public static string GetFileNameToRename(string fileName)
        {

            int index = fileName.LastIndexOf("\\");
            int indexOfExtension = fileName.LastIndexOf(".");

            int extensionLenght = fileName.Length - indexOfExtension;
            int realIndex = index + 1;
            int length = fileName.Length - realIndex - extensionLenght;
            string fileNameResult = fileName.Substring(realIndex, length);
            string fileExtension = fileName.Substring(indexOfExtension, extensionLenght);
            return fileNameResult;
        }

        //Metoda izdvajanja izmjenjenog file name-a iz full path-a
        public static string GetFileNameToRename2(string fileName)
        {

            int index = fileName.LastIndexOf("\\");
            int realIndex = index + 1;
            int length = fileName.Length - realIndex;
            string fileNameResult = fileName.Substring(realIndex, length);
            return fileNameResult;
        }

        // Metoda zamjene subtitle file name-a sa video file name-om te izdvajanje novog full patha
        // za rename-anje. Potrebna je u slucaju da su direktorij i file istog imena, replace
        // metoda u tom slucaju moze izmjenit ime direktorija te dati krivi full path za rename-anje.
        public static string ReplaceMethod(string fileName, string movieName, string subtitleName)
        {
            int index = fileName.LastIndexOf("\\");
            int indexOfExtension = fileName.LastIndexOf(".");
            int extensionLenght = fileName.Length - indexOfExtension;
            int realIndex = index + 1;
            int length = fileName.Length - realIndex - extensionLenght;
            string fileNameResult = fileName.Substring(realIndex, length);
            string fileExtension = fileName.Substring(indexOfExtension, extensionLenght);
            string directoryPath = fileName.Substring(0, realIndex);
            fileNameResult = fileNameResult.Replace(movieName, subtitleName);
            string fullPath = directoryPath + fileNameResult + fileExtension;

            return fullPath;
        }

    }
}


