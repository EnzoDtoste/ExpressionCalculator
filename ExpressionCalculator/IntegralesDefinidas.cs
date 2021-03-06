namespace ExpressionCalculator
{
    public static class IntegralDefinida
    {
        public static Expression Integra(Expression exp, double inf, double sup, List<(char, int)> variables)
        {
            Expression answer = new ConstantOrVariable("0");
            Dictionary<char, double> aux = new Dictionary<char, double>();

            foreach (var variable in variables)
            {
                aux.Add(variable.Item1, inf);
            }

            aux.Add('e', double.Parse(new Taylor(Expression.CreateExpression("e^x", Gui.operators, Gui.less_priority)).Evaluate(34, new double[] {1}).ToString(Gui.less_priority)));
            aux.Add('?', Math.PI);

            for (double i = inf; i < sup; i = i + (double)1 / (double)100000)
            {

                aux[variables[0].Item1] = i;
                answer = new Sum("+", exp, answer).Evaluate(aux);
            }

            answer = new Divide("/", answer, new ConstantOrVariable("100000")).Evaluate(new Dictionary<char, double>());
            return answer;
        }
    }
}