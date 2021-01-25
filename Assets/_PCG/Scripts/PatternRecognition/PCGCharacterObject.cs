using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class PCGElementObject : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private string _assetName;
        //[SerializeField]
        //private GameObject[] _spawnableCharacters;
        [SerializeField]
        private HexData _hexData;
        [SerializeField]
        private GameplayElementType _gameplayElementType;

        #endregion

        #region Properties

        public string AssetName { get => _assetName; }
        //public GameObject[] SpawnableCharacters { get => _spawnableCharacters; }
        public HexData HexData { get => _hexData; set => _hexData = value; }
        public GameplayElementType GameplayElementType { get => _gameplayElementType; }

        #endregion

        #region Constructor

        public void Init(string assetName, HexData hexData, GameplayElementType type)
        {
            _assetName = assetName;
            //_spawnableCharacters = spawnableCharacters;
            _hexData = hexData;
            _gameplayElementType = type;

        }

        #endregion
    }
}