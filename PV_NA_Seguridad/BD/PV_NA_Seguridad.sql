/* ===========================
   DB: Seguridad y Auditoría=========================== */

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
   INSERTS: Bitácora (Ejemplos)
   =========================== */

INSERT INTO Bitacora (ID_Usuario, Accion) VALUES
(1, 'Registro nuevo usuario: {"Email":"juan.perez@cuc.cr","Rol":"Estudiante"}'),
(2, 'Actualización de usuario: {"Anterior":{"Nombre":"Maria Lopez"},"Actual":{"Nombre":"Maria Lopez Quesada"}}'),
(3, 'Eliminación de usuario: {"Email":"usuario.inactivo@cuc.ac.cr","Motivo":"Cuenta desactivada"}'),
(1, 'El usuario consulta listado de roles'),
(2, 'Registro nuevo rol: {"Rol":"Coordinador"}'),
(3, 'Actualización de parámetro: {"Anterior":{"TOKEN_EXP":"5"},"Actual":{"TOKEN_EXP":"10"}}'),
(1, 'El usuario consulta listado de parámetros'),
(2, 'Registro nuevo módulo: {"Modulo":"Matrícula"}'),
(3, 'Eliminación de módulo: {"Modulo":"Pagos","Motivo":"Reestructuración"}'),
(1, 'Error técnico: {"Servicio":"/usuario","Mensaje":"Excepción de conexión a base de datos"}');
