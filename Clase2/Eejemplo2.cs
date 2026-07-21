string nombre = "Ana";
int edad = 25;
double saldo = 1234.5678;

// Interpolación básica
Console.WriteLine($"Hola, {nombre}");

// Formato de números
Console.WriteLine($"Saldo: {saldo:C}");         // Moneda: $1,234.57
Console.WriteLine($"Saldo: {saldo:F2}");         // 2 decimales: 1234.57
Console.WriteLine($"Saldo: {saldo:N0}");         // Sin decimales con comas: 1,235

// Alineación
Console.WriteLine($"|{"Nombre",-10}|{"Edad",5}|");
Console.WriteLine($"|{nombre,-10}|{edad,5}|");