﻿using System;
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
    // U slucaju da nema godine u nazivu onda pravi array resize na osnovu drugih cestih naziva u naslovu.
    // Ukoliko subtitle file sadrzi nazive iz resiz-anog video array-a, video file se rename-a sa subtitleov-im.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;

        }


        string[] videoPathss;
        string[] subtitlePathss;
        string selectedVideos;
        string selectedSubtitles;
        string regularExpression = "[0-9]{1,4}";
        Match Year;
        string resolution = "1080|720|2140|blue-ray|rip|cam|ts|web";
        Match videoResolution;
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
            button3.Enabled = true;
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
            try
            {

                if (videoPathss.Length != 0)
                {
                    if (subtitlePathss.Length != 0)
                    {
                        foreach (string video in videoPathss)
                        {
                            count = 0;
                            videoFiles = GetFileNameToRename(video).Split(new Char[] { '.', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string subtitle in subtitlePathss)
                            {


                                foreach (string videos in videoFiles)
                                {


                                    Year = Regex.Match(video, regularExpression);
                                    searchValue = Year.Value;
                                    searchYearIndex = Array.IndexOf(videoFiles, searchValue);

                                    videoResolution = Regex.Match(video, resolution, RegexOptions.IgnoreCase);
                                    resolutionValue = videoResolution.Value;
                                    searchResolutionIndex = Array.IndexOf(videoFiles, resolutionValue);

                                    if (searchYearIndex != -1 && videos.Contains(searchValue))
                                    {
                                        Array.Resize(ref videoFiles, searchYearIndex);
                                        break;
                                    }

                                    else if (searchResolutionIndex != -1 && videos.Contains(resolutionValue))

                                    {
                                        Array.Resize(ref videoFiles, searchResolutionIndex);
                                        break;
                                    }

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

                            }



                            if (videoFiles.Length == count)
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

            string directoryPath = fileName.Substring(0, realIndex);

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

