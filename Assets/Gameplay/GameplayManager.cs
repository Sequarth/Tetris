using System.Collections.Generic;
using Tetris.Gameplay.Board;
using TMPro;
using UnityEngine;

namespace Tetris.Gameplay
{
    /// <summary>
    /// Gameplay Manager - handles UI, bricks and board presentations
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        // Player 1 data
        [Header("Player 1")]
        [SerializeField]
        private GameObject player1Board;
        [SerializeField] 
        private string player1Name;
        [SerializeField] 
        private TextMeshProUGUI player1NameTMP;
        [SerializeField]
        private int player1Score;
        [SerializeField]
        private TextMeshProUGUI player1ScoreTMP;
        private GameObject _player1Brick;
        private GameObject _player1NextBrick;
        private int _player1NextBrickId;
        private GameObject _player1BoardField;
        
        // Player 2 data
        [Header("Player 2")]
        [SerializeField]
        private GameObject player2Board;
        [SerializeField]
        private string player2Name;
        [SerializeField] 
        private TextMeshProUGUI player2NameTMP;
        [SerializeField]
        private int player2Score;
        [SerializeField]
        private TextMeshProUGUI player2ScoreTMP;
        private GameObject _player2Brick;
        private GameObject _player2NextBrick;
        private int _player2NextBrickId;
        private GameObject _player2BoardField;
        
        [Header("Prefabs")]
        [SerializeField] 
        private List<GameObject> brickPrefabs;
        [SerializeField]
        private GameObject playerBoardPrefab;
        
        // References to board data holders
        private BoardParametersHolder _board1ParametersHolder;
        private BoardParametersHolder _board2ParametersHolder;

        private void Start()
        {
            GetBoardGameObjects();
            
            // Generates board fields for holding already placed blocks
            _player1BoardField = new();
            _player2BoardField = new();
            _player1BoardField.transform.SetParent(transform);
            _player2BoardField.transform.SetParent(transform);
            _player1BoardField.name = "Player 1 Board Field";
            _player2BoardField.name = "Player 2 Board Field";
            
            // Sets names of players
            player1NameTMP.text = player1Name;
            player2NameTMP.text = player2Name;
            
            // Sets flags for grabbing first random brick
            _player1NextBrickId = -1;
            _player2NextBrickId = -1;
        }
        
        private void OnEnable()
        {
            // Subscribes to necessary Actions from Event Manager
            Core.EventManager.player1BrickSpawned += OnPlayer1BrickInstantiated;
            Core.EventManager.player2BrickSpawned += OnPlayer2BrickInstantiated;
            Core.EventManager.player1BrickSpawned += ShowNextPlayer1Brick;
            Core.EventManager.player2BrickSpawned += ShowNextPlayer2Brick;
            Core.EventManager.player1BrickAttached += AttachBlocksToBoard1Field;
            Core.EventManager.player2BrickAttached += AttachBlocksToBoard2Field;
            Core.EventManager.player1BricksCleared += OnPlayer1EndTurn;
            Core.EventManager.player2BricksCleared += OnPlayer2EndTurn;
        }

        private void OnDisable()
        {
            // Unsubscribes from necessary Actions from Event Manager
            Core.EventManager.player1BrickSpawned -= OnPlayer1BrickInstantiated;
            Core.EventManager.player2BrickSpawned -= OnPlayer2BrickInstantiated;
            Core.EventManager.player1BrickSpawned -= ShowNextPlayer1Brick;
            Core.EventManager.player2BrickSpawned -= ShowNextPlayer2Brick;
            Core.EventManager.player1BrickAttached -= AttachBlocksToBoard1Field;
            Core.EventManager.player2BrickAttached -= AttachBlocksToBoard2Field;
            Core.EventManager.player1BricksCleared -= OnPlayer1EndTurn;
            Core.EventManager.player2BricksCleared -= OnPlayer2EndTurn;
        }
        
        // Grabs or generates boards if not present
        private void GetBoardGameObjects()
        {
            if (!transform.Find("Board1"))
            {
                var position = new Vector3(9, 0.5f, 0);
                player1Board = Instantiate(playerBoardPrefab, position, Quaternion.identity , transform);
                player1Board.transform.SetParent(transform);
                player1Board.name = "Board1";
                _board1ParametersHolder = player1Board.GetComponent<BoardParametersHolder>();
                _board1ParametersHolder.SetBoardId(1);
            }
            else player1Board = transform.Find("Board1").gameObject;
            if (!transform.Find("Board2"))
            {
                var position = new Vector3(35, 0.5f, 0);
                player2Board = Instantiate(playerBoardPrefab, position, Quaternion.identity, transform);
                player2Board.transform.SetParent(transform);
                player2Board.name = "Board2";
                _board2ParametersHolder = player2Board.GetComponent<BoardParametersHolder>();
                _board2ParametersHolder.SetBoardId(2);
            }
            else player2Board = transform.Find("Board2").gameObject;
        }

        // Calculates points after clearing rows
        private static int CalculatePoints(int rowsCleared)
        {
            return rowsCleared switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            };
        }
        
        // Instantiates player 1 brick
        private void OnPlayer1BrickInstantiated(int brickId)
        {
            if (_player1NextBrickId == -1)
            {
                _player1NextBrickId = brickId;
                Core.EventManager.player1EndTurn?.Invoke();
                return;
            }
            var position = player1Board.transform.position + new Vector3(0, 44, 0);
            var fallingSpeed = new Vector3(0, -10f, 0);
            _player1Brick = Instantiate(brickPrefabs[_player1NextBrickId], position, Quaternion.identity, player1Board.transform);
            _player1Brick.GetComponent<Rigidbody>().linearVelocity = fallingSpeed;
            _player1NextBrickId = brickId;
            Core.EventManager.player1BrickInstantiated?.Invoke(_player1Brick);
        }
        
        // Instantiates player 2 brick
        private void OnPlayer2BrickInstantiated(int brickId)
        {
            if (_player2NextBrickId == -1)
            {
                _player2NextBrickId = brickId;
                Core.EventManager.player2EndTurn?.Invoke();
                return;
            }
            var position = player2Board.transform.position + new Vector3(0, 44, 0);
            var fallingSpeed = new Vector3(0, -10f, 0);
            _player2Brick = Instantiate(brickPrefabs[_player2NextBrickId], position, Quaternion.identity, player1Board.transform);
            _player2Brick.GetComponent<Rigidbody>().linearVelocity = fallingSpeed;
            _player2NextBrickId = brickId;
            Core.EventManager.player2BrickInstantiated?.Invoke(_player2Brick);
        }

        // Attaches blocks from placed bricks to player 1 board field
        private void AttachBlocksToBoard1Field()
        {
            foreach (var block in _player1Brick.GetComponentsInChildren<Transform>())
            {
                block.transform.parent = _player1BoardField.transform;
            }
            Destroy(_player1Brick);
        }

        // Attaches blocks from placed bricks to player 2 board field
        private void AttachBlocksToBoard2Field()
        {
            foreach (var block in _player2Brick.GetComponentsInChildren<Transform>())
            {
                block.transform.parent = _player2BoardField.transform;
            }
            Destroy(_player2Brick);
        }

        // Handles score calculation for player 1, destroys blocks after filling row and
        // signals end of a cycle effectively creating next brick
        private void OnPlayer1EndTurn()
        {
            var blocksToDestroy = _board1ParametersHolder.GetBlocksToDestroy();
            var linesCleared = blocksToDestroy.Count / 10;
            player1Score += CalculatePoints(linesCleared);
            player1ScoreTMP.text = player1Score.ToString();
            foreach (var block in blocksToDestroy)
            {
                Destroy(block);
            }
            _board1ParametersHolder.ClearBlocksToDestroy();

            MoveRowsInBoard1Field();

            Core.EventManager.player1EndTurn?.Invoke();
        }

        // Handles moving placed blocks after clearing the row in player 1 board
        private void MoveRowsInBoard1Field()
        {
            var rowsToDestroy = _board1ParametersHolder.GetRowsToDestroy();
            foreach (var block in _player1BoardField.GetComponentsInChildren<Transform>())
            {
                var moveOffset = 0f;
                foreach (var rowY in rowsToDestroy)
                {
                    if (block.transform.position.y > rowY) moveOffset += 2f;
                }
                
                block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - moveOffset, block.transform.position.z);
            }
            _board1ParametersHolder.ClearRowsToDestroy();
        }
        
        // Handles score calculation for player 2, destroys blocks after filling row and
        // signals end of a cycle effectively creating next brick
        private void OnPlayer2EndTurn()
        {
            var blocksToDestroy = _board2ParametersHolder.GetBlocksToDestroy();
            var linesCleared = blocksToDestroy.Count / 10;
            player2Score += CalculatePoints(linesCleared);
            player2ScoreTMP.text = player2Score.ToString();
            foreach (var block in blocksToDestroy)
            {
                Destroy(block);
            }
            _board2ParametersHolder.ClearBlocksToDestroy();
            
            MoveRowsInBoard2Field();
            
            Core.EventManager.player2EndTurn?.Invoke();
        }
        
        // Handles moving placed blocks after clearing the row in player 2 board
        private void MoveRowsInBoard2Field()
        {
            var rowsToDestroy = _board2ParametersHolder.GetRowsToDestroy();
            foreach (var block in _player2BoardField.GetComponentsInChildren<Transform>())
            {
                var moveOffset = 0f;
                foreach (var rowY in rowsToDestroy)
                {
                    if (block.transform.position.y > rowY) moveOffset += 2f;
                }
                
                block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - moveOffset, block.transform.position.z);
            }
            _board2ParametersHolder.ClearRowsToDestroy();
        }

        // Instantiates next brick for player 1 to see
        private void ShowNextPlayer1Brick(int context)
        {
            if (_player1NextBrick) Destroy(_player1NextBrick);
            var position = player1Board.transform.position + new Vector3(-25, 20, 0);
            _player1NextBrick = Instantiate(brickPrefabs[_player1NextBrickId], position, Quaternion.identity, player1Board.transform);
        }

        // Instantiates next brick for player 2 to see
        private void ShowNextPlayer2Brick(int context)
        {
            if (_player2NextBrick) Destroy(_player2NextBrick);
            var position = player2Board.transform.position + new Vector3(25, 20, 0);
            _player2NextBrick = Instantiate(brickPrefabs[_player2NextBrickId], position, Quaternion.identity, player2Board.transform);
        }
    }
}
