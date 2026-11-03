using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace LnkFileExplorer
{
    public partial class Form1 : Form
    {
        CONSOLE_PROPS ConsoleDataBlock = new CONSOLE_PROPS();
        CONSOLE_PROPS1 ConsoleDataBlock1 = new CONSOLE_PROPS1();
        CONSOLE_FE_PROPS ConsoleFEDataBlock = new CONSOLE_FE_PROPS();
        DARWIN_PROPS DarwinDataBlock = new DARWIN_PROPS();
        ENVIRONMENT_PROPS EnvironmentVariableDataBlock = new ENVIRONMENT_PROPS();
        ICON_ENVIRONMENT_PROPS IconEnvironmentDataBlock = new ICON_ENVIRONMENT_PROPS();
        KNOWN_FOLDER_PROPS KnownFolderDataBlock = new KNOWN_FOLDER_PROPS();
        PROPERTY_STORE_PROPS PropertyStoreDataBlock = new PROPERTY_STORE_PROPS();
        SHIM_PROPS ShimDataBlock = new SHIM_PROPS();
        SPECIAL_FOLDER_PROPS SpecialFolderDataBlock = new SPECIAL_FOLDER_PROPS();
        TRACKER_PROPS TrackerDataBlock = new TRACKER_PROPS();
        VISTA_AND_ABOVE_IDLIST_PROPS VistaAndAboveIDListDataBlock = new VISTA_AND_ABOVE_IDLIST_PROPS();

       public bool extraDataTables()
        {
            // Table recognition:
            //      Table                           Size (min)  Signature
            //      CONSOLE_PROPS                   0x000000CC  0xA0000002
            //      CONSOLE_FE_PROPS                0x0000000C  0xA0000004
            //      DARWIN_PROPS                    0x00000314  0xA0000006
            //      ENVIRONMENT_PROPS               0x00000314  0xA0000001
            //      ICON_ENVIRONMENT_PROPS          0x00000314  0xA0000007
            //      KNOWN_FOLDER_PROPS              0x0000001C  0xA000000B
            //      PROPERTY_STORE_PROPS            0x0000000C  0xA0000009
            //      SHIM_PROPS                      0x00000088  0xA0000008
            //      SPECIAL_FOLDER_PROPS            0x00000010  0xA0000005
            //      TRACKER_PROPS                   0x00000060  0xA0000003
            //      VISTA_AND_ABOVE_IDLIST_PROPS    0x0000000A  0xA000000C

            Byte[] lenb;
            Byte[] sigb;

            UInt32 len;
            UInt32 sig;

            long start = fs.Position;

            while (true)
            {
                // read length or terminator
                lenb = reader.ReadBytes(4);
                len = ptrToInt32(lenb);

                if (len == 0x0)
                {
                    fs.Position -= 4;
                    dataGridView1.Rows.Add("File Terminator followed");

                    string shlp = (String)dataGridView3.Rows[dataGridView3.RowCount-1].Cells[0].FormattedValue;
                    return true;
                }

                sigb = reader.ReadBytes(4);
                sig = ptrToInt32(sigb);
                
                fs.Position -= 8;
                switch (sig)
                {
                    case 0xA0000001: ENVIRONMENT(); break;
                    case 0xA0000002: CONSOLE(); break;
                    case 0xA0000003: TRACKER(); break;
                    case 0xA0000004: CONSOLE_FE(); break;
                    case 0xA0000005: SPECIAL_FOLDER(); break;
                    case 0xA0000006: DARWIN(); break;           // LinkFileHeader.shortcutFlags & HasDarwinID

                    case 0xA0000007: ICON_ENVIRONMENT(); break;
                    case 0xA0000008: SHIM(); break;
                    case 0xA0000009: PROPERTY_STORE(); break;
                    case 0xA000000B: KNOWN_FOLDER(); break;
                    case 0xA000000C: VISTA_AND_ABOVE_IDLIST(); break;
                    default: break;
                }
                start += len;
                // for a breakpoint in debuging, else remove the if statement
                //if (fs.Position != start)
                    fs.Position = start;

                if (dataGridView3.RowCount > 1)
                    if ((String)dataGridView3.Rows[dataGridView3.RowCount - 2].Cells[0].FormattedValue != "")
                        if(fs.Position < fs.Length -4)
                            dataGridView3.Rows.Add("");
            }
        }
        
        public void CONSOLE()
        { // done
            Byte[] cons;
            Byte[] cons1;
            UInt32 pos = (UInt32)fs.Position;

            // get the first part of the structure
            cons = reader.ReadBytes(Marshal.SizeOf(typeof(CONSOLE_PROPS)));
            handle = GCHandle.Alloc(cons, GCHandleType.Pinned);
            ConsoleDataBlock = (CONSOLE_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CONSOLE_PROPS));

            // Ok, let's display the monster
            dataGridView3.Rows.Add("Console Data Block:   @ 0x" + makeLen8String(pos, 4));
            displayColorsContained();
            dataGridView3.Rows.Add("  Screen Buffer size (X:Y)  0x" + makeLen8String((UInt32)ConsoleDataBlock.ScreenBuffer, 4));
            dataGridView3.Rows.Add("  Window  size (X:Y)  0x" + makeLen8String((UInt32)ConsoleDataBlock.WindowSize, 4));
            dataGridView3.Rows.Add("  Window  Origin (X:Y)  0x" + makeLen8String((UInt32)ConsoleDataBlock.WindowOrigin, 4));
            dataGridView3.Rows.Add("  Font size + 0x" + makeLen8String((UInt32)ConsoleDataBlock.WindowOrigin, 4));
            displayFontFamily();
            if(ConsoleDataBlock.FontWeight >= 700)      // is that 0x700 or 700??!!!
                dataGridView3.Rows.Add("  The Font is Bold");
            else
                dataGridView3.Rows.Add("  The Font is Regular");
            // FaceName Unicode[32] is next
            displayFaceName();

            // get the second part of the structure
            cons1 = reader.ReadBytes(Marshal.SizeOf(typeof(CONSOLE_PROPS1)));
            handle = GCHandle.Alloc(cons1, GCHandleType.Pinned);
            ConsoleDataBlock1 = (CONSOLE_PROPS1)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CONSOLE_PROPS1));

            if(ConsoleDataBlock1.CursorSize <= 25)
                dataGridView3.Rows.Add("  The cursor size is small");
            if(ConsoleDataBlock1.CursorSize > 25 && ConsoleDataBlock1.CursorSize <=50)
                dataGridView3.Rows.Add("  The cursor size is medium");
            if(ConsoleDataBlock1.CursorSize >= 51)
                dataGridView3.Rows.Add("  The cursor size is large");
            if(ConsoleDataBlock1.FullScreen == 0x0)
                dataGridView3.Rows.Add("  Fullscreen is OFF");
            else
                dataGridView3.Rows.Add("  Fullscreen is ON");
            if (ConsoleDataBlock1.QuickEdit == 0x0)
                dataGridView3.Rows.Add("  QuickEdit is OFF");
            else
                dataGridView3.Rows.Add("  QuickEdit is ON");
            if (ConsoleDataBlock1.InsertMode == 0x0)
                dataGridView3.Rows.Add("  InsertMode is OFF");
            else
                dataGridView3.Rows.Add("  InsertMode is ON");
            if (ConsoleDataBlock1.AutoPosition == 0x0)
                dataGridView3.Rows.Add("  Use Window-OriginX and Y to position the Window");
            else
                dataGridView3.Rows.Add("  AutoPosition is ON");
            dataGridView3.Rows.Add("  History Buffer size: 0x" + makeLen8String((UInt32)ConsoleDataBlock1.HistoryBufferSize, 4));
            // Let's call it quit here
        }

        public void displayFaceName()
        {
            Byte[] name;
            int i;
            string shlp = "";
            name = reader.ReadBytes(32);
            for (i = 0; i < 16; i++)
            {
                if(name[i*2] == 0x0)
                    break;
                shlp += name[i*2];
            }
            if(shlp != "")
                dataGridView3.Rows.Add("  The Font Face Name is: ");
        }

        public void displayFontFamily()
        {
            if (ConsoleDataBlock.FontFamily == 0x0)
                dataGridView3.Rows.Add("  Font Unknown");
            if ((ConsoleDataBlock.FontFamily & 0x10) > 0)
                dataGridView3.Rows.Add("  Variable Font width with serifs");
            if ((ConsoleDataBlock.FontFamily & 0x20) > 0)
                dataGridView3.Rows.Add("  Variable Font width without serifs");
            if ((ConsoleDataBlock.FontFamily & 0x30) > 0)
                dataGridView3.Rows.Add("  Fixed Font width with or without serifs");
            if ((ConsoleDataBlock.FontFamily & 0x40) > 0)
                dataGridView3.Rows.Add("  A handwriting look alike Font");
            if ((ConsoleDataBlock.FontFamily & 0x40) > 0)
                dataGridView3.Rows.Add("  A novelty Font");
        }

        public void displayColorsContained()
        {
            string shlp = "  ForeColor has: ";

            if ((ConsoleDataBlock.Attributes & 0x4) > 0)
                shlp += "R";
            if ((ConsoleDataBlock.Attributes & 0x2) > 0)
                shlp += "G";
            if ((ConsoleDataBlock.Attributes & 0x1) > 0)
                shlp += "B";
            if ((ConsoleDataBlock.Attributes & 0x8) > 0)
                shlp += " and is intensified.";

            dataGridView3.Rows.Add(shlp);
            shlp = "  BackColor has: ";
            if ((ConsoleDataBlock.Attributes & 0x40) > 0)
                shlp += "R";
            if ((ConsoleDataBlock.Attributes & 0x20) > 0)
                shlp += "G";
            if ((ConsoleDataBlock.Attributes & 0x10) > 0)
                shlp += "B";
            if ((ConsoleDataBlock.Attributes & 0x80) > 0)
                shlp += " and is intensified.";

            dataGridView3.Rows.Add(shlp);
        }

        public void CONSOLE_FE() 
        { // done
            Byte[] consfe;
            UInt32 pos = (UInt32)fs.Position;
            consfe = reader.ReadBytes(Marshal.SizeOf(typeof(CONSOLE_FE_PROPS)));
            handle = GCHandle.Alloc(consfe, GCHandleType.Pinned);
            ConsoleFEDataBlock = (CONSOLE_FE_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CONSOLE_FE_PROPS));

            dataGridView3.Rows.Add("*Console FE Data Block:   @ 0x" + makeLen8String(pos, 4));
            dataGridView3.Rows.Add("Code Page " + makeLen8String((UInt32)ConsoleFEDataBlock.CodePage, 4) +
                "  (" + ConsoleFEDataBlock.CodePage + ")");
        }



        public void DARWIN()
        { // done
            Byte[] darv;
            UInt32 pos = (UInt32)fs.Position;
            darv = reader.ReadBytes(Marshal.SizeOf(typeof(DARWIN_PROPS)));
            handle = GCHandle.Alloc(darv, GCHandleType.Pinned);
            DarwinDataBlock = (DARWIN_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DARWIN_PROPS));

            dataGridView3.Rows.Add("Darwin Data Block:   @ 0x" + makeLen8String(pos, 4));
            // A NULL–terminated string, defined by the system default code page, which specifies an application identifier. Ignore it.
            // An optional, NULL–terminated, Unicode string that specifies an application identifier.
        }

        public void ENVIRONMENT()
        { // done
            Byte[] env;
            UInt32 pos = (UInt32)fs.Position;
            env = reader.ReadBytes(Marshal.SizeOf(typeof(ENVIRONMENT_PROPS)));
            handle = GCHandle.Alloc(env, GCHandleType.Pinned);
            EnvironmentVariableDataBlock = (ENVIRONMENT_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ENVIRONMENT_PROPS));

            dataGridView3.Rows.Add("Environment Data Block:   @ 0x" + makeLen8String(pos, 4));
        }

        public void ICON_ENVIRONMENT()
        { // done
            Byte[] icon;
            UInt32 pos = (UInt32)fs.Position;
            icon = reader.ReadBytes(Marshal.SizeOf(typeof(ICON_ENVIRONMENT_PROPS)));
            handle = GCHandle.Alloc(icon, GCHandleType.Pinned);
            IconEnvironmentDataBlock = (ICON_ENVIRONMENT_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ICON_ENVIRONMENT_PROPS));
            dataGridView3.Rows.Add("Icon Environment Data Block:   @ 0x" + makeLen8String(pos, 4));
        }

        public void KNOWN_FOLDER() 
        { // done,  points to a part of the relative path
            Byte[] know;
            UInt32 pos = (UInt32)fs.Position;
            know = reader.ReadBytes(Marshal.SizeOf(typeof(KNOWN_FOLDER_PROPS)));
            handle = GCHandle.Alloc(know, GCHandleType.Pinned);
            KnownFolderDataBlock = (KNOWN_FOLDER_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(KNOWN_FOLDER_PROPS));

            dataGridView3.Rows.Add("Known Folder Data Block:   @ 0x" + makeLen8String(pos, 4));
            // Offset points to the fist child entry (IDLIST)
            dataGridView3.Rows.Add("  Known Folder CLSID  " + KnownFolderDataBlock.KnownFolderID);
            dataGridView3.Rows.Add("  Known Folder  @ 0x" + makeLen8String((UInt32)KnownFolderDataBlock.Offset + tagetIdListSize, 4));
        }

        public void PROPERTY_STORE()
        { // done
            Byte[] prop;
            Guid CLSID;
            UInt32 valSize;
            UInt32 id;
            UInt32 stringStart, SizeOfStrings, charCount;
            string name = "";
            UInt32 pos = (UInt32)fs.Position;

            prop = reader.ReadBytes(Marshal.SizeOf(typeof(PROPERTY_STORE_PROPS)));
            handle = GCHandle.Alloc(prop, GCHandleType.Pinned);
            PropertyStoreDataBlock = (PROPERTY_STORE_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(PROPERTY_STORE_PROPS));
            dataGridView3.Rows.Add("Property Store Data Block:   @ 0x" + makeLen8String(pos, 4));

            // A serialized property storage structure ([MS-PROPSTORE] follows
            if (PropertyStoreDataBlock.PropertyStoreSize > 0)
            {
                prop = reader.ReadBytes(4);
                propertyStoreFormatId = ptrToInt32(prop);

                prop = reader.ReadBytes(16);
                handle = GCHandle.Alloc(prop, GCHandleType.Pinned);
                CLSID = (Guid)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Guid));

                dataGridView3.Rows.Add("  CLSID:  " + CLSID);

                if (CLSID == Guid.Parse("D5CDD505-2E9C-101B-9397-08002B2CF9AE"))
                { // String Name follows
                    dataGridView3.Rows.Add("*  String Name @ 0x" + makeLen8String((UInt32)fs.Position, 4));
                }
                else
                { // Integer Name follows
                    stringStart = (UInt32)fs.Position;

                    prop = reader.ReadBytes(4); // Total size
                    SizeOfStrings = ptrToInt32(prop);
                    dataGridView3.Rows.Add("  Block Size 0x" + makeLen8String(SizeOfStrings, 4));

                    prop = reader.ReadBytes(4); // value size
                    valSize = ptrToInt32(prop);

                    // Some links are on EOF here eg. all Windows games, run.lnk
                    if (fs.Position == fs.Length - 4)
                        return;

                    prop = reader.ReadBytes(4); // ID
                    id = ptrToInt32(prop);

                    // High bits set: Then its a string eg. 0xA0000001
                    prop = reader.ReadBytes(4); // ID
                    charCount = ptrToInt32(prop);

                    if (SizeOfStrings == 0x0 || (charCount & 0xf0000000) > 0)
                    {
                        charCount = charCount & 0x3fffffff;
                        name = getCstring();
                        // Ignore the rest of the structure!!!
                    }
                    else
                    {
                        charCount = charCount >> 0x8;
                        while (charCount >= 2)
                        {
                            prop = reader.ReadBytes(2);
                            name += (char)prop[1];
                            charCount--;
                        }
                        dataGridView3.Rows.Add("  Name:  " + name);

                        prop = reader.ReadBytes(2); // get the terminator
                        // Ignore the rest of the structure, Packed records follow!!!
                    }
                }
            }
        }

        public void SHIM()
        { // done, check this as no data where present!!!
            Byte[] shim;
            UInt32 pos = (UInt32)fs.Position;
            shim = reader.ReadBytes(Marshal.SizeOf(typeof(SHIM_PROPS)));
            handle = GCHandle.Alloc(shim, GCHandleType.Pinned);
            ShimDataBlock = (SHIM_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SHIM_PROPS));

            dataGridView3.Rows.Add("Shim Data Block:   @ 0x" + makeLen8String(pos, 4));
            dataGridView3.Rows.Add("  Layer Name: " + getUnicodeString());
        }

        public void SPECIAL_FOLDER()
        { // done, points to a part of the relative path
            Byte[] spec;
            UInt32 pos = (UInt32)fs.Position;
            spec = reader.ReadBytes(Marshal.SizeOf(typeof(SPECIAL_FOLDER_PROPS)));
            handle = GCHandle.Alloc(spec, GCHandleType.Pinned);
            SpecialFolderDataBlock = (SPECIAL_FOLDER_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPECIAL_FOLDER_PROPS));

            // Offset points to the fist child entry (IDLIST)
            dataGridView3.Rows.Add("Special Folder Data Block:   @ 0x" + makeLen8String(pos, 4));
            dataGridView3.Rows.Add("  Special Folder ID  0x" + makeLen8String((UInt32)SpecialFolderDataBlock.SpecialFolderID, 4) +
                "  (" + SpecialFolderDataBlock.SpecialFolderID + ")");
            dataGridView3.Rows.Add("  Special Folder  @ 0x" + makeLen8String((UInt32)SpecialFolderDataBlock.Offset + tagetIdListSize, 4));
        }

        public void TRACKER() 
        { // done
            Byte[] tra;
            UInt32 pos = (UInt32)fs.Position;
            tra = reader.ReadBytes(Marshal.SizeOf(typeof(TRACKER_PROPS)));
            handle = GCHandle.Alloc(tra, GCHandleType.Pinned);
            TrackerDataBlock = (TRACKER_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TRACKER_PROPS));

            MachineID = "";
            tra = reader.ReadBytes(16);
            for (int i = 0; i < 16; i++)
            {
                if (tra[i] == 0x0)
                    break;
                MachineID += (char)tra[i];
            }
            tra = reader.ReadBytes(32);
            handle = GCHandle.Alloc(tra, GCHandleType.Pinned);
            Droid = (Guid)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Guid));
            tra = reader.ReadBytes(32);
            handle = GCHandle.Alloc(tra, GCHandleType.Pinned);
            DroidBirth = (Guid)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Guid));

            dataGridView3.Rows.Add("Tracker:   @ 0x" + makeLen8String(pos, 4));
            dataGridView3.Rows.Add("  Machine Name   " + MachineID);
            dataGridView3.Rows.Add("  Droid CLSID   " + Droid);
            dataGridView3.Rows.Add("  Droid Birth CLSID   " + DroidBirth);
        }

        public void VISTA_AND_ABOVE_IDLIST() 
        { // done
            Byte[] vis;
            UInt32 pos = (UInt32)fs.Position;
            vis = reader.ReadBytes(Marshal.SizeOf(typeof(VISTA_AND_ABOVE_IDLIST_PROPS)));
            handle = GCHandle.Alloc(vis, GCHandleType.Pinned);
            VistaAndAboveIDListDataBlock = (VISTA_AND_ABOVE_IDLIST_PROPS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(VISTA_AND_ABOVE_IDLIST_PROPS));

            dataGridView3.Rows.Add("Vista and above ID List:   @ 0x" + makeLen8String(pos, 4));
            // there is no point to display this list
            // it should be an alternate IDList but terminators where not found
        }

    }
}
