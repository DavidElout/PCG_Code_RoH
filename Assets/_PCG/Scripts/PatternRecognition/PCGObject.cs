using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// COntains all information about the PCGObjects.
/// These Objects are Assets that are saved within the game.
/// PCGObjects are used to spawn assets during the PCG for daily missions.
/// </summary>
namespace PCG
{
    public class PCGObject : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private string _assetName;
        [SerializeField]
        private GameObject _asset;
        [SerializeField]
        private PCGObject[] _associatedAssets;
        [SerializeField]
        private PCGObject[] _areaAssets;
        [SerializeField]
        private PCGTheme _pcgTheme;
        [SerializeField]
        private BuildingType _buildingType;
        [SerializeField]
        private HexData _hexData;
        [SerializeField]
        private bool _canRotate;

        #endregion

        #region Properties

        public string AssetName { get => _assetName; }
        public GameObject Asset { get => _asset; }
        public PCGTheme PcgTheme { get => _pcgTheme; }
        public BuildingType BuildingType { get => _buildingType; }
        public PCGObject[] AssociatedAssets { get => _associatedAssets; }
        public HexData HexData { get => _hexData; }
        public PCGObject[] AreaAssets { get => _areaAssets; }
        public bool CanRotate { get => _canRotate; }

        #endregion


        #region Methods

        /// <summary>
        /// Initialize all PCGObject settins. Due to creating an Asset in the AssetDatabase
        /// </summary>
        /// <param name="assetName">Name</param>
        /// <param name="asset">GameObject to spawn</param>
        /// <param name="mainPattern">HexPattern for the main building</param>
        /// <param name="optionalPattern">Multiple Single hexes used a starting Position for spawing assosiated assets</param>
        /// <param name="associatedAssets">List of PCGObects that can be spawned on the optional pattern</param>
        /// <param name="pcgTheme">Theme the asset belongs to.</param>
        /// <param name="buildingType">Building type the asset belongs to.</param>
        public void Init(string assetName, GameObject asset, PCGObject[] associatedAssets, PCGObject[] areaAssets, PCGTheme pcgTheme, BuildingType buildingType, HexData hexData, bool canRotate)
        {
            _assetName = assetName;
            _asset = asset;
            _associatedAssets = associatedAssets;
            _areaAssets = areaAssets;
            _pcgTheme = pcgTheme;
            _buildingType = buildingType;
            _hexData = hexData;
            _canRotate = canRotate;
        }

        /// <summary>
        /// Spawn the main _asset.
        /// </summary>
        /// <param name="hexFieldWorldPositions">List of world Coordinates</param>
        /// <param name="parent">Transform to instantiate to.</param>
        public void SpawnBuilding(List<Vector3> hexFieldWorldPositions, Transform parent)
        {
            Vector3 centerPosition = CalculateCenter(hexFieldWorldPositions);
            
            if (_asset != null)
            {
                GameObject obj = Instantiate(_asset, centerPosition, Quaternion.identity);
                obj.transform.SetParent(parent);
                if (_canRotate)
                {
                    float rotationValue = Random.Range(0, 360);
                    Quaternion target = Quaternion.Euler(0f, rotationValue, 0f);
                    obj.transform.rotation = target;
                }
            }
            else
            {
                Debug.LogError(_assetName + " does not have an Asset to spawn. Assign an asset first!");
            }

           
        }

        /// <summary>
        /// Spawns a _associated asset
        /// </summary>
        /// <param name="hexFieldWorldPositions">List of world positions</param>
        /// <param name="parent">Tranform to instantiate to.</param>
        /// <param name="index">Index number of the _associatedAssets to spawn.</param>
        public void SpawnAssociatedBuilding(List<Vector3> hexFieldWorldPositions, Transform parent, int index)
        {
            Vector3 centerPosition = CalculateCenter(hexFieldWorldPositions);

            if (_associatedAssets != null)
            {
                GameObject objectToSpawn = _associatedAssets[index].Asset;
                GameObject obj = Instantiate(objectToSpawn, centerPosition, Quaternion.identity);
                obj.transform.SetParent(parent);
            }
            else
            {
                Debug.LogError(_assetName + " does not have an Asset to spawn. Assign an asset first!");
            }
        }

        public void SpawnAreaAsset(List<Vector3> hexFieldWorldPositions, Transform parent, int index)
        {
            Vector3 centerPosition = CalculateCenter(hexFieldWorldPositions);

            if (_areaAssets != null)
            {
                GameObject objectToSpawn = _areaAssets[index].Asset;
                GameObject obj = Instantiate(objectToSpawn, centerPosition, Quaternion.identity);
                obj.transform.SetParent(parent);
            }
            else
            {
                Debug.LogError(_assetName + " does not have an Asset to spawn. Assign an asset first!");
            }
        }


        /// <summary>
        /// Calculates the center of the hexField, to spawn the object on.
        /// </summary>
        /// <param name="hexField">list of world coords of the given hexfield</param>
        /// <returns></returns>
        private Vector3 CalculateCenter(List<Vector3> hexField)
        {
            Vector3 center = new Vector3();

            for (int i = 0; i < hexField.Count; i++)
            {
                center += hexField[i];
            }
            center /= hexField.Count;

            return center;
        }

        #endregion
    }
}