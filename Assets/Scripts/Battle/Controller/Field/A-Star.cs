using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle.Controller.Field
{
    internal readonly struct PathNode
    {
        public PathNode(Vector2Int position, Vector2Int target, double traverseDistance)
        {
            Position = position;
            TraverseDistance = traverseDistance;
            double heuristicDistance = (position - target).DistanceEstimate();
            EstimatedTotalCost = traverseDistance + heuristicDistance;
        }

        public Vector2Int Position { get; }
        public double TraverseDistance { get; }
        public double EstimatedTotalCost { get; }
    }
    
    static class Vector2IntExtensions
    {
        public static double DistanceEstimate(this Vector2Int vector)
        {
            int linearSteps = Math.Abs(Math.Abs(vector.y) - Math.Abs(vector.x));
            int diagonalSteps = Math.Max(Math.Abs(vector.y), Math.Abs(vector.x)) - linearSteps;
            return linearSteps + Math.Sqrt(2) * diagonalSteps;
        }
    }

    internal static class NodeExtensions
    {
        private static readonly (Vector2Int position, double cost)[] NeighboursTemplate = {
            (new Vector2Int(1, 0), 1),
            (new Vector2Int(0, 1), 1),
            (new Vector2Int(-1, 0), 1),
            (new Vector2Int(0, -1), 1),
            (new Vector2Int(1, 1), Math.Sqrt(2)),
            (new Vector2Int(1, -1), Math.Sqrt(2)),
            (new Vector2Int(-1, 1), Math.Sqrt(2)),
            (new Vector2Int(-1, -1), Math.Sqrt(2))
        };

        public static void Fill(this PathNode[] buffer, PathNode parent, Vector2Int target)
        {
            int i = 0;
            foreach ((Vector2Int position, double cost) in NeighboursTemplate)
            {
                Vector2Int nodePosition = position + parent.Position;
                double traverseDistance = parent.TraverseDistance + cost;
                buffer[i++] = new PathNode(nodePosition, target, traverseDistance);
            }
        }
    }

    internal interface IBinaryHeap<in TKey, T>
    {
        void Enqueue(T item);
        T Dequeue();
        void Clear();
        bool TryGet(TKey key, out T value);
        void Modify(T value);
        int Count { get; }
    }

    internal class BinaryHeap<TKey, T> : IBinaryHeap<TKey, T> where TKey : IEquatable<TKey>
    {
        private readonly IDictionary<TKey, int> map;
        private readonly IList<T> collection;
        private readonly IComparer<T> comparer;
        private readonly Func<T, TKey> lookupFunc;

        public BinaryHeap(IComparer<T> comparer, Func<T, TKey> lookupFunc, int capacity)
        {
            this.comparer = comparer;
            this.lookupFunc = lookupFunc;
            collection = new List<T>(capacity);
            map = new Dictionary<TKey, int>(capacity);
        }

        public int Count => collection.Count;

        public void Enqueue(T item)
        {
            collection.Add(item);
            int i = collection.Count - 1;
            map[lookupFunc(item)] = i;
            while (i > 0)
            {
                int j = (i - 1) / 2;

                if (comparer.Compare(collection[i], collection[j]) <= 0)
                    break;

                Swap(i, j);
                i = j;
            }
        }

        public T Dequeue()
        {
            if (collection.Count == 0) return default;

            T result = collection.First();
            RemoveRoot();
            map.Remove(lookupFunc(result));
            return result;
        }

        public void Clear()
        {
            collection.Clear();
            map.Clear();
        }

        public bool TryGet(TKey key, out T value)
        {
            if (!map.TryGetValue(key, out int index))
            {
                value = default;
                return false;
            }

            value = collection[index];
            return true;
        }

        public void Modify(T value)
        {
            if (!map.TryGetValue(lookupFunc(value), out int index))
                throw new KeyNotFoundException(nameof(value));

            collection[index] = value;
        }

        private void RemoveRoot()
        {
            collection[0] = collection.Last();
            map[lookupFunc(collection[0])] = 0;
            collection.RemoveAt(collection.Count - 1);

            var i = 0;
            while (true)
            {
                int largest = LargestIndex(i);
                if (largest == i)
                    return;

                Swap(i, largest);
                i = largest;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;
            map[lookupFunc(collection[i])] = i;
            map[lookupFunc(collection[j])] = j;
        }

        private int LargestIndex(int i)
        {
            int leftInd = 2 * i + 1;
            int rightInd = 2 * i + 2;
            int largest = i;

            if (leftInd < collection.Count && comparer.Compare(collection[leftInd], collection[largest]) > 0) largest = leftInd;

            if (rightInd < collection.Count && comparer.Compare(collection[rightInd], collection[largest]) > 0) largest = rightInd;

            return largest;
        }
    }

    public class Path : IPath
    {
        private const int MaxNeighbours = 8;
        private readonly PathNode[] neighbours = new PathNode[MaxNeighbours];

        private readonly int maxSteps;
        private readonly IBinaryHeap<Vector2Int, PathNode> frontier;
        private readonly HashSet<Vector2Int> ignoredPositions;
        private readonly List<Vector2Int> output;
        private readonly IDictionary<Vector2Int, Vector2Int> links;

        /// <summary>
        /// Creation of new path finder.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Path(int maxSteps = int.MaxValue, int initialCapacity = 0)
        {
            if (maxSteps <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSteps));
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));

            this.maxSteps = maxSteps;
            var comparer = Comparer<PathNode>.Create((a, b) => b.EstimatedTotalCost.CompareTo(a.EstimatedTotalCost));
            frontier = new BinaryHeap<Vector2Int, PathNode>(comparer, a => a.Position, initialCapacity);
            ignoredPositions = new HashSet<Vector2Int>();
            output = new List<Vector2Int>(initialCapacity);
            links = new Dictionary<Vector2Int, Vector2Int>(initialCapacity);
        }

        /// <summary>
        /// Calculate a new path between two points.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Calculate(Vector2Int start, Vector2Int target,
            IReadOnlyCollection<Vector2Int> obstacles,
            out IReadOnlyCollection<Vector2Int> path)
        {
            if (obstacles == null)
                throw new ArgumentNullException(nameof(obstacles));

            if (!GenerateNodes(start, target, obstacles))
            {
                path = Array.Empty<Vector2Int>();
                return false;
            }

            output.Clear();
            output.Add(target);

            while (links.TryGetValue(target, out target)) output.Add(target);
            output.Reverse();
            path = output;
            return true;
        }

        private bool GenerateNodes(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
        {
            frontier.Clear();
            ignoredPositions.Clear();
            links.Clear();

            frontier.Enqueue(new PathNode(start, target, 0));
            ignoredPositions.UnionWith(obstacles);
            var step = 0;
            while (frontier.Count > 0 && step++ <= maxSteps)
            {
                PathNode current = frontier.Dequeue();
                ignoredPositions.Add(current.Position);

                if (current.Position.Equals(target)) return true;

                GenerateFrontierNodes(current, target);
            }

            // All nodes analyzed - no path detected.
            return false;
        }

        private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
        {
            neighbours.Fill(parent, target);
            foreach (PathNode newNode in neighbours)
            {
                // Position is already checked or occupied by an obstacle.
                if (ignoredPositions.Contains(newNode.Position)) continue;

                // Node is not present in queue.
                if (!frontier.TryGet(newNode.Position, out PathNode existingNode))
                {
                    frontier.Enqueue(newNode);
                    links[newNode.Position] = parent.Position;
                }

                // Node is present in queue and new optimal path is detected.
                else if (newNode.TraverseDistance < existingNode.TraverseDistance)
                {
                    frontier.Modify(newNode);
                    links[newNode.Position] = parent.Position;
                }
            }
        }
    }
}