/* ===========================
   DB: Matrícula
   =========================== */

IF DB_ID('PV_NA_Matricula') IS NULL
    CREATE DATABASE PV_NA_Matricula;
GO
USE PV_NA_Matricula;
GO

CREATE TABLE Estudiante (
    ID_Estudiante INT IDENTITY(1,1) PRIMARY KEY,
    Identificacion VARCHAR(20) NOT NULL UNIQUE,
    Tipo_Identificacion VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Nombre VARCHAR(150) NOT NULL,
    Fecha_Nacimiento DATE NOT NULL,
    Direccion VARCHAR(250) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    CONSTRAINT CHK_EmailEstudiante CHECK (Email LIKE '%cuc.cr')
);

CREATE TABLE Provincia (
    ID_Provincia INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Canton (
    ID_Canton INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ID_Provincia INT NOT NULL,
    CONSTRAINT FK_Canton_Provincia FOREIGN KEY (ID_Provincia) REFERENCES Provincia(ID_Provincia)
);

CREATE TABLE Distrito (
    ID_Distrito INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ID_Canton INT NOT NULL,
    ID_Provincia INT NOT NULL,
    CONSTRAINT FK_Distrito_Canton FOREIGN KEY (ID_Canton) REFERENCES Canton(ID_Canton),
    CONSTRAINT FK_Distrito_Provincia FOREIGN KEY (ID_Provincia) REFERENCES Provincia(ID_Provincia)
);

CREATE TABLE PreMatricula (
    ID_Prematricula INT IDENTITY(1,1) PRIMARY KEY,
    ID_Estudiante INT NOT NULL,
    ID_Carrera INT NOT NULL,
    ID_Curso INT NOT NULL,
    Observaciones VARCHAR(255),
    ID_Periodo INT NOT NULL,
    CONSTRAINT FK_PM_Estud FOREIGN KEY (ID_Estudiante) REFERENCES Estudiante(ID_Estudiante)
);

CREATE TABLE Matricula (
    ID_Matricula INT IDENTITY(1,1) PRIMARY KEY,
    ID_Estudiante INT NOT NULL,
    ID_Curso INT NOT NULL,
    ID_Grupo INT NOT NULL,
    ID_Periodo INT NOT NULL,
    CONSTRAINT FK_M_Estud FOREIGN KEY (ID_Estudiante) REFERENCES Estudiante(ID_Estudiante)
);

CREATE TABLE Nota_Rubro (
    ID_Rubro INT IDENTITY(1,1) PRIMARY KEY,
    ID_Grupo INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Porcentaje INT NOT NULL,
    CONSTRAINT CHK_Porc CHECK (Porcentaje > 0 AND Porcentaje <= 100)
);

CREATE TABLE Nota (
    ID_Nota INT IDENTITY(1,1) PRIMARY KEY,
    ID_Rubro INT NOT NULL,
    ID_Estudiante INT NOT NULL,
    Valor DECIMAL(5,2) NOT NULL CHECK (Valor BETWEEN 0 AND 100),
    CONSTRAINT FK_Nota_Rubro FOREIGN KEY (ID_Rubro) REFERENCES Nota_Rubro(ID_Rubro),
    CONSTRAINT FK_Nota_Estud FOREIGN KEY (ID_Estudiante) REFERENCES Estudiante(ID_Estudiante)
);

/* ===========================
   INSERTS: Matrícula (Ejemplos)
   =========================== */

-- ===========================
-- PROVINCIAS
-- ===========================
INSERT INTO Provincia (Nombre) VALUES
('San José'),
('Alajuela'),
('Cartago'),
('Heredia'),
('Guanacaste'),
('Puntarenas'),
('Limón'),
('Los Santos'),
('Sarapiquí'),
('Turrialba');

-- ===========================
-- CANTONES
-- ===========================
INSERT INTO Canton (Nombre, ID_Provincia) VALUES
('Central', 1),
('Desamparados', 1),
('Alajuela', 2),
('Cartago', 3),
('Heredia', 4),
('Liberia', 5),
('Puntarenas', 6),
('Limón', 7),
('Tarrazu', 8),
('Sarapiquí', 9);

-- ===========================
-- DISTRITOS
-- ===========================
INSERT INTO Distrito (Nombre, ID_Canton, ID_Provincia) VALUES
('Carmen', 1, 1),
('San Rafael Arriba', 2, 1),
('San José', 3, 2),
('Oriental', 4, 3),
('Heredia Centro', 5, 4),
('Liberia Centro', 6, 5),
('Barranca', 7, 6),
('Limón Centro', 8, 7),
('San Marcos', 9, 8),
('Puerto Viejo', 10, 9);

-- ===========================
-- ESTUDIANTES
-- ===========================
INSERT INTO Estudiante (Identificacion, Tipo_Identificacion, Email, Nombre, Fecha_Nacimiento, Direccion, Telefono) VALUES
('123456789', 'Cédula', 'juan.perez@cuc.cr', 'Juan Pérez', '2000-04-15', 'Carmen, San José', '8888-1111'),
('987654321', 'Cédula', 'maria.lopez@cuc.cr', 'María López', '1999-06-10', 'Desamparados, San José', '8888-2222'),
('112233445', 'Cédula', 'carlos.gomez@cuc.cr', 'Carlos Gómez', '2001-03-05', 'Alajuela Centro', '8888-3333'),
('223344556', 'Cédula', 'ana.morales@cuc.cr', 'Ana Morales', '2002-01-20', 'Cartago Centro', '8888-4444'),
('334455667', 'Cédula', 'luis.rodriguez@cuc.cr', 'Luis Rodríguez', '1998-11-11', 'Heredia Centro', '8888-5555'),
('445566778', 'Cédula', 'laura.jimenez@cuc.cr', 'Laura Jiménez', '2001-09-09', 'Liberia, Guanacaste', '8888-6666'),
('556677889', 'Cédula', 'pedro.vargas@cuc.cr', 'Pedro Vargas', '2000-02-02', 'Barranca, Puntarenas', '8888-7777'),
('667788990', 'Cédula', 'sofia.ramirez@cuc.cr', 'Sofía Ramírez', '2003-12-12', 'Limón Centro', '8888-8888'),
('778899001', 'Cédula', 'daniel.castro@cuc.cr', 'Daniel Castro', '1999-05-05', 'San Marcos, Tarrazú', '8888-9999'),
('889900112', 'Cédula', 'karla.mendez@cuc.cr', 'Karla Méndez', '2002-07-25', 'Puerto Viejo, Sarapiquí', '8888-0000');

-- ===========================
-- PREMATRÍCULA
-- ===========================
INSERT INTO PreMatricula (ID_Estudiante, ID_Carrera, ID_Curso, Observaciones, ID_Periodo) VALUES
(1, 1, 1, 'Interesado en Programación I', 1),
(2, 1, 2, 'Desea matricular Matemática General', 1),
(3, 2, 3, NULL, 1),
(4, 2, 1, 'Tiene beca activa', 1),
(5, 3, 2, NULL, 1),
(6, 3, 4, 'Cambio de grupo solicitado', 1),
(7, 4, 2, NULL, 1),
(8, 4, 3, NULL, 1),
(9, 5, 1, 'Primer ingreso', 1),
(10, 5, 4, 'Repite el curso', 1);

-- ===========================
-- MATRÍCULA
-- ===========================
INSERT INTO Matricula (ID_Estudiante, ID_Curso, ID_Grupo, ID_Periodo) VALUES
(1, 1, 1, 1),
(2, 2, 1, 1),
(3, 3, 1, 1),
(4, 1, 2, 1),
(5, 2, 1, 1),
(6, 4, 1, 1),
(7, 2, 2, 1),
(8, 3, 2, 1),
(9, 1, 1, 1),
(10, 4, 2, 1);

-- ===========================
-- NOTA RUBRO
-- ===========================
INSERT INTO Nota_Rubro (ID_Grupo, Nombre, Porcentaje) VALUES
(1, 'Examen Parcial 1', 30),
(1, 'Proyecto Final', 40),
(1, 'Tareas', 30),
(2, 'Examen Parcial 1', 30),
(2, 'Laboratorio', 30),
(2, 'Examen Final', 40),
(3, 'Proyecto Integrador', 100),
(4, 'Ensayo Ético', 50),
(4, 'Exposición Final', 50),
(5, 'Evaluación Global', 100);

-- ===========================
-- NOTAS
-- ===========================
INSERT INTO Nota (ID_Rubro, ID_Estudiante, Valor) VALUES
(1, 1, 85.00),
(2, 1, 90.00),
(3, 1, 80.00),
(4, 2, 75.00),
(5, 2, 88.00),
(6, 2, 92.00),
(7, 3, 95.00),
(8, 4, 89.00),
(9, 5, 93.00),
(10, 6, 97.00);
