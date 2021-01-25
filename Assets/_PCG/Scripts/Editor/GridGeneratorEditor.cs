using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PCG
{
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : Editor
    {
        #region Fields

        private GridGenerator _hexGrid;
        private List<string> _generatorTypes = new List<string>();
        private List<ITileMapGenerator> _tileMapGenerators;
        private int _selectedParameterIndex = 1;

        #endregion

        #region Methods

        public void OnEnable()
        {
            _hexGrid = (GridGenerator)target;
            LoadGenerators();
        }

        /// <summary>
        /// Uses reflections to load all different kinds of generator types 
        /// </summary>
        private void LoadGenerators()
        {
            _tileMapGenerators = new List<ITileMapGenerator>();
            var type = typeof(ITileMapGenerator);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
            var typesArray = types.ToArray();

            for (int i = 0; i < typesArray.Length; i++)
            {
                ITileMapGenerator tileMapGenerator = (ITileMapGenerator)Activator.CreateInstance(typesArray[i]);
                _tileMapGenerators.Add(tileMapGenerator);
                _generatorTypes.Add(typesArray[i].Name);
            }
        }

        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector())
            {
                if (_hexGrid.AutoUpdate)
                {
                    //AutoUpdate();
                    //QuickGenerateWithMask();
                    _hexGrid.GenerateBasicGrid();
                    _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);
                }
            }

            //int newIndex = EditorGUILayout.Popup("generation Type: ", _selectedParameterIndex, _generatorTypes.ToArray());
            //if (newIndex != _selectedParameterIndex)
            //{
            //    _selectedParameterIndex = newIndex;
            //    _hexGrid.GenerationSettings = (GenerationSettings)Activator.CreateInstance(_tileMapGenerators[_selectedParameterIndex].SettingsType);
            //}

            if(_hexGrid.GenerationSettings is CellularSettings)
            {
                _selectedParameterIndex = 0;
            }else if (_hexGrid.GenerationSettings is PerlinSettings)
            {
                _selectedParameterIndex = 1;
            }


            //Draw all GUI buttons

            if (GUILayout.Button("Generate Main Grid"))
            {
                _hexGrid.GenerateBasicGrid();
            }

            if (GUILayout.Button("Generate Working Grid"))
            {
                _hexGrid.GenerateBasicGrid();
                _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);
            }

            //if (GUILayout.Button("Generate with Algorithm"))
            //{
            //    _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);

            //}

            //GUILayout.Label("Object Placement Oparator");
            //if (GUILayout.Button("Place Objects"))
            //{
            //    _hexGrid.PlaceObjects();
            //}

            GUILayout.Space(10f);
            GUILayout.Label("Shortcuts", EditorStyles.boldLabel);
            if (GUILayout.Button("Random-Level"))
            {
                //_hexGrid.GenerationSettings.SetSeed(UnityEngine.Random.Range(0, 10000));
                QuickGenerate();
            }

            //if (GUILayout.Button("Random-Assets"))
            //{
            //    QuickGenerate();
            //}

            //if (GUILayout.Button("Apply Mask"))
            //{
            //    _hexGrid.ApplyMask();
            //}

            //if (GUILayout.Button("Quick-Generate-With-Mask"))
            //{
            //    //_hexGrid.GenerationSettings.SetSeed(UnityEngine.Random.Range(0, 10000));
            //    QuickGenerateWithMask();
            //}

            //if (GUILayout.Button("Flood-Fill"))
            //{
            //   _hexGrid.ValidatePlayableLevel();
            //}

        }
        
        private void QuickGenerateWithMask()
        {
            _hexGrid.GenerateBasicGrid();
            _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);
            _hexGrid.ApplyMask();
            _hexGrid.PlaceObjects();
        }

        private void QuickGenerate()
        {
            _hexGrid.Seed = UnityEngine.Random.Range(0, 10000);
            _hexGrid.GenerateBasicGrid();
            _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);
        }


        private void AutoUpdate()
        {
            _hexGrid.ClearGrid();
            _hexGrid.GenerateBasicGrid();
            _hexGrid.GenerateWorkingGrid(_tileMapGenerators[_selectedParameterIndex]);

            
        }

        #endregion
    }
}