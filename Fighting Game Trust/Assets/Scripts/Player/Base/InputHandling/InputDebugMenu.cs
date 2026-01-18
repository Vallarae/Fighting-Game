using TMPro;
using UnityEngine;


/*
 * Don't even bother optimizing this rn, will probably be replaced later (16/01/25)
 */
namespace Player.Base.InputHandling {
    public class InputDebugMenu : MonoBehaviour {
        public bool showDebugMenu;
        public GameObject debugMenu;

        [Space]

        public TMP_Text directionText;
        public TMP_Text frameText;

        [Space] 
        
        public TMP_Text punchText;
        public TMP_Text kickText;
        public TMP_Text slashText;
        public TMP_Text heavySlashText;
        
        private Input _latestInput;
        private InputReader _inputReader;

        private void Start() {
            _inputReader = GetComponent<InputReader>();

            if (!showDebugMenu) {
                Destroy(debugMenu);
                Destroy(this);
            }
            
            if (ReferenceEquals(_inputReader, null)) Destroy(gameObject);
        }
        
        private void Update() {
            _latestInput = _inputReader.GetLastInput();

            directionText.text = _latestInput.direction.ToString();
            frameText.text = _latestInput.frames.ToString();

            punchText.text = "Punching: " + _latestInput.punchButtonDown;
            kickText.text = "Kicking: " + _latestInput.kickButtonDown;
            slashText.text = "Slash: " + _latestInput.slashButtonDown;
            heavySlashText.text = "Heavy Slash: " + _latestInput.heavyButtonDown;
        }
    }
}