/* ===========================
   DB: Acad�mico
   =========================== */

IF DB_ID('PV_NA_Academico') IS NULL
    CREATE DATABASE PV_NA_Academico;
GO
USE PV_NA_Academico;
GO

CREATE TABLE HistorialAcademico (
    TipoIdentificacion VARCHAR(20) NOT NULL,
    Identificacion VARCHAR(20) NOT NULL,
    CodigoCurso VARCHAR(20) NOT NULL,
    NombreCurso VARCHAR(120) NOT NULL,
    Promedio DECIMAL(5,2) NOT NULL CHECK (Promedio BETWEEN 0 AND 100),
    Periodo VARCHAR(20) NOT NULL,
    PRIMARY KEY (TipoIdentificacion, Identificacion, CodigoCurso, Periodo)
);

CREATE INDEX IX_Historial_Estudiante
    ON HistorialAcademico (TipoIdentificacion, Identificacion);

CREATE INDEX IX_Historial_Periodo
    ON HistorialAcademico (Periodo);

CREATE TABLE ListadoEstudiantesPeriodo (
    Periodo VARCHAR(20) NOT NULL,
    TipoIdentificacion VARCHAR(20) NOT NULL,
    Identificacion VARCHAR(20) NOT NULL,
    NombreCompleto VARCHAR(150) NOT NULL,
    Carrera VARCHAR(120) NOT NULL,
    Curso VARCHAR(120) NOT NULL,
    Grupo VARCHAR(20) NOT NULL,
    PRIMARY KEY (Periodo, TipoIdentificacion, Identificacion, Curso, Grupo)
);

CREATE INDEX IX_Listado_Periodo
    ON ListadoEstudiantesPeriodo (Periodo);

    /* ===========================
   INSERTS: Acad�mico (Ejemplos)
   =========================== */

-- ===========================
-- HISTORIAL ACAD�MICO
-- ===========================
INSERT INTO HistorialAcademico (TipoIdentificacion, Identificacion, CodigoCurso, NombreCurso, Promedio, Periodo) VALUES
('C�dula', '123456789', 'PROG1', 'Programaci�n I', 85.50, '2024-1'),
('C�dula', '123456789', 'BD1', 'Bases de Datos I', 90.25, '2024-2'),
('C�dula', '987654321', 'MAT1', 'Matem�tica General', 78.00, '2024-1'),
('C�dula', '987654321', 'PROG1', 'Programaci�n I', 82.75, '2024-2'),
('Pasaporte', 'A112233', 'FIS1', 'F�sica I', 88.00, '2024-2'),
('C�dula', '223344556', 'ING1', 'Ingl�s T�cnico', 92.10, '2025-1'),
('C�dula', '334455667', 'PROG2', 'Programaci�n II', 80.00, '2025-1'),
('C�dula', '445566778', 'ETI1', '�tica Profesional', 95.00, '2025-1'),
('C�dula', '556677889', 'PROY1', 'Proyecto Integrador', 89.50, '2025-1'),
('C�dula', '667788990', 'RED1', 'Redes y Comunicaciones', 84.75, '2025-1');


-- ===========================
-- LISTADO DE ESTUDIANTES POR PERIODO
-- ===========================
INSERT INTO ListadoEstudiantesPeriodo (Periodo, TipoIdentificacion, Identificacion, NombreCompleto, Carrera, Curso, Grupo) VALUES
('2025-1', 'C�dula', '123456789', 'Juan P�rez', 'Ingenier�a en Software', 'Programaci�n II', '01'),
('2025-1', 'C�dula', '987654321', 'Mar�a L�pez', 'Ingenier�a en Software', 'Matem�tica II', '02'),
('2025-1', 'Pasaporte', 'A112233', 'Admin General', 'Administraci�n de Sistemas', 'Proyecto Integrador', '01'),
('2025-1', 'C�dula', '223344556', 'Carlos G�mez', 'Ingenier�a en Software', 'Ingl�s T�cnico', '01'),
('2025-1', 'C�dula', '334455667', 'Ana Morales', 'Ingenier�a en Software', 'Programaci�n II', '02'),
('2025-1', 'C�dula', '445566778', 'Luis Rodr�guez', 'Ingenier�a en Software', '�tica Profesional', '01'),
('2025-1', 'C�dula', '556677889', 'Laura Jim�nez', 'Ingenier�a en Software', 'Proyecto Integrador', '02'),
('2025-1', 'C�dula', '667788990', 'Pedro Vargas', 'Ingenier�a en Software', 'Redes y Comunicaciones', '01'),
('2025-1', 'C�dula', '778899001', 'Sof�a Ram�rez', 'Ingenier�a en Software', 'Bases de Datos II', '02'),
('2025-1', 'C�dula', '889900112', 'Daniel Castro', 'Ingenier�a en Software', 'Arquitectura de Software', '01');
