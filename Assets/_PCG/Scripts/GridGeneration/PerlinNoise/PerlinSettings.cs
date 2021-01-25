using UnityEngine;

namespace PCG
{
    [CreateAssetMenu(fileName = "", menuName = "PCG/Generation Settings/PerlinSettings")]
    [System.Serializable]
    public class PerlinSettings : GenerationSettings
    {
        #region Fields

        //[SerializeField]
        //private int _seed;                      //PRNG value.
        [SerializeField]
        private float _scale = 8f;           //Scope of the grid 
        [SerializeField]
        [Range(1f, 2f)]
        private int _octaves = 2;           //Amount of layers
        [SerializeField]
        [Range(0f, 1f)]
        private float _persistance = 0.25f;     //A multiplier that determines the amplitudes diminish for each octave
        [SerializeField]
        [Range(-10f, 10f)]
        private float _lacunarity = 1.85f;      //A multiplier that determines the frequency diminish for each octave

        #endregion

        #region Properties

        //public int Seed { get => _seed; }
        public float Scale { get => _scale; }
        public int Octaves { get => _octaves; }
        public float Persistance { get => _persistance; }
        public float Lacunarity { get => _lacunarity; }

        #endregion

        //#region Constructor

        ///// <summary>
        ///// Contains all settings regarding perlin generation
        ///// </summary>
        ///// <param name="seed">PRNG value</param>
        ///// <param name="scale">Scope of the grid </param>
        ///// <param name="offset">Perlin grid offset</param>
        ///// <param name="octaves">Amount of layers</param>
        ///// <param name="persistance">A multiplier that determines the amplitudes diminish for each octave</param>
        ///// <param name="lacunarity">A multiplier that determines the frequency diminish for each octave</param>
        //public PerlinSettings(int seed, float scale, int octaves, float persistance, float lacunarity)
        //{
        //    _seed = seed;
        //    _scale = scale;
        //    _octaves = octaves;
        //    _persistance = persistance;
        //    _lacunarity = lacunarity;
        //}

        //#endregion
    }
}