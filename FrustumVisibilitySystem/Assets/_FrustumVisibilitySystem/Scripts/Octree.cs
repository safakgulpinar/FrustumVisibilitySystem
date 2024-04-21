using System.Collections.Generic;
using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class Octree
    {
        private Bounds _bounds;
        private readonly List<VisibilitySubject> _subjects;
        private readonly Octree[] _children;

        public Octree(Bounds boundary)
        {
            _bounds = boundary;
            _subjects = new List<VisibilitySubject>();
            _children = new Octree[8];
        }

        public void Insert(VisibilitySubject subject)
        {
            if (!_bounds.Contains(subject.transform.position)) return;

            if (_children[0] == null)
            {
                if (_subjects.Count < 10)
                {
                    _subjects.Add(subject);
                    return;
                }
                Subdivide();
            }

            foreach (var child in _children)
            {
                if (!child._bounds.Contains(subject.transform.position)) continue;
                child.Insert(subject);
                return;
            }
        }


        private void Subdivide()
        {
            // Subdivide the octree into 8 children and initialize them
            var size = _bounds.size / 2;
            var center = _bounds.center;

            for (var i = 0; i < 8; i++)
            {
                var newCenter = center;
                newCenter.x += ((i & 4) == 0 ? -1 : 1) * size.x / 2;
                newCenter.y += ((i & 2) == 0 ? -1 : 1) * size.y / 2;
                newCenter.z += ((i & 1) == 0 ? -1 : 1) * size.z / 2;
                var newBounds = new Bounds(newCenter, size);
                _children[i] = new Octree(newBounds);
            }
        }

        public List<VisibilitySubject> Query(Bounds queryBounds)
        {
            var results = new List<VisibilitySubject>();
            if (!_bounds.Intersects(queryBounds))
            {
                return results;
            }

            foreach (var subject in _subjects)
            {
                if (queryBounds.Contains(subject.transform.position))
                {
                    results.Add(subject);
                }
            }

            // Recursively query child nodes if they exist
            if (_children[0] == null) return results;
            foreach (var child in _children)
            {
                results.AddRange(child.Query(queryBounds));
            }

            return results;
        }


        public IEnumerable<VisibilitySubject> GetAllSubjects()
        {
            foreach (var subject in _subjects)
            {
                yield return subject;
            }

            // If there are children, get all subjects from them
            if (_children[0] == null) yield break;
            foreach (var child in _children)
            {
                foreach (var childSubject in child.GetAllSubjects())
                {
                    yield return childSubject;
                }
            }
        }
        
        public void DrawAllBounds()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);

            if (_children[0] == null) return;
            foreach (var child in _children)
            {
                child.DrawAllBounds();
            }
        }

    }
}