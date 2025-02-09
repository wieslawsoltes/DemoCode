﻿// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace VDrumExplorer.Model.Midi
{
    /// <summary>
    /// Everything required to confidently identify a module. This is
    /// used to link schemas with devices and files.
    /// </summary>
    public sealed class ModuleIdentifier : IEquatable<ModuleIdentifier?>
    {
        public static ModuleIdentifier AE01 { get; } = new ModuleIdentifier("AE-01", 0x5a, 0x35a, 0);
        public static ModuleIdentifier AE10 { get; } = new ModuleIdentifier("AE-10", 0x2f, 0x32f, 0);
        public static ModuleIdentifier TD07 { get; } = new ModuleIdentifier("TD-07", 0x75, 0x375, 0);
        public static ModuleIdentifier TD17 { get; } = new ModuleIdentifier("TD-17", 0x4b, 0x34b, 0);
        public static ModuleIdentifier TD27 { get; } = new ModuleIdentifier("TD-27", 0x63, 0x363, 0);
        public static ModuleIdentifier TD50 { get; } = new ModuleIdentifier("TD-50", 0x24, 0x324, 0);
        public static ModuleIdentifier TD50X { get; } = new ModuleIdentifier("TD-50X", 0x07, 0x407, 0);

        /// <summary>
        /// The name of the module, e.g. "TD-17".
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The ID of the module.
        /// </summary>
        public int ModelId { get; }

        /// <summary>
        /// The family code as reported by a Midi identity response.
        /// </summary>
        public int FamilyCode { get; }

        /// <summary>
        /// The family number code as reported by a Midi identity response.
        /// </summary>
        public int FamilyNumberCode { get; }

        /// <summary>
        /// The length of the model ID in data set/request packets (DT1/RQ1).
        /// This is usually 4, but is 5 for the TD-50X (for no obvious reason).
        /// (This does not contribute to equality checks.)
        /// </summary>
        public int ModelIdLength { get; }

        public ModuleIdentifier(string name, int modelId, int familyCode, int familyNumberCode) =>
            (Name, ModelId, FamilyCode, FamilyNumberCode, ModelIdLength) =
            (name, modelId, familyCode, familyNumberCode, DetermineModelIdLength(name));

        public override bool Equals(object obj) => Equals(obj as ModuleIdentifier);

        public bool Equals(ModuleIdentifier? other) =>
            other != null &&
            (other.Name, other.ModelId, other.FamilyCode, other.FamilyNumberCode) == (Name, ModelId, FamilyCode, FamilyNumberCode);

        public override int GetHashCode() => (Name, ModelId, FamilyCode, FamilyNumberCode).GetHashCode();

        public override string ToString() => $"Name: {Name}; ModelId: {ModelId}; FamilyCode: {FamilyCode}; FamilyNumberCode: {FamilyNumberCode}";

        // TODO: Make this less hacky.
        private static int DetermineModelIdLength(string name) => name == "TD-50X" ? 5 : 4;
    }
}
