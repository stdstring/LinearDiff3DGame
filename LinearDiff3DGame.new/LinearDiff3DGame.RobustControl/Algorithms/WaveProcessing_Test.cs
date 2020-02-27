using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearDiff3DGame.RobustControl.Algorithms
{
    [TestFixture]
    public class WaveProcessing_Test
    {
        [Test]
        public void Process()
        {
            Int32 sum = 0;
            new WaveProcessing<Node>().Process(CreateNodes(),
                                               null,
                                               node => node.Neighbors,
                                               (parent, node) => sum += node.Value);
            Assert.AreEqual(1321, sum);
        }

        [Test]
        public void ProcessWithEqualityComparer()
        {
            Int32 sum = 0;
            new WaveProcessing<Node>().Process(CreateNodes(),
                                               null,
                                               node => node.Neighbors,
                                               new NodeIdEqualityComparer(),
                                               (parent, node) => sum += node.Value);
            Assert.AreEqual(1111, sum);
        }

        private static Node CreateNodes()
        {
            Node node7 = new Node {Id = 4, Value = 1000, Neighbors = new List<Node>()};
            Node node6 = new Node {Id = 3, Value = 100, Neighbors = new List<Node>()};
            Node node5 = new Node {Id = 3, Value = 100, Neighbors = new List<Node>()};
            Node node4 = new Node {Id = 3, Value = 100, Neighbors = new List<Node>()};
            Node node3 = new Node {Id = 2, Value = 10, Neighbors = new List<Node>()};
            Node node2 = new Node {Id = 2, Value = 10, Neighbors = new List<Node>()};
            Node node1 = new Node {Id = 1, Value = 1, Neighbors = new List<Node>()};
            node6.Neighbors.AddRange(new[] {node3, node4, node5, node7});
            node5.Neighbors.AddRange(new[] {node2, node3, node4, node6, node7});
            node4.Neighbors.AddRange(new[] {node2, node5, node6, node7});
            node3.Neighbors.AddRange(new[] {node1, node2, node5, node6});
            node2.Neighbors.AddRange(new[] {node1, node3, node4, node5});
            node1.Neighbors.AddRange(new[] {node2, node3});
            return node1;
        }

        private class Node
        {
            public Int32 Id { get; set; }
            public Int32 Value { get; set; }
            public List<Node> Neighbors { get; set; }
        }

        private class NodeIdEqualityComparer : IEqualityComparer<Node>
        {
            public bool Equals(Node x, Node y)
            {
                return Equals(x.Id, y.Id);
            }

            public int GetHashCode(Node obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}