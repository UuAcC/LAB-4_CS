using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace LAB_4
{
    internal class Graph
    {
        public int vertexCount;
        public List<int>[] list;
        public List<Point> vertexes;
        Random rnd;
        public Graph(int n)
        {
            rnd = new Random();
            vertexCount = n;
            list = new List<int>[n];
            vertexes = new List<Point>(n);
            for (int i = 0; i < n; i++)
                list[i] = new List<int>();
        }

        public void addEdge(int v1, int v2)
        {
            list[v1].Add(v2);
            list[v2].Add(v1);
        }

        public void GeneratePlanar(int n)
        {
            for (int i = 0; i < n; i++)
                this.addEdge(i, (i + 1) % n);
            if (n <= 3) return;

            List<int> polygon = new List<int>(n);
            for (int i = 0; i < n; i++) { polygon.Add(i); }

            DividePolygon(this, polygon);
        }

        private void DividePolygon(Graph g, List<int> polygon)
        {
            if (polygon.Count <= 3) return;
            int index = rnd.Next(2, polygon.Count - 1);
            g.addEdge(polygon[0], polygon[index]);

            List<int> p1 = polygon.GetRange(0, index + 1);
            List<int> p2 = polygon.GetRange(index, polygon.Count - index);
            p2.Add(polygon[0]);

            DividePolygon(g, p1);
            DividePolygon(g, p2);
        }
    }
}
