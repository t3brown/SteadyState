// See https://aka.ms/new-console-template for more information

using SteadyState;
using SteadyState.Interfaces;
using SteadyState.Test.Models;

Console.WriteLine("Steady State Test!");
List<Vertex> vertices = new List<Vertex>()
{
    new Vertex()
    {
        Id = 1, VoltNom = 330, PowerRe = 20, PowerIm = 30
    },
    new Vertex()
    {
        Id = 2, VoltNom = 330, PowerRe = 60, PowerIm = 30
    },
    new Vertex()
    {
        Id = 3, VoltNom = 330, IsBasic = true
    },
};
List<Edge> edges = new List<Edge>()
{
    new Edge()
    {
        Id = 1, V1 = vertices[2], V2 = vertices[0], R = 10, X = 50, B = 10
    },
    new Edge()
    {
        Id = 2, V1 = vertices[0], V2 = vertices[1], R = 20, X = 60, B = 20
    },
    new Edge()
    {
        Id = 3, V1 = vertices[2], V2 = vertices[1], R = 30, X = 70, B = 30
    }
};



CalculateSteadyState.Calculate(vertices, edges, 0.001f);





foreach (var vertex in vertices)
{
    Console.WriteLine($"{vertex.VoltRe}, {vertex.VoltIm}");
}

Console.ReadKey();
