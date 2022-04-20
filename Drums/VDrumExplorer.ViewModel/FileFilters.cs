// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;

namespace VDrumExplorer.ViewModel
{
    public class FileFilter
    {
        public string Name { get; }

        public string[] Extensions { get; }

        public FileFilter(string name, string[] extensions)
        {
            Name = name;
            Extensions = extensions;
        }
    }

    /// <summary>
    /// Central ized repository of file filters for save/open file dialog boxes
    /// </summary>
    internal static class FileFilters
    {
        internal static readonly FileFilter[] KitFiles = {
            new FileFilter("Kit files", new[] { "vkit" })
        };

        internal static readonly FileFilter[] ModuleFiles = {
            new FileFilter("V-Drum Explorer module files", new[] { "vdrum" })
        };
        
        internal static readonly FileFilter[] InstrumentAudioFiles = {
            new FileFilter("V-Drum Explorer audio files", new[] { "vaudio" })
        };

        internal static readonly FileFilter[] AllExplorerFiles = {
            new FileFilter("All explorer files", new[] { "vdrum", "vkit", "vaudio" }),
            new FileFilter("Module files", new[] { "vdrum" }),
            new FileFilter("Kit files", new[] { "vkit" }),
            new FileFilter("Audio files", new[] { "vaudio" })
        };

        internal static readonly FileFilter[] LogFiles = {
            new FileFilter("Log files", new[] { "json" })
        };
    }
}
