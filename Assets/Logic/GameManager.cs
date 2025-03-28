using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Tetris.Core
{
    /// <summary>
    /// Entry point. Responsible for game logic and loading scenes additively
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Brick rotation logic
        private BrickRotation _brickRotations;
        
        // Player 1 references and flags
        private GameObject _player1BrickGo;
        private Rigidbody _player1BrickRb;
        private float _player1BrickMovement;
        private int _player1GameOver;
        
        // Player 2 references and flags
        private GameObject _player2BrickGo;
        private Rigidbody _player2BrickRb;
        private float _player2BrickMovement;
        private int _player2GameOver;

        // Const values used in calculations
        private const float BrickMovementSpeed = 8.0f;
        private const float BrickRotationTime = 0.1f;
        private const float BrickMovementSpeedCutOff = -0.1f;

        private void Awake()
        {
            _brickRotations = GetComponent<BrickRotation>();
            // Loads Gameplay scene additively
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }

        private void OnEnable()
        {
            // Subscribes to event manager Actions
            EventManager.gameStarted += OnPlayer1BrickSpawned;
            EventManager.gameStarted += OnPlayer2BrickSpawned;
            EventManager.player1EndTurn += OnPlayer1BrickSpawned;
            EventManager.player2EndTurn += OnPlayer2BrickSpawned;

            EventManager.player1BrickInstantiated += GetPlayer1BrickRigidbody;
            EventManager.player2BrickInstantiated += GetPlayer2BrickRigidbody;
            
            EventManager.player1BrickMoved += Player1BrickSetMovementDirection;
            EventManager.player1BrickRotated += Player1BrickSetRotationDirection;
            EventManager.player1BrickPlaced += PlacePlayer1Brick;
            EventManager.player2BrickMoved += Player2BrickSetMovementDirection;
            EventManager.player2BrickRotated += Player2BrickSetRotationDirection;
            EventManager.player2BrickPlaced += PlacePlayer2Brick;
        }

        private void OnDisable()
        {
            // Unsubscribes
            EventManager.gameStarted -= OnPlayer1BrickSpawned;
            EventManager.gameStarted -= OnPlayer2BrickSpawned;
            EventManager.player1EndTurn -= OnPlayer1BrickSpawned;
            EventManager.player2EndTurn -= OnPlayer2BrickSpawned;
            
            EventManager.player1BrickInstantiated -= GetPlayer1BrickRigidbody;
            EventManager.player2BrickInstantiated -= GetPlayer2BrickRigidbody;
            
            EventManager.player1BrickMoved -= Player1BrickSetMovementDirection;
            EventManager.player1BrickRotated -= Player1BrickSetRotationDirection;
            EventManager.player1BrickPlaced -= PlacePlayer1Brick;
            EventManager.player2BrickMoved -= Player2BrickSetMovementDirection;
            EventManager.player2BrickRotated -= Player2BrickSetRotationDirection;
            EventManager.player2BrickPlaced -= PlacePlayer2Brick;
        }

        private void Update()
        {
            // Unsubscribes methods to effectively stop the game for specific players
            if (_player1GameOver == 1)
            {
                EventManager.player1EndTurn -= OnPlayer1BrickSpawned;
                _player1GameOver = 2;
            }
            
            if (_player2GameOver == 1)
            {
                EventManager.player2EndTurn -= OnPlayer2BrickSpawned;
                _player2GameOver = 2;
            }
        }

        private void FixedUpdate()
        {
            // Brick movement, rotation and placement for player 1 brick - based on physics
            if (_player1BrickRb)
            {
                _player1BrickRb.linearVelocity = new Vector3(_player1BrickMovement * BrickMovementSpeed,
                    _player1BrickRb.linearVelocity.y, 0);
            
                if (_brickRotations.player1Brick.WhetherToRotateBrick)
                {
                    _player1BrickRb.rotation = _brickRotations.player1Brick.GetRotationFixedUpdate(Time.fixedDeltaTime/BrickRotationTime);
                }
                // Expensive/unreliable solution - better to check for colliders
                if (_player1BrickRb.linearVelocity.y >= BrickMovementSpeedCutOff && !_brickRotations.player1Brick.WhetherToRotateBrick)
                {
                    _player1GameOver = SetBlocksPositions(_player1BrickGo);
                    OnPlayer1BrickStopped();
                }
            }
            
            // Brick movement, rotation and placement for player 2 brick - based on physics
            if (_player2BrickRb)
            {
                _player2BrickRb.linearVelocity = new Vector3(_player2BrickMovement * BrickMovementSpeed,
                    _player2BrickRb.linearVelocity.y, 0);
            
                if (_brickRotations.player2Brick.WhetherToRotateBrick)
                {
                    _player2BrickRb.rotation = _brickRotations.player2Brick.GetRotationFixedUpdate(Time.fixedDeltaTime/BrickRotationTime);
                }
                
                if (_player2BrickRb.linearVelocity.y >= BrickMovementSpeedCutOff)
                {
                    _player2GameOver = SetBlocksPositions(_player2BrickGo);
                    OnPlayer2BrickStopped();
                }
            }
        }
        
        // Starts the cycle generating random player 1 brick id and invoking next Action
        private void OnPlayer1BrickSpawned()
        {
            _brickRotations.player1Brick.ResetRotationAngles();
            var randomBrick = Random.Range(0, 7);
            EventManager.player1BrickSpawned?.Invoke(randomBrick);
        }
        
        // Starts the cycle generating random player 2 brick id and invoking next Action
        private void OnPlayer2BrickSpawned()
        {
            _brickRotations.player2Brick.ResetRotationAngles();
            var randomBrick = Random.Range(0, 7);
            EventManager.player2BrickSpawned?.Invoke(randomBrick);
        }

        // Invokes events in order for handling player 1 brick placement
        private static void OnPlayer1BrickStopped()
        {
            EventManager.player1BrickAttached?.Invoke();
            EventManager.player1BrickStopped?.Invoke();
            EventManager.player1BricksCleared?.Invoke();
        }

        // Invokes events in order for handling player 2 brick placement
        private static void OnPlayer2BrickStopped()
        {
            EventManager.player2BrickAttached?.Invoke();
            EventManager.player2BrickStopped?.Invoke();
            EventManager.player2BricksCleared?.Invoke();
        }

        // Sets positions of blocks in grid pattern after brick placement
        // Additionally sets the flag for ending the game
        private int SetBlocksPositions(GameObject brick)
        {
            var gameEnded = 0;
            foreach (var block in brick.GetComponentsInChildren<Transform>())
            {
                var remainderX = block.position.x % 2;
                var remainderY = block.position.y % 2;
                var offsetX = remainderX % 2 <= 1 ? 0 : 2;
                var offsetY = remainderY % 2 <= 1 ? 0 : 2;
                block.position = new Vector3(block.position.x - remainderX + offsetX, block.position.y - remainderY + offsetY);
                if (block.position.y >= 39) gameEnded = 1;
            }
            return gameEnded;
        }

        // Grabs player 1 brick references for easier handling
        private void GetPlayer1BrickRigidbody(GameObject brick)
        {
            _player1BrickGo = brick;
            _player1BrickRb = brick.GetComponent<Rigidbody>();
        }

        // Same for player 2
        private void GetPlayer2BrickRigidbody(GameObject brick)
        {
            _player2BrickGo = brick;
            _player2BrickRb = brick.GetComponent<Rigidbody>();
        }

        // Sets movement direction of player 1 brick
        private void Player1BrickSetMovementDirection(float movementDirection)
        {
            _player1BrickMovement = movementDirection;
        }

        // Same for player 2
        private void Player2BrickSetMovementDirection(float movementDirection)
        {
            _player2BrickMovement = movementDirection;
        }

        // Brick rotation logic for player 1
        private void Player1BrickSetRotationDirection(float rotationDirection)
        {
            if (rotationDirection == 0f) return;
            _brickRotations.player1Brick.SetCurrentBrickRotation(_player1BrickRb.transform);
            _brickRotations.player1Brick.AddAngles(rotationDirection);
            _brickRotations.player1Brick.SetTargetBrickRotation(transform);
            _brickRotations.player1Brick.ResetRotationTime();
        }
        
        // Brick rotation logic for player 2
        private void Player2BrickSetRotationDirection(float rotationDirection)
        {
            if (rotationDirection == 0f) return;
            _brickRotations.player2Brick.SetCurrentBrickRotation(_player2BrickRb.transform);
            _brickRotations.player2Brick.AddAngles(rotationDirection);
            _brickRotations.player2Brick.SetTargetBrickRotation(transform);
            _brickRotations.player2Brick.ResetRotationTime();
        }
        
        // Unused - Instant drop of bricks not implemented
        private static void PlacePlayer1Brick()
        {
            // Brick drop implementation
        }
        
        private static void PlacePlayer2Brick()
        {
            // Brick drop implementation
        }
    }
}
