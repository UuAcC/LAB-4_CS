using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LAB_3_WPF;



namespace LAB_4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd;
        Ellipse clicked;
        Graph graph;
        List<Ellipse> points;
        List<Line> lines;
        Dictionary<int, List<Line>> list_inc;
        Dictionary<int, List<bool>> list_reverses;
        int size;
        public MainWindow()
        {
            InitializeComponent();
            Window1 w = new Window1();
            bool? res = w.ShowDialog();
            if (res == true)
            {
                int n = Convert.ToInt32(w.textBox.Text);
                size = n >= 2 ? n : 2;
                rnd = new Random();
                clicked = null;
                mainWin.MouseUp += set_point;
                mainWin.MouseMove += move_point;
                graph = new Graph(size);
                points = new List<Ellipse>();
                list_inc = new Dictionary<int, List<Line>>();
                list_reverses = new Dictionary<int, List<bool>>();
                lines = new List<Line>();
                for (int i = 0; i < size; i++)
                {
                    list_inc.Add(i, new List<Line>());
                    list_reverses.Add(i, new List<bool>());
                }
                startGraph(size);
            }
            else this.Close();
        }

        private void moveLines(int changed)
        {
            for (int i = 0; i < graph.list[changed].Count; i++)
            {
                int ind = lines.IndexOf(list_inc[changed][i]);
                if (!list_reverses[changed][i])
                {
                    list_inc[changed][i].X1 = graph.vertexes[changed].X;
                    list_inc[changed][i].Y1 = graph.vertexes[changed].Y;
                }
                else
                {
                    list_inc[changed][i].X2 = graph.vertexes[changed].X;
                    list_inc[changed][i].Y2 = graph.vertexes[changed].Y;
                }
                lines[ind] = list_inc[changed][i];
            }
        }

        private void startGraph(int count)
        {
            double center_x, center_y, R = 250;
            double seg = Math.PI * 2 / count, a = 0, x, y;
            center_x = center_y = 425;

            graph.GeneratePlanar(size);
            for (int i = 0; i < count; i++)
            {
                x = R * Math.Cos(a);
                y = R * Math.Sin(a);
                Point point = new Point(x + center_x, y + center_y);
                graph.vertexes.Add(point);
                a += seg;
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < graph.list[i].Count; j++)
                {
                    int ind = graph.list[i][j];
                    if (i < ind)
                    {
                        Point p1 = graph.vertexes[i];
                        Point p2 = graph.vertexes[ind];
                        Line l = new Line();
                        l.Stroke = Brushes.Black;

                        l.X1 = p1.X + 10; l.Y1 = p1.Y + 10;
                        l.X2 = p2.X + 10; l.Y2 = p2.Y + 10;
                        list_inc[i].Add(l); list_reverses[i].Add(false);
                        list_inc[ind].Add(l); list_reverses[ind].Add(true);
                        lines.Add(l);
                        grid.Children.Add(l);
                    }
                }
            }

            a = 0;
            for (int i = 0; i < count; i++)
            {
                x = R * Math.Cos(a);
                y = R * Math.Sin(a);
                Ellipse c = new Ellipse();
                c.Height = c.Width = 20;
                c.Margin = new Thickness(x + center_x, y + center_y, 0, 0);
                c.Stroke = Brushes.Black;
                c.Fill = new SolidColorBrush(Color.FromRgb(225, 70, 4));
                //c.Fill = new SolidColorBrush(Colors.Transparent);
                c.HorizontalAlignment = HorizontalAlignment.Left;
                c.VerticalAlignment = VerticalAlignment.Top;
                c.MouseDown += pick_point;
                points.Add(c);
                grid.Children.Add(c);
                a += seg;
            }
            mixPoints();
        }

        private void mixPoints()
        {
            for (int i = 0;i < points.Count;i++)
            {
                int x = rnd.Next(20, 800);
                int y = rnd.Next(70, 800);
                set_point(points[i], new Point(x, y));
            }
        }

        private void pick_point(object sender, MouseButtonEventArgs e)
        {
            Ellipse s = (Ellipse)sender;
            s.Fill = new SolidColorBrush(Color.FromRgb(4, 225, 70));
            clicked = s;
        }

        private void set_point(object sender, MouseButtonEventArgs e)
        {
            if (clicked != null)
            {
                Point p = e.GetPosition(grid);
                int index = points.IndexOf(clicked);
                graph.vertexes[index] = p;

                moveLines(index);

                clicked.Margin = new Thickness(p.X - 10, p.Y - 10, 0, 0);
                clicked.Fill = new SolidColorBrush(Color.FromRgb(225, 70, 4));
                //clicked.Fill = new SolidColorBrush(Colors.Transparent);
                clicked = null;
            }
        }

        private void set_point(Ellipse moved, Point where)
        {
            int index = points.IndexOf(moved);
            graph.vertexes[index] = where;
            moveLines(index);
            moved.Margin = new Thickness(where.X - 10, where.Y - 10, 0, 0);
        }

        private void move_point(object sender, MouseEventArgs e)
        {
            if (clicked != null)
            {
                Point p = e.GetPosition(grid);
                int index = points.IndexOf(clicked);
                graph.vertexes[index] = p;

                moveLines(index);

                clicked.Margin = new Thickness(p.X - 10, p.Y - 10, 0, 0);
            }
        }

        private void check_btn_Click(object sender, RoutedEventArgs e)
        {
            bool check = true;
            for (int i = 0; i < lines.Count && check; i++)
            {
                for (int j = i + 1; j < lines.Count && check; j++)
                {
                    Point p1 = new Point(lines[i].X1, lines[i].Y1);
                    Point p2 = new Point(lines[i].X2, lines[i].Y2);
                    Point p3 = new Point(lines[j].X1, lines[j].Y1);
                    Point p4 = new Point(lines[j].X2, lines[j].Y2);

                    if ((p1 == p3) || (p2 == p4) || (p1 == p4) || (p2 == p3))
                        continue;

                    double d1 = vectorProduct(p3, p4, p1);
                    double d2 = vectorProduct(p3, p4, p2);
                    double d3 = vectorProduct(p1, p2, p3);
                    double d4 = vectorProduct(p1, p2, p4);

                    if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                    { check = false; break; }

                    if (Math.Abs(d1) < double.Epsilon && OnSegment(p3, p4, p1)) 
                    { check = false; break; }
                    if (Math.Abs(d2) < double.Epsilon && OnSegment(p3, p4, p2)) 
                    { check = false; break; }
                    if (Math.Abs(d3) < double.Epsilon && OnSegment(p1, p2, p3)) 
                    { check = false; break; }
                    if (Math.Abs(d4) < double.Epsilon && OnSegment(p1, p2, p4)) 
                    { check = false; break; }
                }
            }
            if (check) { MessageBox.Show("Граф плоский! Вы справились!"); this.Close(); }
            else MessageBox.Show("Остались еще пересечения!");
        }

        private bool OnSegment(Point pi, Point pj, Point pk)
        {
            // попадание точки в отрезок
            return Math.Min(pi.X, pj.X) <= pk.X && pk.X <= Math.Max(pi.X, pj.X) &&
               Math.Min(pi.Y, pj.Y) <= pk.Y && pk.Y <= Math.Max(pi.Y, pj.Y);
        }

        private double vectorProduct(Point pi, Point pj, Point pk)
        {
            // векторы pi->pk и pi->pj
            return (pk.X - pi.X) * (pj.Y - pi.Y) - (pj.X - pi.X) * (pk.Y - pi.Y);
        }
    }
}
