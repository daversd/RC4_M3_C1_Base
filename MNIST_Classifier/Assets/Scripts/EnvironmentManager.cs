using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EnvironmentManager : MonoBehaviour
{
    #region Fields
    
    VoxelGrid _voxelGrid;
    Vector3Int _gridSize = new Vector3Int(28, 1, 28);
    List<VoxelState> _voxelStates;

    Texture2D _preview;

    [SerializeField]
    Text _predictionText;

    bool _erasing = false;

    #endregion

    #region Unity Methods

    void Start()
    {
        _preview = Resources.Load<Texture2D>("Materials/Preview/output");
        _voxelGrid = new VoxelGrid(_gridSize, transform.position, 1f);
        _voxelStates = new List<VoxelState>();
        foreach (var voxel in _voxelGrid.Voxels)
        {
            var go = voxel.SetParent(transform);
            _voxelStates.Add(go.GetComponent<VoxelState>());
        }
    }

    void Update()
    {
        // Use mouse to draw or erase on the grid
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var objectHit = hit.transform;
                if (objectHit.CompareTag("Voxel"))
                {
                    var stateObject = objectHit.GetComponent<VoxelState>();
                    if (_erasing && stateObject.Marked)
                    {
                        stateObject.Unmark();
                    }
                    else if (!_erasing)
                    {
                        stateObject.Mark();
                        var voxel = stateObject.Voxel;

                        Vector3Int ind = voxel.Index;
                        var indXp = ind + new Vector3Int(1, 0, 0);
                        var indXn = ind + new Vector3Int(-1, 0, 0);

                        var indZp = ind + new Vector3Int(0, 0, 1);
                        var indZn = ind + new Vector3Int(0, 0, -1);

                        var indD1 = ind + new Vector3Int(1, 0, 1);
                        var indD2 = ind + new Vector3Int(-1, 0, 1);

                        var indD3 = ind + new Vector3Int(1, 0, -1);
                        var indD4 = ind + new Vector3Int(-1, 0, -1);

                        if (Util.ValidateIndex(_gridSize, indXp)) _voxelGrid.Voxels[indXp.x, indXp.y, indXp.z].StateObj.Mark(0.05f);
                        if (Util.ValidateIndex(_gridSize, indXn)) _voxelGrid.Voxels[indXn.x, indXn.y, indXn.z].StateObj.Mark(0.05f);

                        if (Util.ValidateIndex(_gridSize, indZp)) _voxelGrid.Voxels[indZp.x, indZp.y, indZp.z].StateObj.Mark(0.05f);
                        if (Util.ValidateIndex(_gridSize, indZn)) _voxelGrid.Voxels[indZn.x, indZn.y, indZn.z].StateObj.Mark(0.05f);

                        if (Util.ValidateIndex(_gridSize, indD1)) _voxelGrid.Voxels[indD1.x, indD1.y, indD1.z].StateObj.Mark(0.05f);
                        if (Util.ValidateIndex(_gridSize, indD2)) _voxelGrid.Voxels[indD2.x, indD2.y, indD2.z].StateObj.Mark(0.05f);
                        if (Util.ValidateIndex(_gridSize, indD3)) _voxelGrid.Voxels[indD3.x, indD3.y, indD3.z].StateObj.Mark(0.05f);
                        if (Util.ValidateIndex(_gridSize, indD4)) _voxelGrid.Voxels[indD4.x, indD4.y, indD4.z].StateObj.Mark(0.05f);
                    }
                }
            }
        }

        // Use E key to cycle between draw and erase mode 
        if (Input.GetKeyDown(KeyCode.E))
        {
            _erasing = !_erasing;
            print($"Erasing is now {_erasing}");
        }

        // Use R key to reset the grid
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var voxel in _voxelStates)
            {
                voxel.Unmark();
            }
            
        }
    }

    #endregion

    #region Private Methods


    #endregion
}