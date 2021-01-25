using RoH.Missions;
using RoH.MultiScene;
using UnityEngine;

namespace PCG
{
    public class PCGLevelLoader : MonoBehaviour
    {
        public Mission Mission;


        //Test
        private void Update()
        {
          
        }


        /// <summary>
        /// Procedural grid generation
        /// </summary>
        public void GenerateGridData()
        {

        }

        //public void LoadLevel()
        //{
        //    //Generate hexGrid (pcg)
        //    GridGenerator gridGenerator = new GridGenerator();
        //    gridGenerator.GenerateBasicGrid();

        //    MissionControl.OnInitializationStarted += SpawnAssets;
        //    LoadingSceneManager.Instance.LoadMission(Mission);

        //}

        public void SpawnAssets()
        {
            //TODO: pattern recognition




            BakeNavMesh();
        }

        public void BakeNavMesh()
        {

        }
    }
}