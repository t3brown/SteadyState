using SteadyState.Interfaces;
using static SteadyState.CalculateSteadyState;

namespace SteadyState
{
	public static class DepthFirstSearch
	{
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
			IEnumerable<IEdge> edges = _edges.Where(o => o.V1Id == vertex.Id || o.V2Id == vertex.Id);
			foreach (IEdge edge in edges)
			{
				var neighbor = _vertices.FirstOrDefault(o => !o.IsConnected && (o.Id == edge.V1Id || o.Id == edge.V2Id));
				if (neighbor != null)
				{
					if (!neighbor.IsGround)
					{
						if (neighbor.VoltNom is null && neighbor == edge.V2)
							neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U2 / edge.U1;
						if (neighbor.VoltNom is null && neighbor == edge.V1)
							neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U1 / edge.U2;
					}

					DFSUtil(neighbor);
				}
				var V1 = _vertices.FirstOrDefault(o => o.Id == edge.V1Id);
				var V2 = _vertices.FirstOrDefault(o => o.Id == edge.V2Id);
				if (V1.IsConnected || V2.IsConnected)
					edge.IsConnected = true;
			}
		}
	}
}
