using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class VisibilitySubject : MonoBehaviour
    {
        public Renderer CachedRenderer { get; private set; }
        
        [field: SerializeField] public FrustumVisibilityManager.VisibilityType visibilityType { get; private set; }
        private void Awake()
        {
            CachedRenderer = GetComponent<Renderer>();
        }
        
        /// <summary>
        ///   Set the visibility of the object
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisibility(bool isVisible)
        {
            if(CachedRenderer != null) CachedRenderer.enabled = isVisible;
        }
        
        /// <summary>
        ///  Set the shadow casting of the object
        /// </summary>
        /// /// <param name="isCasting"></param>
        public void SetShadowCasting(bool isCasting)
        {
            if(CachedRenderer != null) CachedRenderer.shadowCastingMode = isCasting ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        
        //Or you can use this method to set the visibility of the object
    }
}