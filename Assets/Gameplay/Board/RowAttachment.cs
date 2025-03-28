using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tetris.Gameplay.Board
{
    /// <summary>
    /// Checks if row in board is filled with blocks
    /// </summary>
    public class RowAttachment : MonoBehaviour
    {
        private List<GameObject> _blocksInRow = new();

        // Holds reference to parent BoardParametersHolder object
        private BoardParametersHolder _parentBoard;

        private void Awake()
        {
            _parentBoard = GetComponentInParent<BoardParametersHolder>();
        }

        private void OnEnable()
        {
            _parentBoard.BrickStopped += CheckIfTenBlocksInRow;
        }

        private void OnDisable()
        {
            _parentBoard.BrickStopped -= CheckIfTenBlocksInRow;
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (!blocksInRow.Contains(other.gameObject)) { blocksInRow.Add(other.gameObject); }
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     blocksInRow.Remove(other.gameObject);
        // }

        // Checks colliders in row - if 10 blocks are present (filled row) sends data to lists
        private void CheckIfTenBlocksInRow()
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(9.5f, 0.5f, 1), Quaternion.identity);
            if (hitColliders.Length < 10) return;
            int i = 0;
            while (i < hitColliders.Length)
            {
                _blocksInRow.Add(hitColliders[i].gameObject);
                i++;
            }
            _parentBoard.AddBlocksToDestroy(_blocksInRow);
            _parentBoard.AddRowsToDestroy(transform.position.y);
            foreach (var t in _blocksInRow.ToList())
            {
                _blocksInRow.Remove(t);
            }
        }
    }
}
