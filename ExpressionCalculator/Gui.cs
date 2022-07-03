namespace ExpressionCalculator;

public class Gui
{
    public static List<Expression> operators = new List<Expression>()
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

    public static List<string>[] less_priority = new List<string>[]
    {

        new List<string>() { "+", "-" },
        new List<string>() { "*", "/" },
        new List<string>() { "^" },
        new List<string>() {"arcsen", "arccos", "arctan", "arccot", "arcsec", "arccsc" },
        new List<string>() { "ln", "log", "!", "root", "sen", "cos", "tan", "cot", "sec", "csc" }

    };

    public static List<string> Languaje = new List<string>()
    {
        "ln", "log", "!", "root", "sen", "cos", "tan", "cot", "sec", "csc", "arcsen", "arccos", "arctan", "arccot",
        "arcsec", "arccsc", "e","pi"
    };

    public static List<char> DeterminateVariables(string s)
    {
        List<char> variables = new List<char>();
        bool[] visited = new bool[256];
        for (int i = 0; i < s.Length; i++)
        {
            if (Char.IsLetter(s[i]))
            {
                bool isVariable = true;
                for (int j = 1; j <= 6; j++)
                {
                    if (i + j > s.Length) break;
                    string aux = s.Substring(i, j);
                    if (Languaje.Contains(aux))
                    {
                        isVariable = false;
                        i += j;
                        break;
                    }
                }

                if (isVariable && !visited[s[i]])
                {
                    variables.Add(s[i]);
                    visited[s[i]] = true;
                }
                
            }
        }

        return variables;
    }
}