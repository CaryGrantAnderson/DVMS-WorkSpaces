using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace DVMSFileManager
{
    internal sealed class NotesEditorForm : XtraForm
    {
        private readonly ListBoxControl notesList = new ListBoxControl();
        private readonly TextEdit titleEdit = new TextEdit();
        private readonly MemoEdit noteEdit = new MemoEdit();
        private readonly CheckEdit pinnedEdit = new CheckEdit();
        private readonly LabelControl timestampsLabel = new LabelControl();
        private readonly SimpleButton addButton = new SimpleButton();
        private readonly SimpleButton deleteButton = new SimpleButton();
        private readonly SimpleButton okButton = new SimpleButton();
        private readonly SimpleButton cancelButton = new SimpleButton();
        private bool changingSelection;
        private FileNote selectedNote;

        public NotesEditorForm(
            string fileName,
            IEnumerable<FileNote> sourceNotes)
        {
            Text = "Notes — " + fileName;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 480);
            Size = new Size(940, 620);

            Notes = sourceNotes.Select(CloneNote).ToList();
            BuildInterface();
            ReloadList(null);
        }

        public List<FileNote> Notes { get; }

        private void BuildInterface()
        {
            var split = new SplitContainerControl
            {
                Dock = DockStyle.Fill,
                SplitterPosition = 260
            };
            Controls.Add(split);

            var leftButtons = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 42,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            split.Panel1.Controls.Add(notesList);
            split.Panel1.Controls.Add(leftButtons);
            notesList.Dock = DockStyle.Fill;

            addButton.Text = "New";
            addButton.Location = new Point(8, 8);
            addButton.Size = new Size(90, 27);
            addButton.Click += addButton_Click;
            leftButtons.Controls.Add(addButton);

            deleteButton.Text = "Delete";
            deleteButton.Location = new Point(104, 8);
            deleteButton.Size = new Size(90, 27);
            deleteButton.Click += deleteButton_Click;
            leftButtons.Controls.Add(deleteButton);

            notesList.SelectedIndexChanged += notesList_SelectedIndexChanged;

            var editorTop = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 84,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            split.Panel2.Controls.Add(noteEdit);
            split.Panel2.Controls.Add(editorTop);
            noteEdit.Dock = DockStyle.Fill;

            var titleLabel = new LabelControl
            {
                Text = "Title:",
                Location = new Point(10, 13)
            };
            editorTop.Controls.Add(titleLabel);

            titleEdit.Location = new Point(58, 9);
            titleEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                AnchorStyles.Right;
            titleEdit.Size = new Size(520, 22);
            editorTop.Controls.Add(titleEdit);

            pinnedEdit.Text = "Pinned";
            pinnedEdit.Location = new Point(10, 42);
            editorTop.Controls.Add(pinnedEdit);

            timestampsLabel.Location = new Point(105, 46);
            timestampsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                AnchorStyles.Right;
            editorTop.Controls.Add(timestampsLabel);

            var bottom = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            Controls.Add(bottom);
            bottom.BringToFront();

            okButton.Text = "OK";
            okButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            okButton.Location = new Point(bottom.Width - 190, 10);
            okButton.Size = new Size(82, 28);
            okButton.DialogResult = DialogResult.OK;
            okButton.Click += okButton_Click;
            bottom.Controls.Add(okButton);

            cancelButton.Text = "Cancel";
            cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cancelButton.Location = new Point(bottom.Width - 100, 10);
            cancelButton.Size = new Size(82, 28);
            cancelButton.DialogResult = DialogResult.Cancel;
            bottom.Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
            UpdateEditorEnabledState(false);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            SaveSelectedNote();
            DateTime now = DateTime.UtcNow;
            var note = new FileNote
            {
                NoteKey = Guid.NewGuid().ToString("D"),
                Title = "New note",
                NoteText = string.Empty,
                CreatedUtc = now,
                ModifiedUtc = now,
                SortOrder = Notes.Count
            };
            Notes.Add(note);
            ReloadList(note);
            titleEdit.SelectAll();
            titleEdit.Focus();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (selectedNote == null)
                return;

            if (XtraMessageBox.Show(
                    this,
                    "Delete the selected note?",
                    "Delete Note",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int oldIndex = notesList.SelectedIndex;
            Notes.Remove(selectedNote);
            FileNote next = Notes.Count == 0
                ? null
                : Notes[Math.Min(oldIndex, Notes.Count - 1)];
            ReloadList(next);
        }

        private void notesList_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (changingSelection)
                return;

            SaveSelectedNote();
            selectedNote = notesList.SelectedItem as FileNote;
            LoadSelectedNote();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSelectedNote();

            for (int index = 0; index < Notes.Count; index++)
                Notes[index].SortOrder = index;
        }

        private void ReloadList(FileNote selectNote)
        {
            changingSelection = true;
            try
            {
                notesList.Items.Clear();
                foreach (FileNote note in Notes
                    .OrderByDescending(item => item.IsPinned)
                    .ThenBy(item => item.SortOrder)
                    .ThenByDescending(item => item.ModifiedUtc))
                {
                    notesList.Items.Add(note);
                }

                if (selectNote != null)
                    notesList.SelectedItem = selectNote;
                else if (notesList.ItemCount > 0)
                    notesList.SelectedIndex = 0;

                selectedNote = notesList.SelectedItem as FileNote;
                LoadSelectedNote();
            }
            finally
            {
                changingSelection = false;
            }
        }

        private void LoadSelectedNote()
        {
            bool hasNote = selectedNote != null;
            UpdateEditorEnabledState(hasNote);

            if (!hasNote)
            {
                titleEdit.Text = string.Empty;
                noteEdit.Text = string.Empty;
                pinnedEdit.Checked = false;
                timestampsLabel.Text = string.Empty;
                return;
            }

            titleEdit.Text = selectedNote.Title ?? string.Empty;
            noteEdit.Text = selectedNote.NoteText ?? string.Empty;
            pinnedEdit.Checked = selectedNote.IsPinned;
            timestampsLabel.Text = string.Format(
                "Created {0:g}    Modified {1:g}",
                selectedNote.CreatedUtc.ToLocalTime(),
                selectedNote.ModifiedUtc.ToLocalTime());
        }

        private void SaveSelectedNote()
        {
            if (selectedNote == null)
                return;

            string title = titleEdit.Text.Trim();
            string noteText = noteEdit.Text;
            bool pinned = pinnedEdit.Checked;

            if (selectedNote.Title != title ||
                selectedNote.NoteText != noteText ||
                selectedNote.IsPinned != pinned)
            {
                selectedNote.Title = title;
                selectedNote.NoteText = noteText;
                selectedNote.IsPinned = pinned;
                selectedNote.ModifiedUtc = DateTime.UtcNow;
            }
        }

        private void UpdateEditorEnabledState(bool enabled)
        {
            titleEdit.Enabled = enabled;
            noteEdit.Enabled = enabled;
            pinnedEdit.Enabled = enabled;
            deleteButton.Enabled = enabled;
        }

        private static FileNote CloneNote(FileNote source)
        {
            return new FileNote
            {
                NoteKey = source.NoteKey,
                Title = source.Title,
                NoteText = source.NoteText,
                IsPinned = source.IsPinned,
                SortOrder = source.SortOrder,
                CreatedUtc = source.CreatedUtc,
                ModifiedUtc = source.ModifiedUtc
            };
        }
    }
}
