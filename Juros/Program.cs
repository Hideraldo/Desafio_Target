using System;
using System.Globalization;

namespace Juros
{
    internal class Program
    {
        private const decimal TaxaJurosDiaria = 0.025m; // 2,5% ao dia

        static void Main(string[] args)
        {
            // Define a cultura padrão para português do Brasil
            CultureInfo culturaPtBr = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culturaPtBr;
            CultureInfo.DefaultThreadCurrentUICulture = culturaPtBr;

            try
            {
                Console.Title = "Cálculo de Juros por Atraso - 2,5% ao dia";
            }
            catch { }

            bool continuar = true;
            while (continuar)
            {
                if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
                {
                    try { Console.Clear(); } catch { }
                }

                ExibirCabecalho();

                decimal valorOriginal = LerValorMonetario(culturaPtBr);
                DateTime dataVencimento = LerDataVencimento(culturaPtBr);
                DateTime dataHoje = DateTime.Today;

                CalcularEExibirResultado(valorOriginal, dataVencimento, dataHoje, culturaPtBr);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Deseja realizar outro cálculo? (S/N): ");
                Console.ResetColor();

                string resposta = Console.ReadLine();
                if (resposta == null)
                {
                    break;
                }

                resposta = resposta.Trim().ToUpper();
                continuar = (resposta == "S" || resposta == "SIM");
            }

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para encerrar o programa...");
                try { Console.ReadKey(); } catch { }
            }
        }

        private static void ExibirCabecalho()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================================");
            Console.WriteLine("           PROGRAMA DE CÁLCULO DE JUROS POR ATRASO            ");
            Console.WriteLine("              Multa/Juros: 2,5% ao dia de atraso              ");
            Console.WriteLine("===============================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static decimal LerValorMonetario(CultureInfo cultura)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Digite o valor do título/boleto (ex: 1000,00): R$ ");
                Console.ResetColor();

                string entrada = Console.ReadLine();
                if (entrada == null) return 0m;

                entrada = entrada.Trim();

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    ExibirErro("O valor não pode ser vazio.");
                    continue;
                }

                // Trata tanto vírgula quanto ponto
                entrada = entrada.Replace("R$", "").Trim();
                if (decimal.TryParse(entrada, NumberStyles.Currency | NumberStyles.Number, cultura, out decimal valor) ||
                    decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                {
                    if (valor <= 0)
                    {
                        ExibirErro("O valor deve ser maior que zero.");
                        continue;
                    }

                    return Math.Round(valor, 2);
                }

                ExibirErro("Valor inválido. Digite um número válido, por exemplo: 1500,50");
            }
        }

        private static DateTime LerDataVencimento(CultureInfo cultura)
        {
            string[] formatosValidos = { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" };

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Digite a data de vencimento no formato brasileiro (dia/mês/ano - ex: 12/03/2026): ");
                Console.ResetColor();

                string entrada = Console.ReadLine();
                if (entrada == null) return DateTime.Today;

                entrada = entrada.Trim();

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    ExibirErro("A data de vencimento não pode ser vazia.");
                    continue;
                }

                if (DateTime.TryParseExact(entrada, formatosValidos, cultura, DateTimeStyles.None, out DateTime dataVencimento))
                {
                    return dataVencimento.Date;
                }

                ExibirErro("Data inválida. Certifique-se de digitar no formato brasileiro: dia/mês/ano (ex: 12/03/2026).");
            }
        }

        private static void CalcularEExibirResultado(decimal valorOriginal, DateTime dataVencimento, DateTime dataHoje, CultureInfo cultura)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------------------- RESUMO DO CÁLCULO ----------------------");
            Console.ResetColor();

            Console.WriteLine($"Valor Original:            {valorOriginal.ToString("C2", cultura)}");
            Console.WriteLine($"Data de Vencimento:        {dataVencimento:dd/MM/yyyy}");
            Console.WriteLine($"Data Atual (Hoje):         {dataHoje:dd/MM/yyyy}");
            Console.WriteLine($"Taxa de Multa/Juros:       2,5% ao dia ({TaxaJurosDiaria:P1})");

            if (dataVencimento >= dataHoje)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                if (dataVencimento == dataHoje)
                {
                    Console.WriteLine(">> O título vence HOJE. Não há incidência de juros por atraso.");
                }
                else
                {
                    int diasParaVencer = (dataVencimento - dataHoje).Days;
                    Console.WriteLine($">> O título ainda não venceu (faltam {diasParaVencer} dia(s)). Não há juros.");
                }

                Console.ResetColor();
                Console.WriteLine($"Valor dos Juros:           {0m.ToString("C2", cultura)}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Valor Total a Pagar:       {valorOriginal.ToString("C2", cultura)}");
                Console.ResetColor();
                Console.WriteLine("--------------------------------------------------------------");
                return;
            }

            int diasAtraso = (dataHoje - dataVencimento).Days;
            decimal percentualJurosTotal = TaxaJurosDiaria * diasAtraso;
            decimal valorJuros = Math.Round(valorOriginal * percentualJurosTotal, 2, MidpointRounding.AwayFromZero);
            decimal valorTotalPagar = valorOriginal + valorJuros;

            Console.WriteLine($"Dias em Atraso:            {diasAtraso} dia(s)");
            Console.WriteLine($"Percentual Total de Juros: {percentualJurosTotal:P1} ({TaxaJurosDiaria:P1} x {diasAtraso} dias)");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Valor dos Juros (2,5%/dia):{valorJuros.ToString("C2", cultura)}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Valor Total Atualizado:    {valorTotalPagar.ToString("C2", cultura)}");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------------------");
        }

        private static void ExibirErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO] {mensagem}");
            Console.ResetColor();
        }
    }
}
