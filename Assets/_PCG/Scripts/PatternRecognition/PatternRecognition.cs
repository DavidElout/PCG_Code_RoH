using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class PatternRecognition : MonoBehaviour
    {
        #region Editor Fields

        private int _seed;
        [SerializeField]
        private int _amountOfPrimary;
        [SerializeField]
        private int _amountOfSecundary;
        [SerializeField]
        private int _amountOfDecoration;
        [SerializeField]
        private int _amountOfObstacle;
        [SerializeField]
        private int _amountOfNature;

        //Editor Debug
        [SerializeField]
        List<Material> mat = new List<Material>();

        #endregion

        #region Fields

        private Dictionary<Vector3, PCGHex> _pcgHexDictionaryCopy;
        Dictionary<BuildingType, List<PCGObject>> _objectsPerBuildingType;

        //Generic data
        private List<int> buildingCountOfType = new List<int>();
        private List<Vector3> _excludedHexes = new List<Vector3>();

        #endregion

        #region Main Algorithm Methods

        /// <summary>
        /// Start the object placement sequence by running pattern recognition
        /// </summary>
        /// <param name="pcgHexDictionary">List of hexes in the HexField/HexField to run the pattern against.</param>
        /// <param name="theme">The theme of the objects that are to be placed.</param>
        public void RunObjectPlacementAlgorithm(Dictionary<Vector3, PCGHex> pcgHexDictionary, PCGTheme theme, int seed)
        {
            _pcgHexDictionaryCopy = DictionaryInsideOut(pcgHexDictionary);
            _seed = seed;

            System.Random prng = new System.Random(_seed);

            LoadGenericData();
            LoadAllPCGObjectsFromResources(theme);

            //All regular building types
            for (int i = 0; i < (int)BuildingType.Props; i++)
            {
                List<PCGObject> currentObjectList = LoadBuildingsOfType((BuildingType)i);

                for (int j = 0; j < buildingCountOfType[i]; j++)
                {
                    if (currentObjectList != null)
                    {
                        //select random item from PCGObjectList
                        PCGObject selectedObject = currentObjectList[prng.Next(0, currentObjectList.Count)];

                        //Run pattern Recognition + Spawn
                        TrySpawnPCGObjects(selectedObject);
                    }
                }
            }

            //Areas
            List<PCGObject> areaObjectList = LoadBuildingsOfType(BuildingType.Area);
            if (areaObjectList != null)
            {
                PCGObject areaObject = areaObjectList[Random.Range(0, areaObjectList.Count)];
                if (areaObject != null)
                {
                    //TrySpawnAreaObject(areaObject);
                }
            }

            //Players

            //Enemies

            //Chests

        }

        public void Spawngameplay(List<PCGHex> playableArea)
        {
            //Players

            //Enemies

            //Chests

        }


        //THIS IS UGLY AF AND NEEDS REFACTOR
        private void TrySpawnAreaObject(PCGObject areaObject)
        {
            List<Vector3> hexCoords = new List<Vector3>();
            List<Vector3> offsetCoords = areaObject.HexData.HexTypeDataList[(int)HexObjectType.Area].OffsetCoords;

            foreach (KeyValuePair<Vector3, PCGHex> keypair in _pcgHexDictionaryCopy)
            {
                PCGHex currentHex = keypair.Value;
                Vector3 startCoord = currentHex.HexCoord;

                hexCoords = GenerateHexCoordList(startCoord, offsetCoords);

                bool isValid = ValidateAreaLocation(hexCoords);

                if (isValid)
                {
                    for (int i = 0; i < hexCoords.Count; i++)
                    {
                        int index = Random.Range(0, areaObject.AreaAssets.Length);

                        List<Vector3> spawnCoords = new List<Vector3>();
                        spawnCoords.Add(hexCoords[i]);
                        areaObject.SpawnAreaAsset(HexToWorldCoord(spawnCoords), transform, index);
                        MarkPattern(hexCoords, 3);
                    }

                    AddToExcludedHexes(hexCoords);
                    //if we find valid position continue, otherwise keep looping
                    break;
                }
            }
        }

        /// <summary>
        /// Looks for a valid position for the given PCGObject on the hexField/hexGrid.
        /// When a valid position is found it will spawn the building attached to the given pcgObject
        /// </summary>
        /// <param name="pcgObject">pcgObject used as input for the pattern recognition algorithm</param>
        private void TrySpawnPCGObjects(PCGObject pcgObject)
        {
            List<Vector3> offsetCoords = pcgObject.HexData.HexTypeDataList[(int)HexObjectType.Main].OffsetCoords;

            foreach (KeyValuePair<Vector3, PCGHex> keypair in _pcgHexDictionaryCopy)
            {
                PCGHex currentHex = keypair.Value;
                Vector3 startCoord = currentHex.HexCoord;
                List<Vector3> hexCoords = GenerateHexCoordList(startCoord, offsetCoords);

                bool isValid = ValidatePatternLocation(hexCoords);

                if (isValid)
                {
                    pcgObject.SpawnBuilding(HexToWorldCoord(hexCoords), transform);
                    AddToExcludedHexes(hexCoords);
                    MarkPattern(hexCoords, 0); //DEBUG

                    //Unusable tiles
                    ExcludeUnusableHexes(pcgObject, currentHex);

                    //VegetationTiles

                    List<Vector3> optionalOffsetCoords = pcgObject.HexData.HexTypeDataList[(int)HexObjectType.Optional].OffsetCoords;

                    //Optional Tiles
                    for (int i = 0; i < pcgObject.HexData.HexTypeDataList[(int)HexObjectType.Optional].OffsetCoords.Count; i++)
                    {
                        if (optionalOffsetCoords.Count != 0)
                        {
                            if (pcgObject.AssociatedAssets.Length != 0)
                            {
                                //select random associated object
                                System.Random prng = new System.Random(_seed);
                                int index = prng.Next(0, pcgObject.AssociatedAssets.Length);
                                PCGObject optionalObject = pcgObject.AssociatedAssets[index];

                                //Coordoffsets of the optionalPattern are aso taken from the starting Hex
                                List<Vector3> optionalObjectOffsetCoords = optionalObject.HexData.HexTypeDataList[(int)HexObjectType.Main].OffsetCoords;
                                Vector3 startPosition = startCoord + optionalOffsetCoords[i];
                                //OffsetCorrection
                                if (Mathf.Abs(startPosition.y % 2) == 1)
                                {
                                    startPosition.x += 1f;
                                }

                                List<Vector3> optionalHexCoords = GenerateHexCoordList(startPosition, optionalObjectOffsetCoords);
                                bool isOptionalPatternValid = ValidatePatternLocation(optionalHexCoords);

                                if (isOptionalPatternValid)
                                {
                                    pcgObject.SpawnAssociatedBuilding(HexToWorldCoord(optionalHexCoords), transform, index);
                                    AddToExcludedHexes(optionalHexCoords);
                                    MarkPattern(optionalHexCoords, 2); //DEBUG
                                    ExcludeUnusableHexes(optionalObject, _pcgHexDictionaryCopy[startPosition]);
                                    //break;
                                }
                            }
                        }
                    }
                    return;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary >
        /// Generates a list of hex coordinates based on the starting position and the list of offset.
        /// </summary>
        /// <param name="startPosition">hex coordinate as startPoint</param>
        /// <param name="offsetPositions">list of offsets</param>
        /// <returns>List of hex coordinates</returns>m
        private List<Vector3> GenerateHexCoordList(Vector3 startPosition, List<Vector3> offsetPositions)
        {
            List<Vector3> coordList = new List<Vector3>();

            for (int i = 0; i < offsetPositions.Count; i++)
            {
                Vector3 coordToCheck = startPosition + offsetPositions[i];

                //Offset correction (because hex-grid)
                if (Mathf.Abs(startPosition.y % 2) == 0)
                {
                    if (Mathf.Abs(coordToCheck.y) % 2 == 1)
                    {
                        coordToCheck.x -= 1f;
                    }
                }

                coordList.Add(coordToCheck);
            }
            return coordList;
        }

        /// <summary>
        /// Validates a pattern(list of hex coordinates) by checking the states of each hex.
        /// </summary>
        /// <param name="hexPatternCoords">List of hex coordinates forming a hexPattern</param>
        /// <returns>true if the pattern is valid</returns>
        private bool ValidatePatternLocation(List<Vector3> hexPatternCoords)
        {
            bool isValid = true;

            //Loop all Coords
            for (int i = 0; i < hexPatternCoords.Count; i++)
            {
                Vector3 coordToCheck = hexPatternCoords[i];

                //If any of the hexes from the pattern do not exists, the pattern fails
                if (_pcgHexDictionaryCopy.ContainsKey(coordToCheck) == false)
                {
                    isValid = false;
                    break;
                }
                //If areatype is not default, the pattern fails
                if (_pcgHexDictionaryCopy[coordToCheck].AreaType != MaskType.Default)
                {
                    isValid = false;
                    break;
                }
                //If the hex being checked is walkable, the pattern fails
                if (_pcgHexDictionaryCopy[coordToCheck].IsWalkable)
                {
                    isValid = false;
                    break;
                }
                //If the hex to be checked is in the excluded list, the pattern fails
                if (_excludedHexes.Contains(coordToCheck))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        private bool ValidateAreaLocation(List<Vector3> hexPatternCoords)
        {
            bool isValid = true;

            //Loop all Coords
            for (int i = 0; i < hexPatternCoords.Count; i++)
            {
                Vector3 coordToCheck = hexPatternCoords[i];

                //If any of the hexes from the pattern do not exists, the pattern fails
                if (_pcgHexDictionaryCopy.ContainsKey(coordToCheck) == false)
                {
                    isValid = false;
                    break;
                }
                //If areatype is not default, the pattern fails
                if (_pcgHexDictionaryCopy[coordToCheck].AreaType != MaskType.Default)
                {
                    isValid = false;
                    break;
                }
                if (_pcgHexDictionaryCopy[coordToCheck].IsWalkable == false)
                {
                    isValid = false;
                    break;
                }
                //If the hex to be checked is in the excluded list, the pattern fails
                if (_excludedHexes.Contains(coordToCheck))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Loops the unusable tiles of the selected PCGObject and excludes these tiles from further iterations.
        /// </summary>
        /// <param name="pcgObject">selected PCGObject of which the unusableTileList will be used</param>
        /// <param name="currentHex">the starting hex from which the CoordList can be generated. </param>
        private void ExcludeUnusableHexes(PCGObject pcgObject, PCGHex currentHex)
        {
            //Unwalkable tiles 
            List<Vector3> unusableOffsetCoords = pcgObject.HexData.HexTypeDataList[(int)HexObjectType.Unusable].OffsetCoords;
            if (unusableOffsetCoords.Count != 0 || unusableOffsetCoords != null)
            {
                List<Vector3> unusableCoords = GenerateHexCoordList(currentHex.HexCoord, unusableOffsetCoords);
                //Excludes walkable tiles, because they are already unusable for spawning objects
                List<Vector3> fixedUnusableCoords = new List<Vector3>();
                for (int i = 0; i < unusableCoords.Count; i++)
                {
                    if (_pcgHexDictionaryCopy.ContainsKey(unusableCoords[i]))
                    {
                        if (_pcgHexDictionaryCopy[unusableCoords[i]].IsWalkable == false)
                        {
                            if (_excludedHexes.Contains(unusableCoords[i]) == false)
                            {
                                fixedUnusableCoords.Add(unusableCoords[i]);
                            }
                        }
                    }
                }
                AddToExcludedHexes(fixedUnusableCoords);
                MarkPattern(fixedUnusableCoords, 1); //DEBUG
            }
        }

        /// <summary>
        /// Loads / reloads all data that is required before starting the Object placement algorithm
        /// </summary>
        private void LoadGenericData()
        {
            _excludedHexes.Clear();
            _objectsPerBuildingType = new Dictionary<BuildingType, List<PCGObject>>();

            //temporary solution
            buildingCountOfType.Clear();
            buildingCountOfType.Add(_amountOfPrimary);
            buildingCountOfType.Add(_amountOfSecundary);
            buildingCountOfType.Add(_amountOfDecoration);
            buildingCountOfType.Add(_amountOfObstacle);
            buildingCountOfType.Add(_amountOfNature);
        }

        /// <summary>
        /// Loads all PCGObjects from the resource folder and stores them in an ordered Dictionary.
        /// </summary>
        /// <param name="theme">THeme of the PCGObjects to load</param>
        private void LoadAllPCGObjectsFromResources(PCGTheme theme)
        {
            string[] buildTypeNames = System.Enum.GetNames(typeof(BuildingType));
            string themePath = "PCG/PCGAssets/" + theme;

            //generate list of pcgObjects for each building type.
            for (int i = 0; i < buildTypeNames.Length; i++)
            {
                string buildingTypePath = themePath + "/" + buildTypeNames[i];
                List<PCGObject> pcgObjectList = new List<PCGObject>();
                Object[] loadedResources = Resources.LoadAll(buildingTypePath, typeof(PCGObject));

                //Convert to list
                for (int j = 0; j < loadedResources.Length; j++)
                {
                    pcgObjectList.Add((PCGObject)loadedResources[j]);
                }

                //Add to dictionary
                _objectsPerBuildingType.Add((BuildingType)i, pcgObjectList);
            }
        }

        /// <summary>
        /// Returns a List of PCGObjects from the ordered Dictionary based on BuildingType
        /// </summary>
        /// <param name="buildingType">The buildingtype to load objects from.</param>
        /// <returns>returns the List of PCGObjects of the given BuildingType</returns>
        private List<PCGObject> LoadBuildingsOfType(BuildingType buildingType)
        {
            List<PCGObject> newList = _objectsPerBuildingType[buildingType];

            if (newList.Count == 0)
            {
                return null;
            }

            return newList;
        }

        /// <summary>
        /// When a pattern is Valid and a building is spawned the hexPattern used for that object
        /// will become excluded in the next cycle of the recognition algorithm,
        /// because these hexes are occupied.
        /// </summary>
        /// <param name="coords">List of coordinates that are to be excluded.</param>
        private void AddToExcludedHexes(List<Vector3> coords)
        {
            _excludedHexes.AddRange(coords);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// re-orders a dictionary from the middle
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private Dictionary<Vector3, PCGHex> DictionaryInsideOut(Dictionary<Vector3, PCGHex> dictionary)
        {
            Dictionary<Vector3, PCGHex> newDictionary = new Dictionary<Vector3, PCGHex>();
            List<Vector3> list = new List<Vector3>();
            List<Vector3> newList = new List<Vector3>();

            foreach (var item in dictionary)
            {
                list.Add(item.Key);
            }

            int upCounter = list.Count / 2;
            int downCounter = upCounter - 1;

            while (downCounter >= 0)
            {
                newList.Add(list[downCounter--]);
                if (upCounter < list.Count) newList.Add(list[upCounter++]);
            }

            for (int k = 0; k < newList.Count; k++)
            {
                newDictionary.Add(newList[k], dictionary[newList[k]]);
            }

            return newDictionary;
        }

        /// <summary>
        /// Translates a list of hex coordinates to a list of world coordinates of those hexes.
        /// Translates a list of hex coordinates to a list of world coordinates of those hexes.
        /// </summary>
        /// <param name="hexCoords">list of hex coordinates.</param>
        /// <returns>List of world coordinates.</returns>
        private List<Vector3> HexToWorldCoord(List<Vector3> hexCoords)
        {
            List<Vector3> worldCoordList = new List<Vector3>();

            for (int i = 0; i < hexCoords.Count; i++)
            {
                worldCoordList.Add(_pcgHexDictionaryCopy[hexCoords[i]].WorldCoord);
            }

            return worldCoordList;
        }

        /// <summary>
        /// DEBUG hex color changer
        /// Changes the color of all excluded hexes (the hexes used by hexPatterns to spawn buildings)
        /// </summary>
        /// <param name="coords">coords of hexes to change color of</param>
        private void MarkPattern(List<Vector3> coords, int index)
        {
            for (int i = 0; i < coords.Count; i++)
            {
                _pcgHexDictionaryCopy[coords[i]].UpdateHexColor(mat[index]);
            }
        }

        #endregion
    }
}