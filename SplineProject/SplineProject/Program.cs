using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplineProject
{
    class Program
    {
        static void Main(string[] args)
        {
            SolveMatrix();
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
    }
}
