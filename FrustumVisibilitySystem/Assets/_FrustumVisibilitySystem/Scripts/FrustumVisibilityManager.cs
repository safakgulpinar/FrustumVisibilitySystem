using System;
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
    

        private enum VisibilityType
        {
            GameObject,
            Renderer,
            CastShadows
        }

        [SerializeField] private VisibilityType visibilityTypeToCheck = VisibilityType.GameObject;

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

            CheckVisibility();
            _nextCheckTime = Time.time + checkInterval;
        }

        private void CheckVisibility()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
            AdjustPlanes(ref planes, visibilityOffset);

            var allSubjects = _rootOctree.GetAllSubjects();
            foreach (var subject in allSubjects)
            {
                var isVisible = IsObjectVisible(planes, subject);
                switch (visibilityTypeToCheck)
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
        }

        private static bool IsObjectVisible(Plane[] planes, VisibilitySubject subject)
        {
            return subject.CachedRenderer != null && GeometryUtility.TestPlanesAABB(planes, subject.CachedRenderer.bounds);
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
