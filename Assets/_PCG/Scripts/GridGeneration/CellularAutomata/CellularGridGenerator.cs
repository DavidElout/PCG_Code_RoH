using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class CellularGridGenerator : ITileMapGenerator
    {
        public Type SettingsType => typeof(CellularSettings);

        #region Methods
        public void GenerateTileMap(Dictionary<Vector3, PCGHex> pcgHexDictionary, GenerationSettings generationSettings, int seed, Vector2 offset)
        {
            CellularSettings cellularSettings = (CellularSettings)generationSettings;
            System.Random prng = new System.Random(seed);

            //Generate starting values
            foreach (KeyValuePair<Vector3, PCGHex> keyPair in pcgHexDictionary)
            {
                PCGHex pcgHex = keyPair.Value;
                bool iswalkable = prng.Next(0, 100) <= cellularSettings.ChangeToStartWalkable ? true : false;
                pcgHex.IsWalkable = iswalkable;
            }

            //CellularIterations
            for (int i = 0; i < cellularSettings.NumberOfIterations; i++)
            {
                //Calculate new walkable state for every hex
                foreach (KeyValuePair<Vector3, PCGHex> keyPair in pcgHexDictionary)
                {
                    //current PCGHex
                    PCGHex currentHex = keyPair.Value;

                    //neighbours trackers
                    int walkableNeighbours = currentHex.GetActiveNeighboursCount();

                    //new walkable State
                    bool isWalkable = currentHex.IsWalkable;

                    //Cellular for walkable cell
                    if (currentHex.IsWalkable)
                    {
                        if (walkableNeighbours < cellularSettings.StarvationLimit)
                        {
                            isWalkable = false;
                        }
                    }
                    //Cellular for nonWalkable cell
                    else
                    {
                        if (walkableNeighbours >= cellularSettings.BirthNumber)
                        {
                            isWalkable = true;
                        }
                    }

                    //Set value
                    currentHex.IsWalkableNew = isWalkable;
                }

                //Apply new walklable values
                foreach (KeyValuePair<Vector3, PCGHex> keyPair in pcgHexDictionary)
                {
                    keyPair.Value.IsWalkable = keyPair.Value.IsWalkableNew;
                    keyPair.Value.UpdateHexColor();

                    keyPair.Value.AreaType = MaskType.Default;
                }
            }
        }
        #endregion
    }
}
