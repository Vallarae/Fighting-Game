using System;
using System.Collections.Generic;
using Player.Base.Controller;
using Player.Base.PlayerStates;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

/*
 * While this script is working as intended, I think it would be good to add some more leniency to the attacks, as they feel qutie jank right now
 * we can leave this for now because there is still more that needs to be added
 * - V (19/01/26)
 *
 * so ICL I'm recoding everything in here ;-;
 * - V (19/01/26)
 */

namespace Player.Base.Attacks.Base {
    public class AttackResolver {
        private readonly PlayerController _player;

        private const int BaseInputScore = 100;
        private const int ComplexityBonus = 50;
        private const int ButtonScore = 200;
        private const int ButtonBufferFrames = 8;

        public AttackResolver(PlayerController player) {
            _player = player;
        }

        public IAttack Resolve() {
            if (_player.fms.currentState is AttackStance) return null;

            IReadOnlyList<Input> buffer = _player.inputReader.GetRecentInputs();

            IAttack bestAttack = null;
            int bestScore = int.MinValue;

            foreach (IAttack attack in _player.attacks) {
                if (!ContextValid(attack)) continue;

                if (TryMatchAttack(attack, buffer, out int score)) {
                    if (score > bestScore) {
                        bestScore = score;
                        bestAttack = attack;
                    }
                }
            }

            return bestAttack;
        }

        private bool ContextValid(IAttack attack) {
            return attack.RequiredStance() switch {
                AttackStance.Any => true,
                AttackStance.Standing => !_player.IsAerial() && !_player.IsCrouching(),
                AttackStance.Crouching => _player.IsCrouching(),
                AttackStance.Aerial => _player.IsAerial(),
                _ => false
            };
        }

        private bool TryMatchAttack(IAttack attack, IReadOnlyList<Input> buffer, out int score) {
            score = 0;
            
            List<Input> required = attack.RequiredInputs();
            int maxGap = attack.MaxInputGap();
            int tolerance = attack.DirectionTolerance();

            int bufferIndex = buffer.Count - 1;

            for (int req = required.Count - 1; req >= 0; req--) {
                Input requiredInput = InputDirectionValidation(required[req]);

                bool found = false;
                int framesWaited = 0;

                for (int i = bufferIndex; i >= 0 && framesWaited <= maxGap; i--) {
                    if (DirectionOnlyMatch(requiredInput, buffer[i], tolerance)) {
                        found = true;
                        bufferIndex = i - 1;
                        
                        score += BaseInputScore;
                        score -= framesWaited;
                    }
                    
                    framesWaited += Mathf.Max(1, buffer[i].frames);
                }

                if (!found) return false;
            }

            if (!MatchBufferedButton(required, buffer, bufferIndex, out int buttonScore)) return false;

            score += buttonScore;

            score += required.Count * ComplexityBonus;

            return true;
        }

        private bool DirectionOnlyMatch(Input required, Input actual, int tolerance) {
            return DirectionUtils.DirectionMatches(
                required.direction,
                actual.direction,
                tolerance
            );
        }

        private bool MatchBufferedButton(List<Input> required, IReadOnlyList<Input> buffer, int startIndex, out int score) {
            score = 0;

            Input buttonRequired = required.Find(i => 
                i.punchButtonDown ||
                i.kickButtonDown ||
                i.slashButtonDown ||
                i.heavyButtonDown
            );

            if (ReferenceEquals(buttonRequired, null)) return true;

            int framesWaited = 0;

            for (int i = startIndex; i >= 0 && framesWaited <= ButtonBufferFrames; i--) {
                if (ButtonMatches(buttonRequired, buffer[i])) {
                    score += ButtonScore;
                    score -= framesWaited;
                    return true;
                }

                framesWaited += Math.Max(1, buffer[i].frames);
            }

            return false;
        }

        private bool ButtonMatches(Input required, Input actual) {
            if (required.punchButtonDown && !actual.punchButtonDown) return false;
            if (required.kickButtonDown && !actual.kickButtonDown) return false;
            if (required.slashButtonDown && !actual.slashButtonDown) return false;
            return !(required.heavyButtonDown && !actual.heavyButtonDown);
        }

        private Input InputDirectionValidation(Input old) {
            if (old.direction == 10) return old;
            if (_player.DirectionToOtherPlayer() == -1) return old;
            
            Vector2Int dir = DirectionUtils.NumpadToVector(old.direction);
            dir.x *= -1;

            Input newInput = old;
            newInput.direction = DirectionUtils.GetDirection(dir);

            return newInput;
        }
    }
}