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
					DFSUtil(neighbor);
				}

				if (edge.V1 is { IsConnected: true } || edge.V2 is { IsConnected: true })
				{
					edge.IsConnected = true;
				}
			}
		}
	}
}
