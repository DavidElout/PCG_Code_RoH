using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// HexObject Types are used for building associations.
/// This cna be expanded in the future if there are more ideas on building connections.
/// </summary>
namespace PCG
{
    /// <summary>
    /// When adding a HexObjectType, add it above the default type.
    /// When adding a HexObjectType, add a color in the PCGObjectGeneratorEditor.cs "GetColor(HexObjectType objectType)" method
    /// </summary>
    public enum HexObjectType
    {
        Main,
        Optional,
        Unusable,
        Vegetation,
        Area,

        //Insert new above default ^
        Default
    }
}