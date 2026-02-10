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
        private bool _duskButtonDown;
        private bool _romanCancelButtonDown;
        private bool _dashButtonDown;

        private bool _pauseInputReading;

        private readonly List<Input> _recentInputs;

        #endregion
        
        #region Unity Methods
        
        private void FixedUpdate() {
            if (_recentInputs.Count == 0) {
                _recentInputs.Add(CreateInput());
                return;
            }
            
            for (int i = 0; i < _recentInputs.Count; i++) {
                Input input = _recentInputs[i];
                input.lifeTime++;
                _recentInputs[i] = input;
            }

            if (_pauseInputReading) return;

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

        public void OnDusk(InputAction.CallbackContext cc) {
            _duskButtonDown = cc.performed;
        }

        public void OnRomanCancel(InputAction.CallbackContext cc) {
            _romanCancelButtonDown = cc.performed;
        }

        public void OnKick(InputAction.CallbackContext cc) {
            _kickButtonDown = cc.performed;
        }

        public void OnDash(InputAction.CallbackContext cc) {
            _dashButtonDown = cc.performed;
        }
        
        #endregion
        
        #region Setters

        public void ClearAllButLastInputs(bool flagAsAttackInput = false) {
            Input lastInput = _recentInputs[^1];
            lastInput.isAttackInput = flagAsAttackInput;
            _recentInputs.Clear();
            _recentInputs.Add(lastInput);
        }
        public void Pause() {
            _pauseInputReading = true;
        }

        public void Resume() {
            _pauseInputReading = false;
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
                dashButtonDown = _dashButtonDown,
                romanCancelButtonDown = _romanCancelButtonDown,
                duskButtonDown =  _duskButtonDown
            };
        }

        public Input GetLastInput() {
            if (_recentInputs.Count == 0) return CreateInput();
            return _recentInputs[^1];
        }
        
        public IReadOnlyList<Input> GetRecentInputs() => _recentInputs;
        
        #endregion
        
        #region Checkers

        public bool SameInput(Input a, Input b) {
            return a.direction == b.direction
                   && a.punchButtonDown == b.punchButtonDown
                   && a.slashButtonDown == b.slashButtonDown
                   && a.heavyButtonDown == b.heavyButtonDown
                   && a.kickButtonDown == b.kickButtonDown
                   && a.dashButtonDown == b.dashButtonDown
                   && a.duskButtonDown == b.duskButtonDown
                   && a.romanCancelButtonDown == b.romanCancelButtonDown;
        }
        
        #endregion
    }
    
    public struct Input {
        public int direction;
        public int frames;
        public int lifeTime;
        
        public bool punchButtonDown;
        public bool slashButtonDown;
        public bool heavyButtonDown;
        public bool kickButtonDown;

        public bool duskButtonDown;
        public bool romanCancelButtonDown;

        public bool dashButtonDown;

        public bool isAttackInput;
    }
}