-- ============================================================
--  Sistema de Órdenes - Script de PostgreSQL
--  Base de datos: SISTEMA_ORDENES
-- ============================================================

-- 1. Crear la base de datos (ejecutar conectado a la BD "postgres").
--    Si ya existe, omitir esta línea.
-- CREATE DATABASE "SISTEMA_ORDENES";

-- 2. Conectarse a SISTEMA_ORDENES y crear la tabla.
--    (en psql:  \c SISTEMA_ORDENES )

CREATE TABLE IF NOT EXISTS orden
(
    id_orden          SERIAL          PRIMARY KEY,          -- identificador generado en PostgreSQL
    numero_orden      VARCHAR(50)     NOT NULL,             -- número de orden (código de negocio)
    nombre_cliente    VARCHAR(150)    NOT NULL,             -- nombre del cliente
    direccion_entrega VARCHAR(250)    NOT NULL,             -- dirección de entrega
    monto             NUMERIC(12, 2)  NOT NULL DEFAULT 0,   -- monto
    estado            VARCHAR(30)     NOT NULL DEFAULT 'REGISTRADA',  -- estado
    fecha_registro    TIMESTAMP       NOT NULL DEFAULT NOW(),         -- fecha de registro
    usuario_operacion VARCHAR(100)    NOT NULL              -- usuario que realizó la operación
);

-- Índice opcional para búsquedas por número de orden.
CREATE INDEX IF NOT EXISTS ix_orden_numero_orden ON orden (numero_orden);

-- 3. Datos de ejemplo (opcional).
INSERT INTO orden (numero_orden, nombre_cliente, direccion_entrega, monto, estado, usuario_operacion)
VALUES
    ('ORD-0001', 'Juan Pérez',   '5a Avenida 10-20, Zona 1, Guatemala', 250.00, 'REGISTRADA', 'admin'),
    ('ORD-0002', 'María López',   'Calle Real 4-15, Antigua Guatemala',  980.50, 'EN_PROCESO', 'admin');

-- 4. Consultas útiles.
-- SELECT * FROM orden ORDER BY id_orden;
-- DELETE FROM orden WHERE id_orden = 1;
