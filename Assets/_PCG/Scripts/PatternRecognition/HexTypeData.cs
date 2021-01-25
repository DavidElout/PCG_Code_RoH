using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    [System.Serializable]
    public class HexTypeData
    {
        [SerializeField]
        private HexObjectType _hexObjectType;
        public List<Vector3> EditorCoords = new List<Vector3>();
        public List<Vector3> OffsetCoords = new List<Vector3>();

        public HexTypeData(HexObjectType type)
        {
            _hexObjectType = type;
        }

        public HexObjectType HexObjectType { get => _hexObjectType; }
    }
}