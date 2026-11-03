using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LnkFileExplorer
{
    public partial class Form1 : Form
    {
        UInt32 tagetIdListSize;

        //------------------------------ Link File Header aera
        public bool displayLinkFileHeaderAndFlags()
        { // Display the whole Link file header and it's flags
            string shlp = "";

            headerBuffer = reader.ReadBytes(Marshal.SizeOf(typeof(lnk_FILE_HEADER)));
            // test or Internet shortcut
            if (headerBuffer[0] == '[')
                return false;

            handle = GCHandle.Alloc(headerBuffer, GCHandleType.Pinned);
            LinkFileHeader = (lnk_FILE_HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(lnk_FILE_HEADER));

            dataGridView1.Rows.Add("Link File Header:", "Location: 0x0000 0000 - 0x" + makeLen8String((UInt32)LinkFileHeader.headerMagic, 4));
            dataGridView1.Rows.Add("  Header Size", "0x" + makeLen8String((UInt32)LinkFileHeader.headerMagic, 4));
            dataGridView1.Rows.Add("  CLSID", LinkFileHeader.CLSID.ToString());
            dataGridView1.Rows.Add("  Link Flags", "0x" + makeLen8String((UInt32)LinkFileHeader.shortcutFlags, 0));
            dataGridView1.Rows.Add("  Target Attributes", "0x" + makeLen8String((UInt32)LinkFileHeader.targetFlags, 0));
            Int64 i64 = LinkFileHeader.creationTime.dwLowDateTime + (LinkFileHeader.creationTime.dwHighDateTime * 0x100000000);
            DateTime dt = DateTime.FromBinary(i64);
            dt = dt.AddYears(1600);
            dataGridView1.Rows.Add("  Creation Time", dt.ToString("F") + " *");
            i64 = LinkFileHeader.lastAccess.dwLowDateTime + (LinkFileHeader.lastAccess.dwHighDateTime * 0x100000000);
            dt = DateTime.FromBinary(i64);
            dt = dt.AddYears(1600);
            dataGridView1.Rows.Add("  Last Accessed", dt.ToString("F") + " *");
            i64 = LinkFileHeader.lastModified.dwLowDateTime + (LinkFileHeader.lastModified.dwHighDateTime * 0x100000000);
            dt = DateTime.FromBinary(i64);
            dt = dt.AddYears(1600);
            dataGridView1.Rows.Add("  Last Modified", dt.ToString("F") + " *");
            dataGridView1.Rows.Add("  Target File Length", "0x" + makeLen8String((UInt32)LinkFileHeader.fileLength, 0) + "   (" +
                LinkFileHeader.fileLength + " Bytes)");
            dataGridView1.Rows.Add("  Icon Index", "0x" + makeLen8String((UInt32)LinkFileHeader.iconIndex, 0));
            /* WindowsState 0x1-0x3 is valid
                SW_HIDE 0 
                SW_NORMAL 1
                SW_SHOWMINIMIZED 2
                SW_SHOWMAXIMIZED 3
                SW_SHOWNOACTIVATE 4
                SW_SHOW 5
                SW_MINIMIZE 6
                SW_SHOWMINNOACTIVE 7
                SW_SHOWNA 8
                SW_RESTORE 9
                SW_SHOWDEFAULT 10
            */
            if (LinkFileHeader.windowsState == 0x1)
                shlp = "Normal";
            if (LinkFileHeader.windowsState == 0x3)
                shlp = "Maximized";
            if (LinkFileHeader.windowsState == 0x7)
                shlp = "Hidden";
            if (shlp == "")
                shlp = makeLen8String((UInt32)LinkFileHeader.windowsState, 0) + "  Invalid???";
            dataGridView1.Rows.Add("  Windows State", "0x" + makeLen8String((UInt32)LinkFileHeader.windowsState, 4) +
                " (" + shlp + ")");
            dataGridView1.Rows.Add("  Hotkey", "0x" + makeLen4String((UInt16)LinkFileHeader.hotkey) + decodeShortcut(LinkFileHeader.hotkey));
            dataGridView1.Rows.Add("  Reserved 1", "0x" + makeLen8String((UInt32)LinkFileHeader.reserved1, 0));
            dataGridView1.Rows.Add("  Reserved 2", "0x" + makeLen8String((UInt32)LinkFileHeader.reserved2, 0));
            displayHeaderFlags();
            return true;
        }

        public void displayHeaderFlags()
        {
            dataGridView2.Rows.Add("Link Flags:");
            if ((LinkFileHeader.shortcutFlags & HasLinkTargetIDList) > 0) // A
                dataGridView2.Rows.Add("  ID List follows the ShellLinkHeader");
            else
                dataGridView2.Rows.Add("  No ID List available after the ShellLinkHeader");
            if ((LinkFileHeader.shortcutFlags & HasLinkInfo) > 0) // B
                dataGridView2.Rows.Add("  Link Info is present");
            else
                dataGridView2.Rows.Add("  No Link Info is available");
            if ((LinkFileHeader.shortcutFlags & HasName) > 0) // C
                dataGridView2.Rows.Add("  A shell link Name is present");
            if ((LinkFileHeader.shortcutFlags & HasRelativePath) > 0) // D
                dataGridView2.Rows.Add("      A relative shell link Path is used");
            //
            if ((LinkFileHeader.shortcutFlags & HasWorkingDir) > 0) // E
                dataGridView2.Rows.Add("  A shell working Directory is entered");
            if ((LinkFileHeader.shortcutFlags & HasArguments) > 0) // F
                dataGridView2.Rows.Add("  Command Line Arguments are entered");
            if ((LinkFileHeader.shortcutFlags & HasIconLocation) > 0) // G
                dataGridView2.Rows.Add("  A Icon Location is entered");
            if ((LinkFileHeader.shortcutFlags & IsUnicode) > 0) // H
                dataGridView2.Rows.Add("  The link contains Unicode");
            //
            if ((LinkFileHeader.shortcutFlags & ForceNoLinkInfo) > 0) // I
                dataGridView2.Rows.Add("  The link info structure is Ignored");
            if ((LinkFileHeader.shortcutFlags & HasExpString) > 0) // J
                dataGridView2.Rows.Add("  The link is saved in an EnvironmentVariableDataBlock");
            if ((LinkFileHeader.shortcutFlags & RunInSeparateProcess) > 0) // K
                dataGridView2.Rows.Add("  Run target in a virtual machine (16 Bit Application)");
            // 0x800 is unused
            if ((LinkFileHeader.shortcutFlags & Unused1) > 0) // L
                dataGridView1.Rows.Add("  unused? Unknown! Bit 0x800");
            //
            if ((LinkFileHeader.shortcutFlags & HasDarwinID) > 0) // M
                dataGridView2.Rows.Add("  The link has a Darwin section");
            if ((LinkFileHeader.shortcutFlags & RunAsUser) > 0) // N
                dataGridView2.Rows.Add("  The link runs as a different user");
            if ((LinkFileHeader.shortcutFlags & HasExpIcon) > 0) // O
                dataGridView2.Rows.Add("  The link is saved with an IconEnvironmentDataBlock");
            if ((LinkFileHeader.shortcutFlags & NoPidlAlias) > 0) // P
                dataGridView2.Rows.Add("  The file system location is represented in the shell namespace");
            //
            // 0x10000 is unused
            if ((LinkFileHeader.shortcutFlags & Unused2) > 0) // Q
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x10000");
            if ((LinkFileHeader.shortcutFlags & RunWithShimLayer) > 0) // R
                dataGridView2.Rows.Add("  The shell link has a ShimDataBlock");
            if ((LinkFileHeader.shortcutFlags & ForceNoLinkTrack) > 0) // S
                dataGridView2.Rows.Add("  The TrackerDataBlock is ignored");
            if ((LinkFileHeader.shortcutFlags & EnableTargetMetadata) > 0) // T
                dataGridView2.Rows.Add("  The shell link collects target properties");
            //
            if ((LinkFileHeader.shortcutFlags & DisableLinkPathTracking) > 0) // U
                dataGridView2.Rows.Add("  The EnvironmentVariableDataBlock is ignored");
            if ((LinkFileHeader.shortcutFlags & DisableKnownFolderTracking) > 0) // V
                dataGridView2.Rows.Add("  SpecialFolderDataBlock & KnownFolderDataBlock are ignored");
            if ((LinkFileHeader.shortcutFlags & DisableKnownFolderAlias) > 0) // W
                dataGridView2.Rows.Add("  Use the KnownFolderDataBlock for translating the target IDList");
            if ((LinkFileHeader.shortcutFlags & AllowLinkToLink) > 0) // X
                dataGridView2.Rows.Add("  Creating a link that references another link is enabled");
            //
            if ((LinkFileHeader.shortcutFlags & UnaliasOnSave) > 0) // Y
                dataGridView2.Rows.Add("  Use unaliased form of the known folder or the target IDList for saving the link");
            if ((LinkFileHeader.shortcutFlags & PreferEnvironmentPath) > 0) // Z
                dataGridView2.Rows.Add("  The target IDList SHOULD NOT be stored");
            if ((LinkFileHeader.shortcutFlags & KeepLocalIDListForUNCTarget) > 0) // AA
                dataGridView2.Rows.Add("  Store the PropertyStoreDataBlock if the target is a UNC name");
            if ((LinkFileHeader.shortcutFlags & Unused3) > 0)
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x8000000");
            //
            // these bits are unused
            if ((LinkFileHeader.shortcutFlags & Unused4) > 0)
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x10000000");
            if ((LinkFileHeader.shortcutFlags & Unused5) > 0)
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x20000000");
            if ((LinkFileHeader.shortcutFlags & Unused6) > 0)
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x40000000");
            if ((LinkFileHeader.shortcutFlags & Unused7) > 0)
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x80000000");

            /*
            1 (HasLinkTargetIDList): The shell link is saved with an item ID list (IDList). If this bit is set, a 
                LinkTargetIDList structure (section 2.2) MUST follow the ShellLinkHeader.
            2 (HasLinkInfo): The shell link is saved with link information. If this bit is set, a LinkInfo structure 
                (section 2.3) MUST be present.
            4 (HasName): The shell link is saved with a name string. If this bit is set, a NAME_STRING
                StringData structure (section 2.4) MUST be present. 
            8 (HasRelativePath): The shell link is saved with a relative path string. If this bit is set, a 
                RELATIVE_PATH StringData structure (section 2.4) MUST be present.
            10 (HasWorkingDir): The shell link is saved with a working directory string. If this bit is set, a 
                WORKING_DIR StringData structure (section 2.4) MUST be present.
            20 (HasArguments): The shell link is saved with command line arguments. If this bit is set, a 
                COMMAND_LINE_ARGUMENTS StringData structure (section 2.4) MUST be present.
            40 (HasIconLocation): The shell link is saved with an icon location string. If this bit is set, an 
                ICON_LOCATION StringData structure (section 2.4) MUST be present.
            80 (IsUnicode): The shell link contains Unicode encoded strings. This bit SHOULD be set.
            100 (ForceNoLinkInfo): The LinkInfo structure (section 2.3) is ignored.
            200 (HasExpString): The shell link is saved with an EnvironmentVariableDataBlock (section 2.5.4).
            400 (RunInSeparateProcess): The target is run in a separate virtual machine when launching a link 
                target that is a 16-bit application.
            800 (Unused1): A bit that is undefined and MUST be ignored.
            1000 (HasDarwinID): The shell link is saved with a DarwinDataBlock (section 2.5.3).
            2000 (RunAsUser): The application is run as a different user when the target of the shell link is activated.
            4000 (HasExpIcon): The shell link is saved with an IconEnvironmentDataBlock (section 2.5.5).
            8000 (NoPidlAlias): The file system location is represented in the shell namespace when the path to 
                an item is parsed into an IDList.
            10000 (Unused2): A bit that is undefined and MUST be ignored.
            20000 (RunWithShimLayer): The shell link is saved with a ShimDataBlock (section 2.5.8).
            40000 (ForceNoLinkTrack): The TrackerDataBlock (section 2.5.10) is ignored.
            80000 (EnableTargetMetadata): The shell link attempts to collect target properties and store them in 
                    the PropertyStoreDataBlock (section 2.5.7) when the link target is set.
            100000 (DisableLinkPathTracking): The EnvironmentVariableDataBlock is ignored.
            200000 (DisableKnownFolderTracking): The SpecialFolderDataBlock (section 2.5.9) and the 
                KnownFolderDataBlock (section 2.5.6) are ignored when loading the shell link. If this bit is set, 
                these extra data blocks SHOULD NOT be saved when saving the shell link.
            400000 (DisableKnownFolderAlias): If the link has a KnownFolderDataBlock (section 2.5.6), the 
                KnownFolderDataBlock be used when translating the target IDList at the 
                time that the link is loaded.
            800000 (AllowLinkToLink): Creating a link that references another link is enabled. Otherwise, specifying 
                a link as the target IDList SHOULD NOT be allowed.
            1000000 (UnaliasOnSave): When saving a link for which the target IDList is under a known folder, either 
                the unaliased form of that known folder or the target IDList SHOULD be used.
            2000000 (PreferEnvironmentPath): The target IDList SHOULD NOT be stored; instead, the path specified 
                in the EnvironmentVariableDataBlock (section 2.5.4) SHOULD be used to refer to the target.
            4000000 (KeepLocalIDListForUNCTarget): When the target is a UNC name that refers to a location on 
                a local machine, the local path IDList in the PropertyStoreDataBlock (section 2.5.7) SHOULD be 
                stored, so it can be used when the link is loaded on the local machine.
            */

            // If no target is available
            if (LinkFileHeader.fileLength == 0x0)
                return;

            dataGridView2.Rows.Add("Target Attributes:");
            if ((LinkFileHeader.targetFlags & 0x1) > 0)                     // The file or directory is read-only.
                dataGridView2.Rows.Add("  Read only file or directory");
            if ((LinkFileHeader.targetFlags & 0x2) > 0)                     // The file or directory is hidden.
                dataGridView2.Rows.Add("  Hidden file or directory");
            if ((LinkFileHeader.targetFlags & 0x4) > 0)                     // The file or directory is part of the operating system.
                dataGridView2.Rows.Add("  System file or directory");
            if ((LinkFileHeader.targetFlags & 0x8) > 0)                     // A bit that MUST be zero.
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x8");
            //
            if ((LinkFileHeader.targetFlags & 0x10) > 0)                    // The link target is a directory instead of a file.
                dataGridView2.Rows.Add("  The link target is a directory");
            if ((LinkFileHeader.targetFlags & 0x20) > 0)                    // The file or directory is an archive file.
                dataGridView2.Rows.Add("  The file or directory is an archive file");
            if ((LinkFileHeader.targetFlags & 0x40) > 0)                    // A bit that MUST be zero.
                dataGridView2.Rows.Add("  unused? Unknown! Bit 0x40");
            if ((LinkFileHeader.targetFlags & 0x80) > 0)                    // The file or directory has no other flags set.
                dataGridView2.Rows.Add("  The file or directory has no other flags set");
            //
            if ((LinkFileHeader.targetFlags & 0x100) > 0)                   // The file is being used for temporary storage.
                dataGridView2.Rows.Add("  The file is being used for temporary storage");
            if ((LinkFileHeader.targetFlags & 0x200) > 0)                   // The file is a sparse file.
                dataGridView2.Rows.Add("  The file is a sparse file");
            if ((LinkFileHeader.targetFlags & 0x400) > 0)                   // The file or directory has an associated reparse point.
                dataGridView2.Rows.Add("  The file or directory has an associated reparse point");
            if ((LinkFileHeader.targetFlags & 0x800) > 0)                   // The file or directory is compressed.
                dataGridView2.Rows.Add("  The file or directory is compressed");
            //
            if ((LinkFileHeader.targetFlags & 0x1000) > 0)                  // The data of the file is not immediately available.
                dataGridView2.Rows.Add("  The data of the file is not immediately available");
            if ((LinkFileHeader.targetFlags & 0x2000) > 0)                  // The contents of the file need to be indexed.
                dataGridView2.Rows.Add("  The contents of the file need to be indexed");
            if ((LinkFileHeader.targetFlags & 0x4000) > 0)                  // The file or directory is encrypted
                dataGridView2.Rows.Add("  The file or directory is encrypted");
        }

        //------------------------------ Target List aera
        public void displayTargetIdList()
        { // The file pointer fs.Position is set to the TargetIdList header 0x4C
            Byte[] tListSize;

            if ((LinkFileHeader.shortcutFlags & HasLinkTargetIDList) > 0)
            {
                // Get the size of the TargetList
                dataGridView1.Rows.Add("Target ID List", "@ 0x" + makeLen8String((UInt32)fs.Position, 4));
                tListSize = reader.ReadBytes(2);
                tagetIdListSize = ptrToInt16(tListSize);

                dataGridView2.Rows.Add("");
                dataGridView2.Rows.Add("ID List Entries:");
                displayIdLocations();
            }
        }

        public void displayIdLocations()
        {
            Byte[] listLength;
            UInt16 ptr = 0;
            while (true)
            {
                listLength = reader.ReadBytes(2);
                if (listLength[0] == 0x0 && listLength[1] == 0x0)
                {
                    dataGridView2.Rows.Add("   Terminator found  @ 0x" + makeLen8String((UInt32)fs.Position - 2, 4));
                    return;
                }
                dataGridView2.Rows.Add("   ID List entry  @ 0x" + makeLen8String((UInt32)fs.Position - 2,4));
                ptr = ptrToInt16(listLength);
                fs.Position += ptr-2;
            }
        }

        //------------------------------ Link Info aera
        public void displayLinkInfo()
        { // The file pointer fs.Position is set to the Link Info Structure
            Byte[] linkInf;
            Byte[] buff;
            Byte[] volId;
            Byte[] netVolTable;

            UInt32 basePath = 0;
            UInt32 suffix = 0;
            string volumeName = "";
            string BasePathString = "";
            string commonBasePath = "";
            UInt32 loc = (UInt32) fs.Position;

            // Get the table
            dataGridView1.Rows.Add("Link Info List", "@ 0x" + makeLen8String((UInt32)fs.Position, 4));
            linkInf = reader.ReadBytes(Marshal.SizeOf(typeof(lnk_FILE_Location_Info)));
            handle = GCHandle.Alloc(linkInf, GCHandleType.Pinned);
            LinkFileLocations = (lnk_FILE_Location_Info)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(lnk_FILE_Location_Info));

            dataGridView2.Rows.Add("");
            dataGridView2.Rows.Add("Link Info Entries:");
            dataGridView2.Rows.Add("  Table Length 0x" + makeLen8String((UInt32)LinkFileLocations.LinkInfoSize, 4));
            dataGridView2.Rows.Add("  Header length  @ 0x" + makeLen8String((UInt32)LinkFileLocations.LinkInfoHeaderSize + loc, 4));
            displayLinkInfoFlags();
            dataGridView2.Rows.Add("  Local volume Table  @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetVolumeTable + loc, 4));
            dataGridView2.Rows.Add("  Base pathname  @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetBasePath + loc, 4));
            if (LinkFileLocations.offsetNetwork != 0x0)
                dataGridView2.Rows.Add("  Network volume Table  @ 0x" + makeLen8String((UInt32)LinkFileLocations.offsetNetwork + loc, 4));
               
            dataGridView2.Rows.Add("  Final pathname 0x" + makeLen8String((UInt32)LinkFileLocations.offsetFinalPath + loc + 1, 4));

            if ((LinkFileLocations.LinkInfoFlags & VolumeIDAndLocalBasePath) > 0)
            {
                if (LinkFileLocations.LinkInfoHeaderSize > 0x1c)
                {
                    buff = reader.ReadBytes(4);
                    LocalBasePathOffsetUnicode = ptrToInt32(buff);
                    basePath = (UInt32)fs.Position + LocalBasePathOffsetUnicode - 4;
                    dataGridView1.Rows.Add("Base Path", "@ 0x" + makeLen8String(basePath, 4));

                    buff = reader.ReadBytes(4);
                    CommonPathSuffixOffsetUnicode = ptrToInt32(buff);
                    if (LinkFileLocations.LinkInfoHeaderSize >= 0x24)
                    {
                        suffix = basePath + CommonPathSuffixOffsetUnicode;
                        dataGridView1.Rows.Add("Suffix", "@ 0x" + makeLen8String(suffix, 4));
                    }
                }
                // Volume ID
                loc = (UInt32)fs.Position;
                volId = reader.ReadBytes(Marshal.SizeOf(typeof(local_Volume_Table)));
                handle = GCHandle.Alloc(volId, GCHandleType.Pinned);
                LocalVolumeTable = (local_Volume_Table)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(local_Volume_Table));
                // VolumeName data follows (string or c_str() ??? !!!)
                volumeName = getCstring();
                string shlp = "Unknown";
                if (LocalVolumeTable.volumeType == 0x1) shlp = "No root directory";
                if (LocalVolumeTable.volumeType == 0x2) shlp = "Removable (Floppy, Zip, etc..)";
                if (LocalVolumeTable.volumeType == 0x3) shlp = "Fixed (Hard disk)";
                if (LocalVolumeTable.volumeType == 0x4) shlp = "Remote (Network drive)";
                if (LocalVolumeTable.volumeType == 0x5) shlp = "CD-ROM";
                if (LocalVolumeTable.volumeType == 0x6) shlp = "Ram drive";
                dataGridView2.Rows.Add("  Volume Entries:");
                dataGridView2.Rows.Add("    Volume Length  0x" + makeLen8String((UInt32)LocalVolumeTable.volumeTableLength, 4) + " Bytes");
                dataGridView2.Rows.Add("    Volume Type =  0x" + makeLen8String((UInt32)LocalVolumeTable.volumeType, 4) + "  -" + shlp);
                dataGridView2.Rows.Add("    Volume Serial #  0x" + makeLen8String((UInt32)LocalVolumeTable.volumeSerialNbr, 0) +
                    "  (" + (long)LocalVolumeTable.volumeSerialNbr + ")");
                dataGridView2.Rows.Add("    Volume Name  @ 0x" + makeLen8String((UInt32)LocalVolumeTable.offsetVolumeName + loc, 4));
            }

            // linkInfoFlags 0x1=VolumeIDAndLocalBasePath, 0x2=CommonNetworkRelativeLinkAndPathSuffix
            if ((LinkFileLocations.LinkInfoFlags & CommonNetworkRelativeLinkAndPathSuffix) > 0)
            {
                long pos = fs.Position;
                netVolTable = reader.ReadBytes(Marshal.SizeOf(typeof(network_Volume_Table)));
                handle = GCHandle.Alloc(netVolTable, GCHandleType.Pinned);
                NetworkVolumeTable = (network_Volume_Table)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(network_Volume_Table));
                if (NetworkVolumeTable.NetNameOffset > 0x14)
                { // get NetNameOffsetUnicode and DeviceNameOffsetUnicode
                    buff = reader.ReadBytes(4);
                    NetNameOffsetUnicode = ptrToInt32(buff);
                    buff = reader.ReadBytes(4);
                    DeviceNameOffsetUnicode = ptrToInt32(buff);
                }
                // Network flags: 0x1=ValidDevice, 0x2=ValidNetType
                if ((NetworkVolumeTable.netwFlags & ValidNetType) >0)
                    displayProvider();
                // NetworkRelativeLink, DeviceNameOffset should be 0x0
                dataGridView1.Rows.Add("Network Relative Link  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getCstring());
                // Net Name exe file
                dataGridView1.Rows.Add("Net Name  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getCstring());
                loc = (UInt32) fs.Position;
                displayNewworkInfo(pos);
                fs.Position = (long) loc;
                return;
            }

            // The file pointer fs.Position is set to the base path
            basePath = (UInt32) fs.Position;
            BasePathString = getCstring();
            dataGridView1.Rows.Add("Base Path  @ 0x" + makeLen8String(basePath, 4), BasePathString);
            commonBasePath = getCstring();
            if(commonBasePath != "")
                dataGridView1.Rows.Add("Common Base Path", commonBasePath);
        }

        public void displayProvider()
        {
            string shlp = "Unknown";
            if (NetworkVolumeTable.NetworkProviderType == 0x001A0000) shlp = "WNNC_NET_AVID";
            if (NetworkVolumeTable.NetworkProviderType == 0x001B0000) shlp = "WNNC_NET_DOCUSPACE";
            if (NetworkVolumeTable.NetworkProviderType == 0x001C0000) shlp = "WNNC_NET_MANGOSOFT";
            if (NetworkVolumeTable.NetworkProviderType == 0x001D0000) shlp = "WNNC_NET_SERNET";
            if (NetworkVolumeTable.NetworkProviderType == 0X001E0000) shlp = "WNNC_NET_RIVERFRONT1";
            if (NetworkVolumeTable.NetworkProviderType == 0x001F0000) shlp = "WNNC_NET_RIVERFRONT2";
            if (NetworkVolumeTable.NetworkProviderType == 0x00200000) shlp = "WNNC_NET_DECORB";
            if (NetworkVolumeTable.NetworkProviderType == 0x00210000) shlp = "WNNC_NET_PROTSTOR";
            if (NetworkVolumeTable.NetworkProviderType == 0x00220000) shlp = "WNNC_NET_FJ_REDIR";
            if (NetworkVolumeTable.NetworkProviderType == 0x00230000) shlp = "WNNC_NET_DISTINCT";
            if (NetworkVolumeTable.NetworkProviderType == 0x00240000) shlp = "WNNC_NET_TWINS";
            if (NetworkVolumeTable.NetworkProviderType == 0x00250000) shlp = "WNNC_NET_RDR2SAMPLE";
            if (NetworkVolumeTable.NetworkProviderType == 0x00260000) shlp = "WNNC_NET_CSC";
            if (NetworkVolumeTable.NetworkProviderType == 0x00270000) shlp = "WNNC_NET_3IN1";
            if (NetworkVolumeTable.NetworkProviderType == 0x00290000) shlp = "WNNC_NET_EXTENDNET";
            if (NetworkVolumeTable.NetworkProviderType == 0x002A0000) shlp = "WNNC_NET_STAC";
            if (NetworkVolumeTable.NetworkProviderType == 0x002B0000) shlp = "WNNC_NET_FOXBAT";
            if (NetworkVolumeTable.NetworkProviderType == 0x002C0000) shlp = "WNNC_NET_YAHOO";
            if (NetworkVolumeTable.NetworkProviderType == 0x002D0000) shlp = "WNNC_NET_EXIFS";
            if (NetworkVolumeTable.NetworkProviderType == 0x002E0000) shlp = "WNNC_NET_DAV";
            if (NetworkVolumeTable.NetworkProviderType == 0x002F0000) shlp = "WNNC_NET_KNOWARE";
            if (NetworkVolumeTable.NetworkProviderType == 0x00300000) shlp = "WNNC_NET_OBJECT_DIRE";
            if (NetworkVolumeTable.NetworkProviderType == 0x00310000) shlp = "WNNC_NET_MASFAX";
            if (NetworkVolumeTable.NetworkProviderType == 0x00320000) shlp = "WNNC_NET_HOB_NFS";
            if (NetworkVolumeTable.NetworkProviderType == 0x00330000) shlp = "WNNC_NET_SHIVA";
            if (NetworkVolumeTable.NetworkProviderType == 0x00340000) shlp = "WNNC_NET_IBMAL";
            if (NetworkVolumeTable.NetworkProviderType == 0x00350000) shlp = "WNNC_NET_LOCK";
            if (NetworkVolumeTable.NetworkProviderType == 0x00360000) shlp = "WNNC_NET_TERMSRV";
            if (NetworkVolumeTable.NetworkProviderType == 0x00370000) shlp = "WNNC_NET_SRT";
            if (NetworkVolumeTable.NetworkProviderType == 0x00380000) shlp = "WNNC_NET_QUINCY";
            if (NetworkVolumeTable.NetworkProviderType == 0x00390000) shlp = "WNNC_NET_OPENAFS";
            if (NetworkVolumeTable.NetworkProviderType == 0X003A0000) shlp = "WNNC_NET_AVID1";
            if (NetworkVolumeTable.NetworkProviderType == 0x003B0000) shlp = "WNNC_NET_DFS";
            if (NetworkVolumeTable.NetworkProviderType == 0x003C0000) shlp = "WNNC_NET_KWNP";
            if (NetworkVolumeTable.NetworkProviderType == 0x003D0000) shlp = "WNNC_NET_ZENWORKS";
            if (NetworkVolumeTable.NetworkProviderType == 0x003E0000) shlp = "WNNC_NET_DRIVEONWEB";
            if (NetworkVolumeTable.NetworkProviderType == 0x003F0000) shlp = "WNNC_NET_VMWARE";
            if (NetworkVolumeTable.NetworkProviderType == 0x00400000) shlp = "WNNC_NET_RSFX";
            if (NetworkVolumeTable.NetworkProviderType == 0x00410000) shlp = "WNNC_NET_MFILES";
            if (NetworkVolumeTable.NetworkProviderType == 0x00420000) shlp = "WNNC_NET_MS_NFS";
            if (NetworkVolumeTable.NetworkProviderType == 0x00430000) shlp = "WNNC_NET_GOOGLE";
            dataGridView2.Rows.Add("  Network Provider Type: " + shlp);
        }

        // --------------------  Display Flags
        public string decodeShortcut(UInt16 hotkey)
        {
            string shlp = "";
            int i;
            int hk;
            char[] charArray = new char[2];
            if ((hotkey == 0x00))
                return ",  (none)";

            if ((hotkey & 0xff) >= 0x70) // F1 - F24
            {
                if ((hotkey & 0xff) >= 0x90)
                {
                    if ((hotkey & 0xff) == 0x90)
                        shlp = "Num-Lock";
                    if ((hotkey & 0xff) == 0x91)
                        shlp = "Scroll-Lock";
                }
                else
                {
                    hk = (hotkey - 0x70) + 1;
                    shlp = "F" + hk + " key";
                }
            }
            else
            {
                i = (int)(hotkey & 0xff) - 0x30; // 0x30-0x39, 0x41-0x5A
                if (i > 10)
                    i -= 7;
                charArray[0] = Digits[i];
                shlp = new String(charArray, 0, 1);
            }
            if (shlp != "" && ((hotkey & 0x700) > 0))
                shlp = " + " + shlp;
            // High Byte: 0x1 = Shift, 0x2 = Control, 0x4 = Alt
            if ((hotkey & 0x400) > 0)
                shlp = "<Alt>" + shlp;
            if ((hotkey & 0x200) > 0)
                shlp = "<Ctrl>" + shlp;
            if ((hotkey & 0x100) > 0)
                shlp = "<Shift>" + shlp;
            return ",  (" + shlp + ")";
        }

        // The string pointer (fs.Position) must be set
        public string getCstring()
        {
            string shlp = "";
            Byte[] character;
            while (true)
            {
                character = reader.ReadBytes(1);
                if (character[0] == 0x0)
                    break;
                shlp += (char)character[0];
            }
            return shlp;
        }

        public void displayLinkInfoFlags()
        {
            dataGridView2.Rows.Add("  Link Info Flags:");
            if ((LinkFileLocations.LinkInfoFlags & 0x1) > 0)
                dataGridView2.Rows.Add("     VolumeIDAndLocalBasePath");
            if ((LinkFileLocations.LinkInfoFlags & 0x2) > 0)
                dataGridView2.Rows.Add("     CommonNetworkRelativeLinkAndPathSuffix");
            string shlp1 = "on a local volume";
            if ((LinkFileLocations.LinkInfoFlags & 0x2) > 0)
                shlp1 = "on a network volume";
            dataGridView2.Rows.Add("     The file is " + shlp1);
        }

        //------------------------------ String area
        public string getUnicodeString()
        { // the string has a character counter at the start
            string shlp="";
            int len=0;
            Byte[] buff = reader.ReadBytes(2);

            len = ptrToInt16(buff);
            if(len == 0)            // pointer to an empty string
                return shlp;
            while(len > 0)
            {
                buff = reader.ReadBytes(2);
                shlp += (char) buff[0];
                len --;
            }
            return shlp;
        }

        public void DisplayStrings()
        {
            // STRING_DATA = [NAME_STRING] [RELATIVE_PATH] [WORKING_DIR] [COMMAND_LINE_ARGUMENTS] [ICON_LOCATION]
            if ((LinkFileHeader.shortcutFlags & HasName) > 0)
                dataGridView1.Rows.Add("Name  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getUnicodeString());
            if ((LinkFileHeader.shortcutFlags & HasRelativePath) > 0)
                dataGridView1.Rows.Add("Relative Path  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getUnicodeString());
            if ((LinkFileHeader.shortcutFlags & HasWorkingDir) > 0)
                dataGridView1.Rows.Add("Working Directory  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getUnicodeString());
            if ((LinkFileHeader.shortcutFlags & HasArguments) > 0)
                dataGridView1.Rows.Add("Arguments  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getUnicodeString());
            if ((LinkFileHeader.shortcutFlags & HasIconLocation) > 0)
                dataGridView1.Rows.Add("Icon Location  @ 0x" + makeLen8String((UInt32)fs.Position, 4), getUnicodeString());
        }

        //------------------------------ Network area
        public void displayNewworkInfo(long pos)
        {
            UInt32 loc = (UInt32)pos; // loc holds the pointer to the LinkFileLocationTable, 
            colColorChange2 = dataGridView2.RowCount - 1;
            // The NetworkVolumeTable and additional info is already read in
            if (LinkFileLocations.offsetNetwork > 0)
            {
                dataGridView2.Rows.Add("");
                dataGridView2.Rows.Add("Network Volume Table Entries:  @ 0x" + makeLen8String(loc, 4));
                dataGridView2.Rows.Add("  Volume Length 0x" + makeLen8String((UInt32)NetworkVolumeTable.netwVolumeLength, 4));
                dataGridView2.Rows.Add("  Network Flags 0x" + makeLen8String((UInt32)NetworkVolumeTable.netwFlags, 4));
                if (NetworkVolumeTable.NetNameOffset > 0x0)
                dataGridView2.Rows.Add("  Volume Name  @ 0x" + makeLen8String((UInt32)NetworkVolumeTable.NetNameOffset + loc, 4));
                if (NetworkVolumeTable.DeviceNameOffset > 0x0)
                    dataGridView2.Rows.Add("  Device Name  @ 0x" + makeLen8String((UInt32)NetworkVolumeTable.DeviceNameOffset + loc, 4));
                if (NetworkVolumeTable.NetNameOffset > 0x14)
                {
                    dataGridView2.Rows.Add("  Network Name Unicode  @ 0x" + makeLen8String((UInt32)NetNameOffsetUnicode + loc, 4));
                    dataGridView2.Rows.Add("  Device Name Unicode  @ 0x" + makeLen8String((UInt32)DeviceNameOffsetUnicode + loc, 4));
                }
            }
        }

        //------------------------------ Misc Area
        private void StartProcess(string file)
        {
            string shlp;
            Process myProcess = new Process();
            try
            {
                // FileName has to be trimmed to run properly!
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = file.Trim(); 
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
                MessageBox.Show(file, shlp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            myProcess.Dispose();
        }

        public void getDesktopLinks()
        {
            string user;
            string[] files; 

            treeView1.Nodes.Clear();
            user = Environment.GetEnvironmentVariable("USERNAME");
            folderBrowserDialog1.SelectedPath = "C:\\Users\\" + user + "\\OneDrive\\Desktop";

            files = Directory.GetFiles(folderBrowserDialog1.SelectedPath,"*.lnk");
            foreach (string file in files)
            {
                treeView1.Nodes.Add("one",Path.GetFileName(file));
                treeView1.Nodes[treeView1.Nodes.Count - 1].ToolTipText = file;
            }
        }

        public void getRecursiveDirectoryLinks(string folder, TreeNode tn)
        {
            string shlp;
            int i;
            // get the directories
            try
            {
                string[] directories = Directory.GetDirectories(folder);
                foreach (string str in directories)
                {
                    int startIndex = str.LastIndexOf("\\") + 1;
                    shlp = str.Substring(startIndex, str.Length - startIndex);
                    if (tn == null)
                    {
                        treeView1.Nodes.Add("1", shlp, 0, 3);
                        treeView1.Nodes[treeView1.Nodes.Count - 1].ToolTipText = str;
                    }
                    else
                    {
                        tn.Nodes.Add("1", shlp, 0, 3);
                        tn.Nodes[tn.Nodes.Count - 1].ToolTipText = str;
                    }
                }
            }
            catch
            {
                return;
            }

            try
            {
                if (tn == null)
                    for (i = 0; i < treeView1.Nodes.Count; i++)
                        getRecursiveDirectoryLinks(treeView1.Nodes[i].ToolTipText, treeView1.Nodes[i]);
            }
            catch { return; }
            // enter the files
            try
            {
                for (int x = 0; x < tn.Nodes.Count; x++)
                    getRecursiveDirectoryLinks(tn.Nodes[x].ToolTipText, tn.Nodes[x]);
            }
            catch { }
            foreach (string str in Directory.GetFiles(folder.Trim(), "*.lnk"))
            {
                int startIndex = str.LastIndexOf("\\") + 1;
                string str2 = str.Substring(startIndex, str.Length - startIndex);
                if (tn == null)
                {
                    treeView1.Nodes.Add("2", str2, 1, 2);
                    treeView1.Nodes[treeView1.Nodes.Count - 1].ToolTipText = str;
                }
                else
                {
                    tn.Nodes.Add("2", str2, 1, 2);
                    tn.Nodes[tn.Nodes.Count - 1].ToolTipText = str;
                }
            }
        }
    }
}
