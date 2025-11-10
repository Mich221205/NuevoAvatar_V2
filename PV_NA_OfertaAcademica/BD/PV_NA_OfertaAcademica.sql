/* ===========================
   DB: Oferta Académica
   =========================== */

IF DB_ID('PV_NA_OfertaAcademica') IS NULL
    CREATE DATABASE PV_NA_OfertaAcademica;
GO
USE PV_NA_OfertaAcademica;
GO

CREATE TABLE Institucion (
    ID_Institucion INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Profesor (
    ID_Profesor INT IDENTITY(1,1) PRIMARY KEY,
    Identificacion VARCHAR(20) NOT NULL UNIQUE,
    Tipo_Identificacion VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Nombre VARCHAR(150) NOT NULL,
    Fecha_Nacimiento DATE NOT NULL,
    CONSTRAINT CHK_EmailProfesor CHECK (Email LIKE '%cuc.ac.cr')
);

CREATE TABLE Carrera (
    ID_Carrera INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ID_Institucion INT NOT NULL,
    ID_Profesor_Director INT NOT NULL,
    CONSTRAINT FK_Carrera_Inst FOREIGN KEY (ID_Institucion) REFERENCES Institucion(ID_Institucion),
    CONSTRAINT FK_Carrera_Prof FOREIGN KEY (ID_Profesor_Director) REFERENCES Profesor(ID_Profesor)
);

CREATE TABLE Curso (
    ID_Curso INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Nivel INT NOT NULL,
    ID_Carrera INT NOT NULL,
    CONSTRAINT CHK_Nivel CHECK (Nivel BETWEEN 1 AND 12),
    CONSTRAINT FK_Curso_Carrera FOREIGN KEY (ID_Carrera) REFERENCES Carrera(ID_Carrera)
);

CREATE TABLE Periodo (
    ID_Periodo INT IDENTITY(1,1) PRIMARY KEY,
    Anno INT NOT NULL,
    Numero INT NOT NULL,
    Fecha_Inicio DATE NOT NULL,
    Fecha_Fin DATE NOT NULL
);

CREATE TABLE Grupo (
    ID_Grupo INT IDENTITY(1,1) PRIMARY KEY,
    NumerO INT NOT NULL,
    ID_Curso INT NOT NULL,
    ID_Profesor INT NOT NULL,
    Horario VARCHAR(200) NOT NULL,
    ID_Periodo INT NOT NULL,
    CONSTRAINT FK_Grupo_Curso FOREIGN KEY (ID_Curso) REFERENCES Curso(ID_Curso),
    CONSTRAINT FK_Grupo_Profesor FOREIGN KEY (ID_Profesor) REFERENCES Profesor(ID_Profesor),
    CONSTRAINT FK_Grupo_Periodo FOREIGN KEY (ID_Periodo) REFERENCES Periodo(ID_Periodo)
);

/* ===========================
   INSERTS: Oferta Académica (Ejemplos)
   =========================== */

-- ===========================
-- INSTITUCIONES
-- ===========================
INSERT INTO Institucion (Nombre) VALUES
('Colegio Universitario de Cartago'),
('Instituto Tecnológico de Costa Rica'),
('Universidad de Costa Rica'),
('Universidad Nacional'),
('Universidad Estatal a Distancia'),
('Universidad Técnica Nacional'),
('Universidad Latina'),
('Universidad Fidélitas'),
('Universidad Hispanoamericana'),
('Universidad Veritas');

-- ===========================
-- PROFESORES
-- ===========================
INSERT INTO Profesor (Identificacion, Tipo_Identificacion, Email, Nombre, Fecha_Nacimiento) VALUES
('123456789', 'Cédula', 'prof.juancastro@cuc.ac.cr', 'Juan Castro', '1980-04-10'),
('987654321', 'Cédula', 'prof.marialopez@cuc.ac.cr', 'María López', '1985-07-22'),
('112233445', 'Cédula', 'prof.carlosgomez@cuc.ac.cr', 'Carlos Gómez', '1978-03-15'),
('223344556', 'Cédula', 'prof.anamorales@cuc.ac.cr', 'Ana Morales', '1982-09-30'),
('334455667', 'Cédula', 'prof.luisrodriguez@cuc.ac.cr', 'Luis Rodríguez', '1979-12-05'),
('445566778', 'Cédula', 'prof.laurajimenez@cuc.ac.cr', 'Laura Jiménez', '1988-11-18'),
('556677889', 'Cédula', 'prof.pedrovargas@cuc.ac.cr', 'Pedro Vargas', '1983-08-27'),
('667788990', 'Cédula', 'prof.sofiaramirez@cuc.ac.cr', 'Sofía Ramírez', '1990-01-14'),
('778899001', 'Cédula', 'prof.danielcastro@cuc.ac.cr', 'Daniel Castro', '1986-02-03'),
('889900112', 'Cédula', 'prof.karlamendez@cuc.ac.cr', 'Karla Méndez', '1991-05-09');

-- ===========================
-- CARRERAS
-- ===========================
INSERT INTO Carrera (Nombre, ID_Institucion, ID_Profesor_Director) VALUES
('Ingeniería en Software', 1, 1),
('Ingeniería en Computadores', 1, 2),
('Administración de Empresas', 2, 3),
('Educación Matemática', 3, 4),
('Ingeniería Industrial', 4, 5),
('Contaduría Pública', 5, 6),
('Ingeniería Electrónica', 2, 7),
('Diseño Publicitario', 9, 8),
('Ciencias de la Educación', 3, 9),
('Arquitectura de Software', 1, 10);

-- ===========================
-- CURSOS
-- ===========================
INSERT INTO Curso (Nombre, Nivel, ID_Carrera) VALUES
('Programación I', 1, 1),
('Matemática General', 1, 1),
('Bases de Datos I', 2, 1),
('Estructuras de Datos', 3, 1),
('Administración I', 1, 3),
('Didáctica General', 2, 4),
('Contabilidad Financiera', 1, 6),
('Diseño Digital', 1, 8),
('Física I', 2, 7),
('Arquitectura de Software', 4, 10);

-- ===========================
-- PERIODOS
-- ===========================
INSERT INTO Periodo (Anno, Numero_, Fecha_Inicio, Fecha_Fin) VALUES
(2024, 1, '2024-01-15', '2024-06-15'),
(2024, 2, '2024-07-01', '2024-12-01'),
(2025, 1, '2025-01-13', '2025-06-13'),
(2025, 2, '2025-07-07', '2025-12-07'),
(2026, 1, '2026-01-12', '2026-06-12'),
(2026, 2, '2026-07-06', '2026-12-06'),
(2027, 1, '2027-01-11', '2027-06-11'),
(2027, 2, '2027-07-05', '2027-12-05'),
(2028, 1, '2028-01-10', '2028-06-10'),
(2028, 2, '2028-07-03', '2028-12-03');

-- ===========================
-- GRUPOS
-- ===========================
INSERT INTO Grupo (Numero, ID_Curso, ID_Profesor, Horario, ID_Periodo) VALUES
(1, 1, 1, 'Lunes y Miércoles 8:00–10:00', 3),
(2, 1, 2, 'Martes y Jueves 10:00–12:00', 3),
(1, 2, 3, 'Martes y Jueves 8:00–10:00', 3),
(1, 3, 4, 'Viernes 13:00–17:00', 3),
(1, 4, 5, 'Lunes y Miércoles 10:00–12:00', 3),
(1, 5, 6, 'Martes y Jueves 13:00–15:00', 3),
(1, 6, 7, 'Miércoles 8:00–12:00', 3),
(1, 7, 8, 'Viernes 8:00–12:00', 3),
(1, 8, 9, 'Martes y Jueves 10:00–12:00', 3),
(1, 10, 10, 'Lunes y Miércoles 13:00–15:00', 3);
