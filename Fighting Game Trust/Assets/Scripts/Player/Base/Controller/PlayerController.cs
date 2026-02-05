using System.Collections.Generic;
using Player.Base.Attacks.Base;
using Player.Base.Attacks.Damage;
using Player.Base.Attacks.DefaultAttacks;
using Player.Base.Attacks.DefaultAttacks.TestAttacks;
using Player.Base.HitboxLookup;
using Player.Base.InputHandling;
using Player.Base.Interfaces;
using Player.Base.PlayerStates;
using Player.Base.StateMachineSystem;
using Player.Base.Utils;
using UnityEngine;
using Input = Player.Base.InputHandling.Input;

namespace Player.Base.Controller {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputReader))]
    public class PlayerController : MonoBehaviour, IDamageable, IHealth {
        public StateMachine Fms { get; private set; }
        public InputReader InputReader { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

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

        public Damager hitbox;
        
        public List<Attack> attacks;
        public AttackResolver attackResolver;
        
        public MovementState movement;
        public AerialState aerial;
        public CrouchState crouch;
        public StunnedState stunned;
        public BlockingState blocking;

        private GameObject OtherPlayer { get; set; }
        
        private int _currentHealth;

        private void Awake() {
            Fms = new StateMachine();

            attacks = new List<Attack>();
            
            InitializeAttacks();
            
            attackResolver = new AttackResolver(this);

            InputReader = GetComponent<InputReader>();
            Rigidbody = GetComponent<Rigidbody>();

            movement = new MovementState(this);
            aerial = new AerialState(this);
            crouch = new CrouchState(this);
            stunned = new StunnedState(this);
            blocking = new BlockingState(this);
            
            CurrentHealth = MaxHealth();

            PlayerRegistry.AddPlayer(GetComponent<Collider>(), this);
            FindOtherPlayer();

            attackResolver.Initialise();
        }

        public void InitializeAttacks() {
            attacks.Add(new BaseKick(this));
            attacks.Add(new ConflictingAttackTest(this));
            attacks.Add(new AdvancedAttackTest(this));
        }

        private void Start() {
            Fms.ChangeState(movement);
        }

        private void FixedUpdate() {
            Fms.Tick();
            attackResolver.Tick(!(Fms.CurrentState is AttackStance));
        }

        private void OnDrawGizmos() {
            FindOtherPlayer();
            Gizmos.color = DirectionToOtherPlayer() == 1 ?  Color.red : Color.green;
            
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, OtherPlayer.transform.position);
        }

        //this can be optimized
        private void FindOtherPlayer() {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject playerOne = players[0];
            GameObject playerTwo = players[1];

            OtherPlayer = ReferenceEquals(playerOne, gameObject) ? playerTwo : playerOne;
        }

        private void TakeKnockback(float force) {
            Rigidbody.AddForce(Vector3.right * DirectionToOtherPlayer() * force, ForceMode.Impulse);
        }

        public int DirectionToOtherPlayer() {
            Vector3 directionVector = (transform.position -  OtherPlayer.transform.position).normalized;
            
            return Mathf.RoundToInt(directionVector.x);
        }
        
        public void Damage(int amount, Attack attack) {
            if (CanBlock(attack)) {
                Fms.ChangeState(blocking);
                Debug.Log("Blocking");
                return;
            }
            health -= amount;
            if (Fms.CurrentState != stunned) Fms.ChangeState(stunned);
            else stunned.ExtraHit();
            TakeKnockback(5f);
        }

        public bool CanBlock(Attack attack) {
            Input input = InputReader.GetLastInput(); 
            Vector2Int playerDirection = DirectionUtils.NumpadToVector(input.direction);
            int otherDirection = DirectionToOtherPlayer();
            
            Debug.Log($"Player Direction:  {playerDirection.x}, AttackDirection: {otherDirection}, Low Blocking: {attack.LowBlockable}, Crouched: {IsCrouching()}");
            
            if (otherDirection == playerDirection.x) {
                if (attack.LowBlockable && IsCrouching()) return true;
                if (!attack.LowBlockable && !IsCrouching()) return true;
            }

            return false;
        }

        public int MaxHealth() {
            return health;
        }

        public int CurrentHealth { get; set; }

        public bool IsAerial() => Fms.CurrentState == aerial;
        public bool IsCrouching() => Fms.CurrentState == crouch;
        
        public void SpawnHitbox(Vector3 position, Vector3 size, Attack attack) {
            Damager damager = Instantiate(hitbox, position, Quaternion.identity);
            damager.gameObject.transform.localScale = size;
            damager.Initialise(this, attack);
        }
    }
}