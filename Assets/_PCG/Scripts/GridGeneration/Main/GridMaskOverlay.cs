using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PCG
{
    public class GridMaskOverlay : MonoBehaviour
    {
        #region Editor Fields

        [SerializeField]
        private Texture2D mask;
        [SerializeField]
        private MaskType _mainAreaType; //Defines rules for the actual space that is masked. e.g. A river
        [SerializeField]
        private MaskType _offAreaType; //Defines rules for the 'extra' space that is masked. e.g. The riverbed
        [SerializeField]
        private Material[] maskMaterial;

        #endregion

        #region Methods

        /// <summary>
        /// Applys a mask over the generated area. Masks can contain multiple colored layers to define different area type. 
        /// e.g.
        /// black = nothing
        /// white = change to unwalkable
        /// cyan = change area type of the hex based on areaType
        /// </summary>
        /// <param name="pcgHexDictionary"></param>
        public void ApplyMask(Dictionary<Vector3, PCGHex> pcgHexDictionary)
        {
            if (mask != null)
            {
                Vector3 smallestValue = GetSmallestValue(pcgHexDictionary);
                Vector3 largestValue = GetLargestValue(pcgHexDictionary);

                //+1 because '0' counts too
                float xLenght = Mathf.Abs(smallestValue.x - largestValue.x + 1);
                float yLenght = Mathf.Abs(smallestValue.y - largestValue.y + 1);

                Color whiteColor = Color.white;
                Color cyanColor = Color.cyan;

                foreach (var item in pcgHexDictionary)
                {
                    Vector3 currentCoord = item.Key;
                    PCGHex currentHex = item.Value;

                    //Translate grid position to mask position
                    Vector2 maskPosition = new Vector2
                    {
                        x = (currentCoord.x + Mathf.Abs(smallestValue.x)) / xLenght * mask.width,
                        y = (currentCoord.y + Mathf.Abs(smallestValue.y)) / yLenght * mask.height
                    };

                    Color color = mask.GetPixel((int)maskPosition.x, (int)maskPosition.y);

                    if (color == whiteColor)
                    {
                        currentHex.IsWalkable = false;
                        currentHex.AreaType = _mainAreaType;
                        currentHex.UpdateHexColor();
                    }

                    if (color == cyanColor)
                    {
                        currentHex.IsWalkable = false;
                        currentHex.AreaType = _offAreaType;
                        currentHex.UpdateHexColor(maskMaterial[1]);
                    }  
                }
            }
        }

        /// <summary>
        /// Returns the smallest key from the dictionary
        /// </summary>
        /// <param name="pcgHexDictionary">given dictionary</param>
        /// <returns>smallest key</returns>
        private Vector3 GetSmallestValue(Dictionary<Vector3, PCGHex> pcgHexDictionary)
        {
            Vector3 smallestValue = new Vector3(0f, 0f, 0f);

            foreach (var item in pcgHexDictionary)
            {
                if (item.Key.x < smallestValue.x || item.Key.y < smallestValue.y)
                {
                    smallestValue = item.Key;
                }
            }

            return smallestValue;
        }

        /// <summary>
        /// returns the largest key from the dictionary
        /// </summary>
        /// <param name="pcgHexDictionary">given dictionary</param>
        /// <returns>largest value</returns>
        private Vector3 GetLargestValue(Dictionary<Vector3, PCGHex> pcgHexDictionary)
        {
            Vector3 biggestValue = new Vector3(0f, 0f, 0f);

            foreach (var item in pcgHexDictionary)
            {
                if (item.Key.x > biggestValue.x || item.Key.y > biggestValue.y)
                {
                    biggestValue = item.Key;
                }
            }

            return biggestValue;
        }

        #endregion
    }
}
