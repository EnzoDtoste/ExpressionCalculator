using ExpressionCalculator;

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

            Expression expression1 = expression.Derivate('x').Evaluate(new Dictionary<char, double>()).Derivate('x').Evaluate(new Dictionary<char, double>());

            textBox2.Text = expression1.ToString(less_priority);

            Taylor t = new Taylor(expression);

            textBox3.Text = t.Evaluate(9, new double[] { 1, 1}).visual.ToString();

        }
    }
}