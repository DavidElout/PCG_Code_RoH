using UnityEngine;

namespace PCG
{
    [CreateAssetMenu(fileName = "", menuName = "PCG/Generation Settings/CellularSettings")]
    [System.Serializable]
    public class CellularSettings : GenerationSettings
    {
        #region Fields

        //[SerializeField]
        //private int _seed;                      //PRNG value.
        [SerializeField]
        private float _changeToStartWalkable = 40;   //Density of initial grid.
        [SerializeField]
        private int _starvationLimit = 2;           //The lower neighbour limit at which cells start dying.
        [SerializeField]
        private int _birthNumber = 3;               //The number of neighbours that cause a dead cell to become alive.
        [SerializeField]
        private int _numberOfSteps = 2;             //The number of times we perform the simulation step.

        #endregion

        #region Properties

        //public int Seed { get => _seed; }
        public float ChangeToStartWalkable { get => _changeToStartWalkable; }
        public int StarvationLimit { get => _starvationLimit; }
        public int BirthNumber { get => _birthNumber; }
        public int NumberOfIterations { get => _numberOfSteps; }

        #endregion

        //#region Constructor

        ///// <summary>
        ///// Contains all settings regarding cellular generation
        ///// </summary>
        ///// /// <param name="seed">PRNG value.</param>
        ///// <param name="changeToStartWalkable">Density of initial grid.</param>
        ///// <param name="starvationLimit">The lower neighbour limit at which cells start dying.</param>
        ///// <param name="overpopLimit">The upper neighbour limit at which cells start dying.</param>
        ///// <param name="birthNumber">The number of neighbours that cause a dead cell to become alive.</param>
        ///// <param name="numberOfSteps">The number of times we perform the simulation step.</param>
        //public CellularSettings(int seed, float changeToStartWalkable, int starvationLimit, int birthNumber, int numberOfSteps)
        //{
        //    _seed = seed;
        //    _changeToStartWalkable = changeToStartWalkable;
        //    _starvationLimit = starvationLimit;
        //    _birthNumber = birthNumber;
        //    _numberOfSteps = numberOfSteps;
        //}

        //#endregion
    }
}