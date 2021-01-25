using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hex used for the hexfield in the PCGObject generator (editor)window
/// </summary>
namespace PCG
{
    public class HexObject
    {
        #region Fields

        private Vector2 _coord;
        private bool _isClicked;
        private HexObjectType _hexObjectType;

        #endregion

        #region Constructor

        public HexObject(Vector2 coord, bool isClicked)
        {
            this._coord = coord;
            this._isClicked = isClicked;
        }

        public HexObject(Vector2 coord, bool isClicked, HexObjectType objectType)
        {
            this._coord = coord;
            this._isClicked = isClicked;
            this._hexObjectType = objectType;
        }

        #endregion

        #region Properties

        public Vector2 Coord { get => _coord;  }
        public bool IsClicked { get { return _isClicked; }  set { _isClicked = value; } }
        public HexObjectType HexObjectType { get => _hexObjectType; set => _hexObjectType = value; }

        #endregion
    }
}