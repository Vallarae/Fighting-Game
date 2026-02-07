using Player.Base.Attacks.Base.Validator.Base;
using System.Collections.Generic;
using Player.Base.Controller;
using Player.Base.InputHandling;

namespace Player.Base.Attacks.Base.Validator {
    public class InputValidator {
        public List<DirectionValidator> directions;
        public ButtonValidation button;
        public int frames;
        private PlayerController _player;
        
        public InputValidator(PlayerController _player) {
            directions = new List<DirectionValidator>();
            this._player = _player;
        }
        
        public void Validate(int direction) {
            for (int i = 0; i < directions.Count; i++) {
                if (directions[i].direction == direction) {
                    if (directions[i].performed) continue;
                    DirectionValidator validator = directions[i];
                    validator.performed = true;
                    directions[i] =  validator;
                    return;
                }
            }
        }

        public void Validate() {
            button.performed = true;
        }

        public void Invalidate() {
            for (int i = 0; i < directions.Count; i++) {
                DirectionValidator direction = directions[i];
                direction.performed = false;
                directions[i] = direction;
            }

            button.performed = false;
        }

        public bool IsValid() {
            for (int i = 0; i < directions.Count; i++) {
                if (!directions[i].performed) return false;
            }
            
            if (!button.performed) return false;
            
            return true;
        }
        
        public void TryValidateDirection(int direction) {
            for (int i = 0; i < directions.Count; i++) {
                if (directions[i].direction == 10) directions[i].performed = true;
                if (directions[i].performed) continue;
                if (!directions[i].performed && InputDirectionValidator(directions[i].direction) == direction) {
                    if (!IsPreviousInputValid(i)) return;
                    directions[i].performed = true;
                    return;
                }
            }
        }

        public void TryValidateButton(Input input) {
            bool pressed = button.button switch {
                ButtonType.Punch => input.punchButtonDown,
                ButtonType.Kick => input.kickButtonDown,
                ButtonType.Slash => input.slashButtonDown,
                ButtonType.HeavySlash => input.heavyButtonDown,
                _ => false
            };

            if (pressed)
                button.performed = true;
        }

        public void Reset() {
            frames = 0;

            for (int i = 0; i < directions.Count; i++)
                directions[i].performed = false;

            button.performed = false;
        }

        private bool IsPreviousInputValid(int index) {
            if (index == 0)
                return true;
            
            return directions[index - 1].performed;
        }

        
        private int InputDirectionValidator(int old) {
            if (old == 10) return old;
            if (_player.DirectionToOtherPlayer() == -1) return old;

            int value = old switch {
                6 => 4,
                4 => 6,
                1 => 3,
                3 => 1,
                7 => 9,
                9 => 7,
                _ => old
            };
            
            return value;
        }
    }
}