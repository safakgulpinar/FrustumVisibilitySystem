using System;
using System.Collections.Generic;
using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class FrustumVisibilityManager : MonoBehaviour
    {
        private enum  VisibilityType
        {
            GameObject,
            Renderer,
            CastShadows
        }
        
        [SerializeField]private float checkInterval = 1f; // Interval in seconds to check visibility
        [SerializeField]private Vector3 visibilityOffset = new (0f, 0f, 0f); // Offset to expand or contract the camera's view frustum on x, y, and z
        [SerializeField]private bool playerStopUpdateStop;
        [SerializeField]private VisibilityType visibilityTypeToCheck = VisibilityType.GameObject;
        
        private Camera _mainCamera;
        private float _nextCheckTime = 0f;
        private readonly List<VisibilitySubject> _visibilitySystemObjects = new ();

        private void Awake()
        {
            _mainCamera = Camera.main;
            _visibilitySystemObjects.AddRange(FindObjectsOfType<VisibilitySubject>());
        }

        private void Start()
        {
            CheckVisibility();
        }

        private void Update()
        {
            if(!Player.Instance.IsMoving && playerStopUpdateStop) return;
            if (!(Time.time >= _nextCheckTime)) return;
            CheckVisibility();
            _nextCheckTime = Time.time + checkInterval;
        }

        // Check the visibility of the objects
        private void CheckVisibility()
        {
            foreach (var visibilitySystemObject in _visibilitySystemObjects)
            {
                if (visibilityTypeToCheck == VisibilityType.GameObject)
                {
                    visibilitySystemObject.gameObject.SetActive(IsObjectVisible(_mainCamera, visibilitySystemObject));
                }

                if (visibilityTypeToCheck == VisibilityType.Renderer)
                {
                    visibilitySystemObject.SetVisibility(IsObjectVisible(_mainCamera, visibilitySystemObject));
                }
                
                if (visibilityTypeToCheck == VisibilityType.CastShadows)
                {
                    visibilitySystemObject.SetShadowCasting(IsObjectVisible(_mainCamera, visibilitySystemObject));
                }
            }
        }

        // Check if the object is visible
        private bool IsObjectVisible(Camera camera, VisibilitySubject visibilitySubject)
        {
            var subjectCachedRenderer = visibilitySubject.CachedRenderer;
            if (subjectCachedRenderer == null) return false;

            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            AdjustPlanes(ref planes, visibilityOffset);
            return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
        }

        // Adjust the planes based on the offset
        private static void AdjustPlanes(ref Plane[] planes, Vector3 offset)
        {
            for (var i = 0; i < planes.Length; i++)
            {
                var normal = planes[i].normal;
                var distance = planes[i].distance;

                // Adjust the plane distance based on the offset
                // Assuming plane normals are aligned with world axes (which they are not in general):
                float adjustedOffset = 0;
                if (Mathf.Abs(normal.x) > 0.9) adjustedOffset = offset.x;
                else if (Mathf.Abs(normal.y) > 0.9) adjustedOffset = offset.y;
                else if (Mathf.Abs(normal.z) > 0.9) adjustedOffset = offset.z;

                planes[i] = new Plane(normal, distance - adjustedOffset);
            }
        }
    }
}
