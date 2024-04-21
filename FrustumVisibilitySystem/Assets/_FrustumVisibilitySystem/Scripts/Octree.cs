using System.Collections.Generic;
using UnityEngine;

namespace _FrustumVisibilitySystem.Scripts
{
    public class Octree
    {
        private Bounds _bounds;
        private List<VisibilitySubject> _subjects;
        private Octree[] _children;

        private const int MaxSubjectsBeforeSubdivide = 10;
        private const int NumChildren = 8;

        public Octree(Bounds boundary)
        {
            _bounds = boundary;
            _subjects = new List<VisibilitySubject>();
            _children = null;
        }

        public void Insert(VisibilitySubject subject)
        {
            if (!_bounds.Contains(subject.transform.position)) return;

            if (_children != null)
            {
                // Insert into children if already subdivided
                InsertIntoChildren(subject);
                return;
            }

            _subjects.Add(subject);
            if (_subjects.Count >= MaxSubjectsBeforeSubdivide)
            {
                Subdivide();
            }
        }

        private void InsertIntoChildren(VisibilitySubject subject)
        {
            foreach (var child in _children)
            {
                if (child._bounds.Contains(subject.transform.position))
                {
                    child.Insert(subject);
                    return;
                }
            }
        }

        private void Subdivide()
        {
            _children = new Octree[NumChildren];
            var size = _bounds.size / 2;
            var center = _bounds.center;

            for (var i = 0; i < NumChildren; i++)
            {
                var newCenter = center;
                newCenter.x += ((i & 4) == 0 ? -1 : 1) * size.x / 2;
                newCenter.y += ((i & 2) == 0 ? -1 : 1) * size.y / 2;
                newCenter.z += ((i & 1) == 0 ? -1 : 1) * size.z / 2;
                var newBounds = new Bounds(newCenter, size);
                _children[i] = new Octree(newBounds);
            }

            // Move existing subjects to new children
            List<VisibilitySubject> oldSubjects = new List<VisibilitySubject>(_subjects);
            _subjects.Clear();
            foreach (var subject in oldSubjects)
            {
                InsertIntoChildren(subject);
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

            if (_children == null) return results;
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

            if (_children == null) yield break;
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

            if (_children == null) return;
            foreach (var child in _children)
            {
                child.DrawAllBounds();
            }
        }
    }
}
