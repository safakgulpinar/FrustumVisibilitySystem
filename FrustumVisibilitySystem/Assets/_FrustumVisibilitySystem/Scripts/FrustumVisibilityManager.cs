using System;
using System.Collections;
using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class FrustumVisibilityManager : MonoBehaviour
    {
        private Camera _mainCamera;
        private float _nextCheckTime = 0f;
        private Octree _rootOctree;
        
        private const int FrameInterval = 10;

        [SerializeField] private float checkInterval = 1f;
        [SerializeField] private Vector3 visibilityOffset = Vector3.zero;
        [SerializeField] private bool playerStopUpdateStop;
        [SerializeField] private bool octreeView;

        public enum VisibilityType
        {
            GameObject,
            Renderer,
            CastShadows
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rootOctree = new Octree(new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000)));
            var allSubjects = FindObjectsOfType<VisibilitySubject>();
            foreach (var subject in allSubjects)
            {
                _rootOctree.Insert(subject);
            }
        }

        private void Update()
        {
            if (!Player.Instance.IsMoving && playerStopUpdateStop) return;
            if (Time.time < _nextCheckTime) return;

            StartCoroutine(PerformVisibilityCheck());
            _nextCheckTime = Time.time + checkInterval;
        }

        private IEnumerator PerformVisibilityCheck()
        {
            yield return CheckVisibilityCoroutine();
        }

        private IEnumerator CheckVisibilityCoroutine()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
            AdjustPlanes(ref planes, visibilityOffset);

            var allSubjects = _rootOctree.GetAllSubjects();
            foreach (var subject in allSubjects)
            {
                var isVisible = IsObjectVisible(planes, subject);
                HandleVisibility(subject, isVisible);
                // Yield to spread processing over multiple frames if needed
                if (Time.frameCount % FrameInterval == 0)  // Adjust the modulus for performance tuning
                    yield return null;
            }
        }
        
        private static void HandleVisibility(VisibilitySubject subject, bool isVisible)
        {
            switch (subject.visibilityType)
            {
                case VisibilityType.GameObject:
                    subject.gameObject.SetActive(isVisible);
                    break;
                case VisibilityType.Renderer:
                    subject.SetVisibility(isVisible);
                    break;
                case VisibilityType.CastShadows:
                    subject.SetShadowCasting(isVisible);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subject.visibilityType), "Unsupported visibility type");
            }
        }


        private static bool IsObjectVisible(Plane[] planes, VisibilitySubject subject)
        {
            // Create a bounding sphere from the center and radius
            var sphere = new BoundingSphere(subject.Center, subject.Radius);
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(sphere.position, Vector3.one * (sphere.radius * 2)));
        }


        /// <summary>
        /// Adjusts the planes of the camera frustum according to the specified offset.
        /// This helps in managing visibility based on camera perspective adjustments.
        /// </summary>
        /// <param name="planes">The planes of the camera frustum to adjust.</param>
        /// <param name="offset">The offset to apply for adjustment.</param>
        private static void AdjustPlanes(ref Plane[] planes, Vector3 offset)
        {
            for (var i = 0; i < planes.Length; i++)
            {
                var normal = planes[i].normal;
                var distance = planes[i].distance;

                float adjustedOffset = 0;
                if (Mathf.Abs(normal.x) > 0.9) adjustedOffset = offset.x;
                else if (Mathf.Abs(normal.y) > 0.9) adjustedOffset = offset.y;
                else if (Mathf.Abs(normal.z) > 0.9) adjustedOffset = offset.z;

                planes[i] = new Plane(normal, distance - adjustedOffset);
            }
        }

        private void OnDrawGizmos()
        {
            if (octreeView && _rootOctree != null)
            {
                _rootOctree.DrawAllBounds();
            }
        }
    }
}
