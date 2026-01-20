using System;
using System.Collections.Generic;
using Player.Base.Attacks.Base.Validator.Base;

namespace Player.Base.Attacks.Base.Validator {
    public struct InputValidator {
        public List<DirectionValidator> directions;
        public ButtonValidation button;
        public int frames;
        
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
    }
}