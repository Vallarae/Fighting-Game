using System;
using System.Collections.Generic;
using Player.Base.Attacks.Base;
using Player.Base.Attacks.Damage;
using Player.Base.Attacks.DefaultAttacks;
using Player.Base.HitboxLookup;
using Player.Base.InputHandling;
using Player.Base.Interfaces;
using Player.Base.PlayerStates;
using Player.Base.StateMachineSystem;
using UnityEngine;

namespace Player.Base.Controller {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputReader))]
    public class PlayerController : MonoBehaviour, IDamageable, IHealth {
        public StateMachine fms { get; private set; }
        public InputReader inputReader { get; private set; }
        public new Rigidbody rigidbody { get; private set; }

        public float walkSpeed;
        public float runSpeed;
        public float dashTowards;
        public float dashAway;
        public float acceleration;
        public float deceleration;
        public float jumpForce;
        public int health;
        public int recoveryFrames;
        public int recoveryFramesAfterCombo; //this will be used later when combo-ing is implemented properly
        public int dashCooldown;
        public int damage;

        public GameObject hitbox;

        public List<MonoBehaviour> test;
        public List<IAttack> attacks;
        public AttackResolver attackResolver;
        
        public MovementState movement;
        public AerialState aerial;
        public CrouchState crouch;
        private StunnedState _stunned;

        public GameObject otherPlayer { get; private set; }
        
        private int _currentHealth;

        private void Awake() {
            fms = new StateMachine();
            attackResolver = new AttackResolver(this);
            inputReader = GetComponent<InputReader>();
            rigidbody = GetComponent<Rigidbody>();
            
            movement = new MovementState(this);
            aerial = new  AerialState(this);
            crouch = new CrouchState(this);
            _stunned = new StunnedState(this);
            
            currentHealth = maxHealth();

            attacks = new List<IAttack>();
            
            attacks.Add(new DefaultKick(this));
            attacks.Add(new TestComplexAttack(this));
            
            PlayerRegistry.AddPlayer(GetComponent<Collider>(), this);
            FindOtherPlayer();
        }

        private void Start() {
            fms.ChangeState(movement);
        }

        private void FixedUpdate() {
            fms.Tick();
        }

        private void OnDrawGizmos() {
            FindOtherPlayer();
            Gizmos.color = DirectionToOtherPlayer() == 1 ?  Color.red : Color.green;
            
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, otherPlayer.transform.position);
        }

        //this can be optimized
        private void FindOtherPlayer() {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject playerOne = players[0];
            GameObject playerTwo = players[1];

            otherPlayer = ReferenceEquals(playerOne, gameObject) ? playerTwo : playerOne;
        }

        private void TakeKnockback(float force) {
            rigidbody.AddForce(Vector3.right * DirectionToOtherPlayer() * force, ForceMode.Impulse);
        }

        public int DirectionToOtherPlayer() {
            Vector3 directionVector = (transform.position -  otherPlayer.transform.position).normalized;
            
            return Mathf.RoundToInt(directionVector.x);
        }
        
        public void Damage(int amount) {
            health -= amount;
            if (fms.currentState != _stunned) fms.ChangeState(_stunned);
            else _stunned.ExtraHit();
            TakeKnockback(5f);
            Debug.Log($"damaged {amount}");
        }

        public int maxHealth() {
            return health;
        }

        public int currentHealth { get; set; }

        public bool IsAerial() => fms.currentState == aerial;
        public bool IsCrouching() => fms.currentState == crouch;

        //this function is intended for non-moving hitboxes, for moving ones use animations
        public void SpawnHitbox(Vector3 position, Vector3 size, IAttack attack) {
            Damager damager = Instantiate(hitbox, position, Quaternion.identity).GetComponent<Damager>();
            damager.gameObject.transform.localScale = size;
            damager.Initialise(this, attack);
        }
    }
}