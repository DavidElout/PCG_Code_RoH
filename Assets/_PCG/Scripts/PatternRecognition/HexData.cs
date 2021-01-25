using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    [System.Serializable]
    public class HexData
    {
        [SerializeField]
        public List<HexTypeData> HexTypeDataList;
        
        

        public void Init()
        {
            HexTypeDataList = new List<HexTypeData>();
            for (int i = 0; i < (int)HexObjectType.Default; i++)
            {
                HexTypeDataList.Add(new HexTypeData((HexObjectType)i));
            }
        }
    }
}