using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;

namespace SplineProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SolveMatrix();
            SolveSpline();
            SolveSplineWithFunction();
        }

        private static TriDiagonalMatrix SolveMatrix()
        {
            TriDiagonalMatrix matrix = new TriDiagonalMatrix(3);

            for (int i = 0; i < matrix.N; i++)
            {
                matrix.A[i] = -1f;
                matrix.B[i] = 2f;
                matrix.C[i] = -1f;
            }

            Console.WriteLine("Matrix:\n{0}", matrix.ToDisplayString(",4:0.00", "    "));

            Random rand = new Random(1);
            float[] d = new float[matrix.N];

            d[0] = 1f;
            d[1] = 0f;
            d[2] = 1f;

            float[] x = matrix.Solve(d);

            Console.WriteLine("Solve returned: ");

            for (int i = 0; i < x.Length; i++)
            {
                Console.Write("{0:0.0000}, ", x[i]);
            }

            Console.WriteLine();
            return matrix;
        }

        private static void SolveSpline()
        {
            float[] x = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f };
            float[] y = { 0.0f, 0.0f, 209.0f, 209.0f, 0.0f, 209.0f };

            int n = x.Length;

            int upsampleFactor = 10;
            int nInterpolated = n*upsampleFactor;
            float[] xs = new float[nInterpolated];

            for (int i = 0; i < nInterpolated; i++)
            {
                xs[i] = (float) i*(n - 1)/(float) (nInterpolated - 1);
            }

            float[] ys = CubicSpline.Compute(x, y, xs, 0.0f, 6.0f, true);

            string path = @"..\..\manosplainasbefunkcijos.png";
            PlotSplineSolution("Cubic Spline Interpolation", x, y, xs, ys, path);
        }

        private static float SolveFunction(float x)
        {

            double answer = Math.Sin(2*x) - 0.2*(x*x);

            return (float) answer;
        }

        private static void SolveSplineWithFunction()
        {
            float[] x = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f };
            int n = x.Length;
            float[] y = new float[n];
            for (int i = 0; i < n; i++)
            {
                y[i] = SolveFunction(x[i]);
            }

            int upsampleFactor = 10;
            int nInterpolated = n * upsampleFactor;
            float[] xs = new float[nInterpolated];

            for (int i = 0; i < nInterpolated; i++)
            {
                xs[i] = (float)i * (n - 1) / (float)(nInterpolated - 1);
            }

            float[] ys = CubicSpline.Compute(x, y, xs, 0.0f, 6.0f, true);

            string path = @"..\..\manosplainassufunkcija.png";
            PlotSplineSolution("Cubic Spline Interpolation", x, y, xs, ys, path);
        }

        private static void PlotSplineSolution(string title, float[] x, float[] y, float[] xs, float[] ys, string path, float[] qPrime = null)
        {
            var chart = new Chart();
            chart.Size = new Size(600, 400);
            chart.Titles.Add(title);
            chart.Legends.Add(new Legend("Legend"));

            ChartArea ca = new ChartArea("DefaultChartArea");
            ca.AxisX.Title = "X";
            ca.AxisY.Title = "Y";
            chart.ChartAreas.Add(ca);

            Series s1 = CreateSeries(chart, "Spline", CreateDataPoints(xs, ys), Color.Blue, MarkerStyle.None);
            Series s2 = CreateSeries(chart, "Original", CreateDataPoints(x, y), Color.Green, MarkerStyle.Diamond);

            chart.Series.Add(s2);
            chart.Series.Add(s1);

            if (qPrime != null)
            {
                Series s3 = CreateSeries(chart, "Slope", CreateDataPoints(xs, qPrime), Color.Red, MarkerStyle.None);
                chart.Series.Add(s3);
            }

            ca.RecalculateAxesScale();
            ca.AxisX.Minimum = Math.Floor(ca.AxisX.Minimum);
            ca.AxisX.Maximum = Math.Ceiling(ca.AxisX.Maximum);
            int nIntervals = (x.Length - 1);
            nIntervals = Math.Max(4, nIntervals);
            ca.AxisX.Interval = (ca.AxisX.Maximum - ca.AxisX.Minimum) / nIntervals;

            // Save
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                chart.SaveImage(fs, ChartImageFormat.Png);
            }
        }

        private static List<DataPoint> CreateDataPoints(float[] x, float[] y)
        {
            Debug.Assert(x.Length == y.Length);
            List<DataPoint> points = new List<DataPoint>();

            for (int i = 0; i < x.Length; i++)
            {
                points.Add(new DataPoint(x[i], y[i]));
            }

            return points;
        }

        private static Series CreateSeries(Chart chart, string seriesName, IEnumerable<DataPoint> points, Color color, MarkerStyle markerStyle = MarkerStyle.None)
        {
            var s = new Series()
            {
                XValueType = ChartValueType.Double,
                YValueType = ChartValueType.Double,
                Legend = chart.Legends[0].Name,
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Line,
                Name = seriesName,
                ChartArea = chart.ChartAreas[0].Name,
                MarkerStyle = markerStyle,
                Color = color,
                MarkerSize = 8
            };

            foreach (var p in points)
            {
                s.Points.Add(p);
            }

            return s;
        }
    }
}