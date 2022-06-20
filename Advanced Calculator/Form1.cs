using ExpressionCalculator;

namespace Advanced_Calculator
{
    public partial class Form1 : Form
    {
        int oo = 0;
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
            new List<string>() { "ln", "log", "!", "root", "sen", "cos", "tan", "cot", "sec", "csc", "arcsen", "arccos", "arctan", "arccot", "arcsec", "arccsc" }

        };

        public Form1()
        {
            
            InitializeComponent();

            Expression expression = Expression.CreateExpression("15 + 27*x", operators, less_priority);

            textBox1.Text = (expression.ToString(less_priority));

            Dictionary<char, double> d = new Dictionary<char, double>();

            d.Add('x', 10);

            Expression expression1 = expression.Evaluate(d);

            textBox2.Text = (expression1.ToString(less_priority));

        }
    }
}