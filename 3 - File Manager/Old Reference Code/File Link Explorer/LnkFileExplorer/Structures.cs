using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LnkFileExplorer
{
    using WORD = System.UInt16;
    using DWORD = System.UInt32;
    using ULONG = System.UInt32;
    using FTIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    public partial class Form1 : Form
    {
        lnk_FILE_HEADER LinkFileHeader = new lnk_FILE_HEADER();
        lnk_FILE_Location_Info LinkFileLocations = new lnk_FILE_Location_Info();
        local_Volume_Table LocalVolumeTable = new local_Volume_Table();
        network_Volume_Table NetworkVolumeTable = new network_Volume_Table();
        
        // Header shortcut flags
        // No ...list present means the header is present but no data are entered
        public DWORD HasLinkTargetIDList = 0x1;     // The shell link is saved with an item ID list (IDList).
        public DWORD HasLinkInfo = 0x2;             // The shell link is saved with link information or without information. The header is available.
        public DWORD HasName = 0x4;                 // The shell link is saved with a name string. 
        public DWORD HasRelativePath = 0x8;         // The shell link is saved with a relative path string.
        public DWORD HasWorkingDir = 0x10;          // The shell link is saved with a working directory string.
        public DWORD HasArguments = 0x20;           // The shell link is saved with command line arguments.
        public DWORD HasIconLocation = 0x40;        // The shell link is saved with an icon location string.
        public DWORD IsUnicode = 0x80;              // The shell link contains Unicode encoded strings. This bit SHOULD be set.

        public DWORD ForceNoLinkInfo = 0x100;       // The LinkInfo structure is ignored.
        public DWORD HasExpString = 0x200;          // The shell link is saved with an EnvironmentVariableDataBlock
        public DWORD RunInSeparateProcess = 0x400;  // The target is run in a separate virtual machine when launching a link target that is a 16-bit application.
        public DWORD Unused1 = 0x800;               // A bit that is undefined and MUST be ignored.
        public DWORD HasDarwinID = 0x1000;          // The shell link is saved with a DarwinDataBlock
        public DWORD RunAsUser = 0x2000;            // The application is run as a different user when the target of the shell link is activated.
        public DWORD HasExpIcon = 0x4000;           // The shell link is saved with an IconEnvironmentDataBlock
        public DWORD NoPidlAlias = 0x8000;          // The file system location is represented in the shell namespace when the path to an item is parsed into an IDList.

        public DWORD Unused2 = 0x10000;             // A bit that is undefined and MUST be ignored.
        public DWORD RunWithShimLayer = 0x20000;    // The shell link is saved with a ShimDataBlock
        public DWORD ForceNoLinkTrack = 0x40000;    //  The TrackerDataBlock is ignored
        public DWORD EnableTargetMetadata = 0x80000;            //  The shell link attempts to collect target properties and store them in 
                                                                //      the PropertyStoreDataBlock (section 2.5.7) when the link target is set.
        public DWORD DisableLinkPathTracking = 0x100000;        // The EnvironmentVariableDataBlock is ignored.
        public DWORD DisableKnownFolderTracking = 0x200000;     // The SpecialFolderDataBlock (section 2.5.9) and the KnownFolderDataBlock are ignored when loading the shell link.
        public DWORD DisableKnownFolderAlias = 0x400000;        // If the link has a KnownFolderDataBlock, the unaliased form of the known folder IDList SHOULD be used when 
                                                                //      translating the target IDList at the time that the link is loaded.
        public DWORD AllowLinkToLink = 0x800000;                // Creating a link that references another link is enabled. Otherwise, specifying a link as the target IDList SHOULD NOT be allowed

        public DWORD UnaliasOnSave = 0x1000000;                 // When saving a link for which the target IDList is under a known folder, either the unaliased form of that known folder or the target IDList SHOULD be used.
        public DWORD PreferEnvironmentPath = 0x2000000;         // The target IDList SHOULD NOT be stored; instead, the path specified in the EnvironmentVariableDataBlock SHOULD be used to refer to the target.
        public DWORD KeepLocalIDListForUNCTarget = 0x4000000;   // When the target is a UNC name that refers to a location on a local machine, the local path IDList in the PropertyStoreDataBlock 
                                                                //      SHOULD be stored, so it can be used when the link is loaded on the local machine.
        public DWORD Unused3 = 0x8000000;           // A bit that is undefined and MUST be ignored.
        public DWORD Unused4 = 0x10000000;          // A bit that is undefined and MUST be ignored.
        public DWORD Unused5 = 0x20000000;          // A bit that is undefined and MUST be ignored.
        public DWORD Unused6 = 0x40000000;          // A bit that is undefined and MUST be ignored.
        public DWORD Unused7 = 0x80000000;          // A bit that is undefined and MUST be ignored.

        // .URL file structure contains lines of Stings
        List<string> urlFile = new List<string>();


        // Data buffers
        Byte[] headerBuffer;

        // .LNK file structure
        struct lnk_FILE_HEADER
        //public class lnk_FILE_HEADER
        {   // Length = #4c Bytes
            public DWORD headerMagic;   // Always 4C 00 00 00 	The header size
            public Guid CLSID;          // The current GUID for shortcuts. It may change in the future.
            public DWORD shortcutFlags; // Shortcut flags 	Shortcut flags are explained in [MS-SHLLlink].pdf
            public DWORD targetFlags;   // Target file flags 	Flags are explained below
            public FTIME creationTime;  // Creation time  (UTC) 100-nanosecond intervals, elapsed since 12:00 midnight, January 1, 1601 A.D. 
            public FTIME lastAccess;    // Last access time (UTC) 
            public FTIME lastModified;  // Modification time (UTC)
            public DWORD fileLength;    // File length 	The length of the target file. 0 if the target is not a file. This value is used to find the target when the link is broken.
            public DWORD iconIndex;     // Icon number 	If the file has a custom icon (set by the flags bit 6), then this long integer indicates the index of the icon to use. Otherwise it is zero.
            public DWORD windowsState;  // Show Window 	the ShowWnd value to pass to the target application when starting it. 1:Normal Window 2:Minimized 3:Maximized
            public WORD hotkey;         // Hot Key 	The hot key assigned for this shortcut
            public WORD patch;          // Has a 0x00 entry
            public DWORD reserved1;     // Reserved 	Always 0
            public DWORD reserved2;     // Reserved 	Always 0
        };

        // Link Info flags
        public DWORD VolumeIDAndLocalBasePath = 0x1;
        public DWORD CommonNetworkRelativeLinkAndPathSuffix = 0x2;
        struct lnk_FILE_Location_Info // LinkTargetIDList
        //public class lnk_FILE_Location_Info
        {   // Length = #1c Bytes
            public DWORD LinkInfoSize;      // Length value includes all the assorted pathnames and other data structures. All offsets are relative to the start of this section.
            public DWORD LinkInfoHeaderSize;  // Offset at which the basic file info structure ends. Should be 0x1C.
            public DWORD LinkInfoFlags;       // File available on local volume (0) or network share(1)
            public DWORD offsetVolumeTable; // Offset to the local volume table.
            public DWORD offsetBasePath;    // Offset to the base path on the local volume.
            public DWORD offsetNetwork;     // Offset to the network volume table.
            public DWORD offsetFinalPath;   // Offset to the final part of the pathname.
        };
        // If LinkInfoHeaderSize > 0x1C these optional fields are present (0x24)
            public UInt32 LocalBasePathOffsetUnicode;       // (optional) 0x0 terminated, VolumeIDAndLocalBasePath (0x1) flag is set
            public UInt32 CommonPathSuffixOffsetUnicode;    // (optional) 0x0 terminated, LocalBasePath flag is set
            public UInt32 CommonNetworkRelativeLinkOffset;

        struct local_Volume_Table
        //public class local_Volume_Table
        {   // Length = #11 Bytes
            public DWORD volumeTableLength;         // Length of this structure including the volume label string.
            public DWORD volumeType;                // Type of volume
            public ULONG volumeSerialNbr;           // Volume serial number
            public DWORD offsetVolumeName;          // Offset of the volume name (Always 0x10)
        };
        public ULONG VolumeLabelOffsetUnicode;  // (optional) ptr to a NULL terminated Unicode string, if VolumeLabelOffset != 0x14 then ignore it and
                                                //     use it to locate the VolumeLabelString
        //   Data (variable):  A buffer of data that contains the volume label of the drive as a string defined 
        //   by the system default code page or Unicode characters, as specified by preceding fields.
        /*
        Type of volumes
        0	Unknown, 1	No root directory, 2	Removable (Floppy, Zip ...), 3	Fixed (Hard disk)
        4	Remote (Network drive), 5	CD-ROM, 6	Ram drive
        */

        public DWORD ValidDevice = 0x1;
        public DWORD ValidNetType = 0x2;
        struct network_Volume_Table
        //public class network_Volume_Table
        {   // Length = #15 Bytes

            public DWORD netwVolumeLength;  // Length of this structure
            public DWORD netwFlags;         // Network Flags
            public DWORD NetNameOffset;     // Offset to the network share name (Always 0x14) from the end of the structure
            public DWORD DeviceNameOffset;  // Offset to the Device Name
            public DWORD NetworkProviderType; // Network provider type
        };
        public UInt32 NetNameOffsetUnicode;     // (optional)  // valid if NetNameOffset > 0x14
        public UInt32 DeviceNameOffsetUnicode;  // (optional)   // valid if NetNameOffset > 0x14
        // NetName (variable)           // A NULL–terminated string (c_str)
        // DeviceName (variable)        // A NULL–terminated string (c_str) as defined by the system default code page
        // NetNameUnicode (variable)    // A NULL–terminated string if NetNameOffset field is greater than 0x00000014
        // DeviceNameUnicode (variable) // // A NULL–terminated string if NetNameOffset field is greater than 0x00000014

        /*
        StringData
            StringData refers to a set of structures that convey user interface and path identification 
            information. The presence of these optional structures is controlled by LinkFlags (section 2.1.1) in 
            the ShellLinkHeader (section 2.1).
            The StringData structures conform to the following ABNF rules [RFC5234].
                STRING_DATA = [NAME_STRING] [RELATIVE_PATH] [WORKING_DIR]
                [COMMAND_LINE_ARGUMENTS] [ICON_LOCATION]
         * 
        Name String     
         *              This section is present if bit 2 is set in the flags value in the header. 
         *              The first word value indicates the length of the string. Following the 
         *              length value is a string of ASCII characters. It is a description of the item.

        Relative path string
                        This section is present if bit 3 is set in the flags value in the header. 
         *              The first word value indicates the length of the string. Following the 
         *              length value is a string of ASCII characters. It is a relative path to the target.

        Working directory
                        This section is present if bit 4 is set in the flags value in the header. 
         *              The first word value indicates the length of the string. Following the 
         *              length value is a string of ASCII characters. It is the working directory 
         *              as specified in the shortcut properties.

        Command line arguments
                        This section is present if bit 5 is set in the flags value in the header. 
         *              The first word value indicates the length of the string. Following the 
         *              length value is a string of ASCII characters. The command line string 
         *              includes everything except the program name.

        Icon filename
                        This section is present if bit 6 is set in the flags value in the header. 
         *              The first word value indicates the length of the string. Following the 
         *              length value is a string of ASCII characters. This the name of the file containing the icon.
        */

        /*
         ExtraData
            ExtraData refers to a set of structures that convey additional information about a link target. These 
            optional structures can be present in an extra data section that is appended to the basic Shell Link 
            Binary File Format.
            The ExtraData structures conform to the following ABNF rules [RFC5234]:
                    EXTRA_DATA = *EXTRA_DATA_BLOCK TERMINAL_BLOCK 
                    EXTRA_DATA_BLOCK = CONSOLE_PROPS / CONSOLE_FE_PROPS / DARWIN_PROPS /
                    ENVIRONMENT_PROPS / ICON_ENVIRONMENT_PROPS /
                    KNOWN_FOLDER_PROPS / PROPERTY_STORE_PROPS /
                    SHIM_PROPS / SPECIAL_FOLDER_PROPS /
                    TRACKER_PROPS / VISTA_AND_ABOVE_IDLIST_PROPS
                            Additonal Info Usualy consists of a dword with the value 0. 
          
         EXTRA_DATA: A structure consisting of zero or more property data blocks followed by a terminal block.
          
            See [MS-Shellink].pdf for further structure information, Page 26!
         
            EXTRA_DATA_BLOCK: A structure consisting of any one of the following property data blocks.
         */
        struct CONSOLE_PROPS               // A ConsoleDataBlock structure (section 2.5.1).
        {
            public DWORD BlockSize;        // This value MUST be 0x000000CC.
            public DWORD BlockSignature;   // This value MUST be 0xA0000002.
            public DWORD Attributes;       // FillAttributes, PopupFillAttributes
            // FillAttributes:
            //  FOREGROUND_BLUE 0x0001      The foreground text color contains blue.
            //  FOREGROUND_GREEN 0x0002     The foreground text color contains green.
            //  FOREGROUND_RED 0x0004       The foreground text color contains red.
            //  FOREGROUND_INTENSITY 0x0008 The foreground text color is intensified.
            //  BACKGROUND_BLUE 0x0010      The background text color contains blue.
            //  BACKGROUND_GREEN 0x0020     The background text color contains green.
            //  BACKGROUND_RED 0x0040       The background text color contains red.
            //  BACKGROUND_INTENSITY 0x0080 The background text color is intensified.

            // PopupFillAttributes: see FillAttributes
            public DWORD ScreenBuffer;         // ScreenBufferSizeX, ScreenBufferSizeY  // X,Y characters, of the console window buffer.
            public DWORD WindowSize;           // WindowSizeX, WindowSizeY              // X,Y characters, of the console window buffer.
            public DWORD WindowOrigin;         // WindowOriginX, WindowOriginY          // X,Y in pixels, of the console window origin.
            public DWORD Unused1;
            public DWORD Unused2;
            public DWORD FontSize;             // Font in pixels, of the font used in the console window.
            public DWORD FontFamily;
            // FontFamily:
            //  FF_DONTCARE 0x0000          The font family is unknown.
            //  FF_ROMAN 0x0010             The font is variable-width with serifs; for example, "Times New Roman".
            //  FF_SWISS 0x0020             The font is variable-width without serifs; for example, "Arial".
            //  FF_MODERN 0x0030            The font is fixed-width, with or without serifs; for example, "Courier New".
            //  FF_SCRIPT 0x0040            The font is designed to look like handwriting; for example, "Cursive".
            //  FF_DECORATIVE 0x0050        The font is a novelty font; for example, "Old English".
            public DWORD FontWeight;       // 700 ≤ value A bold font, value < 700 A regular-weight font
            //public DWORD FaceName;         // A 32-character Unicode string
        }
            struct CONSOLE_PROPS1
            {
                      // (Face Name cont'd for 8 rows)
                public DWORD CursorSize;       // in pixels, <= 25 = small, 26-50 = medium, 51-100 large
                public DWORD FullScreen;       // 0x0 = off
                public DWORD QuickEdit;        // 0x0 = off
                public DWORD InsertMode;       // 0x0 = disabled
                public DWORD AutoPosition;     // 0x0 = WindowOriginX and WindowOriginY are used for positioning
                public DWORD HistoryBufferSize;        // in characters
                public DWORD NumberOfHistoryBuffers;    
                public DWORD HistoryNoDup;             // 0x0 = Duplicates are not allowed.
                public DWORD ColorTable;       // A table of 16 32-bit, unsigned integers specifying the RGB colors
                                               // FillAttributes and PopupFillAttributes are used as indexes into this table
                      // (ColorTable cont'd for 8 rows)
            }
           
            struct CONSOLE_FE_PROPS            // A ConsoleFEDataBlock structure (section 2.5.2).
            {
                public DWORD BlockSize;        // This value MUST be 0x0000000C.
                public DWORD BlockSignature;   // This value MUST be 0xA0000004.
                public DWORD CodePage;         // unsigned integer that specifies a code page language
            }
         
            struct DARWIN_PROPS                // A DarwinDataBlock structure (section 2.5.3).
            {
                public DWORD BlockSize;        // This value MUST be 0x00000314.
                public DWORD BlockSignature;   // This value MUST be 0xA0000006.
                public DWORD DarwinDataAnsi;   // A NULL–terminated string, defined by the system default code page 260 Bytes. Ignore It
                    // (DarwinDataAnsi cont'd for 57 rows)
                public DWORD DarwinDataUnicode; // (optional)  // An optional, NULL–terminated, Unicode string 520 Bytes
                    // (DarwinDataUnicode (optional) cont'd for 122 rows)
            }
           
            struct ENVIRONMENT_PROPS           // An EnvironmentVariableDataBlock structure (section 2.5.4).
            {
                public DWORD BlockSize;        // This value MUST be 0x00000314.
                public DWORD BlockSignature;   // This value MUST be 0xA0000001.
                public DWORD TargetAnsi;       // A NULL-terminated string 260 Bytes
                    // (TargetAnsi cont'd for 57 rows)
                public DWORD TargetUnicode;    // An optional, NULL-terminated, Unicode string 520 Bytes
                    // (TargetUnicode cont'd for 122 rows)
            }
           
            struct ICON_ENVIRONMENT_PROPS      // An IconEnvironmentDataBlock structure (section 2.5.5).           
            {
                public DWORD BlockSize;        // This value MUST be 0x00000314
                public DWORD BlockSignature;   // This value MUST be 0xA0000007.
                public DWORD TargetAnsi;       // A NULL-terminated string 260 Bytes
                    // (TargetAnsi cont'd for 57 rows)
                public DWORD TargetUnicode;    // An optional, NULL-terminated, Unicode string 520 Bytes
                    // (TargetUnicode cont'd for 122 rows)
            }
         
            struct KNOWN_FOLDER_PROPS   // A KnownFolderDataBlock structure (section 2.5.6).           
            {
                public DWORD BlockSize;        // This value MUST be 0x0000001C.
                public DWORD BlockSignature;   // This value MUST be 0xA000000B
                public Guid  KnownFolderID; 
                public DWORD Offset;           // the location of the ItemID of the first 
            }                                  //   child segment of the IDList specified by KnownFolderID.
           
         
            struct PROPERTY_STORE_PROPS // A PropertyStoreDataBlock structure (section 2.5.7).           
            {
                public DWORD BlockSize;        // This value MUST be greater than or equal to 0x0000000C.
                public DWORD BlockSignature;   // This value MUST be 0xA0000009.
                public DWORD PropertyStoreSize;    // (variable)  Structure  [MS-PROPSTORE], DWORD continued until 0x0, 
            }
            public DWORD propertyStoreFormatId;
           
            struct SHIM_PROPS           // A ShimDataBlock structure (section 2.5.8).           
            {
                public DWORD BlockSize;        // This value MUST be greater than or equal to 0x00000088.
                public DWORD BlockSignature;   // This value MUST be 0xA0000008.
                //public DWORD LayerName;        // (variable) A Unicode string that specifies the name of a shim layer
            }
           
            struct SPECIAL_FOLDER_PROPS // A SpecialFolderDataBlock structure (section 2.5.9).           
            {
                public DWORD BlockSize;        // This value MUST be 0x00000010.
                public DWORD BlockSignature;   // This value MUST be 0xA0000005.
                public DWORD SpecialFolderID;  // unsigned integer that specifies the folder integer ID.
                public DWORD Offset;           // the first child segment of the IDList specified by SpecialFolderID.
            }
           
            struct TRACKER_PROPS        // A TrackerDataBlock structure (section 2.5.10).           
            {
                public DWORD BlockSize;        // This value MUST be 0x00000060
                public DWORD BlockSignature;   // This value MUST be 0xA0000003.
                public DWORD Length;           // This value MUST be greater than or equal to 0x0000058.
                public DWORD Version;          // This value MUST be 0x00000000.
                }
            public string MachineID;       // (variable) A character c_string NetBIOS name (16Bytes???)
            public Guid  Droid;            // Two values in GUID packet representation to find the link target with the Link Tracking service [MS-DTYP]
            public Guid  DroidBirth;       // See Droid

           
            struct VISTA_AND_ABOVE_IDLIST_PROPS     // A VistaAndAboveIDListDataBlock structure (section 2.5.11).
            {
                public DWORD BlockSize;        // This value MUST be greater than or equal to 0x0000000A.
                public DWORD BlockSignature;   // This value MUST be 0xA000000C.
                public DWORD IDList;           // (variable) An IDList structure
            }
           
            // TERMINAL_BLOCK A structure that indicates the end of the extra data section.
    }
}
