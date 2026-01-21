using System;
using System.Collections.Generic;
using Player.Base.Attacks.Base.Validator;
using Player.Base.Attacks.Base.Validator.Base;
using Player.Base.Controller;
using Player.Base.InputHandling;
using Player.Base.Utils;
using UnityEngine;
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
 *
 * Previous comment was regarding an old system, testing new one now :D
 * - V (20/01/26)
 *
 * The new system works really well :D
 * - V (21/01/26)
 */

namespace Player.Base.Attacks.Base {
    public class AttackResolver {
        private readonly PlayerController _player;

        private const int BaseInputScore = 100;
        private const int ComplexityBonus = 50;

        private const int InputBufferFrames = 6;     // QoL: leniency window
        private const int MaxInputFrames = 30;       // QoL: timeout per attack

        public Dictionary<Attack, InputValidator> attackInputs;
        public List<Attack> attacks;

        private bool _canAttack = true;
        private Input _inputUsedOnAttack;

        public AttackResolver(PlayerController player) {
            _player = player;
        }

        public void Initialise() {
            attacks = _player.attacks;
            attackInputs = new Dictionary<Attack, InputValidator>();

            foreach (Attack attack in attacks) {
                var validator = new InputValidator();

                foreach (int dir in attack.directionInputs) {
                    validator.directions.Add(new DirectionValidator {
                        direction = dir
                    });
                }

                validator.button = new ButtonValidation {
                    button = attack.button
                };

                attackInputs.Add(attack, validator);
            }
        }

        public void Tick() {
            IReadOnlyList<Input> recentInputs = _player.InputReader.GetRecentInputs();
            if (recentInputs.Count == 0) return;
            
            if (recentInputs.Count < 3) {
                return;
            }

            _canAttack = true;

            foreach (Attack attack in attacks) {
                InputValidator validator = attackInputs[attack];

                // Timeout reset (QoL)
                if (validator.frames > MaxInputFrames) {
                    validator.Reset();
                    continue;
                }

                // Process buffered inputs
                int start = Math.Max(0, recentInputs.Count - InputBufferFrames);

                for (int i = start; i < recentInputs.Count; i++) {
                    Input input = recentInputs[i];

                    if (input.lifeTime > MaxInputFrames) continue;

                    validator.TryValidateDirection(input.direction);
                    validator.TryValidateButton(input);
                }

                validator.frames++;
            }
        }

        public Attack Resolve() {
            ValidAttack bestAttack = null;

            if (!_canAttack) return null;

            foreach (Attack attack in attacks) {
                InputValidator validator = attackInputs[attack];

                if (!ContextValid(attack)) continue;
                if (!validator.IsValid()) continue;

                int score = CalculatePoints(attack) - validator.frames;

                if (bestAttack == null || score > bestAttack.points) {
                    bestAttack = new ValidAttack {
                        attack = attack,
                        points = score
                    };
                }
            }

            if (bestAttack != null) {
                attackInputs[bestAttack.attack].Invalidate();
                _player.InputReader.ClearAllButLastInputs();
                _player.InputReader.Pause();
                _canAttack = false;
                return bestAttack.attack;
            }

            return null;
        }

        private int CalculatePoints(Attack attack) {
            return BaseInputScore + ComplexityBonus * attack.directionInputs.Count;
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

        private int InputDirectionValidator(int direction) {
            return 5;
        }
        
        [Obsolete("This function is Obsolete, use InputDirectionValidator instead")]
        private Input InputDirectionValidation(Input old) {
            if (old.direction == 10) return old;
            if (_player.DirectionToOtherPlayer() == -1) return old;
            
            Vector2Int dir = DirectionUtils.NumpadToVector(old.direction);
            dir.x *= -1;
            int direction = DirectionUtils.GetDirection(dir);

            Input newInput = old;
            newInput.direction = direction;
            newInput.direction = DirectionUtils.GetDirection(dir);

            return newInput;
        }
    }
}