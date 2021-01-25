using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    [CreateAssetMenu(fileName = "", menuName = "PCG/GameplayElements/PlayerProfile")]
    public class PlayerCharacterProfile : ScriptableObject
    {
        [Header("Characters selection for the level")]
        public List<GameObject> Characters = new List<GameObject>();
    }
}
