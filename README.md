# Proyecto Nuevo Avatar

Este repositorio contiene la suite de microservicios Nuevo Avatar, un sistema académico modular desarrollado con .NET 8, Dapper y SQL Server, organizado por dominios funcionales (UsuariosRoles, OfertaAcadémica, Pagos, Bitácora, Seguridad, etc.).

Cada módulo es un microservicio independiente con su propio Program.cs, appsettings.json y estructura MVC mínima.

------------------------------------------------------------

## Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instalados:

- Visual Studio 2022 o Visual Studio Code
- .NET SDK 8.0 o superior
- SQL Server 2019 o superior
- Git

------------------------------------------------------------

## Clonar y Restaurar Dependencias

1. Clonar el repositorio:

   git clone URL DEL REPOSITORIO
   cd NuevoAvatar

2. Restaurar dependencias:

   dotnet restore

   Esto descargará automáticamente todos los paquetes requeridos, incluyendo:

   - Dapper
   - Microsoft.Data.SqlClient
   - Swashbuckle.AspNetCore (Swagger)
   - Microsoft.Extensions.Configuration
   - Microsoft.AspNetCore.Authentication.JwtBearer (para módulos con autenticación)
   - Otros definidos en cada archivo .csproj

3. Compilar la solución completa:

   dotnet build

4. Ejecutar un microservicio específico (ejemplo UsuariosRoles):

   cd PV_NA_UsuariosRoles
   dotnet run

   El servicio se levantará en la dirección (según configuración):
   https://localhost:5230/swagger

------------------------------------------------------------

## Restauración Manual de Paquetes (si es necesario)

Si el comando dotnet restore no instala correctamente algún paquete, puedes hacerlo manualmente:

   cd PV_NA_UsuariosRoles
   dotnet add package Dapper
   dotnet add package Microsoft.Data.SqlClient
   dotnet add package Swashbuckle.AspNetCore

------------------------------------------------------------

## Comandos Útiles

Acción                          | Comando
--------------------------------|-------------------------------------
Restaurar dependencias          | dotnet restore
Compilar solución               | dotnet build
Ejecutar un servicio            | dotnet run
Limpiar binarios                | dotnet clean
Borrar archivos temporales      | git clean -xdf

------------------------------------------------------------

COMO BUENA PRACTICA

recuerda limpiar el proyecto antes de subirlo

entra a VIEW, busca TERMINAL y si estas en la raiz del proyecto (C:\Users\Usuario\workspace\NuevoAvatar_V2) corre el siguiente comando:

dotnet clean 

