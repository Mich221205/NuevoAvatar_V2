/* ===========================
   DB: Académico
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
   INSERTS: Académico (Ejemplos)
   =========================== */

-- ===========================
-- HISTORIAL ACADÉMICO
-- ===========================
INSERT INTO HistorialAcademico (TipoIdentificacion, Identificacion, CodigoCurso, NombreCurso, Promedio, Periodo) VALUES
('Cédula', '123456789', 'PROG1', 'Programación I', 85.50, '2024-1'),
('Cédula', '123456789', 'BD1', 'Bases de Datos I', 90.25, '2024-2'),
('Cédula', '987654321', 'MAT1', 'Matemática General', 78.00, '2024-1'),
('Cédula', '987654321', 'PROG1', 'Programación I', 82.75, '2024-2'),
('Pasaporte', 'A112233', 'FIS1', 'Física I', 88.00, '2024-2'),
('Cédula', '223344556', 'ING1', 'Inglés Técnico', 92.10, '2025-1'),
('Cédula', '334455667', 'PROG2', 'Programación II', 80.00, '2025-1'),
('Cédula', '445566778', 'ETI1', 'Ética Profesional', 95.00, '2025-1'),
('Cédula', '556677889', 'PROY1', 'Proyecto Integrador', 89.50, '2025-1'),
('Cédula', '667788990', 'RED1', 'Redes y Comunicaciones', 84.75, '2025-1');


-- ===========================
-- LISTADO DE ESTUDIANTES POR PERIODO
-- ===========================
INSERT INTO ListadoEstudiantesPeriodo (Periodo, TipoIdentificacion, Identificacion, NombreCompleto, Carrera, Curso, Grupo) VALUES
('2025-1', 'Cédula', '123456789', 'Juan Pérez', 'Ingeniería en Software', 'Programación II', '01'),
('2025-1', 'Cédula', '987654321', 'María López', 'Ingeniería en Software', 'Matemática II', '02'),
('2025-1', 'Pasaporte', 'A112233', 'Admin General', 'Administración de Sistemas', 'Proyecto Integrador', '01'),
('2025-1', 'Cédula', '223344556', 'Carlos Gómez', 'Ingeniería en Software', 'Inglés Técnico', '01'),
('2025-1', 'Cédula', '334455667', 'Ana Morales', 'Ingeniería en Software', 'Programación II', '02'),
('2025-1', 'Cédula', '445566778', 'Luis Rodríguez', 'Ingeniería en Software', 'Ética Profesional', '01'),
('2025-1', 'Cédula', '556677889', 'Laura Jiménez', 'Ingeniería en Software', 'Proyecto Integrador', '02'),
('2025-1', 'Cédula', '667788990', 'Pedro Vargas', 'Ingeniería en Software', 'Redes y Comunicaciones', '01'),
('2025-1', 'Cédula', '778899001', 'Sofía Ramírez', 'Ingeniería en Software', 'Bases de Datos II', '02'),
('2025-1', 'Cédula', '889900112', 'Daniel Castro', 'Ingeniería en Software', 'Arquitectura de Software', '01');
