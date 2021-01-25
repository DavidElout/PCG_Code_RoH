using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PCG version of the regular hex used in the game
///This will be translated later.
/// </summary>
namespace PCG {
    public class PCGHex : MonoBehaviour
    {
        #region Editor Fields

        [SerializeField]
        private Vector3 _hexCoord;                              //Position in the grid
        [SerializeField]
        private Vector3 _worldCoord;                            //Position in world space
        [SerializeField]
        private bool _isWalkable;                               //Walkable state
        [SerializeField]
        private int _neighbourCount;                            //Amount of neighbours
        [SerializeField]
        private Material _walkable;                             //Editor: material for walkable tiles
        [SerializeField]
        private Material _nonWalkable;                          //Editor: material for non-walkable tiles
        [SerializeField]
        private List<PCGHex> _neighbours = new List<PCGHex>();  //Refernece to all neighbours
        [SerializeField]
        private MaskType _areaType;

        #endregion

        #region Fields

        private GameObject _hex;
        public bool IsFilled = false;
        private bool _isWalkableNew;

        #endregion

        #region Properties

        public bool IsWalkable { get { return _isWalkable; } set { _isWalkable = value; } }
        public bool IsWalkableNew { get { return _isWalkableNew; } set { _isWalkableNew = value; } }
        public int NeighbourCount { get { return _neighbourCount; } set { _neighbourCount = value; } }
        public Vector3 HexCoord { get { return _hexCoord; } set { _hexCoord = value; } }
        public Vector3 WorldCoord { get => _worldCoord; set => _worldCoord = value; }
        public MaskType AreaType { get => _areaType; set => _areaType = value; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the color in the editor based on walkable state
        /// </summary>
        public void UpdateHexColor(Material mat = null)
        {
            //Debug purposes
            if (mat != null)
            {
                GetComponent<Renderer>().sharedMaterial = mat;
            }
            //Standard Color Update
            else if (_isWalkable)
            {
                GetComponent<Renderer>().sharedMaterial = _walkable;
            }
            else
            {
                GetComponent<Renderer>().sharedMaterial = _nonWalkable;
            }
        }
        public void Hide()
        {
            //if (!IsWalkable)
            //{
            //    gameObject.SetActive(false);
            //}
        }

        #endregion

        #region Cellular Methods

        /// <summary>
        /// Adds a pcgHex/neighbour to the neighbour list
        /// </summary>
        /// <param name="pcgHex">pcgHex to add</param>
        public void AddNeighbour(PCGHex pcgHex)
        {
            _neighbours.Add(pcgHex);
            _neighbourCount++;
        }

        /// <summary>
        /// Removes a pcgHex/neighbour from the list
        /// </summary>
        /// <param name="pcgHex">pcgHex to remove</param>
        public void RemoveNeighbour(PCGHex pcgHex)
        {
            if (_neighbours.Contains(pcgHex))
            {
                _neighbours.Remove(pcgHex);
                _neighbourCount--;
            }
        }

        /// <summary>
        /// Get pcgHex/neighbour
        /// </summary>
        /// <param name="index"></param>
        /// <returns>pcgHex at index</returns>
        public PCGHex GetNeighbourAtIndex(int index)
        {
            return _neighbours[index];
        }

        /// <summary>
        /// Get the number of neighbours that are currently in the walkable state.
        /// </summary>
        /// <returns>Number of walkable neighbours/pcgHexes.</returns>
        public int GetActiveNeighboursCount()
        {
            int activeNeighbours = 0;
            for (int i = 0; i < _neighbours.Count; i++)
            {
                if (_neighbours[i].IsWalkable)
                {
                    activeNeighbours++;
                }
            }
            return activeNeighbours;
        }

        #endregion
    }
}