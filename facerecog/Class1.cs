using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mate
{

   public  class Matrix
    {
        private int row, col;
        private double[,] data;
        public Matrix(int row, int col, double[,] data)
        {
            this.row = row;
            this.col = col;
            this.data = data;
        }

        public Matrix(int row, int col)
        {
            this.row = row;
            this.col = col;
            this.data = new double[row, col];
        }


        public void random()
        {
            Random r = new Random();
            double x, y, epi = 0.12;
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    //  x = r.Next(-a, b)*0.12-b;
                    //y = r.Next(-b, a)*0.12+a;
                    x = r.NextDouble() * 2 * epi - epi;


                    this.data[a, b] = x;




                }
            }
        }
        //GET THE SUM OF THE MATRIX
        public double sum()
        {
            double sum = 0;
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    sum += data[a, b];
                }
            }

            return sum;
        }
        public void display(int r, int c)
        {
            for (int a = 0; a < r; a++)
            {
                for (int b = 0; b < c; b++)
                {
                    Console.Write(data[a, b] + " ");
                }
                Console.WriteLine();
            }
        }
        /*public Matrix sigmoid()
        {

            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    
                    data[a,b] = (1 / (1 + Math.Pow(2.7183, -data[a,b])));
                }
            }
            return this;

        }*/
        public void getRow(Matrix x2, int num)
        {

            for (int b = 0; b < col; b++)
            {
                data[0, b] = x2.data[num, b];
            }

        }
        public Matrix getRows(int rows)
        {
            Matrix mat;

            double[,] data1 = new double[rows, this.col];

            for (int a = 0; a < rows; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    data1[a, b] = data[a, b];
                }
            }
            mat = new Matrix(rows, this.col, data1);
            return mat;

        }
        public static Matrix operator +(Matrix mat1, Matrix mat2)
        {

            if ((mat1.row != mat2.row) || mat1.col != mat2.col)
            {
                Console.WriteLine("Dimension of both matrices should be same for addition ");
                return null;
            }
            Matrix mat = new Matrix(mat1.row, mat1.col);

            for (int a = 0; a < mat1.row; a++)
            {
                for (int b = 0; b < mat1.col; b++)
                {
                    mat.data[a, b] = mat1.data[a, b] + mat2.data[a, b];
                }
            }

            return mat;
        }


        public static Matrix operator -(Matrix mat1, Matrix mat2)
        {

            if ((mat1.row != mat2.row) || mat1.col != mat2.col)
            {
                Console.WriteLine("Dimension of both matrices should be same for addition ");
                return null;
            }
            Matrix mat = new Matrix(mat1.row, mat1.col);

            for (int a = 0; a < mat1.row; a++)
            {
                for (int b = 0; b < mat1.col; b++)
                {
                    mat.data[a, b] = mat1.data[a, b] - mat2.data[a, b];
                }
            }

            return mat;
        }

        public static Matrix operator *(Matrix mat1, Matrix mat2)
        {

            if (mat1.col != mat2.row)
            {
                Console.WriteLine("COLUMN OF FIRST MUST BE EQUAL TO ROW OF SECOND MATRIX");
                return null;
            }
            else
            {
                Matrix mat = new Matrix(mat1.row, mat2.col);

                for (int i = 0; i < mat1.row; i++)
                {
                    for (int j = 0; j < mat2.col; j++)
                    {

                        mat.data[i, j] = 0;
                        for (int x = 0; x < mat1.col; x++)
                        {
                            mat.data[i, j] += mat1.data[i, x] * mat2.data[x, j];

                        }
                    }
                }

                return mat;
            }


        }


        public Matrix elementMult(Matrix mat1)
        {
            Matrix mat = new Matrix(this.row, this.col);

            if ((this.row != mat1.row))
            {
                Console.WriteLine("Dimensions must be equal for element wise multiplication");
                return null;
            }
            else
            {

                for (int a = 0; a < mat.row; a++)
                {
                    for (int b = 0; b < mat.col; b++)
                    {
                        mat.data[a, b] = data[a, b] * mat1.data[a, b];
                    }
                }

            }
            return mat;

        }
        public void setData(int row, int col, double data1)
        {
            this.data[row, col] = data1;
        }
        public Matrix T()
        {
            Matrix mat = new Matrix(this.col, this.row);

            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[b, a] = data[a, b];
                }
            }
            return mat;
        }
        //add bias row
        public Matrix addBiasrow()
        {
            Matrix mat = new Matrix(row + 1, col);

            for (int a = 0; a < col; a++)
            {
                mat.data[0, a] = 1;
            }

            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a + 1, b] = data[a, b];
                }
            }


            return mat;
        }
        public Matrix addBiascol()
        {
            Matrix mat = new Matrix(row, col + 1);
            for (int a = 0; a < row; a++)
            {
                mat.data[a, 0] = 1;
            }
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b + 1] = data[a, b];
                }
            }
            return mat;
        }
        public double mean()
        {


            double max = double.MinValue, min = double.MaxValue;

            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    if (data[a, b] > max)
                    {
                        max = data[a, b];

                    }
                    if (data[a, b] < min)
                    {
                        min = data[a, b];
                    }
                }
            }

            return max - min;
        }
        public Matrix subfrnum(double number)
        {
            Matrix mat = new Matrix(row, col);
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b] = number - data[a, b];
                }
            }
            return mat;
        }

        //removes the first row
        public Matrix removeRow()
        {
            Matrix mat = new Matrix(row - 1, col);

            for (int a = 1; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a - 1, b] = data[a, b];
                }
            }

            return mat;
        }
        public Matrix removecol()
        {
            Matrix mat = new Matrix(row, col - 1);
            for (int a = 0; a < row; a++)
            {
                for (int b = 1; b < col; b++)
                {
                    mat.data[a, b - 1] = data[a, b];
                }
            }

            return mat;
        }

        public Matrix Z()
        {
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    this.data[a, b] = 0;
                }
            }
            return this;
        }


        public Matrix sqr()
        {
            Matrix mat = new Matrix(row, col);

            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b] = Math.Pow(this.data[a, b], 2);
                }
            }

            return mat;
        }
        public Matrix add(double number)
        {
            Matrix mat = new Matrix(row, col);
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b] = data[a, b] + number;
                }
            }
            return mat;
        }
        public Matrix div(double number)
        {
            Matrix mat = new Matrix(row, col);
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {

                    mat.data[a, b] = data[a, b] / number;

                }
            }
            return mat;
        }

        public int getRowCount()
        {
            return row;
        }
        public int getColCount()
        {
            return col;
        }
        public double[,] getData()
        {
            return data;
        }
        public Matrix mul(double number)
        {
            Matrix mat = new Matrix(row, col);
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b] = data[a, b] * number;
                }
            }
            return mat;
        }
        public Matrix log()
        {
            Matrix mat = new Matrix(row, col);
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    mat.data[a, b] = Math.Log(data[a, b]);
                }
            }
            return mat;
        }




    }
}

