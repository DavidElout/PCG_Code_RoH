using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    #region Fields

    public class CharacterPlacementHandler : MonoBehaviour
    {
        private List<PlayerCharacterProfile> _playerCharacterProfiles;
        private List<EnemyProfile> _enemyProfiles;
        [SerializeField]
        private GameObject _chest;

        private List<PCGHex> _playableArea;
        private List<PCGHex> _excludedHexes = new List<PCGHex>();
        private List<PCGHex> _chestLocations = new List<PCGHex>();
        private System.Random prng;
        private PCGHex _playerHex;
        [SerializeField]
        private int _chestCount = 3;
        [SerializeField]
        private int _minDistancePlayerChest = 8;
        [SerializeField]
        private int _maxDistancePlayerChest = 100;
        [SerializeField]
        private int _minDistanceChestsEnemy = 6;
        [SerializeField]
        private int _maxDistanceChestsEnemy = 9;
        [SerializeField]
        private int _minDistanceChestChest = 6;

        #endregion

        #region Methods

        public void PlaceGamePlayElements(List<PCGHex> playableArea, Dictionary<Vector3, PCGHex> pcgHexDictionary, int seed)
        {
            prng = new System.Random(seed);
            _playableArea = playableArea;
            _excludedHexes = new List<PCGHex>();
            _chestLocations = new List<PCGHex>();
            LoadProfiles();
            PlacePlayerCharacters();
            PlaceChests();
            PlaceEnemyCharacters();
        }


        private void PlacePlayerCharacters()
        {
            Vector3 lowestValue = new Vector3(1000, 1000, 0); //Faulty starter value

            PCGHex targetHex = new PCGHex();

            for (int i = 0; i < _playableArea.Count; i++)
            {
                if (lowestValue == new Vector3(1000f, 1000f, 0f))
                {
                    lowestValue = _playableArea[i].HexCoord;
                    targetHex = _playableArea[i];
                }
                else if (_playableArea[i].HexCoord.x < lowestValue.x || _playableArea[i].HexCoord.y < lowestValue.y)
                {
                    lowestValue = _playableArea[i].HexCoord;
                    targetHex = _playableArea[i];
                }
            }


            //Random profile
            int index = prng.Next(0, _playerCharacterProfiles.Count);
            PlayerCharacterProfile profile = _playerCharacterProfiles[index];

            for (int i = 0; i < profile.Characters.Count; i++)
            {
                if (i == 0)
                {
                    GameObject character = (GameObject)Instantiate(profile.Characters[i], transform);
                    character.transform.position = targetHex.WorldCoord;
                    _playerHex = targetHex;
                }
                else
                {
                    List<PCGHex> neighbours = new List<PCGHex>();
                    for (int j = 0; j < targetHex.GetActiveNeighboursCount(); j++)
                    {
                        if (targetHex.GetNeighbourAtIndex(j).IsWalkable)
                        {
                            neighbours.Add(targetHex.GetNeighbourAtIndex(j));
                        }
                    }

                    int randomNeighbour = prng.Next(0, neighbours.Count);
                    if (neighbours[randomNeighbour] != targetHex)
                    {
                        targetHex = neighbours[randomNeighbour];
                    }

                    GameObject character = (GameObject)Instantiate(profile.Characters[i], transform);
                    character.transform.position = targetHex.WorldCoord;
                }
                _excludedHexes.Add(targetHex);
            }


        }

        private void PlaceChests()
        {
            for (int i = 0; i < _chestCount; i++)
            {
                PCGHex targetHex = SelectHexWithDistance(_playerHex, _minDistancePlayerChest, _maxDistancePlayerChest);

                GameObject character = (GameObject)Instantiate(_chest, transform);
                character.transform.position = targetHex.WorldCoord;

                _excludedHexes.Add(targetHex);
                _chestLocations.Add(targetHex);
            }
        }

        private bool IsToCloseToChest(PCGHex hex)
        {
            bool isToClose = false;

            for (int i = 0; i < _chestLocations.Count; i++)
            {
                int xDistance = (int)hex.HexCoord.x - (int)_chestLocations[i].HexCoord.x;
                int yDistance = (int)hex.HexCoord.y - (int)_chestLocations[i].HexCoord.y;
                int totalDistance = xDistance + yDistance;

                if (totalDistance < _minDistanceChestChest)
                {
                    isToClose = true;
                }
                else
                {
                    isToClose = false;
                }

            }

            return isToClose;
        }

        private PCGHex SelectHexWithDistance(PCGHex startPos, int minDistance, int maxDistance)
        {
            PCGHex selectedHex;
            int index;
            int xDistance;
            int yDistance;
            int totalDistance;

            do
            {
                index = prng.Next(0, _playableArea.Count);
                selectedHex = _playableArea[index];
                xDistance = Mathf.Abs((int)selectedHex.HexCoord.x - (int)startPos.HexCoord.x);
                yDistance = Mathf.Abs((int)selectedHex.HexCoord.y - (int)startPos.HexCoord.y);
                totalDistance = xDistance + yDistance;
            }
            while (totalDistance < minDistance || totalDistance > maxDistance || _excludedHexes.Contains(selectedHex));

            return selectedHex;
        }

        private void PlaceEnemyCharacters()
        {
            int chestIndex = 0;

            EnemyProfile profile = _enemyProfiles[prng.Next(0, _enemyProfiles.Count)];

            for (int i = 0; i < profile.Characters.Count; i++)
            {
                
                PCGHex targetHex = SelectHexWithDistance(_chestLocations[chestIndex], _minDistanceChestsEnemy, _maxDistanceChestsEnemy);
                GameObject enemy = (GameObject)Instantiate(profile.Characters[i], transform);
                enemy.transform.position = targetHex.WorldCoord;

                _excludedHexes.Add(targetHex);

                chestIndex++;
                if(chestIndex > _chestLocations.Count -1)
                {
                    chestIndex = 0;
                }
            }
        }

        private void LoadProfiles()
        {
            _playerCharacterProfiles = new List<PlayerCharacterProfile>();
            _enemyProfiles = new List<EnemyProfile>();

            string playerProfilesPath = "PCG/Gameplayelements/PlayerProfiles";
            string enemyProfilesPath = "PCG/Gameplayelements/EnemyProfiles";

            Object[] loadedPlayerResources = Resources.LoadAll(playerProfilesPath);
            Object[] loadedEnemyResources = Resources.LoadAll(enemyProfilesPath);

            //Convert to list
            for (int j = 0; j < loadedPlayerResources.Length; j++)
            {
                _playerCharacterProfiles.Add((PlayerCharacterProfile)loadedPlayerResources[j]);
            }

            for (int j = 0; j < loadedEnemyResources.Length; j++)
            {
                _enemyProfiles.Add((EnemyProfile)loadedEnemyResources[j]);
            }
        }

        #endregion
    }
}