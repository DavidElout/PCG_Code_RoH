using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PCG
{
    public class PCGObjectGeneratorEditor : EditorWindow
    {
        #region Fields

        //HexEditorMode
        private HexEditorMode _editorMode;
        private HexObjectType _hexObjectType = HexObjectType.Main;

        //HexField fields
        private int _gridSize = 10;
        private Texture2D _hexTexture;
        private Dictionary<Vector2, HexObject> _editorHexes = new Dictionary<Vector2, HexObject>();

        //PCGObject variables
        private string _pcgObjectName;
        private int _associatedAssetsCount = 0;
        private int _areaAssetsCount = 0;
        private int _gameplayElementCount = 0;
       
        private GameObject _asset;
        private GameObject[] _gameplatElementObjects = new GameObject[0];
        private HexData _hexData;
        private PCGObject[] _associatedAssets = new PCGObject[0];
        private PCGObject[] _areaAssets = new PCGObject[0];
        private PCGTheme _pcgTheme;
        private BuildingType _buildingType;
        private GameplayElementType _gameplayElementType;
        private bool _canRotate;

        //Loading/Saving 
        private const string PCGObjectAssetPath = "Assets/Resources/PCG/PCGAssets";
        private const string GameplayElementAssetPath = "Assets/Resources/PCG/PCGElements";
        private PCGObject _dataToLoad;

        #endregion

        #region Unity Methods

        [MenuItem("Rise Of Humanity/PCG/PCGObjectGeneratorEditor")]
        private static void Init()
        {
            PCGObjectGeneratorEditor window = GetWindow<PCGObjectGeneratorEditor>("PCGObjectGenerator");
            window.maxSize = new Vector2(750f, 1000f);
            window.minSize = new Vector2(530f, 700f);
        }

        private void OnEnable()
        {
            _hexTexture = (Texture2D)Resources.Load("PCG/EditorHex");
            RefreshHexField();
        }

        private void OnGUI()
        {
            HexEditorMode newMode = (HexEditorMode)EditorGUILayout.EnumPopup("Editor Mode", _editorMode);
            if(newMode != _editorMode)
            {
                //Make sure to refresh data when switching to avoid 'loose' data being saved
                RefreshHexField();
                _editorMode = newMode;
            }

            if(_editorMode == HexEditorMode.BuildingMode)
            {
                EditorGUILayout.LabelField("HexPattern Settings", EditorStyles.boldLabel);
                DrawRefreshButton();
                DrawHexButtons();
                DrawPCGObjectCreationSettings();
                DrawPCGObjectGenerationButton();
                DrawDataLoadingSection();
            }
            else
            {
                _hexObjectType = HexObjectType.Main;
                DrawRefreshButton();
                DrawHexButtons();
                DrawCharacterAreaCreationSettings();
            }

            
        }

        #endregion

        #region GUIMethods

        /// <summary>
        /// Draw a button to refresh the hexField in the window.
        /// </summary>
        private void DrawRefreshButton()
        {
            if (GUILayout.Button("RefreshHexField"))
            {
                RefreshHexField();
            }
            _hexObjectType = (HexObjectType)EditorGUILayout.EnumPopup("Draw Type", _hexObjectType);
        }

        /// <summary>
        /// OnButtonClick: 
        /// Draw the hexes as buttons to become a clickable hexField.
        /// Changes the state and color of the clicked hex.
        /// </summary>
        private void DrawHexButtons()
        {
            //Draw the hexes as buttons
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    float xPos = 48f * x;
                    float yPos = 42f * y;

                    //offset correction
                    if (y % 2 == 1)
                    {
                        xPos += 24;
                    }

                    Vector2 coord = new Vector2(x, y);
                    bool isClicked = _editorHexes[coord].IsClicked;

                    if (isClicked)
                    {
                        //check object type of current hex
                        GUI.color = GetColor(_editorHexes[coord].HexObjectType);
                    }
                    else
                    {
                        GUI.color = GetColor(HexObjectType.Default);
                    }
                    //DEBUG Mode
                    //GUI.Label(new Rect(xPos + 15, yPos + 100, 50, 50), "  " + x + "," + y + "\n" + _editorHexes[coord].IsClicked);
                    Rect hexButton = new Rect(xPos + 10, yPos + 100, 50, 50);
                    if (GUI.Button(hexButton, _hexTexture, GUIStyle.none))
                    {
                        if (isClicked)
                        {
                            isClicked = false;
                            _editorHexes[coord].HexObjectType = HexObjectType.Default;
                        }
                        else
                        {
                            isClicked = true;
                            _editorHexes[coord].HexObjectType = _hexObjectType;
                        }

                        _editorHexes[coord].IsClicked = isClicked;
                    }
                }
            }
        }

        /// <summary>
        /// Draw the setting fields for the PCGObject.
        /// </summary>
        private void DrawPCGObjectCreationSettings()
        {
            GUI.color = Color.white;

            EditorGUILayout.Space(450f);
            EditorGUILayout.LabelField("PCGObject Settings", EditorStyles.boldLabel);
            _pcgObjectName = EditorGUILayout.TextField("Pattern Name: ", _pcgObjectName);
            _pcgTheme = (PCGTheme)EditorGUILayout.EnumPopup("Pcg Theme", _pcgTheme);
            _buildingType = (BuildingType)EditorGUILayout.EnumPopup("Building Type", _buildingType);
            _asset = (GameObject)EditorGUILayout.ObjectField("GameObject: ", _asset, typeof(GameObject), false);
            _canRotate = EditorGUILayout.Toggle("Can Rotate", _canRotate);
            GUILayout.BeginHorizontal();
            _associatedAssetsCount = EditorGUILayout.IntField("Associated PCGObjects count: ", _associatedAssetsCount);
           

            if (GUILayout.Button("<- Update Counter"))
            {
                _associatedAssets = new PCGObject[_associatedAssetsCount];
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < _associatedAssets.Length; i++)
            {
                _associatedAssets[i] = (PCGObject)EditorGUILayout.ObjectField(("Associated PCGObject " + i + ": "), _associatedAssets[i], typeof(PCGObject), false);
            }

            GUILayout.BeginHorizontal();
            _areaAssetsCount = EditorGUILayout.IntField("Area PCGObjects count: ", _areaAssetsCount);

            if (GUILayout.Button("<- Update Counter"))
            {
                _areaAssets = new PCGObject[_areaAssetsCount];
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < _areaAssets.Length; i++)
            {
                _areaAssets[i] = (PCGObject)EditorGUILayout.ObjectField(("Area PCGObject " + i + ": "), _areaAssets[i], typeof(PCGObject), false);
            }
        }

        private void DrawCharacterAreaCreationSettings()
        {
            GUI.color = Color.white;

            EditorGUILayout.Space(500f);
            EditorGUILayout.LabelField("PCGElement Settings", EditorStyles.boldLabel);
            _pcgObjectName = EditorGUILayout.TextField("Pattern Name: ", _pcgObjectName);
            _gameplayElementType = (GameplayElementType)EditorGUILayout.EnumPopup("Gameplay Element", _gameplayElementType);
         
            //object(s)
            //GUILayout.BeginHorizontal();
            //_gameplayElementCount = EditorGUILayout.IntField(_gameplayElementType.ToString() + " count: ", _gameplayElementCount);
            //if (GUILayout.Button("<- Update Counter"))
            //{
            //    _gameplatElementObjects = new GameObject[_gameplayElementCount];
            //}
            //GUILayout.EndHorizontal();
            //if(_gameplatElementObjects.Length > 0)
            //{
            //    EditorGUILayout.LabelField(_gameplayElementType.ToString() + " Pool", EditorStyles.boldLabel);
            //}
            //for (int i = 0; i < _gameplatElementObjects.Length; i++)
            //{
            //    _gameplatElementObjects[i] = (GameObject)EditorGUILayout.ObjectField(_gameplayElementType.ToString() + " Element " + i + " : ", _gameplatElementObjects[i], typeof(GameObject), false);
            //}

            if (GUILayout.Button("Generate PCG Element", GUILayout.Height(50f)))
            {
                GeneratePCGElement();
            }
        }

        /// <summary>
        /// OnButtonClick: 
        /// CReates HexPatterns based on the coored hexes from the hexField.
        /// Takes all data from the PCGObject settings and creates a PCGObject
        /// </summary>
        private void DrawPCGObjectGenerationButton()
        {
            if (GUILayout.Button("Generate PCG Object", GUILayout.Height(50f)))
            {
                GeneratePCGObject();
            }
        }

        /// <summary>
        /// Draws all fields and buttons required for Loading and saving Data
        /// </summary>
        private void DrawDataLoadingSection()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Edit PCG Object", EditorStyles.boldLabel);
            _dataToLoad = (PCGObject)EditorGUILayout.ObjectField(("Data to load: "), _dataToLoad, typeof(PCGObject), false);

            if (GUILayout.Button("Load PCGObject"))
            {
                LoadData();
            }

            if (GUILayout.Button("Update PCG Object", GUILayout.Height(50f)))
            {
                UpdateData();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh the hex list with its given size
        /// </summary>
        private void RefreshHexField()
        {
            _hexData = new HexData();
            _hexData.Init();

            _editorHexes.Clear();
            for (int x = 0; x < _gridSize; x++)
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    HexObject obj = new HexObject(new Vector2(x, y), false);
                    _editorHexes.Add(new Vector2(x, y), obj);
                }
            }

            _pcgObjectName = string.Empty;
            _asset = null;
            _associatedAssetsCount = 0;
            _associatedAssets = new PCGObject[_associatedAssetsCount];
            _areaAssetsCount = 0;
            _areaAssets = new PCGObject[_areaAssetsCount];
        }

        /// <summary>
        /// Updates/Saves the loaded PCGObject data
        /// </summary>
        private void UpdateData()
        {
            GenerateHexData();
            _dataToLoad.Init(_pcgObjectName, _asset, _associatedAssets, _areaAssets, _pcgTheme, _buildingType, _hexData, _canRotate);
            _dataToLoad = null;
            RefreshHexField();
        }

        /// <summary>
        /// Loads the selected PCGObject data to modify
        /// </summary>
        private void LoadData()
        {
            RefreshHexField();
            _hexData = _dataToLoad.HexData;
            _pcgObjectName = _dataToLoad.AssetName;
            _asset = _dataToLoad.Asset;

            if (_dataToLoad.AssociatedAssets != null)
            {
                _associatedAssetsCount = _dataToLoad.AssociatedAssets.Length;
                _associatedAssets = new PCGObject[_associatedAssetsCount];
                for (int i = 0; i < _dataToLoad.AssociatedAssets.Length; i++)
                {
                    _associatedAssets[i] = _dataToLoad.AssociatedAssets[i];
                }
            }

            if (_dataToLoad.AreaAssets != null)
            {
                _areaAssetsCount = _dataToLoad.AreaAssets.Length;
                _areaAssets = new PCGObject[_areaAssetsCount];
                for (int i = 0; i < _dataToLoad.AreaAssets.Length; i++)
                {
                    _areaAssets[i] = _dataToLoad.AreaAssets[i];
                }
            }

            _pcgTheme = _dataToLoad.PcgTheme;
            _buildingType = _dataToLoad.BuildingType;

            //When adding new HexObjectTypes the index changes as this might cause error 'out of bounds' Because
            //previously generated PCGObjects do not have this list yet
            int size = _hexData.HexTypeDataList.Count;
            for (int i = 0; i < size; i++)
            {
                if (_hexData.HexTypeDataList[i].EditorCoords != null)
                {
                    for (int j = 0; j < _hexData.HexTypeDataList[i].EditorCoords.Count; j++)
                    {
                        Vector2 coord = _hexData.HexTypeDataList[i].EditorCoords[j];

                        _editorHexes[coord].IsClicked = true;
                        _editorHexes[coord].HexObjectType = (HexObjectType)i;
                    }
                }
            }
        }

        /// <summary>
        /// Generates a new PCGObject
        /// </summary>
        private void GeneratePCGObject()
        {
            GenerateHexData();
            PCGObject pcgObject = CreateInstance<PCGObject>();
            pcgObject.Init(_pcgObjectName, _asset, _associatedAssets, _areaAssets, _pcgTheme, _buildingType, _hexData, _canRotate);
            CreatePCGAsset(pcgObject);
            RefreshHexField();
        }

        /// <summary>
        /// Generates a new PCGObject
        /// </summary>
        private void GeneratePCGElement()
        {
            GenerateHexData();
            PCGElementObject pcgObject = CreateInstance<PCGElementObject>();
            pcgObject.Init(_pcgObjectName, _hexData, _gameplayElementType);
            CreatePCGElement(pcgObject);
            RefreshHexField();
        }

        /// <summary>
        /// Generates HexData for the pcgObject
        /// </summary>
        private void GenerateHexData()
        {
            Vector2 startCoord = new Vector2(-1000f, -1000f);

            for (int i = 0; i < _hexData.HexTypeDataList.Count; i++)
            {
                _hexData.Init();
            }

            //When of type(Area) the start position is measured in a different way.
            HexObjectType hexObjectType;
            if(_buildingType == BuildingType.Area)
            {
                hexObjectType = HexObjectType.Area;
            }
            else
            {
                hexObjectType = HexObjectType.Main;
            }

            foreach (KeyValuePair<Vector2, HexObject> keyPair in _editorHexes)
            {
                HexObject hex = keyPair.Value;

                if (hex.IsClicked)
                {
                    _hexData.HexTypeDataList[(int)hex.HexObjectType].EditorCoords.Add(hex.Coord);

                    //The first hex of type<Main> will serve as starting hex.
                    if (startCoord == new Vector2(-1000f, -1000f))
                    {
                        if (hex.HexObjectType == hexObjectType)
                        {
                            startCoord = hex.Coord;
                        }
                    }
                }
            }


            foreach (KeyValuePair<Vector2, HexObject> keyPair in _editorHexes)
            {
                HexObject hex = keyPair.Value;

                if (hex.IsClicked)
                {
                    _hexData.HexTypeDataList[(int)hex.HexObjectType].OffsetCoords.Add(CalculateOffsetCoord(startCoord, hex.Coord));
                }
            }
        }

        private void CreatePCGElement(PCGElementObject pcgObject)
        {
            if (AssetDatabase.IsValidFolder(GameplayElementAssetPath + "/" + _gameplayElementType.ToString()) == false)
            {
                AssetDatabase.CreateFolder(GameplayElementAssetPath, _gameplayElementType.ToString());
            }

            AssetDatabase.CreateAsset(pcgObject, GameplayElementAssetPath + "/" + _gameplayElementType + "/" + _pcgObjectName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = pcgObject;
        }

        /// <summary>
        /// Creates a menu Asset 
        /// </summary>
        /// <param name="pcgObject">PCGObject to create an menu-asset for</param>
        private void CreatePCGAsset(PCGObject pcgObject)
        {
            if (AssetDatabase.IsValidFolder(PCGObjectAssetPath + "/" + _pcgTheme.ToString()) == false)
            {
                AssetDatabase.CreateFolder(PCGObjectAssetPath, _pcgTheme.ToString());
            }
            if (AssetDatabase.IsValidFolder(PCGObjectAssetPath + "/" + _pcgTheme.ToString() + "/" + _buildingType.ToString()) == false)
            {
                AssetDatabase.CreateFolder(PCGObjectAssetPath + _pcgTheme.ToString(), _buildingType.ToString());
            }

            AssetDatabase.CreateAsset(pcgObject, PCGObjectAssetPath + "/" + _pcgTheme + "/" + _buildingType + "/" + _pcgObjectName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = pcgObject;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Selects the color based on HexObjectType
        /// When adding HexObjectTypes-manually add a color! This color only matters in the editor window
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private Color GetColor(HexObjectType objectType)
        {
            Color color = new Color();

            switch (objectType)
            {
                case HexObjectType.Main:
                    color = Color.cyan;
                    break;
                case HexObjectType.Optional:
                    color = Color.yellow;
                    break;
                case HexObjectType.Unusable:
                    color = Color.red;
                    break;
                case HexObjectType.Vegetation:
                    color = Color.green;
                    break;
                case HexObjectType.Area:
                    color = Color.gray;
                    break;
                case HexObjectType.Default:
                    color = Color.white;
                    break;
                default:
                    color = Color.white;
                    break;
            }
            return color;
        }

        /// <summary>
        /// Calculates the offset between Vector3 A and Vector3 B
        /// </summary>
        /// <param name="startPosition">Vector3 A</param>
        /// <param name="targetPosition">Vector3 B</param>
        /// <returns>Offset as Vector3</returns>
        private Vector3 CalculateOffsetCoord(Vector3 startPosition, Vector3 targetPosition)
        {
            Vector2 newCoord = targetPosition - startPosition;

            //offset Correction
            if (startPosition.y % 2 == 0)
            {
                if (Mathf.Abs(newCoord.y) % 2 == 1)
                {
                    newCoord.x += 1f;
                }
            }
            return newCoord;
        }

        #endregion
    }
}