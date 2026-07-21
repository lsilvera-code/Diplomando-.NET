using System;
using System.IO;

namespace ValidadorTarjetas
{
    class Program
    {
        
        static int totalValidas = 0;
        static int totalInvalidas = 0;
        static int statVisa = 0, statMastercard = 0, statAmex = 0, statDiscover = 0, statDesconocida = 0;

        static void Main(string[] args)
        {
            int opcion = 0;

            do
            {
                Console.WriteLine("//// VALIDADOR DE TARJETAS ////");
                Console.WriteLine("1. Validar una tarjeta");
                Console.WriteLine("2. Validar desde archivo");
                Console.WriteLine("3. Generar número válido");
                Console.WriteLine("4. Estadísticas");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                try
                {
                    opcion = int.Parse(Console.ReadLine());

                    switch (opcion)
                    {
                        case 1:
                            Console.Write("Ingrese el número de tarjeta: ");
                            string numero = Console.ReadLine().Trim();
                            ProcesarYMostrarTarjeta(numero);
                            break;
                        case 2:
                            Console.Write("Ingrese la ruta del archivo (ej. tarjetas.txt): ");
                            string ruta = Console.ReadLine().Trim();
                            ValidarDesdeArchivo(ruta);
                            break;
                        case 3:
                            string numeroValido = GenerarNumeroValido();
                            Console.WriteLine($"\nNúmero generado: {numeroValido}");
                            Console.WriteLine($"Marca: {IdentificarMarca(numeroValido)}");
                            Console.WriteLine("Estado: VÁLIDA");
                            break;
                        case 4:
                            MostrarEstadisticas();
                            break;
                        case 5:
                            Console.WriteLine("Saliendo del programa...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Intente de nuevo.");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: Por favor ingrese un número válido para el menú.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                }

            } while (opcion != 5);
        }

        // Método auxiliar para no repetir código entre validación manual y archivo
        static void ProcesarYMostrarTarjeta(string numero)
        {
            string marca = IdentificarMarca(numero);
            bool esValida = ValidarTarjeta(numero);

            Console.WriteLine($"Número: {numero}");
            Console.WriteLine($"Marca: {marca}");
            Console.WriteLine($"Estado: {(esValida ? "VÁLIDA" : "INVÁLIDA")}\n");

            ActualizarEstadisticas(esValida, marca);
        }

        static bool ValidarTarjeta(string numero)
        {
            if (string.IsNullOrEmpty(numero)) return false;

            int sumaTotal = 0;
            bool duplicar = false;

            // Recorrer de derecha a izquierda
            for (int i = numero.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(numero[i])) return false; 

                int digito = (int)char.GetNumericValue(numero[i]);

                if (duplicar)
                {
                    digito *= 2;
                    if (digito >= 10)
                    {
                        
                        digito -= 9; 
                    }
                }

                sumaTotal += digito;
                duplicar = !duplicar; 
            }

            return sumaTotal % 10 == 0;
        }

        static string IdentificarMarca(string numero)
        {
            if (string.IsNullOrEmpty(numero)) return "Desconocida";

            int longitud = numero.Length;

            // Visa: Empieza con 4, longitud 13 o 16
            if ((longitud == 13 || longitud == 16) && numero.StartsWith("4"))
            {
                return "Visa";
            }

            // Mastercard: Empieza con 51-55, longitud 16
            if (longitud == 16)
            {
                for (int i = 51; i <= 55; i++)
                {
                    if (numero.StartsWith(i.ToString())) return "Mastercard";
                }
            }

            // American Express: Empieza con 34 o 37, longitud 15
            if (longitud == 15 && (numero.StartsWith("34") || numero.StartsWith("37")))
            {
                return "American Express";
            }

            // Discover: Longitud 16-19, prefijos específicos
            if (longitud >= 16 && longitud <= 19)
            {
                if (numero.StartsWith("6011") || numero.StartsWith("65"))
                {
                    return "Discover";
                }

                // Extraer los primeros 6 dígitos para validar rangos numéricos de Discover
                if (numero.Length >= 6)
                {
                    int prefijo6;
                    if (int.TryParse(numero.Substring(0, 6), out prefijo6))
                    {
                        if (prefijo6 >= 622126 && prefijo6 <= 622925) return "Discover";
                    }
                }
                
                if (numero.Length >= 3)
                {
                    int prefijo3;
                    if (int.TryParse(numero.Substring(0, 3), out prefijo3))
                    {
                        if (prefijo3 >= 644 && prefijo3 <= 649) return "Discover";
                    }
                }
            }

            return "Desconocida";
        }

        static void ValidarDesdeArchivo(string ruta)
        {
            try
            {
                string[] lineas = File.ReadAllLines(ruta);
                Console.WriteLine("\nProcesando archivo...");
                
                int conteoLinea = 0;
                foreach (string linea in lineas)
                {
                    string numero = linea.Trim();
                    if (!string.IsNullOrEmpty(numero))
                    {
                        conteoLinea++;
                        ProcesarYMostrarTarjeta(numero);
                    }
                }
                Console.WriteLine($"Se procesaron {conteoLinea} tarjetas del archivo.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: El archivo no fue encontrado. Verifique la ruta y el nombre.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al leer el archivo: {ex.Message}");
            }
        }

        static string GenerarNumeroValido()
        {
            Random rnd = new Random();
            string prefijo = "4";
            string baseNum = prefijo;

            // Generar 14 dígitos aleatorios adicionales (Total 15 por ahora)
            for (int i = 0; i < 14; i++)
            {
                baseNum += rnd.Next(0, 10).ToString();
            }

            // Calcular el dígito de control usando Luhn para que la tarjeta sea válida
            int sumaTotal = 0;
            bool duplicar = true; 

            for (int i = baseNum.Length - 1; i >= 0; i--)
            {
                int digito = (int)char.GetNumericValue(baseNum[i]);

                if (duplicar)
                {
                    digito *= 2;
                    if (digito >= 10) digito -= 9;
                }

                sumaTotal += digito;
                duplicar = !duplicar;
            }

            
            int digitoControl = (10 - (sumaTotal % 10)) % 10;
            
            return baseNum + digitoControl;
        }

        static void ActualizarEstadisticas(bool esValida, string marca)
        {
            if (esValida) totalValidas++;
            else totalInvalidas++;

            switch (marca)
            {
                case "Visa": statVisa++; break;
                case "Mastercard": statMastercard++; break;
                case "American Express": statAmex++; break;
                case "Discover": statDiscover++; break;
                default: statDesconocida++; break;
            }
        }

        static void MostrarEstadisticas()
        {
            Console.WriteLine("\n=== ESTADÍSTICAS ===");
            Console.WriteLine($"Total Válidas: {totalValidas}");
            Console.WriteLine($"Total Inválidas: {totalInvalidas}");
            Console.WriteLine("Desglose por marca (procesadas):");
            Console.WriteLine($"- Visa: {statVisa}");
            Console.WriteLine($"- Mastercard: {statMastercard}");
            Console.WriteLine($"- American Express: {statAmex}");
            Console.WriteLine($"- Discover: {statDiscover}");
            Console.WriteLine($"- Desconocidas: {statDesconocida}\n");
        }
    }
}