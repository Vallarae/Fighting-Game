using System;
using Player.Base.Controller;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : UnityEditor.Editor {
        private PlayerController _player;
        
        private SerializedProperty _walkSpeedField;
        private SerializedProperty _runSpeedField;
        private SerializedProperty _accelerationField;
        private SerializedProperty _decelerationField;
        private SerializedProperty _dashTowardsField;
        private SerializedProperty _dashAwayField;
        private SerializedProperty _jumpForceField;
        private SerializedProperty _dashCooldownField;
        
        private SerializedProperty _healthField;
        
        private SerializedProperty _damageField;
        private SerializedProperty _hitboxField;
        private SerializedProperty _attacksField;

        private SerializedProperty _recoveryFramesField;
        private SerializedProperty _recoveryFramesAfterComboField;

        private Int32 _tab;

        private void OnEnable() {
            _player = (PlayerController) target;
            
            _walkSpeedField = serializedObject.FindProperty(nameof(_player.walkSpeed));
            _runSpeedField = serializedObject.FindProperty(nameof(_player.runSpeed));
            _accelerationField = serializedObject.FindProperty(nameof(_player.acceleration));
            _decelerationField = serializedObject.FindProperty(nameof(_player.deceleration));
            _dashTowardsField = serializedObject.FindProperty(nameof(_player.dashTowards));
            _dashAwayField = serializedObject.FindProperty(nameof(_player.dashAway));
            _jumpForceField = serializedObject.FindProperty(nameof(_player.jumpForce));
            _dashCooldownField = serializedObject.FindProperty(nameof(_player.dashCooldown));
            
            _healthField = serializedObject.FindProperty(nameof(_player.health));
            
            _damageField = serializedObject.FindProperty(nameof(_player.damage));
            _hitboxField = serializedObject.FindProperty(nameof(_player.hitbox));
            _attacksField = serializedObject.FindProperty(nameof(_player.attacks));
            
            _recoveryFramesField = serializedObject.FindProperty(nameof(_player.recoveryFrames));
            _recoveryFramesAfterComboField = serializedObject.FindProperty(nameof(_player.recoveryFramesAfterCombo));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            GUI.enabled = true;

            _tab = GUILayout.Toolbar(_tab, new string[] { "Movement", "Health", "Damage", "Stun" });

            switch (_tab) {
                case 0:
                    EditorGUILayout.PropertyField(_walkSpeedField);
                    EditorGUILayout.PropertyField(_runSpeedField);
                    EditorGUILayout.PropertyField(_accelerationField);
                    EditorGUILayout.PropertyField(_decelerationField);
                    EditorGUILayout.PropertyField(_dashTowardsField);
                    EditorGUILayout.PropertyField(_dashAwayField);
                    EditorGUILayout.PropertyField(_jumpForceField);
                    EditorGUILayout.PropertyField(_dashCooldownField);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(_healthField);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(_damageField);
                    EditorGUILayout.PropertyField(_hitboxField);
                    EditorGUILayout.PropertyField(_attacksField);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(_recoveryFramesField);
                    EditorGUILayout.PropertyField(_recoveryFramesAfterComboField);
                    break;
            }
            
            EditorGUILayout.Space(20);
            
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
