-- Base de datos HOTEL (PostgreSQL)
-- Crear la BD:  CREATE DATABASE "HOTEL";   y conectarse a ella antes de ejecutar el resto.

DROP TABLE IF EXISTS reserva;
DROP TABLE IF EXISTS habitacion;

CREATE TABLE habitacion (
    numero_habitacion INT PRIMARY KEY,
    tipo              VARCHAR(50)   NOT NULL,
    precio_noche      NUMERIC(10,2) NOT NULL CHECK (precio_noche > 0),
    estado            VARCHAR(20)   NOT NULL DEFAULT 'DISPONIBLE'
                      CHECK (estado IN ('DISPONIBLE', 'OCUPADA'))
);

CREATE TABLE reserva (
    id_reserva        SERIAL PRIMARY KEY,
    nombre_cliente    VARCHAR(150)  NOT NULL,
    numero_habitacion INT           NOT NULL REFERENCES habitacion(numero_habitacion),
    fecha_entrada     DATE          NOT NULL,
    fecha_salida      DATE          NOT NULL,
    cantidad_personas INT           NOT NULL CHECK (cantidad_personas > 0),
    monto_total       NUMERIC(12,2) NOT NULL,
    estado            VARCHAR(20)   NOT NULL DEFAULT 'ACTIVA',
    fecha_registro    TIMESTAMP     NOT NULL DEFAULT NOW(),
    usuario           VARCHAR(100)  NOT NULL,
    CHECK (fecha_salida > fecha_entrada)
);

CREATE INDEX idx_reserva_habitacion ON reserva(numero_habitacion);

-- Datos semilla
INSERT INTO habitacion (numero_habitacion, tipo, precio_noche, estado) VALUES
    (101, 'SENCILLA', 250.00, 'DISPONIBLE'),
    (102, 'SENCILLA', 250.00, 'DISPONIBLE'),
    (103, 'DOBLE',    400.00, 'DISPONIBLE'),
    (201, 'DOBLE',    420.00, 'DISPONIBLE'),
    (202, 'SUITE',    750.00, 'DISPONIBLE'),
    (301, 'SUITE',    950.00, 'DISPONIBLE');
