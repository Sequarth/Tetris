using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Gameplay.Board
{
    /// <summary>
    /// Holds information about which blocks to destroy and methods for Gameplay Manager to use and grab necessary data
    /// </summary>
    public class BoardParametersHolder : MonoBehaviour
    {
        private int _boardId = 0;
        
        // Action for Rows in board - used to detect and return filled rows
        public Action BrickStopped;
        
        // List of references to blocks and rows to destroy
        private List<GameObject> _blocksToDestroy = new();
        private List<float> _rowsToDestroy = new();

        private void OnEnable()
        {
            // Subscribes to Action based on board ID
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
            // Unsubscribes from Action based on board ID
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

        // Sets board ID - used by Gameplay Manager
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
        
        // Invokes Action used by Rows in board
        private void OnBrickStopped()
        {
            BrickStopped?.Invoke();
        }

        // Adds references of blocks to destroy to list - used by Rows
        public void AddBlocksToDestroy(List<GameObject> blocksToDestroy)
        {
            _blocksToDestroy.AddRange(blocksToDestroy);
        }

        // Adds value of row height to destroy to list for future calculations - used by Rows
        public void AddRowsToDestroy(float rowToDestroy)
        {
            _rowsToDestroy.Add(rowToDestroy);
        }

        // Grabs list of blocks to destroy - used by Gameplay Manager
        public List<GameObject> GetBlocksToDestroy()
        {
            return _blocksToDestroy;
        }

        // Grabs list of rows to destroy - useb by Gameplay Manager
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
