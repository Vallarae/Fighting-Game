using System.Collections.Generic;
using Player.Base.Controller;
using Player.Base.Utils;
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

        public GameObject inputTextPrefab;
        public RectTransform spawnPosition;

        [Space] 
        public GameObject directionDebug;
        public List<RectTransform> directionToPositions;
        public LineRenderer lineRenderer;
        
        private Input _latestInput;
        private Input _latestUsedInput;
        private InputReader _inputReader;
        private PlayerController _player;

        private GameObject _currentUsedInputText;
        
        private List<GameObject> _existingInputs;

        private void Start() {
            _inputReader = GetComponent<InputReader>();
            _player = GetComponent<PlayerController>();

            _existingInputs = new List<GameObject>();

            if (!showDebugMenu) {
                Destroy(debugMenu);
                Destroy(this);
            }
            
            if (ReferenceEquals(_inputReader, null)) Destroy(gameObject);
        }

        private void Update() {
            _latestInput = _inputReader.GetLastInput();

            
            if (!_player.InputReader.SameInput(_latestInput, _latestUsedInput)) {
                _currentUsedInputText = Instantiate(inputTextPrefab, spawnPosition);
                _latestUsedInput = _latestInput;

                if (_existingInputs.Count != 0) {
                    foreach (GameObject text in _existingInputs) {
                        text.transform.position -= new Vector3(0, 100, 0);
                    }
                }
            }

            if (ReferenceEquals(_currentUsedInputText, null)) {
                _currentUsedInputText = Instantiate(inputTextPrefab, spawnPosition);
            }

            TMP_Text[] texts = _currentUsedInputText.GetComponentsInChildren<TMP_Text>();

            texts[0].text = CreateText();
            texts[1].text = _latestInput.frames.ToString();

            if (!_existingInputs.Contains(_currentUsedInputText)) {
                _existingInputs.Add(_currentUsedInputText);
            }

            if (_existingInputs.Count == 20) {
                GameObject input = _existingInputs[0];
                _existingInputs.Remove(input);
                Destroy(input);
            }

            var recentInputs = _inputReader.GetRecentInputs();
            int inputCount = recentInputs.Count;

            int drawCount = Mathf.Min(inputCount, 5);
            lineRenderer.positionCount = drawCount;
            
            for (int i = 0; i < drawCount; i++) {
                int inputIndex = inputCount - 1 - i;

                Vector3 pos = DirectionUtils.GetLinePosition(recentInputs[inputIndex].direction);
                lineRenderer.SetPosition(i, pos);
            }

            directionDebug.transform.position = lineRenderer.gameObject.transform.position +
                                                GetPositionForDebug(_latestInput.direction);
        }

        private Vector3 GetPositionForDebug(int direction) {
            return direction switch {
                1 => new Vector3(-50, -50),
                2 => new Vector3(0, -50),
                3 => new Vector3(50, -50),
                4 => new Vector3(-50, 0),
                5 => new Vector3(0, 0),
                6 => new Vector3(50, 0),
                7 => new Vector3(-50, 50),
                8 => new Vector3(0, 50),
                9 => new Vector3(50, 50),
                _ => new(0, 0)
            };
        }

        //<Sprite=50>
        
        private string CreateText() {
            string text = _latestUsedInput.direction switch {
                1 => "<Sprite=38> ",
                2 => "<Sprite=39> ",
                3 => "<Sprite=40> ",
                4 => "<Sprite=41> ",
                5 => "<Sprite=42> ",
                6 => "<Sprite=43> ",
                7 => "<Sprite=44> ",
                8 => "<Sprite=45> ",
                9 => "<Sprite=50> ",
                _ => "<Sprite=0> "
            };

            if (_latestUsedInput.punchButtonDown) text += "<Sprite=35> ";
            if (_latestUsedInput.kickButtonDown) text += "<Sprite=33> ";
            if (_latestUsedInput.slashButtonDown) text += "<Sprite=36> ";
            if (_latestUsedInput.heavyButtonDown) text += "<Sprite=34> ";
            if (_latestUsedInput.dashButtonDown) text += "<Sprite=13> ";

            return text;
        }
    }
}