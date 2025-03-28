using UnityEngine;
using UnityEngine.InputSystem;

namespace Tetris.Gameplay.Input
{
    /// <summary>
    /// Handles new Input System
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // Input maps for both players
        [SerializeField] private InputActionMap player1Actions;
        [SerializeField] private InputActionMap player2Actions;

        // Starts the game after pressing space
        private InputAction _startGame;
        
        // Player 1 inputs
        private InputAction _player1MoveBrick;
        private InputAction _player1RotateBrick;
        private InputAction _player1PlaceBrick;
        
        // Player 2 inputs
        private InputAction _player2MoveBrick;
        private InputAction _player2RotateBrick;
        private InputAction _player2PlaceBrick;

        private void Awake()
        {
            
            _startGame = player1Actions["StartGame"];
            
            _player1MoveBrick = player1Actions["MoveBrick"];
            _player1RotateBrick = player1Actions["RotateBrick"];
            _player1PlaceBrick = player1Actions["PlaceBrick"];

            _player2MoveBrick = player2Actions["MoveBrick"];
            _player2RotateBrick = player2Actions["RotateBrick"];
            _player2PlaceBrick = player2Actions["PlaceBrick"];
        }

        private void OnEnable()
        {
            // Subscribes methods to input Actions
            _startGame.performed += OnGameStarted;
            _player1MoveBrick.performed += OnPlayer1BrickMoved;
            _player1RotateBrick.performed += OnPlayer1BrickRotated;
            _player1PlaceBrick.performed += OnPlayer1BrickPlaced;
            _player2MoveBrick.performed += OnPlayer2BrickMoved;
            _player2RotateBrick.performed += OnPlayer2BrickRotated;
            _player2PlaceBrick.performed += OnPlayer2BrickPlaced;
            
            //Enables input maps
            player1Actions.Enable();
            player2Actions.Enable();
        }

        private void OnDisable()
        {
            // Unsubscribes methods to input Actions
            _startGame.performed -= OnGameStarted;
            _player1MoveBrick.performed -= OnPlayer1BrickMoved;
            _player1RotateBrick.performed -= OnPlayer1BrickRotated;
            _player1PlaceBrick.performed -= OnPlayer1BrickPlaced;
            _player2MoveBrick.performed -= OnPlayer2BrickMoved;
            _player2RotateBrick.performed -= OnPlayer2BrickRotated;
            _player2PlaceBrick.performed -= OnPlayer2BrickPlaced;
            
            // Disables input maps
            player1Actions.Disable();
            player2Actions.Disable();
        }

        // Method for starting the game
        private static void OnGameStarted(InputAction.CallbackContext context)
        {
            Core.EventManager.gameStarted?.Invoke();
        }
        
        // Sets movement direction of player 1 brick
        private static void OnPlayer1BrickMoved(InputAction.CallbackContext context)
        {
            var movementDirection = context.ReadValue<float>();
            Core.EventManager.player1BrickMoved?.Invoke(movementDirection);
        }
        
        // Sets rotation direction of player 1 brick
        private static void OnPlayer1BrickRotated(InputAction.CallbackContext context)
        {
            var rotationDirection = context.ReadValue<float>();
            Core.EventManager.player1BrickRotated?.Invoke(rotationDirection);
        }
        
        // Unused - instant drop of brick not implemented
        private static void OnPlayer1BrickPlaced(InputAction.CallbackContext context)
        {
            Core.EventManager.player1BrickPlaced?.Invoke();
        }

        // Sets movement direction of player 2 brick
        private static void OnPlayer2BrickMoved(InputAction.CallbackContext context)
        {
            var movementDirection = context.ReadValue<float>();
            Core.EventManager.player2BrickMoved?.Invoke(movementDirection);
        }
        
        // Sets rotation direction of player 2 brick
        private static void OnPlayer2BrickRotated(InputAction.CallbackContext context)
        {
            var rotationDirection = context.ReadValue<float>();
            Core.EventManager.player2BrickRotated?.Invoke(rotationDirection);
        }
        
        // Unused - instant drop of brick not implemented
        private static void OnPlayer2BrickPlaced(InputAction.CallbackContext context)
        {
            Core.EventManager.player2BrickPlaced?.Invoke();
        }
    }
}
