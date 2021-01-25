using System.Collections.Generic;
using UnityEngine;
using HexGridEditor;
using System;
using RoH.Grid;
using UnityEditor;
using System.IO;
using RoH.Missions;
using System.Linq;

namespace PCG
{
    public class GridGenerator : MonoBehaviour
    {
        #region Editor Fields

        //[Header("Grid Setting")]
        //[SerializeField]
        //private Vector3 _gridDimensions;            //Does not represent the amount of hexes.
        //[Space(1)]


        public bool AutoUpdate;
        public bool debugMode = false;
        [SerializeField]
        [Range(0, 10000)]
        public int Seed;
        [SerializeField]
        private Vector3 dimensions = new Vector3(40f, 0f, 40f);
        [SerializeReference]
        public GenerationSettings GenerationSettings;
        [SerializeField]
        private PCGTheme _pcgTheme;

        [Space(1)]
        [Header("Generation Settings Settings")]


        #endregion

        #region Fields

        private GameObject _mainGrid;
        private GameObject _hexPreviewerObject;
        public Dictionary<Vector3, PCGHex> _pcgHexDictionary = new Dictionary<Vector3, PCGHex>();
        //private Dictionary<Vector3, HexPreviewData> _hexPreviewPositions = new Dictionary<Vector3, HexPreviewData>();
        private Dictionary<Vector3, Vector3> _hexPositions = new Dictionary<Vector3, Vector3>();
        private GameObject _pcgHexPrefab;
        private ITileMapGenerator tileMapGenerator;

        //FloodFill
        private List<PCGHex> _playableArea = new List<PCGHex>();
        private int _requiredSizeInPercentage = 50;
        private Vector2 _offset = new Vector2(0f, 0f);

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the starting grid.
        /// </summary>
        public void GenerateBasicGrid()
        {
            //clear previous grid
            ClearGrid();

            //Create Grid
            //_hexPreviewPositions = GetComponent<GridGenerationHelper>().CalculateHexPositions(GenerationSettings.Dimensions);
            _hexPositions = GetComponent<GridGenerationHelper>().CalculateHexPositionsFix(dimensions);

            SpawnHexPreview();
            SetHexNeighbours();

        }

        /// <summary>
        /// Resets/Clears the generated grid.
        /// </summary>
        public void ClearGrid()
        {
            int count = transform.childCount;
            for (int i = count; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(i - 1).gameObject);
            }
            _pcgHexDictionary.Clear();
            HexGenerationArea.HexPreviewPositions.Clear();
        }

        /// <summary>
        /// Spawns pcgHexes for editor preview
        /// </summary>
        private void SpawnHexPreview()
        {
            _pcgHexPrefab = (GameObject)Resources.Load("PCG/PCGHex");

            //can this be done at runtime?
            foreach (var data in _hexPositions)
            {
                GameObject hexObject = Instantiate(_pcgHexPrefab);
                hexObject.transform.SetParent(transform);
                hexObject.transform.position = new Vector3(data.Value.x, data.Value.y + transform.position.y, data.Value.z);
                PCGHex pcgHex = hexObject.GetComponent<PCGHex>();


                pcgHex.HexCoord = data.Key;
                pcgHex.WorldCoord = data.Value;

                _pcgHexDictionary.Add(data.Key, pcgHex);
            }
        }

        /// <summary>
        /// Sets the neighbours for each pcgHex.
        /// </summary>
        private void SetHexNeighbours()
        {
            foreach (KeyValuePair<Vector3, PCGHex> keyPair in _pcgHexDictionary)
            {
                //current coord
                Vector3 coord = keyPair.Value.HexCoord;

                //offset Correction
                int rightOffset = Math.Abs((int)coord.y % 2);
                int leftOffset = 0;
                if (rightOffset == 0) { leftOffset = 1; }
                else if (rightOffset == 1) { leftOffset = 0; }

                //create neighbour coords
                List<Vector3> neighbourHexes = new List<Vector3>();
                neighbourHexes.Add(new Vector3(coord.x - 1, coord.y, coord.z));                 //right
                neighbourHexes.Add(new Vector3(coord.x + 1, coord.y, coord.z));                 //left
                neighbourHexes.Add(new Vector3(coord.x + rightOffset, coord.y + 1, coord.z));   //rightTop
                neighbourHexes.Add(new Vector3(coord.x + rightOffset, coord.y - 1, coord.z));   //rightBottom
                neighbourHexes.Add(new Vector3(coord.x - leftOffset, coord.y + 1, coord.z));    //leftTop
                neighbourHexes.Add(new Vector3(coord.x - leftOffset, coord.y - 1, coord.z));    //leftBottom

                //check neighbours
                for (int i = 0; i < neighbourHexes.Count; i++)
                {
                    if (_pcgHexDictionary.ContainsKey(neighbourHexes[i]))
                    {
                        keyPair.Value.AddNeighbour(_pcgHexDictionary[neighbourHexes[i]]);
                    }
                }
            }
        }

        /// <summary>
        /// Check if neighbour exists at given position
        /// </summary>
        /// <param name="coordToCheck">The position of a grid-cel to check</param>
        /// <returns></returns>
        private bool CheckIfNeighbourExists(Vector3 coordToCheck)
        {
            bool doesExists = false;

            if (_pcgHexDictionary.ContainsKey(coordToCheck))
            {
                doesExists = true;
            }

            return doesExists;
        }

        #endregion

        #region InspectorButtons

        /// <summary>
        /// Generates the working grid. Has walkable and unwalkable tiles.
        /// </summary>
        /// <param name="generator">generator type</param>
        public void GenerateWorkingGrid(ITileMapGenerator generator)
        {
            if (debugMode == false)
            {
                generator.GenerateTileMap(_pcgHexDictionary, GenerationSettings, Seed, _offset);
                ValidatePlayableLevel(generator);
            }
            else
            {
                GenerateDebugHexGrid();
            }

        }

        /// <summary>
        /// Initiate object spawning system
        /// </summary>
        public void PlaceObjects()
        {
            GetComponent<PatternRecognition>().RunObjectPlacementAlgorithm(_pcgHexDictionary, _pcgTheme, Seed);
        }

        #endregion

        //public List<Hex> PCGHexToHex()
        //{
        //    List<Hex> hexList = new List<Hex>();

        //    foreach (KeyValuePair<Vector3, PCGHex> keyPair in _pcgHexDictionary)
        //    {
        //        PCGHex pcgHex = keyPair.Value;

        //        Hex newHex = new Hex();
        //        newHex.Coord = pcgHex.HexCoord;
        //        newHex.WorldCoord = pcgHex.WorldCoord;

        //        if (pcgHex.IsWalkable)
        //        {
        //            newHex.State = HexState.Walkable;
        //        }
        //        else
        //        {
        //            newHex.State = HexState.NotWalkable;
        //        }
        //        //newHex.MetaData = new HexMetaData();

        //        hexList.Add(newHex);
        //    }

        //    return hexList;
        //}

        /// <summary>
        /// Applies a mask to the hexgrid
        /// </summary>
        public void ApplyMask()
        {
            GetComponent<GridMaskOverlay>().ApplyMask(_pcgHexDictionary);
        }

        /// <summary>
        /// Checks is a level is playable and saves the playable area.
        /// </summary>
        public void ValidatePlayableLevel(ITileMapGenerator generator)
        {
            //DEBUG color 
            Material markMaterial = (Material)Resources.Load("PCG/mat");
            bool mapIsplayable = false;
            _playableArea = new List<PCGHex>();

            foreach (var item in _pcgHexDictionary)
            {
                PCGHex currentHex = item.Value;
                if (currentHex.IsWalkable && currentHex.IsFilled == false)
                {

                    Floodfill(currentHex, markMaterial);

                    float islandPercentage = (float)_playableArea.Count / (float)_pcgHexDictionary.Count * 100f;
                    if ((int)islandPercentage > _requiredSizeInPercentage && (int)islandPercentage < 68)
                    {
                        mapIsplayable = true;
                        break;
                    }
                    else
                    {
                        mapIsplayable = false;
                        _playableArea = new List<PCGHex>();
                    }
                }
            }

            if (mapIsplayable)
            {
                _offset = new Vector2(0f, 0f);
                //Continue
                GetComponent<CharacterPlacementHandler>().PlaceGamePlayElements(_playableArea, _pcgHexDictionary, Seed);
                PlaceObjects();
                HideUnwalkableHexes();
            }
            else
            {
                _offset.x += 50f;
                GenerateBasicGrid();
                GenerateWorkingGrid(generator);
            }
        }

        /// <summary>
        /// Recursiv algorithm to detect islands
        /// </summary>
        /// <param name="currentHex">Hex to check</param>
        /// <param name="mat">Debug material for in the editor</param>
        public void Floodfill(PCGHex currentHex, Material mat)
        {
            if (currentHex.IsWalkable == false) { return; }
            if (currentHex.IsFilled) { return; }

            currentHex.IsFilled = true;
            _playableArea.Add(currentHex);
            //DEBUG
            //currentHex.GetComponent<Renderer>().sharedMaterial = mat;

            for (int i = 0; i < currentHex.NeighbourCount; i++)
            {
                if (currentHex.GetNeighbourAtIndex(i).IsWalkable)
                {
                    Floodfill(currentHex.GetNeighbourAtIndex(i), mat);
                }
            }
        }

        private void HideUnwalkableHexes()
        {
            foreach (var item in _pcgHexDictionary)
            {
                item.Value.Hide();
            }
        }

        public void GenerateDebugHexGrid()
        {
            foreach (var item in _pcgHexDictionary)
            {
                item.Value.IsWalkable = false;
                item.Value.UpdateHexColor();
            }

            PlaceObjects();
        }
    }
}