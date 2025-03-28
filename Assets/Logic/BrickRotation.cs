using UnityEngine;

namespace Tetris.Core
{
    /// <summary>
    /// Handles smooth brick rotation calculations
    /// </summary>
    public class BrickRotation : MonoBehaviour
    {
        public BrickRotationParameters player1Brick = new();
        public BrickRotationParameters player2Brick = new();
        
        public class BrickRotationParameters
        {
            // Current and target rotation angles, time for FixedUpdate calculations
            private Quaternion _fromAngle;
            private Quaternion _toAngle;
            private int _angles;
            private float _timeSpent = 1;

            // Determines if brick should still rotate
            public bool WhetherToRotateBrick => _timeSpent < 1;

            // Sets and rounds up angle value
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

            // Sets current brick rotation
            public void SetCurrentBrickRotation(Transform brickTransform)
            {
                _fromAngle = brickTransform.rotation;
            }

            // Sets target brick rotation
            public void SetTargetBrickRotation(Transform transform)
            {
                _toAngle = Quaternion.Euler(transform.eulerAngles + Vector3.forward * _angles);
            }

            // Updates timeSpent flag and grabs next rotation angle step
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
