using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public static class HexGridCleanUp
    {
        private static List<PCGHex> fullList = new List<PCGHex>();

        public static List<PCGHex> GetIsland(List<PCGHex> hexList, int counter = 0)
        {
            int totalNeighbourCount = counter;
            List<PCGHex> currentList = hexList;
            List<PCGHex> newHexList = new List<PCGHex>();

            //foreach hex
            for (int i = 0; i < currentList.Count; i++)
            {
                //Count up
                fullList.Add(currentList[i]);
                totalNeighbourCount++;

                //Loop neighbours
                int neighbours = currentList[i].NeighbourCount;
                for (int j = 0; j < neighbours; j++)
                {
                    PCGHex currentHex = currentList[i].GetNeighbourAtIndex(j);
                    //check state
                    if (currentHex.IsWalkable)
                    {
                        //Check if not alrdy done
                        if (fullList.Contains(currentHex) == false)
                        {
                            //Add to next iteration
                            newHexList.Add(currentHex);
                        }
                    }
                }
            }

            //If there are new neighbours repeat the loop
            if (newHexList.Count != 0)
            {
                GetIsland(newHexList, totalNeighbourCount);
            }

            //If no new neighbours close and return results
            return fullList;
        }
    }
}
