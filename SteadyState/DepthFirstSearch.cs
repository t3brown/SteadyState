using SteadyState.Interfaces;
using static SteadyState.CalculateSteadyState;

namespace SteadyState
{
	public static class DepthFirstSearch
	{
		public static void DFS(IVertex? startVertex = null)
		{
			foreach (var vertex in _vertices)
			{
				vertex.IsConnected = false;
			}

			foreach (var edge in _edges)
			{
				edge.IsConnected = false;
			}

			if (startVertex != null)
			{
				DFSUtil(startVertex);
			}
		}

		private static void DFSUtil(IVertex vertex)
		{
			vertex.IsConnected = true;
			var edges = _edges.Where(o => o.V1Id == vertex.Id || o.V2Id == vertex.Id);
			foreach (var edge in edges)
			{
				var neighbor = _vertices.FirstOrDefault(o => !o.IsConnected && (o.Id == edge.V1Id || o.Id == edge.V2Id));
				if (neighbor != null)
				{
					if (!neighbor.IsGround)
					{
						/*if (neighbor.VoltNom is null && neighbor == edge.V2)
							neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U2 / edge.U1;
						if (neighbor.VoltNom is null && neighbor == edge.V1)
							neighbor.VoltNom = edge.U1 is null && edge.U2 is null ? vertex.VoltNom : vertex.VoltNom * edge.U1 / edge.U2;*/
					}

					DFSUtil(neighbor);
				}
				//var V1 = _vertices.FirstOrDefault(o => o.Id == edge.V1Id);
				//var V2 = _vertices.FirstOrDefault(o => o.Id == edge.V2Id);
				//if (edge.V1 == null || edge.V2 == null)
				//{
				//	return;
				//}

				if (edge.V1 is { IsConnected: true } || edge.V2 is { IsConnected: true })
				{
					edge.IsConnected = true;
				}
			}
		}
	}
}
