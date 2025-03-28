using UnityEngine;

namespace Tetris.Core
{
    public class BrickRotation : MonoBehaviour
    {
        public BrickRotationParameters player1Brick = new();
        public BrickRotationParameters player2Brick = new();
        
        [System.Serializable]
        public class BrickRotationParameters
        {
            private Quaternion _fromAngle;
            private Quaternion _toAngle;
            private int _angles;
            private float _timeSpent = 1;

            public bool WhetherToRotateBrick => _timeSpent < 1;

            public void AddAngles(float rotationDirection)
            {
                if (_angles is 360 or -360)
                {
                    _angles = 0;
                }
                if (rotationDirection == 1f)
                {
                    _angles += 90;
                }
                if (rotationDirection == -1f)
                {
                    _angles -= 90;
                }
            }

            public void SetCurrentBrickRotation(Transform brickTransform)
            {
                _fromAngle = brickTransform.rotation;
            }

            public void SetTargetBrickRotation(Transform transform)
            {
                _toAngle = Quaternion.Euler(transform.eulerAngles + Vector3.forward * _angles);
            }

            public Quaternion GetRotationFixedUpdate(float fixedTimeSpent)
            {
                _timeSpent += fixedTimeSpent;
                return Quaternion.Slerp(_fromAngle, _toAngle, _timeSpent);
            }
            
            public void ResetRotationTime()
            {
                _timeSpent = 0;
            }

            public void ResetRotationAngles()
            {
                _angles = 0;
            }
        }
        
        
        

    }
}
