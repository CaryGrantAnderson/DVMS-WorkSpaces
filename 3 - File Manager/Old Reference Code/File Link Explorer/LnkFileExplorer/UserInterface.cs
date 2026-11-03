using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

namespace LnkFileExplorer
{
    public partial class Form1 : Form
    {
        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Height = Height - 80;
            dataGridView2.Height = dataGridView1.Height;
            dataGridView3.Height = dataGridView1.Height;
            panel1.Height = dataGridView1.Height;
        }

        Color celColor = Color.Pink;
        Color alternateColor = Color.LightCyan;
        List<int> cellPtrs1 = new List<int>();
        List<int> cellPtrs2 = new List<int>();
        int colColorChange;
        int colColorChange1;
        int colColorChange2 = 0;
        int stringsStart;

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string shlp;
            if (internetShortcut)
            {
                celColor = Color.Pink;
                if (e.RowIndex < dataGridView1.RowCount-2)
                CellPainting(sender, e, dataGridView1);
                return;
            }
            celColor = e.CellStyle.BackColor;
            if (e.FormattedValue != null && e.RowIndex >= 0)
            {
                if (cellPtrs1.Count > 0)
                {
                    if (e.RowIndex < cellPtrs1[0] - 2)
                        celColor = Color.Pink;
                    if (e.RowIndex >= cellPtrs1[0] - 1 && e.RowIndex < cellPtrs1[1] - 1)
                        celColor = Color.FromArgb(255, 255, 233, 110);
                    if (e.RowIndex >= cellPtrs1[1] && e.RowIndex < dataGridView1.Rows.Count - 4)
                        celColor = Color.GreenYellow;

                    shlp = (String)dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue;
                    if (shlp.Contains("Base Path") || shlp.Contains("Link Info List"))
                        celColor = Color.Orange;
                    if (shlp.Contains("Network Relative Link"))
                        celColor = Color.Orange;
                    if (shlp.Contains("Net Name"))
                        celColor = Color.LightBlue;

                    if( e.RowIndex >= stringsStart)
                        celColor = Color.LightBlue;
                    if (e.RowIndex >= dataGridView1.RowCount - 4)
                        celColor = dataGridView1.DefaultCellStyle.BackColor;
                }
                if (dataGridView1.SelectedCells.Contains(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    celColor = e.CellStyle.SelectionBackColor;
            }
            CellPainting(sender, e, dataGridView1);
        }

        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (internetShortcut)
                return;
            celColor = e.CellStyle.BackColor;

            if (e.FormattedValue != null && e.RowIndex >= 0)
            {
                if (e.RowIndex <= colColorChange)
                    celColor = Color.FromArgb(255, 255, 233, 110);
                else
                    celColor = Color.GreenYellow;
                if (e.RowIndex >= colColorChange1)
                    celColor = Color.Orange;
                if (e.RowIndex >= colColorChange2)
                    celColor = Color.LightGreen;

                if ((String)e.FormattedValue == "")
                    celColor = e.CellStyle.BackColor;
                if (dataGridView2.SelectedCells.Contains(dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    celColor = e.CellStyle.SelectionBackColor;
            }
            else
                celColor = e.CellStyle.BackColor;
            CellPainting(sender, e, dataGridView2);
        }

        private void dataGridView3_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (internetShortcut)
                return;

            if (e.FormattedValue != null && e.RowIndex >= 0)
            {
                if (celColor != Color.Lavender || celColor != Color.LightGray)
                    celColor = Color.Lavender;

                if ((String)e.FormattedValue == "")
                    celColor = e.CellStyle.BackColor;

                if (dataGridView3.SelectedCells.Contains(dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    celColor = e.CellStyle.SelectionBackColor;
            }
            else
                celColor = e.CellStyle.BackColor;
            CellPainting(sender, e, dataGridView3);
        }

        private void CellPainting(object sender, DataGridViewCellPaintingEventArgs e, DataGridView Grid)
        {
            string shlp = (String)e.FormattedValue; 
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                Rectangle newRect = new Rectangle(e.CellBounds.X + 1,
                    e.CellBounds.Y + 1, e.CellBounds.Width - 4,
                    e.CellBounds.Height - 4);
                using (
                    Brush gridBrush = new SolidBrush(Grid.GridColor),
                    backColorBrush = new SolidBrush(celColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        // Erase the cell.
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                            e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom - 1);
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                            e.CellBounds.Top, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom);
                        // Draw the text content of the cell, ignoring alignment.
                        if (e.FormattedValue != null)
                        {
                            if(shlp != "")
                            {
                                if (shlp[0] == '*')
                                { // draw not implemented extensions in red
                                    e.Graphics.DrawString((String)e.FormattedValue, e.CellStyle.Font,
                                            Brushes.Red,
                                            e.CellBounds.X + 2,
                                            e.CellBounds.Y + 2, StringFormat.GenericDefault);
                                }
                                else
                                {
                                    if (Grid == dataGridView1 && e.RowIndex >= dataGridView1.RowCount - 3)
                                        if (internetShortcut)
                                            e.Graphics.DrawString((String)e.FormattedValue, e.CellStyle.Font,
                                            Brushes.Black,
                                            e.CellBounds.X + 2,
                                            e.CellBounds.Y + 2, StringFormat.GenericDefault);
                                        else
                                            e.Graphics.DrawString((String)e.FormattedValue, e.CellStyle.Font,
                                            Brushes.Red,
                                            e.CellBounds.X + 2,
                                            e.CellBounds.Y + 2, StringFormat.GenericDefault);
                                    else
                                        e.Graphics.DrawString((String)e.FormattedValue, e.CellStyle.Font,
                                            Brushes.Black,
                                            e.CellBounds.X + 2,
                                            e.CellBounds.Y + 2, StringFormat.GenericDefault);
                                }
                            }
                        }
                        e.Handled = true;
                    }
                }
            }
        }

        private void saveReport_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            int i;
            string shlp;
            using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("Link File Explorer - Link content Report");
                    sw.WriteLine("File & Location:  " + openFileDialog1.FileName);
                    sw.WriteLine("");
                    for (i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        shlp = (String)dataGridView1.Rows[i].Cells[0].FormattedValue;
                        shlp = shlp.PadRight(40, ' ');
                        shlp += (String)dataGridView1.Rows[i].Cells[1].FormattedValue;
                        sw.WriteLine(shlp);
                    }
                    sw.WriteLine("");
                    for (i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        shlp = (String)dataGridView2.Rows[i].Cells[0].FormattedValue;
                        sw.WriteLine(shlp);
                    }
                    sw.WriteLine("");
                    for (i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        shlp = (String)dataGridView3.Rows[i].Cells[0].FormattedValue;
                        sw.WriteLine(shlp);
                    }
                    sw.Flush();
                }
            }
        }

        private void runHexEdit_Click(object sender, EventArgs e)
        { // copy the link file to a backup (.bak) file before displaying it in the hexeditor
            // check if the Hexeditor is present in a subdirectory
            string hexedit, shlp = "", shlp1, shlp2;
            int i;

            // is a file opened?
            if (dataGridView1.Rows.Count < 5)
            {
                MessageBox.Show("Open a link file", "No link file opened!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            hexedit = Application.StartupPath;
            hexedit += "\\HexEdit\\HexEdit.exe";
            if (!File.Exists(hexedit))
            {
                MessageBox.Show("The file (" + hexedit + ") is missing!", "Hexedit does not exist!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Some link names have 2 extensions, so lets take the first one
            // Hexedit will use the executable in the link file, so lets rename it to a back file
            i = openFileDialog1.FileName.IndexOf(".");
            shlp = openFileDialog1.FileName.Substring(0, i + 1);
            shlp += "bak";

            // myProcess.StartInfo.Arguments can not cope with blanks in the Path and FileName, so lets use the root
            shlp1 = "\\" + Path.GetFileName(shlp);
            shlp = "";
            for (i = 0; i < shlp1.Length; i++)
            {
                if (shlp1[i] != ' ')
                    shlp += shlp1[i];
            }

            // virusckeckers can cause troubles with file copies to the root
            shlp = "C:\\Users\\" + Environment.GetEnvironmentVariable("USERNAME") + "\\Documents" + shlp;

            try
            {
                File.Copy(openFileDialog1.FileName, shlp, true);
            }
            catch
            {
                MessageBox.Show("The file can not be opened now!", "The file is in use by another Application", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = hexedit;
                myProcess.StartInfo.Arguments = shlp;
                myProcess.StartInfo.CreateNoWindow = false;
                try
                {
                    myProcess.Start();
                }
                catch
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.Start();
                }
            }
            catch (Win32Exception w)
            {
                shlp = w.Message;
                MessageBox.Show(shlp, "Could not Execute this file!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            myProcess.Dispose();
        }

        private void help_Click(object sender, EventArgs e)
        {
            aboutBox.ShowDialog();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void executeLink_Click(object sender, EventArgs e)
        {
            string link, shlp = "", shlp1, shlp2;
            // is a file opened?
            if (dataGridView1.Rows.Count < 5)
            {
                MessageBox.Show("Open a link file", "No link file opened!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            link = openFileDialog1.FileName;
            if (!File.Exists(link))
            {
                MessageBox.Show("The file (" + link + ") is missing!", "Hexedit does not exist!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = link;
                myProcess.StartInfo.Arguments = shlp;
                myProcess.StartInfo.CreateNoWindow = false;
                try
                {
                    myProcess.Start();
                }
                catch
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.Start();
                }
            }
            catch (Win32Exception w)
            {
                shlp = w.Message;
                MessageBox.Show(shlp, "Could not Execute this file!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            myProcess.Dispose();
        }

        private void reportEdit_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() != DialogResult.OK)
                return;
            StartProcess(openFileDialog2.FileName);
            Visible = true;
        }

        private void editJustSavedReport_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.FileName == "")
                return;
            StartProcess(saveFileDialog1.FileName);
            Visible = true;
        }

        private void startInternet_Click(object sender, EventArgs e)
        {
            if (textBox1.Visible == false)
            {
                MessageBox.Show("Open an Internet Link!", "No Internet Link opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int i = textBox1.Text.IndexOf("URL=");
            if (i < 4)
            {
                MessageBox.Show("The Link does not contain any location!", "Internet Location missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            i += 4;
            string shlp = textBox1.Text.Substring(i, textBox1.Text.Length - i);
            shlp = shlp.Substring(0, shlp.IndexOf("\r"));
            StartProcess(shlp);
            // The link explorer disappears but the process is still running, so
            Visible = true;
        }

        private void pdfFile_Click(object sender, EventArgs e)
        { // get the application path

            string shlp = Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog3.InitialDirectory = shlp;
            if (openFileDialog3.ShowDialog() != DialogResult.OK)
                return;
            StartProcess(openFileDialog3.FileName);
            Visible = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 112)      // F1 About
            {
                aboutBox.ShowDialog();
            }
            if (e.KeyValue == 113)      // F2 Open a link file
            {
                openFile_Click(null, null);
            }
            if (e.KeyValue == 114)      // F3 Open a PDF file
            {
                openFile_Click(null, null);
            }
            if (e.Control == true)
            {
                if (e.KeyValue == 79)   // ctrl-o
                    openFile_Click(null, null);

                if (e.KeyValue == 83)   // ctrl-s
                    saveReport_Click(null, null);

                if (e.KeyValue == 69)   // ctrl-e
                    editJustSavedReport_Click(null, null);

                if (e.KeyValue == 82)   // ctrl-r
                    reportEdit_Click(null, null);

                if (e.KeyValue == 72)   // ctrl-h
                    runHexEdit_Click(null, null);

                if (e.KeyValue == 88)   // ctrl-x run the link
                    executeLink_Click(null, null);

                if (e.KeyValue == 73)   // ctrl-i
                    startInternet_Click(null, null);

                if (e.KeyValue == 81)   // ctrl-q
                    exit_Click(null, null);
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        { // display a link file
            string shlp = treeView1.SelectedNode.ToolTipText;
            if (shlp.Contains(".lnk"))
            {
                openFileDialog1.FileName = shlp;
                arbitrateLinkFile();
            }
        }

        private void LinkTreeDirectory_Click(object sender, EventArgs e)
        { // show desktop link file entries
            toolStripLabel1.Text = "Desktop Links";
            getDesktopLinks();
        }

        private void startMenuButton_Click(object sender, EventArgs e)
        { // show StartMenu link file entries
            linkPath = @"C:\Users\" + Environment.GetEnvironmentVariable("USERNAME") +
                @"\AppData\Roaming\Microsoft\Windows\Start Menu";
            toolStripLabel1.Text = "StartMenu Links";
            treeView1.Nodes.Clear();
            getRecursiveDirectoryLinks(linkPath, null);
        }

        private void userLinkButton_Click(object sender, EventArgs e)
        { // display the user choosen link file entries
            if(folderBrowserDialog2.ShowDialog() != DialogResult.OK)
                return;
            toolStripLabel1.Text = "User Links";
            linkPath = folderBrowserDialog2.SelectedPath;
            treeView1.Nodes.Clear();
            getRecursiveDirectoryLinks(linkPath, null);
        }
    }
}
