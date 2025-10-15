/* ===========================
   DB: Seguridad y Auditor�a=========================== */

IF DB_ID('PV_NA_Seguridad') IS NULL
    CREATE DATABASE PV_NA_Seguridad;
GO
USE PV_NA_Seguridad;
GO

CREATE TABLE Bitacora (
    ID_Bitacora INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario INT NOT NULL,
    Accion VARCHAR(MAX) NOT NULL,
    Fecha DATETIME DEFAULT GETDATE()
);

/* ===========================
   INSERTS: Bit�cora (Ejemplos)
   =========================== */

INSERT INTO Bitacora (ID_Usuario, Accion) VALUES
(1, 'Registro nuevo usuario: {"Email":"juan.perez@cuc.cr","Rol":"Estudiante"}'),
(2, 'Actualizaci�n de usuario: {"Anterior":{"Nombre":"Maria Lopez"},"Actual":{"Nombre":"Maria Lopez Quesada"}}'),
(3, 'Eliminaci�n de usuario: {"Email":"usuario.inactivo@cuc.ac.cr","Motivo":"Cuenta desactivada"}'),
(1, 'El usuario consulta listado de roles'),
(2, 'Registro nuevo rol: {"Rol":"Coordinador"}'),
(3, 'Actualizaci�n de par�metro: {"Anterior":{"TOKEN_EXP":"5"},"Actual":{"TOKEN_EXP":"10"}}'),
(1, 'El usuario consulta listado de par�metros'),
(2, 'Registro nuevo m�dulo: {"Modulo":"Matr�cula"}'),
(3, 'Eliminaci�n de m�dulo: {"Modulo":"Pagos","Motivo":"Reestructuraci�n"}'),
(1, 'Error t�cnico: {"Servicio":"/usuario","Mensaje":"Excepci�n de conexi�n a base de datos"}');
