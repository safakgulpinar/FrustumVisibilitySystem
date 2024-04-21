using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class VisibilitySubject : MonoBehaviour
    {
        private Renderer CachedRenderer { get; set; }
        public Vector3 Center { get; private set; }
        public float Radius { get; private set; }

        [field: SerializeField] public FrustumVisibilityManager.VisibilityType VisibilityType { get; private set; }

        private void Awake()
        {
            CachedRenderer = GetComponent<Renderer>();
            UpdateBoundingSphere();
        }

        private void UpdateBoundingSphere()
        {
            if (CachedRenderer == null) return;
            Center = CachedRenderer.bounds.center;
            Radius = CachedRenderer.bounds.extents.magnitude; // Extents.magnitude gives the radius of the bounding sphere
        }

        public void SetVisibility(bool isVisible)
        {
            if (CachedRenderer != null) CachedRenderer.enabled = isVisible;
        }

        public void SetShadowCasting(bool isCasting)
        {
            if (CachedRenderer != null) CachedRenderer.shadowCastingMode = isCasting ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}