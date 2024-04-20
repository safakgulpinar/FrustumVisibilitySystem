using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class Player : MonoBehaviour
    {
        public static Player Instance = null; 
        [SerializeField] private float speed = 5.0f;
        
        private bool _isMoving = false;

        #region Properties

        public bool IsMoving => _isMoving;

        #endregion

        private void Awake()
        {
            if (Instance == null)                
                Instance = this;            
            else if (Instance != this)                
                Destroy(gameObject);                
            DontDestroyOnLoad(gameObject);            
        }
        private void Update()
        {
            var moveHorizontal = Input.GetAxis("Horizontal");
            var moveVertical = Input.GetAxis("Vertical");
            
            _isMoving = moveHorizontal != 0 || moveVertical != 0;
            
            if (!_isMoving) return;
            
            var movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            transform.Translate(movement * (speed * Time.deltaTime));
        }
    }
}
