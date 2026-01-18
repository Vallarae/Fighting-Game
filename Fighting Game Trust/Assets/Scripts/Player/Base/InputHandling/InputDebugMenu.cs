using Player.Base.Controller;
using TMPro;
using UnityEngine;

/*
 * Don't even bother optimizing this rn, will probably be replaced later (16/01/26)
 * FUCK THE NAMING CONVENTIONS, I AM ADDING DIFFERENT SHIT IN HERE (18/01/26)
 */
namespace Player.Base.InputHandling {
    public class InputDebugMenu : MonoBehaviour {
        public bool showDebugMenu;
        public GameObject debugMenu;

        [Space]

        public TMP_Text directionText;
        public TMP_Text speedText;
        public TMP_Text frameText;

        [Space] 
        
        public TMP_Text punchText;
        public TMP_Text kickText;
        public TMP_Text slashText;
        public TMP_Text heavySlashText;
        
        private Input _latestInput;
        private InputReader _inputReader;
        private PlayerController _player;

        private void Start() {
            _inputReader = GetComponent<InputReader>();
            _player = GetComponent<PlayerController>();

            if (!showDebugMenu) {
                Destroy(debugMenu);
                Destroy(this);
            }
            
            if (ReferenceEquals(_inputReader, null)) Destroy(gameObject);
        }
        
        private void Update() {
            _latestInput = _inputReader.GetLastInput();

            directionText.text = "Direction: " + _latestInput.direction;
            speedText.text = "Speed: " + _player.rigidbody.linearVelocity.x;
            frameText.text = "Frame: " +_latestInput.frames;

            punchText.text = "Punching: " + _latestInput.punchButtonDown;
            kickText.text = "Kicking: " + _latestInput.kickButtonDown;
            slashText.text = "Slash: " + _latestInput.slashButtonDown;
            heavySlashText.text = "Heavy Slash: " + _latestInput.heavyButtonDown;
        }
    }
}