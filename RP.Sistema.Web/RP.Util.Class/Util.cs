using System;
using System.Text;
using System.Security.Cryptography;
using RestSharp.Serializers;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace RP.Util.Class
{
    public class Util
    {
        public static string TF2SN(string value)
        {
            if (value.ToLower().Trim() == "sim" || value.ToLower().Trim() == "não")
                return value;
            if (value.ToLower().Trim() == "true")
                return "Sim";
            return "Não";
        }

        public static string SN2TF(string value)
        {
            if (value.ToLower().Trim() == "sim")
                return "true";
            return "false";
        }

        public static string randomString(int size, bool lowerCase, bool onlyCaracter)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            int nr;

            for (int i = 0; i < size; i++)
            {
                ch = 'y';
                if (!onlyCaracter)
                {
                    nr = random.Next(25);
                    if (nr > 9)
                    {
                        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    }
                    else
                    {
                        ch = nr.ToString()[0];
                    }
                }
                if (ch == '0' || ch.ToString().ToLower() == "o")
                {
                    ch = 't';
                }

                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();

            return builder.ToString();
        }

        public static string getHash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            using (HashAlgorithm sha = new SHA256Managed())
            {
                byte[] encryptedBytes = sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        public static string getMd5Hash(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static object INTDB(int? value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value.Value;
        }

        public static string IdadeExtenso(DateTime dtNascimento)
        {
            DateTime dtIdade;
            try
            {
                dtIdade = new DateTime(DateTime.Now.Subtract(dtNascimento).Ticks);
            }
            catch (System.Exception)
            {
                return "Idade invalida";
            }

            int ano = dtIdade.Year - 1;
            int mes = dtIdade.Month - 1;
            int dia = dtIdade.Day - 1;
            string dsAno = string.Empty;
            string dsMes = string.Empty;
            string dsDia = string.Empty;

            if (ano == 1)
                dsAno = "1 ano";
            else if (ano > 1)
                dsAno = ano.ToString() + " anos";

            if (mes == 1)
                dsMes = " 1 mês";
            else if (mes > 1)
                dsMes = mes.ToString() + " meses";

            if ((ano > 0) & (mes > 0)) dsMes = " e " + dsMes;

            if (dia > 0 && mes == 0 && ano == 0)
            {
                if (dia == 1)
                    dsDia = "1 dia";
                else
                    dsDia = dia.ToString() + " dias";

                return dsDia;
            }
            return (dsAno + dsMes);
        }

        // o metodo isCPFCNPJ recebe dois parâmetros:
        // uma string contendo o cpf ou cnpj a ser validado
        // e um valor do tipo boolean, indicando se o método irá
        // considerar como válido um cpf ou cnpj em branco.
        // o retorno do método também é do tipo boolean:
        // (true = cpf ou cnpj válido; false = cpf ou cnpj inválido)
        public static bool isCPFCNPJ(string cpfcnpj, bool vazio)
        {
            if (string.IsNullOrEmpty(cpfcnpj))
                return vazio;
            else
            {
                int[] d = new int[14];
                int[] v = new int[2];
                int j, i, soma;
                string Sequencia, SoNumero;

                SoNumero = Regex.Replace(cpfcnpj, "[^0-9]", string.Empty);

                //verificando se todos os numeros são iguais
                if (new string(SoNumero[0], SoNumero.Length) == SoNumero) return false;

                // se a quantidade de dígitos numérios for igual a 11
                // iremos verificar como CPF
                if (SoNumero.Length == 11)
                {
                    for (i = 0; i <= 10; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 8 + i; j++) soma += d[j] * (10 + i - j);

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[9] & v[1] == d[10]);
                }
                // se a quantidade de dígitos numérios for igual a 14
                // iremos verificar como CNPJ
                else if (SoNumero.Length == 14)
                {
                    Sequencia = "6543298765432";
                    for (i = 0; i <= 13; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 11 + i; j++)
                            soma += d[j] * Convert.ToInt32(Sequencia.Substring(j + 1 - i, 1));

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[12] & v[1] == d[13]);
                }
                // CPF ou CNPJ inválido se
                // a quantidade de dígitos numérios for diferente de 11 e 14
                else return false;
            }
        }

        public static string IdadeExtenso(DateTime? nullable)
        {
            throw new NotImplementedException();
        }

        public static int Idade(DateTime dtNascimento)
        {
            if ((DateTime.Now.Month > dtNascimento.Month) || (DateTime.Now.Month == dtNascimento.Month && DateTime.Now.Day >= dtNascimento.Day))
            {
                return (DateTime.Now.Date.Year - dtNascimento.Year);
            }
            return (DateTime.Now.Date.Year - dtNascimento.Year) - 1;
        }
    }

    public class RestSharpDataContractJsonSerializer : ISerializer
    {
        public RestSharpDataContractJsonSerializer()
        {
            ContentType = "application/json";
        }

        ///
        /// Serialize the object as JSON
        ///
        /// <param name="obj" />Object to serialize
        /// JSON as String
        public string Serialize(object obj)
        {
            //Create a stream to serialize the object to.
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            ms.Close();
            return System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
        }

        ///
        /// Unused for JSON Serialization
        ///
        public string DateFormat { get; set; }
        ///
        /// Unused for JSON Serialization
        ///
        public string RootElement { get; set; }
        ///
        /// Unused for JSON Serialization
        ///
        public string Namespace { get; set; }
        ///
        /// Content type for serialized content
        ///
        public string ContentType { get; set; }
    }
}