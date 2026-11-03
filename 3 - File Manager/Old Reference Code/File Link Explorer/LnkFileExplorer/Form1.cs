using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Runtime.InteropServices;

/*
 Even Encrypted and compressed files are supported. But this seems to be Disk IO matter.
            
    Link File Structure:
        SHELL_LINK_HEADER   length 0x1c
        LINKTARGET_IDLIST   Optional, set by LinkFileHeader.shortcutFlags (HasLinkTargetIDList Bit 0 (& 0x1))
        LINKINFO            Optional, set by LinkFileHeader.shortcutFlags (HasLinkInfo Bit 1 (& 0x2))
        STRING_DATA         Depending on the flags
        EXTRA_DATA
   
   
   To Do:   Location transfer list for colors for HexEdit and modification of HexEdit
            !!!     (The usual marker for locations to be checked up).
            
   
 * Calculating Icon location pointers:
 
        In the Link file: DWORD shortcutFlags @ 0x14
        -(LinkFileHeader.shortcutFlags & HasExpIcon (x4000) >0
            First Unicode string of the IconEnvironmentDataBlock pointer to bmp file named *.exe
                Yes, there are exe files which are bmp files! Check the exe files offset 0x0 for the lack of  "MZ"
   
        -(LinkFileHeader.shortcutFlags & HasIconLocation (0x40)) >0
                0x4c + DWORD (0x4c) + trace throught the Unicode strings using LinkFileHeader.shortcutFlags
                        HasLinkInfo = 0x2;       
                        HasName = 0x4;           
                        HasRelativePath = 0x8;   
                        HasWorkingDir = 0x10;    
                        HasArguments = 0x20; 
                pointer to file named *.exe. Check the signature (WORD @ 0x0) if it's a bmp or an exe file
                
        -Exe file Icon resources, get the first icon
*/

namespace LnkFileExplorer
{
    public partial class Form1 : Form
    {
        FileStream fs;
        BinaryReader reader;
        GCHandle handle;
        bool internetShortcut = false;
        string linkPath;
        About aboutBox;

        public Form1()
        {
            InitializeComponent();
            aboutBox = new About();
            linkPath = "C:\\Users\\" + Environment.GetEnvironmentVariable("USERNAME") + "\\Desktop";
            getDesktopLinks();
            dataGridView1.Focus();
            Form1_Resize(null, null);
        }

        private void openFile_Click(object sender, EventArgs e)
        { // openFileDialog1.DereferenceLinks Property is set to false since we want the shortcut file itself
            dataGridView1.Focus();
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            arbitrateLinkFile();
        }

        public void arbitrateLinkFile()
        {
            internetShortcut = false;
            textBox1.Visible = false;
            dataGridView1.Rows.Clear(); // Header & Tables
            dataGridView2.Rows.Clear(); // Table Entries
            dataGridView3.Rows.Clear(); // Extra Tables
            cellPtrs1.Clear();
            cellPtrs2.Clear();
            colColorChange2 = 255;

            fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            reader = new BinaryReader(fs);

            Text = "Link File Explorer:  " +openFileDialog1.FileName;
            // display some file info
            dataGridView1.Rows.Add("File Info:", " ");
            dataGridView1.Rows.Add("  Link Name", Path.GetFileName(openFileDialog1.FileName));
            dataGridView1.Rows.Add("  Created in Windows on", File.GetCreationTime(openFileDialog1.FileName).ToString("F"));
            dataGridView1.Rows.Add("  Last Accessed in Windows on", File.GetLastAccessTime(openFileDialog1.FileName).ToString("F"));
            dataGridView1.Rows.Add("  Last Modified in Windows on", File.GetLastWriteTime(openFileDialog1.FileName).ToString("F"));
            dataGridView1.Rows.Add("  File Size (Bytes)", fs.Length);
            dataGridView1.Rows.Add("  File Attributes",File.GetAttributes(openFileDialog1.FileName));
            dataGridView1.Rows.Add("", "");

            cellPtrs1.Add(dataGridView1.Rows.Count);
            // If displayLinkFileHeaderAndFlags returns false then we deal with an Internet shortcut text file
            if(displayLinkFileHeaderAndFlags() != true)
            { // Internet shortcut text files
                internetShortcut = true;
                fs.Close();
                textBox1.Text = "";
                textBox1.Visible = true;
                string shlp;

                FileStream textStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader textReader = new StreamReader(textStream, System.Text.Encoding.ASCII);
                while (textReader.Peek() >= 0)
                {
                    shlp = textReader.ReadLine();
                    textBox1.Text += shlp+(char)0xd + (char)0xa;
                }
                textStream.Close();
                textStream.Dispose();
                textReader.Close();
                textReader.Dispose();
                return;
            }
            // The file pointer fs.Position is set to the TargetIdList
            cellPtrs1.Add(dataGridView1.Rows.Count);
            dataGridView1.Rows.Add("", "");
            colColorChange = dataGridView2.Rows.Count - 1;
            
            // win explorer: No ID, No, Link, shell lnk present; struct: HDR, LnkHdr, shell lnk
            // This item is only present if bit 0 is set in the flags word of the header.
            if ((LinkFileHeader.shortcutFlags & HasLinkTargetIDList) > 0) // is there a linkIdList following the header?
                displayTargetIdList();

            cellPtrs1.Add(dataGridView1.Rows.Count);
            colColorChange1 = dataGridView2.Rows.Count - 1;
            if ((LinkFileHeader.shortcutFlags & HasLinkInfo) > 0)
                displayLinkInfo();

            cellPtrs1.Add(dataGridView2.Rows.Count);

            // Display the location strings
            stringsStart = dataGridView1.Rows.Count - 1;

            DisplayStrings();

            // Display extra data tables
            extraDataTables();

            // enable for test purposes only
            /*
            dataGridView2.Rows.Add("");
            dataGridView2.Rows.Add("Link Info Header size" + makeLen8String((UInt32)LinkFileLocations.LinkInfoHeaderSize, 4));
            dataGridView2.Rows.Add("Link Info size" + makeLen8String((UInt32)LinkFileLocations.LinkInfoSize, 4));
            dataGridView2.Rows.Add("Base Path @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetBasePath + 0x4c, 4));
            dataGridView2.Rows.Add("Final Path @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetFinalPath + 0x4c, 4));
            dataGridView2.Rows.Add("Network @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetNetwork + 0x4c, 4));
            dataGridView2.Rows.Add("Volume Table @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetVolumeTable + 0x4c, 4));
            dataGridView2.Rows.Add("Shortcut Flags 0x" + makeLen8String((UInt32)LinkFileHeader.shortcutFlags, 4));
            dataGridView2.Rows.Add("Net Name Unicode @ 0x" + makeLen8String((UInt32)NetNameOffsetUnicode + 0x4c, 4));
            dataGridView2.Rows.Add("Device Name Unicode @ 0x" + makeLen8String((UInt32)DeviceNameOffsetUnicode + 0x4c, 4));
            dataGridView2.Rows.Add("Volume Flags @ 0x" + makeLen8String((UInt32)LinkFileLocations.LinkInfoFlags, 4));
             * */
            // end test

            dataGridView1.Rows.Add("");
            dataGridView1.Rows.Add("* Dates displayed are Link internal");
            dataGridView1.Rows.Add("    dates in UTC format!");
            handle.Free();
            fs.Close();
            dataGridView1.Focus();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fs != null)
            {
                fs.Dispose();
                reader.Dispose();
            }
            aboutBox.Dispose();
        }
    }

    class Images
    {
        public static Image SplashScreen = Image.FromStream(Program.GetResource("Splash Screen.png"));
    }
}
