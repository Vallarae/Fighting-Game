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
 */

namespace Player.Base.Attacks.Base {
    public class AttackResolver {
        private readonly PlayerController _player;

        public AttackResolver(PlayerController player) {
            _player = player;
        }

        public IAttack Resolve() {
            if (_player.fms.currentState is AttackState) return null;
            IReadOnlyList<Input> buffer = _player.inputReader.GetRecentInputs();

            foreach (IAttack attack in _player.attacks) {
                if (!ContextValid(attack)) continue;
                if (MatchesLenient(attack, buffer)) return attack;
            }

            return null;
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

        private bool MatchesLenient(IAttack attack, IReadOnlyList<Input> buffer) {
            List<Input> required = attack.RequiredInputs();
            int maxGap = attack.MaxInputGap();
            int tolerance = attack.DirectionTolerance();

            int bufferIndex = buffer.Count - 1;

            for (int req = required.Count - 1; req >= 0; req--) {
                int framesWaited = 0;
                bool found = false;

                while (bufferIndex >= 0 && framesWaited <= maxGap) {
                    if (InputMatches(InputDirectionValidation(required[req]), buffer[bufferIndex], tolerance)) {
                        found = true;
                        bufferIndex--;
                        break;
                    }

                    framesWaited += Mathf.Max(1, buffer[bufferIndex].frames);
                    bufferIndex--;
                }

                if (!found) return false;
            }

            return true;
        }

        //This function is in charge of changing the directions of an input for directional based attacks
        private Input InputDirectionValidation(Input old) {
            if (old.direction == 10) return old;
            if (_player.DirectionToOtherPlayer() == -1) return old;
            Vector2Int dir = DirectionUtils.NumpadToVector(old.direction);
            dir.x *= -1;
            int direction = DirectionUtils.GetDirection(dir);
            Input newInput = old;
            newInput.direction = direction;

            return newInput;
        }

        private bool InputMatches(Input required, Input actual, int tolerance) {
            if (!DirectionUtils.DirectionMatches(
                    required.direction,
                    actual.direction,
                    tolerance))
                return false;

            if (required.punchButtonDown && !actual.punchButtonDown) return false;
            if (required.kickButtonDown && !actual.kickButtonDown) return false;
            if (required.slashButtonDown && !actual.slashButtonDown) return false;
            return !(required.heavyButtonDown && !actual.heavyButtonDown);
        }
    }
}