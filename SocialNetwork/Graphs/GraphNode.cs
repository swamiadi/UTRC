using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GraphCollection
{
    [Serializable]
    public class GraphNode<T>
    {
       
        public readonly ConcurrentBag<Neighbour> Neighbours;
        public bool Visited = false;
      
        public T Value;
        public int TentativeDistance;

        public GraphNode(T value)
        {
            Value = value;
            Neighbours = new ConcurrentBag<Neighbour>();
        }

        public void AddNeighbour(GraphNode<T> graphNode, int distance)
        {
            Neighbours.Add(new Neighbour(graphNode, distance));
            graphNode.Neighbours.Add(new Neighbour(this, distance));
        }

        [Serializable]
        public struct Neighbour
        {
            public int Distance;
            public GraphNode<T> GraphNode;

            public Neighbour(GraphNode<T> graphNode, int distance)
            {
                GraphNode = graphNode;
                Distance = distance;
            }
        }
    }
}