using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    [CreateAssetMenu(fileName = "", menuName = "PCG/GameplayElements/EnemyProfile")]
    public class EnemyProfile : ScriptableObject
    {
        [Header("Enemy selection for the level")]
        public List<GameObject> Characters = new List<GameObject>();
    }
}