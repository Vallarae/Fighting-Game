using System.Collections.Generic;
using Player.Base.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Base.InputHandling {
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour {
        
        #region Variables

        private Vector2 _directionInput;
        private bool _punchButtonDown;
        private bool _slashButtonDown;
        private bool _heavyButtonDown;
        private bool _kickButtonDown;
        private bool _dashButtonDown;
        
        private readonly List<Input> _recentInputs = new List<Input>();

        #endregion
        
        #region Unity Methods
        
        private void FixedUpdate() {
            if (_recentInputs.Count == 0) {
                _recentInputs.Add(CreateInput());
                return;
            }

            Input newInput = CreateInput();
            Input lastInput = _recentInputs[^1];

            if (SameInput(newInput, lastInput) && lastInput.frames < 99) {
                lastInput.frames++;
                _recentInputs[^1] = lastInput;
                return;
            }

            if (!SameInput(newInput, lastInput)) {
                newInput.frames = 1;
                _recentInputs.Add(newInput);
            }
        }

        #endregion
        
        #region InputMethods
        public void OnDirection(InputAction.CallbackContext cc) {
            _directionInput = cc.ReadValue<Vector2>();
        }

        public void OnPunch(InputAction.CallbackContext cc) {
            _punchButtonDown = cc.performed;
        }

        public void OnSlash(InputAction.CallbackContext cc) {
            _slashButtonDown = cc.performed;
        }

        public void OnHeavySlash(InputAction.CallbackContext cc) {
            _heavyButtonDown = cc.performed;
        }

        public void OnKick(InputAction.CallbackContext cc) {
            _kickButtonDown = cc.performed;
        }

        public void OnDash(InputAction.CallbackContext cc) {
            _dashButtonDown = cc.performed;
        }
        
        #endregion
        
        #region Setters

        public void ClearInputList() {
            _recentInputs.Clear();
        }
        
        #endregion
        
        #region Getters
        private Input CreateInput() {
            return new Input {
                direction = DirectionUtils.GetDirection(_directionInput),
                frames = 0,
                punchButtonDown = _punchButtonDown,
                slashButtonDown = _slashButtonDown,
                heavyButtonDown = _heavyButtonDown,
                kickButtonDown = _kickButtonDown,
                dashButtonDown = _dashButtonDown
            };
        }

        public Input GetLastInput() {
            if (_recentInputs.Count == 0) return CreateInput();
            return _recentInputs[^1];
        }
        
        public IReadOnlyList<Input> GetRecentInputs() => _recentInputs;
        
        #endregion
        
        #region Checkers

        private bool SameInput(Input a, Input b) {
            return a.direction == b.direction
                   && a.punchButtonDown == b.punchButtonDown
                   && a.slashButtonDown == b.slashButtonDown
                   && a.heavyButtonDown == b.heavyButtonDown
                   && a.kickButtonDown == b.kickButtonDown
                   && a.dashButtonDown == b.dashButtonDown;
        }
        
        #endregion
    }
    
    public struct Input {
        public int direction;
        public int frames;
        
        public bool punchButtonDown;
        public bool slashButtonDown;
        public bool heavyButtonDown;
        public bool kickButtonDown;

        public bool dashButtonDown;
    }
}