using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RP.Util
{

    public static class Fonetiza
    {

        public static string Fonetizar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            str = RemoveAcentos(str.ToUpperInvariant());

            if (str.Equals("H"))
            {
                str = "AGA";
            }

            str = SomenteLetras(str);

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            //Eliminar palavras especiais
            str = str.Replace(" LTDA ", " ");

            //Eliminar preposições
            var preposicoes = new[] { " DE ", " DA ", " DO ", " AS ", " OS ", " AO ", " NA ", " NO ", " DOS ", " DAS ", " AOS ", " NAS ", " NOS ", " COM " };
            str = preposicoes.Aggregate(str, (current, preposicao) => current.Replace(preposicao, " "));

            //Converte algarismos romanos para números
            var algRomanos = new[] { " V ", " I ", " IX ", " VI ", " IV ", " II ", " VII ", " III ", " X ", " VIII " };
            var numeros = new[] { " 5 ", " 1 ", " 9 ", " 6 ", " 4 ", " 2 ", " 7 ", " 3 ", " 10 ", " 8 " };
            for (int i = 0; i < algRomanos.Length; i++)
            {
                str = str.Replace(algRomanos[i], numeros[i]);
            }

            //Converte numeros para literais
            var algarismosExtenso = new[] { "ZERO", "UM", "DOIS", "TRES", "QUATRO", "CINCO", "SEIS", "SETE", "OITO", "NOVE" };
            for (int i = 0; i < 10; i++)
            {
                str = str.Replace(i.ToString(), algarismosExtenso[i]);
            }

            //Elimina preposições e artigos
            var letras = new[] { " A ", " B ", " C ", " D ", " E ", " F ", " G ", " H ", " I ", " J ", " K ", " L ", " M ", " N ", " O ", " P ", " Q ", " R ", " S ", " T ", " U ", " V ", " X ", " Z ", " W ", " Y " };
            str = letras.Aggregate(str, (current, letra) => current.Replace(letra, " "));

            str = str.Trim();
            var particulas = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var fonetizados = new string[particulas.Length];
            for (var i = 0; i < particulas.Length; i++)
            {
                fonetizados[i] = FonetizarParticula(particulas[i]);
            }
            string fonetizado = string.Join(" ", fonetizados).Trim();

            return fonetizado;
        }

        private static string SomenteLetras(string texto)
        {
            const string letras = "ABCDEFGHIJKLMNOPQRSTUVXZWY ";
            var resultado = string.Empty;
            var letraAnt = texto[0];
            foreach (var letraT in texto)
            {
                foreach (var letraC in letras.Where(letraC => letraC == letraT).TakeWhile(letraC => letraAnt != ' ' || letraT != ' '))
                {
                    resultado += letraC;
                    letraAnt = letraT;
                    break;
                }
            }

            return resultado.ToUpperInvariant();
        }

        private static string FonetizarParticula(string str)
        {
            string aux2;
            int j;
            const string letras = "ABPCKQDTEIYFVWGJLMNOURSZX9";
            const string codFonetico = "123444568880AABCDEEGAIJJL9";

            str = str.ToUpperInvariant();
            string aux = str[0].ToString();

            //Elimina os caracteres repetidos
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i - 1] != str[i])
                {
                    aux += str[i];
                }
            }

            //Iguala os fonemas parecidos
            if (aux[0].Equals('W'))
            {
                if (aux[1].Equals('I'))
                {
                    aux = aux.Remove(0, 1).Insert(0, "U");
                }
                else if ("A,E,O,U".Contains(aux[1]))
                {
                    aux = aux.Remove(0, 1).Insert(0, "V");
                }
            }
            aux = SubstituiTerminacao(aux);

            var caracteres = new[]
                                  {
                                      "TSCH", "SCH", "TSH", "TCH", "SH", "CH", "LH", "NH", "PH", "GN", "MN", "SCE", "SCI", "SCY"
                                      , "CS", "KS", "PS", "TS", "TZ", "XS", "CE", "CI", "CY", "GE", "GI", "GY", "GD", "CK", "PC"
                                      , "QU", "SC", "SK", "XC", "SQ", "CT", "GT", "PT"
                                  };
            var caracteresSub = new[]
                                     {
                                         "XXXX", "XXX", "XXX", "XXX", "XX", "XX", "LI", "NN", "FF", "NN", "NN", "SSI", "SSI",
                                         "SSI", "SS", "SS", "SS", "SS", "SS", "SS", "SE", "SI", "SI", "JE", "JI", "JI", "DD",
                                         "QQ", "QQ", "QQ", "SQ", "SQ", "SQ", "99", "TT", "TT", "TT"
                                     };
            for (int i = 0; i < caracteres.Length; i++)
            {
                aux = aux.Replace(caracteres[i], caracteresSub[i]);
            }

            //Trata consoantes mudas
            aux = TrataConsoanteMuda(aux, 'B', 'I');
            aux = TrataConsoanteMuda(aux, 'D', 'I');
            aux = TrataConsoanteMuda(aux, 'P', 'I');

            //Trata as letras
            //Retira letras iguais
            if (aux[0].Equals('H'))
            {
                if (aux.Length > 1)
                {
                    aux2 = Convert.ToString(aux[1]);
                }
                else
                {
                    aux2 = string.Empty;
                }
                j = 2;
            }
            else
            {
                aux2 = Convert.ToString(aux[0]);
                j = 1;
            }

            while (j < aux.Length)
            {
                if (aux[j] != aux[j - 1] && aux[j] != 'H')
                {
                    aux2 += aux[j];
                }
                j++;
            }

            aux = aux2;

            //Transforma letras em códigos fonéticos
            return aux.Select(chr => letras.IndexOf(chr)).Aggregate(string.Empty, (current, n) => current + codFonetico[n]);
        }

        private static string TrataConsoanteMuda(string str, char consoante, char complemento)
        {
            var i = str.IndexOf(consoante);
            while (i > -1)
            {
                if (i >= str.Length - 1 || !"AEIOU".Contains(str[i + 1]))
                {
                    str = str.Insert(i + 1, Convert.ToString(complemento));
                    i++;
                }
                i = str.IndexOf(consoante, ++i);
            }
            return str;
        }

        private static string SubstituiTerminacao(string str)
        {
            str = RemoveAcentos(str);

            var terminacao = new[] { "N", "B", "D", "T", "W", "AM", "OM", "OIM", "UIM", "CAO", "AO", "OEM", "ONS", "EIA", "X", "US", "TH" };
            var terminacaoSub = new[] { "M", "", "", "", "", "N", "N", "N", "N", "SSN", "N", "N", "N", "IA", "IS", "OS", "TI" };
            var tamanhoMinStr = new[] { 2, 3, 3, 3, 3, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3 };
            int tamanho = 0;
            do
            {
                for (int i = 0; i < terminacao.Length; i++)
                {
                    if (str.EndsWith(terminacao[i]) && str.Length >= tamanhoMinStr[i])
                    {
                        var startIndex = str.Length - terminacao[i].Length;
                        str = str.Remove(startIndex, terminacao[i].Length)
                            .Insert(startIndex, terminacaoSub[i]);
                    }
                    else if (str.Length < tamanhoMinStr[i])
                    {
                        tamanho = tamanhoMinStr[i];
                        break;
                    }
                }
            } while (str.EndsWith("N") && str.Length >= tamanho);
            return str;
        }

        private static string RemoveAcentos(string texto)
        {
            const string comAcento = "áÁàÀâÂãÃéÉèÈêÊíÍìÌîÎóÓòÒôÔõÕúÚùÙûÛüÜçÇñÑ";
            const string semAcento = "AAAAAAAAEEEEEEIIIIIIOOOOOOOOUUUUUUUUCCNN";

            for (var i = 0; i < comAcento.Length; i++)
            {
                texto = texto.Replace(comAcento[i], semAcento[i]);
            }
            return texto;
        }
    }

}