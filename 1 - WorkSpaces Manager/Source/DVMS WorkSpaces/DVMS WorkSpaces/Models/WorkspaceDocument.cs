using DVMS_WorkSpaces.Core;
using System;
using System.Collections.Generic;

namespace DVMS_WorkSpaces.Models
{
    public sealed class WorkspaceGridSettings
    {
        public bool ShowGrid { get; set; } = true;
        public bool SnapToGrid { get; set; }
        public bool LockGrid { get; set; }
        public int Spacing { get; set; } = 20;
        public string GridType { get; set; } = "Lines";
    }

    public sealed class WorkspaceItemPlacement
    {
        public WorkspaceItemPlacement()
        {
            PlacementId = Guid.NewGuid().ToString("N");
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string PlacementId { get; set; }
        public string ComponentInstanceId { get; set; }
        public string AppSystemId { get; set; }
        public PresentationKind Presentation { get; set; }
        public HostKind HostKind { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IDictionary<string, string> Settings { get; private set; }
    }

    public sealed class WorkspaceDocument
    {
        public WorkspaceDocument()
        {
            WorkspaceId = Guid.NewGuid().ToString("N");
            Title = "New WorkSpace";
            Grid = new WorkspaceGridSettings();
            Items = new List<WorkspaceItemPlacement>();
        }

        public string WorkspaceId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public bool IsDirty { get; set; }
        public WorkspaceGridSettings Grid { get; private set; }
        public IList<WorkspaceItemPlacement> Items { get; private set; }
    }
}
