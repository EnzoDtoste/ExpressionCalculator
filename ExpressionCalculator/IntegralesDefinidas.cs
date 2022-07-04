namespace ExpressionCalculator
{
    public static class IntegralDefinida
    {
        public static double Integra(Expression exp, int inf, int sup)
        {
            Expression answer = new ConstantOrVariable("0");
            Dictionary<char, double> aux = new Dictionary<char, double>();
            aux.Add('x', inf);
            for (double i = inf; i <= sup; i = i + (double) 1 / (double) 1000)
            {
                aux['x'] = i;
                answer = new Sum("+", exp, answer).Evaluate(aux);
            }

            answer = new Divide("/", answer, new ConstantOrVariable("1000")).Evaluate(new Dictionary<char, double>());
            return Double.Parse(answer.ToString());
        }
    }
}