using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCalculator
{
    public class Taylor
    {
        //valores del centro
        Dictionary<char, double> values;
        double[] center;

        //derivadas del nivel actual
        public List<Expression> vectorDer;

        //indice de derivación
        List<int[]> exps;

        //k factorial, k derivada
        int k = 0;

        Fac fac = new Fac("!", new ConstantOrVariable("k"));

        //variables de la función
        List<char> vars;

        public Taylor(Expression function)
        {
            values = new Dictionary<char, double>();

            vars = function.GetVariables(new List<char>() {'e'});

            //centro 0
            foreach (char var in vars)
            {
                values.Add(var, 0);
            }

            center = new double[vars.Count];

            //primer elemento es la función sin derivar
            vectorDer = new List<Expression>() {function};
            //indices de derivación en 0
            exps = new List<int[]>() {new int[vars.Count]};
        }

        //centro específico
        public Taylor(Expression function, double[] center)
        {
            if (center.Length != function.GetVariables(new List<char>() {'e'}).Count)
                throw new ArgumentException("Invalid Vector X0");

            values = new Dictionary<char, double>();

            vars = function.GetVariables(new List<char>() {'e'});

            //centro especificado
            for (int i = 0; i < vars.Count; i++)
            {
                values.Add(vars[i], center[i]);
            }

            this.center = center;

            //primer elemento es la función sin derivar
            vectorDer = new List<Expression>() {function};
            //indices de derivación en 0
            exps = new List<int[]>() {new int[vars.Count]};
        }

        //Próximo valor de la serie
        public Expression NextValue(double[] value)
        {
            //inicializo suma
            Expression sum = new ConstantOrVariable("0");

            //por cada función
            for (int i = 0; i < vectorDer.Count; i++)
            {
                //evalúo en el centro
                var mult = vectorDer[i].Evaluate(values);

                //multiplico las diferencias elevadas al indice de derivación correspondiente, de las variables con el centro 
                for (int j = 0; j < center.Length; j++)
                {
                    mult = new Multiply("*", mult,
                        new ConstantOrVariable(Math.Pow(value[j] - center[j], exps[i][j]).ToString()));
                }

                //voy sumando
                sum = new Sum("+", sum, mult.Evaluate(values));
            }

            List<Expression> ders = new List<Expression>();
            List<int[]> nextExps = new List<int[]>();

            for (int i = 0; i < vectorDer.Count; i++)
            {
                //derivo cada función por cada variable
                for (int j = 0; j < vars.Count; j++)
                {
                    ders.Add(vectorDer[i].Derivate(vars[j]).Evaluate(new Dictionary<char, double>()));

                    int[] c = new int[vars.Count];
                    exps[i].CopyTo(c, 0);

                    //aumento índice de derivación correspondiente
                    c[j]++;

                    nextExps.Add(c);
                }
            }

            //reemplazo
            vectorDer = ders;
            exps = nextExps;

            var dic = new Dictionary<char, double>();
            dic.Add('k', k);

            k++;

            //divido la sumatoria entre el factorial de k
            return new Divide("/", sum, fac.Evaluate(dic)).Evaluate(new Dictionary<char, double>());
        }

        /// <summary>
        /// returns the sum up to n of the Taylor series decomposed function
        /// </summary>
        /// <param name="n"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Expression Evaluate(int n, double[] value)
        {
            //inicializo suma
            Expression sum = new ConstantOrVariable("0");

            //sumatoria de los términos
            for (int i = 0; i < n; i++)
            {
                sum = new Sum("+", sum, NextValue(value));
            }

            return sum.Evaluate(new Dictionary<char, double>());
        }

        /// <summary>
        /// returns collection of Taylor series terms
        /// </summary>
        /// <param name="n"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IEnumerable<Expression> N_Terms(int n, double[] value)
        {
            for (int i = 0; i < n; i++)
            {
                yield return NextValue(value);
            }
        }
    }
}