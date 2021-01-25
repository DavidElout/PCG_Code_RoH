using System;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class PerlinGridGenerator : ITileMapGenerator
    {
        #region Fields

        public Type SettingsType => typeof(PerlinSettings);

        

        #endregion

        #region Public Methods

        public void GenerateTileMap(Dictionary<Vector3, PCGHex> pcgHexDictionary, GenerationSettings generationSettings, int seed, Vector2 offset)
        {
            PerlinSettings perlinGenerator = (PerlinSettings)generationSettings;

            System.Random prng = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[perlinGenerator.Octaves];
            for (int i = 0; i < perlinGenerator.Octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            foreach (KeyValuePair<Vector3, PCGHex> keyPair in pcgHexDictionary)
            {
                PCGHex pcgHex = keyPair.Value;
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < perlinGenerator.Octaves; i++)
                {
                    float sampleX = pcgHex.HexCoord.x / perlinGenerator.Scale * frequency + octaveOffsets[i].x;
                    float sampleY = pcgHex.HexCoord.y / perlinGenerator.Scale * frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= perlinGenerator.Persistance;
                    frequency *= perlinGenerator.Lacunarity;

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    if (noiseHeight > 0.5f)
                    {
                        pcgHex.IsWalkable = true;
                    }
                    else
                    {
                        pcgHex.IsWalkable = false;
                    }
                }
                pcgHex.UpdateHexColor();
                pcgHex.AreaType = MaskType.Default;
            }
            //TileMapCorrection(pcgHexDictionary);
        }


        void TileMapCorrection(Dictionary<Vector3, PCGHex> pcgHexDictionary)
        {
            foreach (KeyValuePair<Vector3, PCGHex> keyPair in pcgHexDictionary)
            {
                PCGHex currentHex = keyPair.Value;
                List<PCGHex> inactiveGroup = new List<PCGHex>();

                //check state
                if (currentHex.IsWalkable == false)
                {
                    inactiveGroup.Add(currentHex);

                    //loop al neighbours
                    for (int i = 0; i < currentHex.NeighbourCount; i++)
                    {
                        //check state
                        if (currentHex.GetNeighbourAtIndex(i) == false)
                        {
                            //add to list
                            inactiveGroup.Add(currentHex.GetNeighbourAtIndex(i));
                        }
                    }
                }

                //compare list size
                if (inactiveGroup.Count < 6)
                {
                    //change state
                    for (int j = 0; j < inactiveGroup.Count; j++)
                    {
                        inactiveGroup[j].IsWalkable = true;
                        inactiveGroup[j].UpdateHexColor();
                    }
                }
            }
        }
        #endregion
    }
}