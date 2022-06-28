using ExpressionCalculator;
using System.Drawing;
namespace Advanced_Calculator
{
    public partial class Form1 : Form
    {
        
        List<Expression> operators = new List<Expression>()
        {

            new Sum("+", null, null),
            new Minus("-", null, null),
            new Multiply("*", null, null),
            new Divide("/", null, null),
            new Exponent("^", null, null),
            new Ln("ln", null),
            new Log("log", null, null),
            new Fac("!", null),
            new Root("root", null, null),
            new Sen("sen", null),
            new Cos("cos", null),
            new Tan("tan", null),
            new Cot("cot", null),
            new Sec("sec", null),
            new Csc("csc", null),
            new Arcsen("arcsen", null),
            new Arccos("arccos", null),
            new Arctan("arctan", null),
            new Arccot("arccot", null),
            new Arcsec("arcsec", null),
            new Arccsc("arccsc", null)

        };

        List<string>[] less_priority = new List<string>[]
        {

            new List<string>() { "+", "-" },
            new List<string>() { "*", "/" },
            new List<string>() { "^" },
            new List<string>() { "arcsen", "arccos", "arctan", "arccot", "arcsec", "arccsc" },
            new List<string>() { "sen", "cos", "tan", "cot", "sec", "csc" },
            new List<string>() { "ln", "log", "!", "root" }

        };

        public Form1()
        {
            
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Expression expression = Expression.CreateExpression(textBox1.Text, operators, less_priority);

            Dictionary<char, double> d = new Dictionary<char, double>();

            //d.Add('x', 0);

            Expression expression1 = expression.Evaluate(d);

            textBox2.Text = (expression1.ToString(less_priority));

        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Bitmap chart = new Bitmap(panel1.Width, panel1.Height);
            Graphics g = Graphics.FromImage(chart);
            Pen black = new Pen(Color.Black);
            

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            PaintGraph();
            PaintFunc();
            PaintDer();

        }

        public void PaintDer()
        {
            Expression func = Expression.CreateExpression(textBox2.Text, operators, less_priority);

            double intervalx = (double)20 / (double)panel1.Width;
            double intervaly = (double)20 / (double)panel1.Height;
            List<PointF> funcLine = new List<PointF>();

            for (int i = 0; i < panel1.Width; i++)
            {
                double val = (-10 + (intervalx * i));
                Dictionary<char, double> x = new Dictionary<char, double>();
                x.Add('x', val);
                double point = func.Evaluate(x).ToDouble();
                float xcoord = (float)i;
                double ysize = -10;
                float ycoord = (float)panel1.Height;
                if (point <= 10 && point >= -10)
                {

                    while (ysize < point)
                    {
                        ysize += intervaly;
                        ycoord--;
                    }

                    funcLine.Add(new PointF(xcoord, ycoord));

                }

            }
            Graphics g = panel1.CreateGraphics();
            Pen blue = new Pen(Color.Blue, 2);
            g.DrawLines(blue, funcLine.ToArray());
        }

        public void PaintFunc()
        {
            Expression func = Expression.CreateExpression(textBox1.Text, operators, less_priority);

            double intervalx = (double)20 / (double)panel1.Width;
            double intervaly = (double)20 / (double)panel1.Height;
            List<PointF> funcLine = new List<PointF>();

            for (int i = 0; i < panel1.Width; i++)
            {
                double val = (-10 + (intervalx * i));
                Dictionary<char, double> x = new Dictionary<char, double>();
                x.Add('x', val);
                double point = func.Evaluate(x).ToDouble();
                float xcoord = (float)i;
                double ysize = -10;
                float ycoord = (float)panel1.Height;
                if (point <= 10 && point >= -10)
                {

                    while (ysize < point)
                    {
                        ysize += intervaly;
                        ycoord--;
                    }

                    funcLine.Add(new PointF(xcoord, ycoord));

                }

            }
            Graphics g = panel1.CreateGraphics();
            Pen red = new Pen(Color.Red, 2);
            g.DrawLines(red, funcLine.ToArray());
            
        }

        public void PaintGraph()
        {
            int a = panel1.Width / 20;
            int b = panel1.Height / 20;
            PointF[] intervalsX = new PointF[66];
            for (int i = 0; i < intervalsX.Length; i++)
            {

                intervalsX[i++] = new PointF(a * (i / 3), panel1.Height / 2);
                intervalsX[i++] = new PointF(a * (i / 3), panel1.Height / 2 - 6);
                intervalsX[i] = new PointF(a * (i / 3), panel1.Height / 2);
            }
            PointF[] intervalsY = new PointF[66];
            for (int i = 0; i < intervalsY.Length; i++)
            {

                intervalsY[i++] = new PointF(panel1.Width / 2, b * (i / 3));
                intervalsY[i++] = new PointF(panel1.Width / 2 + 6, b * (i / 3));
                intervalsY[i] = new PointF(panel1.Width / 2, b * (i / 3));
            }
            Graphics g = panel1.CreateGraphics();
            Pen black = new Pen(Color.Black, 3);

            g.DrawLines(black, intervalsX);
            g.DrawLines(black, intervalsY);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Expression func = Expression.CreateExpression(textBox1.Text, operators, less_priority);
            Expression expression1 = func.Derivate('x').Evaluate(new Dictionary<char, double>());

            textBox2.Text = (expression1.ToString(less_priority));
            Graphics g = panel1.CreateGraphics();
            Color white = Color.White;
            g.Clear(white);
            PaintGraph();
            PaintFunc();
            PaintDer();
            
        }
    }
}