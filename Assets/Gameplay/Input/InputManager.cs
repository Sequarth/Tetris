using UnityEngine;
using UnityEngine.InputSystem;

namespace Tetris.Gameplay.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionMap player1Actions;
        [SerializeField] private InputActionMap player2Actions;

        private InputAction _startGame;
        
        private InputAction _player1MoveBrick;
        private InputAction _player1RotateBrick;
        private InputAction _player1PlaceBrick;
        
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
            _startGame.performed += OnGameStarted;
            _player1MoveBrick.performed += OnPlayer1BrickMoved;
            _player1RotateBrick.performed += OnPlayer1BrickRotated;
            _player1PlaceBrick.performed += OnPlayer1BrickPlaced;
            _player2MoveBrick.performed += OnPlayer2BrickMoved;
            _player2RotateBrick.performed += OnPlayer2BrickRotated;
            _player2PlaceBrick.performed += OnPlayer2BrickPlaced;
            
            player1Actions.Enable();
            player2Actions.Enable();
        }

        private void OnDisable()
        {
            _startGame.performed -= OnGameStarted;
            _player1MoveBrick.performed -= OnPlayer1BrickMoved;
            _player1RotateBrick.performed -= OnPlayer1BrickRotated;
            _player1PlaceBrick.performed -= OnPlayer1BrickPlaced;
            _player2MoveBrick.performed -= OnPlayer2BrickMoved;
            _player2RotateBrick.performed -= OnPlayer2BrickRotated;
            _player2PlaceBrick.performed -= OnPlayer2BrickPlaced;
            
            player1Actions.Disable();
            player2Actions.Disable();
        }

        private void OnGameStarted(InputAction.CallbackContext context)
        {
            Core.EventManager.gameStarted?.Invoke();
        }
        
        private void OnPlayer1BrickMoved(InputAction.CallbackContext context)
        {
            var movementDirection = context.ReadValue<float>();
            Core.EventManager.player1BrickMoved?.Invoke(movementDirection);
        }

        private void OnPlayer1BrickRotated(InputAction.CallbackContext context)
        {
            var rotationDirection = context.ReadValue<float>();
            Core.EventManager.player1BrickRotated?.Invoke(rotationDirection);
        }

        private void OnPlayer1BrickPlaced(InputAction.CallbackContext context)
        {
            Core.EventManager.player1BrickPlaced?.Invoke();
        }

        private void OnPlayer2BrickMoved(InputAction.CallbackContext context)
        {
            var movementDirection = context.ReadValue<float>();
            Core.EventManager.player2BrickMoved?.Invoke(movementDirection);
        }

        private void OnPlayer2BrickRotated(InputAction.CallbackContext context)
        {
            var rotationDirection = context.ReadValue<float>();
            Core.EventManager.player2BrickRotated?.Invoke(rotationDirection);
        }

        private void OnPlayer2BrickPlaced(InputAction.CallbackContext context)
        {
            Core.EventManager.player2BrickPlaced?.Invoke();
        }
    }
}
