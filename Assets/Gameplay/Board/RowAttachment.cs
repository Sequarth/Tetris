using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tetris.Gameplay.Board
{
    public class RowAttachment : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> blocksInRow = new();

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

        private void CheckIfTenBlocksInRow()
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(9.5f, 0.5f, 1), Quaternion.identity);
            if (hitColliders.Length < 10) return;
            int i = 0;
            while (i < hitColliders.Length)
            {
                blocksInRow.Add(hitColliders[i].gameObject);
                i++;
            }
            _parentBoard.AddBlocksToDestroy(blocksInRow);
            _parentBoard.AddRowsToDestroy(transform.position.y);
            foreach (var t in blocksInRow.ToList())
            {
                blocksInRow.Remove(t);
            }
        }
    }
}
