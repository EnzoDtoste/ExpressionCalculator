namespace ExpressionCalculator
{

    public abstract class Expression
    {
        
        //indicador de operación, constante o variable
        public string visual;
        protected Expression(string visual)
        {
            this.visual = visual;
        }

        //dada una cadena halla el operador con menos prioridad más externo
        public static Expression CreateExpression(string expression, List<Expression> operators, List<string>[] less_priority)
        {

            expression = expression.Replace(" ", "");

            expression = RemoveExternalP(expression);

            int[] indexes = new int[operators.Count];

            for (int i = 0; i < operators.Count; i++)
                indexes[i] = IndexOf(expression, expression, operators[i].visual, 0);

            int max = -1;
            int max_index = -1;

            //recorre de menor prioridad a mayor
            foreach (var equal in less_priority)
            {

                foreach (var op in equal)
                {

                    //ubica el operador en la colección
                    int index = indexInColl(operators, op);

                    //si el operador con igual prioridad está más lejano me quedo con él
                    if (indexes[index] > max)
                    { max = indexes[index]; max_index = index; }

                }

                //si en esta prioridad encontré alguno ya rompo el ciclo
                if (max != -1)
                    break;

            }

            //si nunca encontré un operador es una constante o una variable
            if (max == -1)
                return new ConstantOrVariable(expression);

            //extraigo esa operación de la expresión
            return operators[max_index].ExtractExpression(expression, max, operators, less_priority);

        }

        //remueve los parentesis externos
        private static string RemoveExternalP(string expression)
        {

            if(expression[0] == '(' && expression[expression.Length - 1] == ')')
            {

                string temp = expression.Substring(1, expression.Length - 2);

                int count = 0;

                foreach(char c in temp)
                {
                  
                    if (c == '(')
                        count++;
                    if (c == ')')
                        count--;

                    if (count < 0)
                        return expression;

                }

                if (count == 0)
                    return temp;

                return expression;

            }

            return expression;

        }

        //indice de una operación en la colección
        private static int indexInColl(List<Expression> operators, string visual)
        {
            for(int i = 0; i < operators.Count; i++)
            {
                if(operators[i].visual == visual)
                    return i;
            }
            return -1;
        }

        //ultima posición de un operador en una expresión que no esté dentro de un paréntesis
        private static int IndexOf(string expression, string subexpression, string visual, int arrastre)
        {

            int indexof = subexpression.IndexOf(visual);

            if (indexof != -1)
            {

                if (!IsInP(expression, arrastre + indexof))
                    return Math.Max(arrastre + indexof, IndexOf(expression, subexpression.Substring(indexof + 1), visual, arrastre + indexof + 1));

                return IndexOf(expression, subexpression.Substring(indexof + 1), visual, arrastre + indexof + 1);

            }

            return -1;

        }

        //si una operación está dentro de un paréntesis
        private static bool IsInP(string expression, int index)
        {

            int count = 0;

            for(int i = 0; i < index; i++)
            {
                if(expression[i] == '(')
                    count++;
                if (expression[i] == ')')
                    count--;
            }

            return count > 0;

        }

        //si es una variable específica
        protected bool IsVariable(string value, string variable)
        {
            try
            {
                double.Parse(value);
                return false;
            }
            catch { return value == variable; }
        }
        //si es una variable
        protected bool IsVariable(string value)
        {
            try
            {
                double.Parse(value);
                return false;
            }
            catch { return true; }
        }

        /// <summary>
        /// returns if a have less or equal priority than b
        /// </summary>
        /// <param name="less_priority"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="equal"> include equal case </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsLessOrEqualPriority(List<string>[] less_priority, string a, string b, bool equal)
        {

            foreach(var item in less_priority)
            {

                bool found_a = false;
                bool found_b = false;

                foreach(var e in item)
                {

                    if (e == a)
                        found_a = true;

                    if(e == b)
                        found_b = true;

                }

                if (found_a && found_b && equal)
                    return true;

                else if (found_a && found_b && !equal)
                    return false;

                if (found_a)
                    return true;

                if (found_b)
                    return false;

            }

            throw new ArgumentException("the operators do not exist");

        }

        //cada operación debe saber extraerse de una expresión
        protected abstract Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority);

        //cada operación debe saber como imprimirse
        public abstract string ToString(List<string>[] less_priority);
        public override string ToString()
        {
            return visual;
        }
        public double ToDouble()
        {
            string exp = this.ToString();
            return Double.Parse(exp);
        }

        public abstract List<char> GetVariables(List<char> constants);

        //cada operación debe saber como evaluarse
        public abstract Expression Evaluate(Dictionary<char, double> variables);

        //cada operación debe saber como derivarse
        public abstract Expression Derivate(char variable);
    }

    public abstract class BinaryExpression : Expression
    {

        protected Expression left;
        protected Expression right;
        public BinaryExpression(string visual, Expression left, Expression right) : base(visual)
        {
            this.left = left;
            this.right = right;
        }

        public override List<char> GetVariables(List<char> constants)
        {

            return new List<char>(left.GetVariables(constants).Union(right.GetVariables(constants)));

        }

    }

    public abstract class UnaryExpression : Expression
    {

        protected Expression content;

        public UnaryExpression(string visual, Expression content) : base(visual)
        {
            this.content = content;
        }

        public override List<char> GetVariables(List<char> constants)
        {

            return content.GetVariables(constants);

        }

        public override string ToString(List<string>[] less_priority)
        {
            if (content is ConstantOrVariable)
                return visual + content.ToString(less_priority);

            return visual + "(" + content.ToString(less_priority) + ")";
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {
            
            Expression content;

            content = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            return (Expression)Activator.CreateInstance(this.GetType(), visual, content);

        }

    }

    #region Expressions

    #region Basics

    public class ConstantOrVariable : Expression
    {
        public ConstantOrVariable(string visual) : base(visual)
        {
        }

        public override List<char> GetVariables(List<char> constants)
        {

            if (IsVariable(this.visual) && !constants.Contains(this.visual[0]))
                return new List<char>() { this.visual[0] };

            return new List<char>();

        }

        public override Expression Derivate(char variable)
        {
            if (visual == variable.ToString())
                return new ConstantOrVariable("1");

            return new ConstantOrVariable("0");
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {
            throw new NotImplementedException();
        }

        public override string ToString(List<string>[] less_priority)
        {
            return visual;
        }
        
        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            //si es una variable devuelvo su evaluación
            if(variables.ContainsKey(visual[0]))
                return new ConstantOrVariable(variables[visual[0]].ToString());

            //es una constante por lo tanto la devuelvo así mismo
            return this;
        }
        
    }

    public class Sum : BinaryExpression
    {
        public Sum(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {
            
            Expression right;
            Expression left;

            right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);
            left = CreateExpression(expression.Substring(0, index), operators, less_priority);

            return new Sum(visual, left, right);

        }

        public override string ToString(List<string>[] less_priority)
        {

            string result = "";

            if (left is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, left.visual, true))
                result += left.ToString(less_priority);

            else
                result += "(" + left.ToString(less_priority) + ")";

            result += " " + visual;

            if (right is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, right.visual, true))
                result += " " + right.ToString(less_priority);

            else
                result += " (" + right.ToString(less_priority) + ")";

            return result;

        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            
            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            //si son constantes sumo
            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((double.Parse(newLeft.visual.ToString()) + double.Parse(newRight.visual.ToString())).ToString());

            //reduzco
            if (newLeft.visual == "0")
                return newRight;

            if (newRight.visual == "0")
                return newLeft;

            return new Sum("+", newLeft, newRight);

        }

        public override Expression Derivate(char variable)
        {

            return new Sum(visual, left.Derivate(variable), right.Derivate(variable));

        }

    }

    public class Minus : BinaryExpression
    {
        public Minus(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Minus(visual, left.Derivate(variable), right.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
          
            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((double.Parse(newLeft.visual.ToString()) - double.Parse(newRight.visual.ToString())).ToString());

            if (newLeft.visual == "0")
                return new Multiply("*", new ConstantOrVariable("-1"), newRight);

            if (newRight.visual == "0")
                return newLeft;

            return new Minus("-", newLeft, newRight);
        
        }

        public override string ToString(List<string>[] less_priority)
        {

            string result = "";

            if (left is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, left.visual, true))
                result += left.ToString(less_priority);

            else
                result += "(" + left.ToString(less_priority) + ")";

            result += " " + visual;

            if (right is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, right.visual, false))
                result += " " + right.ToString(less_priority);

            else
                result += " (" + right.ToString(less_priority) + ")";

            return result;

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            //si la izquierda no existe entonces es un (-1 *)
            try
            {

                left = CreateExpression(expression.Substring(0, index), operators, less_priority);
                return new Minus(visual, left, right);

            }

            catch
            {
                return new Multiply("*", new ConstantOrVariable("-1"), right);
            }

        }
    }

    public class Multiply : BinaryExpression
    {
        public Multiply(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Sum("+", new Multiply(visual, left.Derivate(variable), right), new Multiply(visual, left, right.Derivate(variable)));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((double.Parse(newLeft.visual.ToString()) * double.Parse(newRight.visual.ToString())).ToString());

            if (newLeft.visual == "0")
                return new ConstantOrVariable("0");

            if (newLeft.visual == "1")
                return newRight;

            if (newRight.visual == "0")
                return new ConstantOrVariable("0");

            if (newRight.visual == "1")
                return newLeft;

            return new Multiply("*", newLeft, newRight);

        }

        public override string ToString(List<string>[] less_priority)
        {

            string result = "";

            if (left is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, left.visual, true))
                result += left.ToString(less_priority);

            else
                result += "(" + left.ToString(less_priority) + ")";

            result += " " + visual;

            if (right is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, right.visual, true))
                result += " " + right.ToString(less_priority);

            else
                result += " (" + right.ToString(less_priority) + ")";

            return result;

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);
            left = CreateExpression(expression.Substring(0, index), operators, less_priority);

            return new Multiply(visual, left, right);

        }
    }

    public class Divide : BinaryExpression
    {
        public Divide(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Divide(visual, new Minus("-", new Multiply("*", left.Derivate(variable), right), new Multiply("*", left, right.Derivate(variable))), new Exponent("^", right, new ConstantOrVariable("2")));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (newRight.visual == "0")
                throw new ArgumentException("Cannot divide by zero");

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((double.Parse(newLeft.visual.ToString()) / double.Parse(newRight.visual.ToString())).ToString());

            if (newLeft.visual == "0")
                return new ConstantOrVariable("0");

            if (newRight.visual == "1")
                return newLeft;

            return new Divide("/", newLeft, newRight);

        }

        public override string ToString(List<string>[] less_priority)
        {

            string result = "";

            if (left is ConstantOrVariable || IsLessOrEqualPriority(less_priority, visual, left.visual, true))
                result += left.ToString(less_priority);

            else
                result += "(" + left.ToString(less_priority) + ")";

            result += " " + visual;

            if (right is ConstantOrVariable)
                result += " " + right.ToString(less_priority);

            else
                result += " (" + right.ToString(less_priority) + ")";

            return result;

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);
            left = CreateExpression(expression.Substring(0, index), operators, less_priority);

            return new Divide(visual, left, right);

        }
    }

    #endregion

    #region Others

    public class Exponent : BinaryExpression
    {
        public Exponent(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            
            //si el exponente es una constante deriva como x^a
            if (right is ConstantOrVariable && !IsVariable(right.visual, variable.ToString()))
                return new Multiply("*", new Multiply("*", right, new Exponent("^", left, new Minus("-", right, new ConstantOrVariable("1")))), left.Derivate(variable));

            //si la base es una constante deriva como a^x
            if (left is ConstantOrVariable && !IsVariable(left.visual, variable.ToString()))
                return new Multiply("*", new Multiply("*", this, new Ln("ln", left)), right.Derivate(variable));

            //si ambas son variables deriva como e^((exp)*ln(base))
            return new Exponent("^", new ConstantOrVariable("e"), new Multiply("*", right, new Ln("ln", left))).Derivate(variable);

        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((Math.Pow(double.Parse(newLeft.visual.ToString()), double.Parse(newRight.visual.ToString()))).ToString());

            if (newLeft.visual == "0")
                return new ConstantOrVariable("0");

            if (newLeft.visual == "1")
                return new ConstantOrVariable("1");

            if (newRight.visual == "0")
                return new ConstantOrVariable("1");

            if (newRight.visual == "1")
                return newLeft;

            return new Exponent("^", newLeft, newRight);

        }

        public override string ToString(List<string>[] less_priority)
        {

            string result = "";

            if (left is ConstantOrVariable || left is UnaryExpression || left is Root || left is Log)
                result += left.ToString(less_priority);

            else
                result += "(" + left.ToString(less_priority) + ")";

            result += " " + visual;

            if (right is ConstantOrVariable || right is UnaryExpression || right is Root || right is Log)
                result += " " + right.ToString(less_priority);

            else
                result += " (" + right.ToString(less_priority) + ")";

            return result;

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);
            left = CreateExpression(expression.Substring(0, index), operators, less_priority);

            return new Exponent(visual, left, right);

        }
    }

    public class Ln : UnaryExpression
    {
        public Ln(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Divide("/", new ConstantOrVariable("1"), content), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Log(double.Parse(newContent.visual)).ToString());

            if (newContent.visual == "e")
                return new ConstantOrVariable("1");

            return new Ln(visual, newContent);

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression content;

            if (expression[index + visual.Length] == '(' && expression[expression.Length - 1] == ')')
                content = CreateExpression(expression.Substring(index + visual.Length + 1, expression.Length - 1 - index - visual.Length - 1), operators, less_priority);

            else
                content = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            return new Ln(visual, content);

        }
    }

    public class Log : BinaryExpression
    {
        public Log(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Divide("/", new Ln("ln", right), new Ln("ln", left)).Derivate(variable);
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((Math.Log(double.Parse(newRight.visual.ToString()), double.Parse(newLeft.visual.ToString()))).ToString());

            if ((!IsVariable(newLeft.visual) && double.Parse(newLeft.visual) <= 0) || newLeft.visual == "1")
                throw new ArgumentException("base must be > 0 & != 1");

            if (!IsVariable(newRight.visual) && double.Parse(newRight.visual) <= 0)
                throw new ArgumentException("argument must be > 0");

            if (newRight.visual == "1")
                return new ConstantOrVariable("0");

            return new Log("log", newLeft, newRight);

        }

        public override string ToString(List<string>[] less_priority)
        {
            return visual + "(" + left.ToString(less_priority) + "," + right.ToString(less_priority) + ")";
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            if (expression[index + visual.Length] == '(' && expression[expression.Length - 1] == ')')
            {

                expression = expression.Substring(index + visual.Length + 1, expression.Length - 1 - index - visual.Length - 1);

                string[] parts = expression.Split(',');

                //si se especifica la base
                if (parts.Length == 2)
                {

                    if (parts[0][0] == '(' && parts[0][parts[0].Length - 1] == ')')
                        left = CreateExpression(parts[0].Substring(1, parts[0].Length - 2), operators, less_priority);

                    else
                        left = CreateExpression(parts[0], operators, less_priority);

                    if (parts[1][0] == '(' && parts[1][parts[1].Length - 1] == ')')
                        right = CreateExpression(parts[1].Substring(1, parts[1].Length - 2), operators, less_priority);

                    else
                        right = CreateExpression(parts[1], operators, less_priority);

                }

                //sino es 10
                else
                {

                    left = new ConstantOrVariable("10");
                    right = CreateExpression(parts[0], operators, less_priority);

                }

            }

            //la base es 10
            else
            {

                left = new ConstantOrVariable("10");
                right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            }

            return new Log(visual, left, right);

        }
    }

    public class Root : BinaryExpression
    {
        public Root(string visual, Expression left, Expression right) : base(visual, left, right)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Exponent("*", right, new Divide("/", new ConstantOrVariable("1"), left)).Derivate(variable);
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newLeft = left.Evaluate(variables);
            Expression newRight = right.Evaluate(variables);

            if (!IsVariable(newLeft.visual) && !IsVariable(newRight.visual))
                return new ConstantOrVariable((Math.Pow(double.Parse(newRight.visual.ToString()), 1 / double.Parse(newLeft.visual.ToString()))).ToString());

            if (newLeft.visual == "0")
                throw new ArgumentException("cannot root by zero");

            if (newLeft.visual == "1")
                return newRight;

            if (newRight.visual == "0")
                return new ConstantOrVariable("0");

            if (newRight.visual == "1")
                return new ConstantOrVariable("1");

            return new Root("root", newLeft, newRight);

        }

        public override string ToString(List<string>[] less_priority)
        {
            return visual + "(" + left.ToString(less_priority) + "," + right.ToString(less_priority) + ")";
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression right;
            Expression left;

            if (expression[index + visual.Length] == '(' && expression[expression.Length - 1] == ')')
            {

                expression = expression.Substring(index + visual.Length + 1, expression.Length - 1 - index - visual.Length - 1);

                string[] parts = expression.Split(',');

                //si se especifica el indice del radical
                if (parts.Length == 2)
                {

                    if (parts[0][0] == '(' && parts[0][parts[0].Length - 1] == ')')
                        left = CreateExpression(parts[0].Substring(1, parts[0].Length - 2), operators, less_priority);

                    else
                        left = CreateExpression(parts[0], operators, less_priority);

                    if (parts[1][0] == '(' && parts[1][parts[1].Length - 1] == ')')
                        right = CreateExpression(parts[1].Substring(1, parts[1].Length - 2), operators, less_priority);

                    else
                        right = CreateExpression(parts[1], operators, less_priority);

                }

                //sino es 2
                else
                {

                    left = new ConstantOrVariable("2");
                    right = CreateExpression(parts[0], operators, less_priority);

                }

            }

            //el indice del radical es 2
            else
            {

                left = new ConstantOrVariable("2");
                right = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            }

            return new Root(visual, left, right);

        }
    }

    public class Fac : UnaryExpression
    {
        public Fac(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            throw new NotImplementedException();
        }

        static Dictionary<int, int> facs = new Dictionary<int, int>();
        static int Factorial(int number)
        {

            if (number == 0)
                return 1;

            if (facs.ContainsKey(number))
                return facs[number];

            int f = number * Factorial(number - 1);
            facs[number] = f;

            return f;

        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Factorial(int.Parse(newContent.visual)).ToString());

            return new Fac("!", newContent);

        }

        public override string ToString(List<string>[] less_priority)
        {

            if (content is ConstantOrVariable)
                return content.ToString(less_priority) + visual;

            return "(" + content.ToString(less_priority) + ")" + visual;

        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {

            Expression content;

            content = CreateExpression(expression.Substring(0, index), operators, less_priority);

            return new Fac(visual, content);

        }
    }

    #endregion

    #region Trigonometry

    public class Sen : UnaryExpression
    {
        public Sen(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Cos("cos", content), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {

            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Sin(double.Parse(newContent.visual)).ToString());

            return new Sen("sen", newContent);

        }

    }

    public class Cos : UnaryExpression
    {
        public Cos(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Multiply("*", new ConstantOrVariable("-1"), new Sen("sen", content)), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Cos(double.Parse(newContent.visual)).ToString());

            return new Cos("cos", newContent);
        }

    }

    public class Tan : UnaryExpression
    {
        public Tan(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Exponent("^", new Sec("sec", content), new ConstantOrVariable("2")), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Tan(double.Parse(newContent.visual)).ToString());

            return new Tan("tan", newContent);
        }

    }

    public class Cot : UnaryExpression
    {
        public Cot(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Multiply("*", new ConstantOrVariable("-1"), new Exponent("^", new Csc("csc", content), new ConstantOrVariable("2"))), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable((1 / Math.Tan(double.Parse(newContent.visual))).ToString());

            return new Cot("cot", newContent);
        }

    }

    public class Sec : UnaryExpression
    {
        public Sec(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Multiply("*", this, new Tan("tan", content)), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable((1 / Math.Cos(double.Parse(newContent.visual))).ToString());

            return new Sec("sec", newContent);
        }

    }

    public class Csc : UnaryExpression
    {
        public Csc(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new Multiply("*", new ConstantOrVariable("-1"), new Multiply("*", this, new Cot("cot", content))), content.Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable((1 / Math.Sin(double.Parse(newContent.visual))).ToString());

            return new Csc("csc", newContent);
        }

    }

    public class Arcsen : UnaryExpression
    {
        public Arcsen(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Divide("/", content.Derivate(variable), new Root("root", new ConstantOrVariable("2"), new Minus("-", new ConstantOrVariable("1"), new Exponent("^", content, new ConstantOrVariable("2")))));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Asin(double.Parse(newContent.visual)).ToString());

            return new Arcsen("arcsen", newContent);
        }

        protected override Expression ExtractExpression(string expression, int index, List<Expression> operators, List<string>[] less_priority)
        {
            Expression content;

            if (expression[index + visual.Length] == '(' && expression[expression.Length - 1] == ')')
                content = CreateExpression(expression.Substring(index + visual.Length + 1, expression.Length - 1 - index - visual.Length - 1), operators, less_priority);

            else
                content = CreateExpression(expression.Substring(index + visual.Length), operators, less_priority);

            return new Arcsen(visual, content);
        }
    }

    public class Arccos : UnaryExpression
    {
        public Arccos(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new ConstantOrVariable("-1"), new Arcsen("arcsen", content).Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Acos(double.Parse(newContent.visual)).ToString());

            return new Arccos("arccos", newContent);
        }

    }

    public class Arctan : UnaryExpression
    {
        public Arctan(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Divide("/", content.Derivate(variable), new Sum("+", new ConstantOrVariable("1"), new Exponent("^", content, new ConstantOrVariable("2"))));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            Expression newContent = content.Evaluate(variables);

            if (!IsVariable(newContent.visual))
                return new ConstantOrVariable(Math.Atan(double.Parse(newContent.visual)).ToString());

            return new Arctan("arctan", newContent);
        }

    }

    public class Arccot : UnaryExpression
    {
        public Arccot(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new ConstantOrVariable("-1"), new Arctan("arctan", content).Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            throw new NotImplementedException();
        }

    }

    public class Arcsec : UnaryExpression
    {
        public Arcsec(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Divide("/", content.Derivate(variable), new Multiply("*", content, new Root("root", new ConstantOrVariable("2"), new Minus("-", new Exponent("^", content, new ConstantOrVariable("2")), new ConstantOrVariable("1")))));
        }


        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            throw new NotImplementedException();
        }
    }

    public class Arccsc : UnaryExpression
    {
        public Arccsc(string visual, Expression content) : base(visual, content)
        {
        }

        public override Expression Derivate(char variable)
        {
            return new Multiply("*", new ConstantOrVariable("-1"), new Arcsec("arcsec", content).Derivate(variable));
        }

        public override Expression Evaluate(Dictionary<char, double> variables)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #endregion

}