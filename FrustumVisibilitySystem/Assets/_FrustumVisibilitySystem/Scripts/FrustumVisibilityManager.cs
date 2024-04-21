using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class FrustumVisibilityManager : MonoBehaviour
    {
        private Camera _mainCamera;
        private float _nextCheckTime = 0f;
        private Octree _rootOctree;

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
            var task = CheckVisibilityAsync();
            while (!task.IsCompleted)
                yield return null;
        }

        private Task CheckVisibilityAsync()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
            AdjustPlanes(ref planes, visibilityOffset);
            
            var allSubjects = _rootOctree.GetAllSubjects();
            foreach (var subject in allSubjects)
            {
                var isVisible = IsObjectVisible(planes, subject);
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
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Task.CompletedTask;
        }

        private static bool IsObjectVisible(Plane[] planes, VisibilitySubject subject)
        {
            // Create a bounding sphere from the center and radius
            var sphere = new BoundingSphere(subject.Center, subject.Radius);
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(sphere.position, Vector3.one * (sphere.radius * 2)));
        }


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
