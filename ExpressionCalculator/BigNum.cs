namespace ExpressionCalculator;

public class BigNum
{
    
    private string _numero;
    public BigNum(string s)
    {
        this._numero = s;
    }

    public override string ToString()
    {
        return _numero;
    }
    public static BigNum operator +(BigNum a, BigNum b) => new BigNum(Suma(a.ToString(), b.ToString()));

    public static BigNum operator -(BigNum a, BigNum b) => new BigNum(Resta(a.ToString(), b.ToString()));

    public static BigNum operator *(BigNum a, BigNum b) => new BigNum(Multiplica(a.ToString(), b.ToString()));

    public static BigNum operator /(BigNum a, BigNum b) => new BigNum(Divide(a.ToString(), b.ToString()));

    public static BigNum operator %(BigNum a, BigNum b) => new BigNum(Divide(a.ToString(), b.ToString(), true));

    private static int Normalizar(ref string a, ref string b, bool mayor)
    {
        int i = FindPoint(a);
        int j = FindPoint(b);

        if (i > j)
        {
            b = AgregateCeros(b, i - j);
        }

        if (j > i)
        {
            a = AgregateCeros(a, j - i);
        }

        a = EliminatePoint(a);
        b = EliminatePoint(b);

        return mayor ? Math.Max(i, j) : Math.Max(i, j) * 2;

    }

    private static string AgregateCeros(string a, int i)
    {
        for (int j = 0; j < i; j++)
        {
            a = a + "0";
        }
        return a;
    }

    private static int FindPoint(string a)
    {
        int i = 0;
        for (i = 0; i < a.Length; i++)
        {
            if (a[a.Length - 1 - i] == '.') break;
        }
        return (i == a.Length) ? 0 : i;
    }

    private static string EliminatePoint(string a)
    {
        string s = "";
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] == '.') continue;
            s = s + a[i];
        }
        return s;
    }

    private static string AgregatePoint(string a, int ind)
    {
        string s = "";
        if (ind == 0) return a;
        if (ind >= a.Length)
        {
            s = a;
            for (int i = 0; i < ind - a.Length; i++)
            {
                s = "0" + s;
            }
            return "0." + s;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (i == ind) s = "." + s;
            s = a[a.Length - 1 - i] + s;
        }

        return s;
    }
    private static string Eleva(string a, int b)
    {
        string producto = "1";
        for (int i = 0; i < b; i++)
        {
            producto = Multiplica(producto, a);
        }
        return producto;
    }
    private static string CeroIzquierda(string a)
    {
        int j = -1;
        string s = "";
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != '0')
            {
                j = (a[i] == '.') ? i - 1 : i;
                break;
            }
        }
        if (j == -1) return "0";
        for (int i = j; i < a.Length; i++)
        {
            s = s + a[i];
        }
        return s;
    }

    private static string Divide(string a, string b, bool resto = false)
    {
        a = CeroIzquierda(a);
        b = CeroIzquierda(b);
        string cociente = CompararCadena(a, b);
        string intermedio = "";
        string multiplica = "";
        if (cociente == "igual")
        {
            if (resto) return "0";
            return "1";
        }
        if (cociente == "menor")
        {
            if (resto) return a;
            return "0";
        }
        cociente = "";
        intermedio = a.Substring(0, b.Length - 1);

        for (int i = b.Length - 1; i < a.Length; i++)
        {
            intermedio = intermedio + a[i];
            for (int j = 9; j >= 0; j--)
            {
                multiplica = Multiplica(j + "", b);
                if (CompararCadena(multiplica, intermedio) != "mayor")
                {

                    intermedio = Resta(intermedio, multiplica);
                    cociente = cociente + j;
                    break;
                }

            }
            if (intermedio == "0") intermedio = "";
        }
        if (resto) return CeroIzquierda(intermedio);
        return CeroIzquierda(cociente);
    }

    private static string Multiplica(string a, string b)
    {
        a = CeroIzquierda(a);
        b = CeroIzquierda(b);

        int index = Normalizar(ref a, ref b, false);

        string menor = "";
        string mayor = "";
        int resultado = 0;
        int resto = 0;
        string suma = "0";
        string producto = "";
        menor = CompararCadena(a, b);
        if (menor == "igual")
        {
            menor = a;
            mayor = b;
        }
        if (menor == "menor")
        {
            menor = a;
            mayor = b;
        }
        else
        {
            menor = b;
            mayor = a;
        }
        for (int i = 0; i < menor.Length; i++)
        {
            producto = "";
            for (int j = mayor.Length - 1; j >= 0; j--)
            {
                resultado = int.Parse(menor[i] + "") * int.Parse(mayor[j] + "") + resto;
                if (resultado < 10) resto = 0;
                else resto = resultado / 10;
                if (j == 0) producto = resultado + producto;
                else producto = (resultado % 10) + producto;
            }
            producto = Arreglar(producto, menor.Length - 1 - i, false);
            suma = Suma(suma, producto);
        }
        return AgregatePoint(suma, index);
    }

    private static string Resta(string a, string b)
    {
        a = CeroIzquierda(a);
        b = CeroIzquierda(b);

        int index = Normalizar(ref a, ref b, true);

        string menor = "";
        string mayor = "";
        int resto = 0;
        int resultado = 0;
        bool negativo = false;
        menor = CompararCadena(a, b);
        if (menor == "igual")
        {
            return "0";
        }
        if (menor == "menor")
        {
            negativo = true;
            menor = a;
            mayor = b;
        }
        else
        {
            menor = b;
            mayor = a;
        }
        menor = Arreglar(menor, mayor.Length - menor.Length);
        a = "";
        for (int i = mayor.Length - 1; i >= 0; i--)
        {
            resultado = int.Parse(mayor[i] + "") - int.Parse(menor[i] + "") - resto;
            if (resultado < 0)
            {
                resultado += 10;
                resto = 1;
            }
            else
            {
                resto = 0;
            }
            if (i == 0)
            {
                if (resultado != 0) a = resultado + a;
            }
            else a = resultado + a;
        }
        if (negativo) return "-" + CeroIzquierda(AgregatePoint(a, index));
        return CeroIzquierda(AgregatePoint(a, index));
    }

    private static string CompararCadena(string a, string b)
    {
        if (a.Length > b.Length)
        {
            return "mayor";
        }
        if (b.Length > a.Length)
        {
            return "menor";
        }
        for (int i = 0; i < a.Length; i++)
        {
            if (int.Parse(a[i] + "") > int.Parse(b[i] + ""))
            {
                return "mayor";
            }
            if (int.Parse(b[i] + "") > int.Parse(a[i] + ""))
            {
                return "menor";
            }
        }
        return "igual";
    }
    private static string Arreglar(string a, int indice, bool alante = true)
    {
        for (int j = 0; j < indice; j++)
        {
            if (alante) a = "0" + a;
            else a = a + "0";
        }
        return a;
    }
    private static string Suma(string a, string b)
    {
        a = CeroIzquierda(a);
        b = CeroIzquierda(b);

        int index = Normalizar(ref a, ref b, true);

        string suma = "";
        int resto = 0;
        int resultado = 0;
        if (a.Length > b.Length)
        {
            b = Arreglar(b, a.Length - b.Length);
        }
        if (a.Length < b.Length)
        {
            a = Arreglar(a, b.Length - a.Length);
        }
        for (int i = Math.Max(a.Length, b.Length) - 1; i >= 0; i--)
        {
            resultado = int.Parse(a[i] + "") + int.Parse(b[i] + "") + resto;
            if (resultado < 10) resto = 0;
            else resto = 1;
            if (i == 0) suma = resultado + suma;
            else suma = (resultado % 10) + suma;
        }
        return AgregatePoint(suma, index);
    }
}
