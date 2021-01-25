using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public interface ITileMapGenerator
    {
        System.Type SettingsType { get; }
        

        #region Methods

        void GenerateTileMap(Dictionary<Vector3, PCGHex> pcgHexDictionary, GenerationSettings generationSettings, int seed, Vector2 offset);

        #endregion
    }
}