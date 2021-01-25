using HexGridEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoH.Grid;

/// <summary>
/// Due to the previously used code to generate the starting grid being EditorOnly and the PCG
/// system being required to generate at runtime, this script uses duplicated code in order to generate the starting grid at runtime.
/// </summary>
public class GridGenerationHelper : MonoBehaviour
{
    private float _hexWidth = 4f;
    private float _hexHeight = 3f;
    public Dictionary<Vector3, HexPreviewData> HexPreviewPositions;

    Vector3 _dimensions;



    public Dictionary<Vector3, Vector3> CalculateHexPositionsFix(Vector3 dimensions)
    {
        _dimensions = dimensions;
        HexPreviewPositions = new Dictionary<Vector3, HexPreviewData>();
        Dictionary<Vector3, Vector3> hexPositions = new Dictionary<Vector3, Vector3>();

        Vector3 startPos = (gameObject.transform.position - new Vector3(_dimensions.x / 2.0f, 0.0f, _dimensions.z / 2.0f));
        startPos.x = Mathf.Floor(startPos.x / _hexWidth) * _hexWidth;
        startPos.y = Mathf.Floor(startPos.y);
        startPos.z = Mathf.Floor(startPos.z / _hexHeight) * _hexHeight;

        for (float z = startPos.z; z < gameObject.transform.position.z + _dimensions.z / 2.0f; z += _hexHeight)
        {
            for (float x = startPos.x; x < gameObject.transform.position.x + _dimensions.x / 2.0f; x += _hexWidth)
            {
                bool isEven = ((z / _hexHeight % 2) == 0);
                var positionToCheck = new Vector3(x, startPos.y, z);

                Vector3 coord = new Vector3();
                coord.x = positionToCheck.x / _hexWidth;
                coord.y = positionToCheck.z / _hexHeight;
                coord.z = 0;

                CalculateOffset(ref positionToCheck);
                if (!isEven) positionToCheck += new Vector3(_hexWidth / 2.0f, 0.0f);

          
                hexPositions.Add(coord, positionToCheck);
            }
        }

        return hexPositions;
    }

    private void CalculateOffset(ref Vector3 position)
    {
        Vector3 offset = Vector3.zero;
        offset.x = HexEditorPreviewer.GridSettings.GridOffset.x * HexEditorPreviewer.GridSettings.HexWidth;
        offset.z = HexEditorPreviewer.GridSettings.GridOffset.y * HexEditorPreviewer.GridSettings.HexHeight;
        position += offset;
    }
}
