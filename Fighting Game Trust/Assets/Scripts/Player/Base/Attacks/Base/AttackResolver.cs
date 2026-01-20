using System.Collections.Generic;
using Player.Base.Attacks.Base.Validator;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.Controller;
using Input = Player.Base.InputHandling.Input;

/*
 * While this script is working as intended, I think it would be good to add some more leniency to the attacks, as they feel jank right now
 * we can leave this for now because there is still more that needs to be added
 * - V (19/01/26)
 *
 * so ICL I'm recoding everything in here ;-;
 * - V (19/01/26)
 *
 * WHY DOES IT NOT WORK CORRECT
 * - V (20/01/26)
 */

namespace Player.Base.Attacks.Base {
    public class AttackResolver {
        private readonly PlayerController _player;

        private const int BaseInputScore = 100;
        private const int ComplexityBonus = 50;

        public AttackResolver(PlayerController player) {
            _player = player;
            Initialise();
        }

        public Dictionary<Attack, InputValidator> attackInputs;
        public List<Attack> attacks;

        private void Initialise() {
            attacks = _player.attacks;

            foreach (Attack attack in attacks) {
                InputValidator validator = new InputValidator();

                for (int i = 0; i < attack.directionInputs.Count; i++) {
                    DirectionValidator inputValidator = new DirectionValidator {
                        direction = attack.directionInputs[i]
                    };
                    validator.directions.Add(inputValidator);
                }
                
                ButtonValidation buttonValidation = new ButtonValidation {
                    button = attack.button
                };

                validator.button = buttonValidation;

                attackInputs.Add(attack, validator);
            }
        }

        public void Tick() {
            foreach (Attack attack in attacks) {
                InputValidator validator = attackInputs[attack];

                Input recentInput = _player.InputReader.GetRecentInputs()[^1];

                for (int i = 0; i < validator.directions.Count; i++) {
                    DirectionValidator directionValidator = validator.directions[i];

                    if (directionValidator.performed) continue;
                    
                    if (directionValidator.direction == recentInput.direction) {
                        directionValidator.performed = true;
                        validator.Validate(recentInput.direction);
                        
                        break;
                    }
                }

                bool rightButton = attack.button switch {
                    ButtonType.Punch => recentInput.punchButtonDown,
                    ButtonType.Kick => recentInput.kickButtonDown,
                    ButtonType.Slash => recentInput.slashButtonDown,
                    ButtonType.HeavySlash => recentInput.heavyButtonDown,
                    _ => false
                };
                
                ButtonValidation buttonValidation = validator.button;
                buttonValidation.performed = rightButton;
                
                validator.button = buttonValidation;
                validator.Validate();
                
                validator.frames++;
            }
        }

        public Attack Resolve() {
            List<ValidAttack> validAttacks = new List<ValidAttack>();
            
            for (int i = 0; i < attacks.Count; i++) {
                Attack attack =  attacks[i];
                InputValidator validator = attackInputs[attack];
                
                if (!ContextValid(attack)) continue;
                if (!validator.IsValid()) continue;

                ValidAttack validAttack = new ValidAttack {
                    attack = attack,
                    points = CalculatePoints(attack)
                };
                
                validAttacks.Add(validAttack);
            }
            
            if (validAttacks.Count == 0) return null;

            ValidAttack chosenAttack = new ValidAttack();

            foreach (ValidAttack attack in validAttacks) {
                if (ReferenceEquals(chosenAttack.attack, null))
                    chosenAttack = attack;

                if (chosenAttack.points < attack.points)
                    chosenAttack = attack;
            }

            return chosenAttack.attack;
        }

        private int CalculatePoints(Attack attack) {
            int score = BaseInputScore + ComplexityBonus * attack.directionInputs.Count;
            
            return score;
        }

        private bool ContextValid(Attack attack) {
            return attack.RequiredStance switch {
                AttackStance.Any => true,
                AttackStance.Standing => !_player.IsAerial() && !_player.IsCrouching(),
                AttackStance.Crouching => _player.IsCrouching(),
                AttackStance.Aerial => _player.IsAerial(),
                _ => false
            };
        }
    }
}