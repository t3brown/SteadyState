using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;
using static SteadyState.CalculateSteadyState;

namespace SteadyState
{
    public static class DepthFirstSearch
    {
        //if (neighbor.VoltNom is null && neighbor == edge.V2)
        //    neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U2 / edge.U1;
        //if (neighbor.VoltNom is null && neighbor == edge.V1)
        //    neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U1 / edge.U2;

        public static void DFS(IVertex startVertex)
        {
            foreach (IVertex vertex in _vertices)
            {
                vertex.IsConnected = false;
            }

            foreach (IEdge edge in _edges)
            {
                edge.IsConnected = false;
            }
            DFSUtil(startVertex);
        }

        private static void DFSUtil(IVertex vertex)
        {
            vertex.IsConnected = true;
            IEnumerable<IEdge> edges = _edges.Where(o => o.V1 == vertex || o.V2 == vertex);
            foreach (IEdge edge in edges)
            {
                var neighbor = _vertices.FirstOrDefault(o => !o.IsConnected && (o == edge.V1 || o == edge.V2));
                if (neighbor != null)
                {
                    DFSUtil(neighbor);
                }
                if (edge.V1.IsConnected || edge.V2.IsConnected)
                    edge.IsConnected = true;
            }
        }
    }
}
