using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star_Trek_Button_Tool
{
    public partial class SoundButtonTool_Form : Form
    {

        string soundDirectory = @"C:\";

        WMPLib.WindowsMediaPlayer wplayer;

        public SoundButtonTool_Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            wplayer = new WMPLib.WindowsMediaPlayer();

            //wplayer.URL = @"C:\Grant\Dropbox (Personal)\0 - Star Trek Button Tool\01-16- Buttons On Bridge.mp3";

            //wplayer.URL = @".\Sound Effect Files\01-16- Buttons On Bridge.mp3";

            // wplayer.URL = @"C:\Users\Adil M\Documents\Visual Studio 2012\adil.mp3";

            textBox_SoundDirectory.Text = @"C:\";
        }


        private void Play_Button(string fileName)
        {
            string soundFilePath = System.IO.Path.Combine(soundDirectory, fileName);
            wplayer.URL = soundFilePath;
            wplayer.controls.play();
        }


        private void button_Play_Sound_1_Click(object sender, EventArgs e)
        {
            Play_Button("01 Button Tap.wav");
        }

        private void button_Play_Sound_2_Click(object sender, EventArgs e)
        {
            Play_Button("02 Button Tap Soft.wav");
        }

        private void button_Play_Sound_3_Click(object sender, EventArgs e)
        {
            Play_Button("03 Confirm.wav");
        }

        private void button_Play_Sound_4_Click(object sender, EventArgs e)
        {
            Play_Button("04 Cancel.wav");
        }

        private void button_Play_Sound_5_Click(object sender, EventArgs e)
        {
            Play_Button("05 Attention Chirp.wav");
        }

        private void button_Play_Sound_6_Click(object sender, EventArgs e)
        {
            Play_Button("06 Warning.wav");
        }

        private void button_Play_Sound_7_Click(object sender, EventArgs e)
        {
            Play_Button("07 Critical Alert.wav");
        }

        private void button_Play_Sound_8_Click(object sender, EventArgs e)
        {
            Play_Button("08 Processing Loop.wav");
        }

        private void button_Play_Sound_9_Click(object sender, EventArgs e)
        {
            Play_Button("09 Scan Sweep.wav");
        }

        private void button_Play_Sound_10_Click(object sender, EventArgs e)
        {
            Play_Button("10 Data Transfer.wav");
        }

        private void button_Play_Sound_11_Click(object sender, EventArgs e)
        {
            Play_Button("11 System Startup.wav");
        }

        private void button_Play_Sound_12_Click(object sender, EventArgs e)
        {
            Play_Button("12 System Shutdown.wav");
        }

        private void button_Play_Sound_13_Click(object sender, EventArgs e)
        {
            Play_Button("13 Task Complete.wav");
        }

        private void button_Play_Sound_14_Click(object sender, EventArgs e)
        {
            Play_Button("14 Program Launch.wav");
        }

        private void button_Play_Sound_15_Click(object sender, EventArgs e)
        {
            Play_Button("15 Window Open.wav");
        }

        private void button_Play_Sound_16_Click(object sender, EventArgs e)
        {
            Play_Button("16 Window Close.wav");
        }

        private void button_Play_Sound_17_Click(object sender, EventArgs e)
        {
            Play_Button("17 Transition Shimmer.wav");
        }

        private void button_Play_Sound_18_Click(object sender, EventArgs e)
        {
            Play_Button("18 Ambient Console.wav");
        }



        private void button_SetSoundDirectory_Click(object sender, EventArgs e)
        {
            soundDirectory = textBox_SoundDirectory.Text;
        }

        private void button_Select_Click(object sender, EventArgs e)
        {
            // Use FolderBrowserDialog instead of OpenFolderDialog
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a directory";
                folderDialog.SelectedPath = @"C:\"; // Optional default path

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    soundDirectory = folderDialog.SelectedPath;

                    textBox_SoundDirectory.Text = soundDirectory;
                }
            }
        }
    }
}
