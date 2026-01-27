using UnityEngine;

namespace Scene {
    public class CameraController : MonoBehaviour {
        public float minZoom = 40f;
        public float maxZoom = 60f;
        public float minDistance = 3f;
        public float maxDistance = 10f;

        [Space] 
        
        public float positionSmooth = 5f;
        public float zoomSmooth = 5f;

        private Transform _playerOne;
        private Transform _playerTwo;
        
        private Camera _camera;

        private void Awake() {
            _camera = GetComponent<Camera>();
            GetPlayers();
        }

        private void GetPlayers() {
            GameObject[] players  = GameObject.FindGameObjectsWithTag("Player");
            _playerOne = players[0].transform;
            _playerTwo = players[1].transform;
        }

        private void LateUpdate() {
            if (!_playerOne || !_playerTwo) return;

            Vector3 midPoint = (_playerOne.position + _playerTwo.position) * 0.5f;

            Vector3 targetPosition = new Vector3(
                midPoint.x,
                transform.position.y,
                transform.position.z
            );
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionSmooth);
            
            float distance = Vector3.Distance(_playerOne.position, _playerTwo.position);
            float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);
            
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetZoom, Time.deltaTime * zoomSmooth);
        }
    }
}