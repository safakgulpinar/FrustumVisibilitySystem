using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class VisibilitySubject : MonoBehaviour
    {
        private Renderer _renderer;
        
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        
        /// <summary>
        ///   Set the visibility of the object
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisibility(bool isVisible)
        {
            if(_renderer != null) _renderer.enabled = isVisible;
        }
        
        /// <summary>
        ///  Set the shadow casting of the object
        /// </summary>
        /// /// <param name="isCasting"></param>
        public void SetShadowCasting(bool isCasting)
        {
            if(_renderer != null) _renderer.shadowCastingMode = isCasting ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        
        //Or you can use this method to set the visibility of the object
    }
}