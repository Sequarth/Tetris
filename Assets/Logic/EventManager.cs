using System;
using UnityEngine;

namespace Tetris.Core
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance) return _instance;
                
                _instance = new GameObject("EventManager").AddComponent<EventManager>();
                _instance.name = _instance.GetType().ToString();
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }
        
        public static Action gameStarted;
        
        public static Action<int> player1BrickSpawned;
        public static Action<GameObject> player1BrickInstantiated;
        public static Action<float> player1BrickMoved;
        public static Action<float> player1BrickRotated;
        public static Action player1BrickPlaced;
        public static Action player1BrickStopped;
        public static Action player1BrickAttached;
        public static Action player1BricksCleared;
        public static Action player1EndTurn;
        
        public static Action<int> player2BrickSpawned;
        public static Action<GameObject> player2BrickInstantiated;
        public static Action<float> player2BrickMoved;
        public static Action<float> player2BrickRotated;
        public static Action player2BrickPlaced;
        public static Action player2BrickStopped;
        public static Action player2BrickAttached;
        public static Action player2BricksCleared;
        public static Action player2EndTurn;
    }
}