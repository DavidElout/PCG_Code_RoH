using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public interface IAreaTypeGenerator
    {
        System.Type AreaType { get; }

        #region Methods

        /// <summary>
        /// Create procedural algorithm to spawn 'things' according to the areaType
        /// </summary>
        /// <param name="area">List of hexes belonging to the Area</param>
        void ProcessArea(Dictionary<Vector3, PCGHex> area);

        #endregion
    }
}