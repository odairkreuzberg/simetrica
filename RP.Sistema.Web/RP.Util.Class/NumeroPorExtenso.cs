using System;
using System.Collections;

namespace RP.Util.Class
{
    public class NumeroPorExtenso
    {
        public static string GetValorExtenco(decimal valor)
        {
            string _valorExtenco = "";
            int _contador = 0;
            bool _bilhao = false;
            bool _milhao = false;
            bool _mil = false;
            bool _unidade = false;

            if (valor == 0 || valor <= 0)
            {
                _valorExtenco = "Verificar se há valor negativo ou nada foi informado";
            }

            if (valor > (decimal)9999999999.99)
            {
                _valorExtenco = "Valor não suportado pela Função";
            }
            else 
            {
                decimal _centavos = valor - (Int64)valor;
                decimal _valorInteiro = (Int64)valor;
                string _numero;

                if (_valorInteiro > 0)
                {
                    if (_valorInteiro > 999)
                    {
                        _mil = true;
                    }

                    if (_valorInteiro > 999999)
                    {
                        _milhao = true;
                        _mil = false;
                    }

                    if (_valorInteiro > 999999999)
                    {
                        _mil = false;
                        _milhao = false;
                        _bilhao = true;
                    }

                    for (int i = (_valorInteiro.ToString().Trim().Length) - 1; i >= 0; i--)
                    {
                        _numero = Mid(_valorInteiro.ToString().Trim(), (_valorInteiro.ToString().Trim().Length - i) - 1, 1);

                        switch (i)
                        { 
                            case 9: 
                                {
                                    _valorExtenco = GetUnidades(_numero) + ((int.Parse(_numero) > 1) ? " Bilhões de" : " Bilhão de");

                                    _bilhao = true;
                                    break;
                                }

                            case 8: 

                            case 5:

                            case 2: 
                                {
                                    if (int.Parse(_numero) > 0)
                                    {
                                        string _centena = Mid(_valorInteiro.ToString().Trim(), (_valorInteiro.ToString().Trim().Length - i) - 1, 3);

                                        if (int.Parse(_centena) > 100 && int.Parse(_centena) < 200)
                                        {
                                            _valorExtenco = _valorExtenco + " Cento e ";
                                        }
                                        else
                                        {
                                            _valorExtenco = _valorExtenco + " " + GetCentenas(_numero);
                                        }
                                        if (_contador == 8)
                                        {
                                            _milhao = true;
                                        }
                                            else if (_contador == 5)
                                        {
                                            _mil = true;
                                        }
                                    }
                                    break;
                                }

                            case 7: 

                            case 4: 

                            case 1:
                                {
                                    if (int.Parse(_numero) > 0)
                                    {
                                        string _dezena = Mid(_valorInteiro.ToString().Trim(), (_valorInteiro.ToString().Trim().Length - i) - 1, 2);

                                        if (int.Parse(_dezena) > 10 && int.Parse(_dezena) < 20)
                                        {
                                            _valorExtenco = _valorExtenco + (Right(_valorExtenco, 5).Trim() == "entos" ? " e " : " ") + GetOnzeADezenove(Right(_dezena, 1));//corrigido

                                            _unidade = true;
                                        }
                                        else
                                        {
                                            _valorExtenco = _valorExtenco + (Right(_valorExtenco, 5).Trim() == "entos" ? " e " : " ") + GetDezenas(Left(_dezena, 1));

                                            _unidade = false;
                                        }
                                        if (_contador == 7)
                                        {
                                            _milhao = true;
                                        }
                                        else if (_contador == 4)
                                        {
                                            _mil = true;
                                        }
                                    }
                                    break;
                                }

                            case 6: 

                            case 3: 

                            case 0: 
                                {
                                    if (int.Parse(_numero) > 0 && !_unidade)
                                    {
                                        if ((Right(_valorExtenco, 5).Trim()) == "entos" || (Right(_valorExtenco, 3).Trim()) == "nte" || (Right(_valorExtenco, 3).Trim()) == "nta")
                                        {
                                            _valorExtenco = _valorExtenco + " e ";
                                        }
                                        else
                                        {
                                            _valorExtenco = _valorExtenco + " ";
                                        }
                                        _valorExtenco = _valorExtenco + GetUnidades(_numero);
                                    }
                                    if (i == 6)
                                    {
                                        if (_milhao || int.Parse(_numero) > 0)
                                        {
                                            _valorExtenco = _valorExtenco + ((int.Parse(_numero) == 1) && !_unidade ? " Milhão de" : " Milhões de");

                                            _milhao = true;
                                        }
                                    }
                                    if (i == 3)
                                    {
                                        if (_mil || int.Parse(_numero) > 0)
                                        {
                                            _valorExtenco = _valorExtenco + " Mil";

                                            _mil = true;
                                        }
                                    }

                                    if (i == 0)
                                    {
                                        if ((_bilhao && !_milhao && !_mil && Right((_valorInteiro.ToString().Trim()), 3) == "0") || (!_bilhao && _milhao && !_mil && Right((_valorInteiro.ToString().Trim()), 3) == "0"))
                                        {
                                            _valorExtenco = _valorExtenco + " de ";
                                        }
                                        _valorExtenco = _valorExtenco + ((Int64.Parse(_valorInteiro.ToString())) > 1 ? " Reais " : " Real ");
                                    }
                                    _unidade = false;

                                    break;
                                }
                        }
                    }
                }

                if (_centavos > 0)
                {
                    if ((Decimal.Parse(_centavos.ToString()) > 0) && (_centavos < (decimal)0.1))
                    {
                        _numero = Right((Decimal.Round(_centavos, 2)).ToString().Trim(), 1);

                        _valorExtenco = _valorExtenco + ((Decimal.Parse(_centavos.ToString()) > 0) ? " e " : " ") + GetUnidades(_numero) + ((Decimal.Parse(_numero) > 1) ? " Centavos " : " Centavo ");
                    }
                    else if (_centavos > (decimal)0.1 && _centavos < (decimal)0.2)
                    {
                        _numero = Right(((Decimal.Round(_centavos, 2) - (decimal)0.1).ToString().Trim()), 1);

                        _valorExtenco = _valorExtenco + ((Decimal.Parse(_centavos.ToString()) > 0) ? " e " : " ") + GetOnzeADezenove(_numero) + " Centavos ";
                    }
                    else
                    {
                        if (_centavos > 0) 
                        {
                            _numero = Right(_centavos.ToString("#,##0.00"), 2);

                            _valorExtenco = _valorExtenco + ((int.Parse(_numero) > 0) ? " e " : "") + GetDezenas(Left(_numero, 1));

                            if ((_centavos.ToString().Trim().Length) > 2)
                            {
                                _numero = Right((Decimal.Round(_centavos, 2)).ToString().Trim(), 1);

                                if (int.Parse(_numero) > 0)
                                {
                                    if (Mid(_valorExtenco.Trim(), _valorExtenco.Trim().Length - 2, 1) == "e")
                                    {
                                        _valorExtenco = _valorExtenco + " " + GetUnidades(_numero);
                                    }
                                    else
                                    {
                                        _valorExtenco = _valorExtenco + " e " + GetUnidades(_numero);
                                    }
                                }
                            }
                            _valorExtenco = _valorExtenco + " Centavos ";
                        }
                    }
                }
                if (_valorInteiro < 1) _valorExtenco = Mid(_valorExtenco.Trim(), 2, _valorExtenco.Trim().Length - 2);
            }
            return _valorExtenco.Trim();
        }

        private static string GetOnzeADezenove(string pstrDezena0)
        {
            var _result = new ArrayList { "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezesseis", "Dezessete", "Dezoito", "Dezenove" };

            return _result[((int.Parse(pstrDezena0)) - 1)].ToString();
        }

        private static string GetDezenas(string pstrDezena1)
        {
            var _result = new ArrayList { "Dez", "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };

            return _result[((int.Parse(pstrDezena1)) - 1)].ToString();
        }

        private static string GetCentenas(string pstrCentena)
        {
            var _result = new ArrayList { "Cem", "Duzentos", "Trezentos", "Quatrocentos", "Quinhentos", "Seiscentos", "Setecentos", "Oitocentos", "Novecentos" };

            return _result[((int.Parse(pstrCentena)) - 1)].ToString();
        }

        private static string GetUnidades(string pstrUnidade)
        {
            var _result = new ArrayList { "Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove" };

            return _result[((int.Parse(pstrUnidade)) - 1)].ToString();
        }

        public static string Left(string param, int length)
        {
            return (param == "") ? "" : param.Substring(0, length);
        }

        public static string Right(string param, int length)
        {
            return (param == "") ? "" : param.Substring(param.Length - length, length);
        }

        public static string Mid(string param, int startIndex, int length)
        {
            return param.Substring(startIndex, length);
        }

        public static string Mid(string param, int startIndex)
        {
            return param.Substring(startIndex);
        }
    }
}