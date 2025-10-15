/* ===========================
   DB: Usuarios y Roles
   =========================== */

IF DB_ID('PV_NA_UsuariosRoles') IS NULL
    CREATE DATABASE PV_NA_UsuariosRoles;
GO
USE PV_NA_UsuariosRoles;
GO

CREATE TABLE Rol (
    ID_Rol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    CONSTRAINT CHK_Rol_Nombre CHECK (Nombre NOT LIKE '%[^A-Za-z ]%')
);

CREATE TABLE Usuario (
    ID_Usuario INT IDENTITY(1,1) PRIMARY KEY,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Tipo_Identificacion VARCHAR(20) NOT NULL,
    Identificacion VARCHAR(20) NOT NULL UNIQUE,
    Nombre VARCHAR(150) NOT NULL,
    Contrasena VARCHAR(255) NOT NULL,
    ID_Rol INT NOT NULL,
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (ID_Rol) REFERENCES Rol(ID_Rol),
    CONSTRAINT CHK_Email CHECK (Email LIKE '%cuc.cr' OR Email LIKE '%cuc.ac.cr')
);

CREATE TABLE Parametro (
    ID_Parametro VARCHAR(10) PRIMARY KEY,
    Valor VARCHAR(500) NOT NULL
);

CREATE TABLE Modulo (
    ID_Modulo INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Sesion (
    ID_Sesion INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario INT NOT NULL,
    Token_JWT VARCHAR(500) NOT NULL,
    Refresh_Token VARCHAR(500) NOT NULL,
    Expira DATETIME NOT NULL,
    CONSTRAINT FK_Sesion_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

/* ===========================
   INSERTS: Usuarios y Roles (Ejemplos)
   =========================== */

-- ===========================
-- ROLES
-- ===========================
INSERT INTO Rol (Nombre) VALUES
('Estudiante'),
('Profesor')

-- ===========================
-- PARÁMETROS DEL SISTEMA
-- ===========================
INSERT INTO Parametro (ID_Parametro, Valor) VALUES
('TOKENEXP', '5'), -- minutos de expiración
('REFEXP', '15'),  -- minutos de expiración del refresh token
('MAILDOM', 'cuc.ac.cr'),
('STUDDOM', 'cuc.cr'),
('MAXLOGIN', '3'),
('HASHALG', 'bcrypt'),
('LOGLEVEL', 'INFO'),
('MAXBITA', '5000'),
('MAILSRV', 'smtp.cuc.ac.cr'),
('MAILUSER', 'notificaciones@cuc.ac.cr');

-- ===========================
-- MÓDULOS DISPONIBLES
-- ===========================
INSERT INTO Modulo (Nombre) VALUES
('Usuarios y Roles'),
('Académico'),
('Matrícula'),
('Pagos'),
('Notificaciones'),
('Bitácora'),
('Catálogos'),
('Seguridad'),
('Integración'),
('Reportes');

