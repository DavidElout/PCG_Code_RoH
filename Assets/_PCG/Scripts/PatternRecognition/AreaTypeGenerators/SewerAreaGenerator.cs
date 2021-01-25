using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class SewerAreaGenerator : MonoBehaviour, IAreaTypeGenerator
    {
        public Type AreaType => typeof(SewerAreaGenerator);

        private GameObject _straightPiece;
        private GameObject _cornerPiece;
        private GameObject _holePiece;


        void Awake()
        {
            LoadSewerAssets();
        }

        #region Methods

        public void ProcessArea(Dictionary<Vector3, PCGHex> area)
        {
            LoadSewerAssets();



        }

        private void LoadSewerAssets()
        {
            _straightPiece = (GameObject)Resources.Load("PCG/PCGAreaAssets/Sewer/Sewer_Straight_Piece");
            _cornerPiece = (GameObject)Resources.Load("PCG/PCGAreaAssets/Sewer/Sewer_Corner_Piece");
            _holePiece = (GameObject)Resources.Load("PCG/PCGAreaAssets/Sewer/Sewer_Hole_Piece");

            Debug.Log(_straightPiece);
            Debug.Log(_cornerPiece);
            Debug.Log(_holePiece);
        }

        #endregion

    }
}