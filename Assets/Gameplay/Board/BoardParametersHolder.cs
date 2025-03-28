using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Gameplay.Board
{
    public class BoardParametersHolder : MonoBehaviour
    {
        private int _boardId = 0;
        
        public Action BrickStopped;
        
        private List<GameObject> _blocksToDestroy = new();
        private List<float> _rowsToDestroy = new();

        private void OnEnable()
        {
            switch (_boardId)
            {
                case 1:
                    Core.EventManager.player1BrickStopped += OnBrickStopped;
                    break;
                case 2:
                    Core.EventManager.player2BrickStopped += OnBrickStopped;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (_boardId)
            {
                case 1:
                    Core.EventManager.player1BrickStopped -= OnBrickStopped;
                    break;
                case 2:
                    Core.EventManager.player2BrickStopped -= OnBrickStopped;
                    break;
            }
        }

        public void SetBoardId(int id)
        {
            _boardId = id;
            
            switch (_boardId)
            {
                case 1:
                    Core.EventManager.player1BrickStopped += OnBrickStopped;
                    break;
                case 2:
                    Core.EventManager.player2BrickStopped += OnBrickStopped;
                    break;
            }
        }
        
        private void OnBrickStopped()
        {
            BrickStopped?.Invoke();
        }

        public void AddBlocksToDestroy(List<GameObject> blocksToDestroy)
        {
            _blocksToDestroy.AddRange(blocksToDestroy);
        }

        public void AddRowsToDestroy(float rowToDestroy)
        {
            _rowsToDestroy.Add(rowToDestroy);
        }

        public List<GameObject> GetBlocksToDestroy()
        {
            return _blocksToDestroy;
        }

        public List<float> GetRowsToDestroy()
        {
            return _rowsToDestroy;
        }

        public void ClearBlocksToDestroy()
        {
            _blocksToDestroy.Clear();
        }

        public void ClearRowsToDestroy()
        {
            _rowsToDestroy.Clear();
        }
    }
}
