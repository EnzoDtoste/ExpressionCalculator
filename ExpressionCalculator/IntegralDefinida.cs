namespace ExpressionCalculator
{
    public class IntegralDefinida
    {
        public double Integra(Expression exp,int sup,int inf)
        {
            Expression answer=new ConstantOrVariable("0");
            Dictionary<char,double> aux=new Dictionary<char,double>();
            aux.Add('x',inf);
            for(int i=inf;i<=sup;i=i+1/1000)
            {
                aux['x']=i;
                answer= new Sum("+",exp.Evaluate(aux),answer);
            }
            return Double.Parse(answer.ToString());
        }
        // public double SumaStg(string string1,string string2)
        // {
        //     List<char> char1=string1.ToCharArray().ToList();
        //     List<char> char2=string2.ToCharArray().ToList();
        //     int index1=char1.IndexOf(',');
        //     int index2=char2.IndexOf(',');
            


        // }
    }
}