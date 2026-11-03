using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

namespace DVMSFileManager
{
    internal sealed class MetadataPresentation
    {
        public string CustomIconPath { get; set; }
        public int? FileNameColorArgb { get; set; }
        public bool IsFileNameBold { get; set; }
    }

    internal sealed class FileNote
    {
        public string NoteKey { get; set; }
        public string Title { get; set; }
        public string NoteText { get; set; }
        public bool IsPinned { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Title)
                ? "Untitled note"
                : Title;
        }
    }

    internal sealed class NotesSummary
    {
        public int Count { get; set; }
        public DateTime? LastModifiedUtc { get; set; }
        public string LatestTitle { get; set; }

        public override string ToString()
        {
            if (Count == 0)
                return "No notes";

            string countText = Count == 1 ? "1 note" : Count + " notes";

            return string.IsNullOrWhiteSpace(LatestTitle)
                ? countText
                : countText + " — Latest: " + LatestTitle;
        }
    }

    internal sealed class FileMetadataViewModel
    {
        private readonly Action<FileMetadataViewModel> editNotes;

        public FileMetadataViewModel(
            Action<FileMetadataViewModel> editNotes)
        {
            this.editNotes = editNotes;
            Notes = new NotesSummary();
            Tags = string.Empty;
            Description = string.Empty;
            DisplayName = string.Empty;
            Status = string.Empty;
            Workspace = string.Empty;
            SourceUrl = string.Empty;
            CustomIconPath = string.Empty;
        }

        [Browsable(false)]
        public string FileKey { get; set; }

        [Browsable(false)]
        public string FullPath { get; set; }

        [Browsable(false)]
        public List<FileNote> NoteItems { get; set; } =
            new List<FileNote>();

        [Category("General"), DisplayName("Display name")]
        public string DisplayName { get; set; }

        [Category("General"), DisplayName("Description")]
        public string Description { get; set; }

        [Category("Organization"), DisplayName("Tags")]
        [Description("Comma-separated tags for this file.")]
        public string Tags { get; set; }

        [Category("Organization"), DisplayName("Favorite")]
        public bool IsFavorite { get; set; }

        [Category("Organization"), DisplayName("Rating")]
        [Description("A rating from zero through five.")]
        public int Rating { get; set; }

        [Category("Organization"), DisplayName("Status")]
        public string Status { get; set; }

        [Category("Organization"), DisplayName("Workspace / Collection")]
        public string Workspace { get; set; }

        [Category("Appearance"), DisplayName("Custom icon")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CustomIconPath { get; set; }

        [Category("Appearance"), DisplayName("File-name color")]
        public Color FileNameColor { get; set; } = Color.Empty;

        [Category("Appearance"), DisplayName("Bold file name")]
        public bool IsFileNameBold { get; set; }

        [Category("References"), DisplayName("Source URL")]
        public string SourceUrl { get; set; }

        [Category("Notes"), DisplayName("Notes")]
        [Editor(typeof(NotesSummaryEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public NotesSummary Notes { get; set; }

        [Category("DVMS Information"), DisplayName("Metadata created")]
        [ReadOnly(true)]
        public DateTime? MetadataCreatedUtc { get; set; }

        [Category("DVMS Information"), DisplayName("Metadata modified")]
        [ReadOnly(true)]
        public DateTime? MetadataModifiedUtc { get; set; }

        [Category("DVMS Information"), DisplayName("Last seen")]
        [ReadOnly(true)]
        public DateTime? LastSeenUtc { get; set; }

        [Category("DVMS Information"), DisplayName("Last known path")]
        [ReadOnly(true)]
        public string LastKnownPath => FullPath;

        [Category("DVMS Information"), DisplayName("Missing")]
        [ReadOnly(true)]
        public bool IsMissing =>
            !string.IsNullOrWhiteSpace(FullPath) &&
            !System.IO.File.Exists(FullPath) &&
            !System.IO.Directory.Exists(FullPath);

        internal void EditNotes()
        {
            editNotes?.Invoke(this);
        }

        internal int? GetColorArgb()
        {
            return FileNameColor.IsEmpty
                ? (int?)null
                : FileNameColor.ToArgb();
        }

        internal void SetColorArgb(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                FileNameColor = Color.Empty;
                return;
            }

            FileNameColor = Color.FromArgb(
                Convert.ToInt32(value, CultureInfo.InvariantCulture));
        }
    }

    internal sealed class NotesSummaryEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            var metadata = context?.Instance as FileMetadataViewModel;
            metadata?.EditNotes();
            return metadata?.Notes ?? value;
        }
    }
}
