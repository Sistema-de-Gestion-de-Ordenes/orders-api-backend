-- Database schema: order_management
-- Compatible with MySQL 8.0+

CREATE DATABASE IF NOT EXISTS order_management CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE order_management;

-- System administrator accounts
CREATE TABLE users (
    id            INT          NOT NULL AUTO_INCREMENT,
    name          VARCHAR(150) NOT NULL,
    email         VARCHAR(200) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_users_email (email)
);

-- End clients who place deliveries
CREATE TABLE clientes (
    id            INT          NOT NULL AUTO_INCREMENT,
    nombre        VARCHAR(150) NOT NULL,
    correo        VARCHAR(200) NOT NULL,
    telefono      VARCHAR(20)  NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    foto_url      VARCHAR(500) NULL,
    fcm_token     VARCHAR(500) NULL,
    created_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_clientes_correo (correo)
);

-- Delivery drivers
CREATE TABLE repartidores (
    id         INT          NOT NULL AUTO_INCREMENT,
    nombre     VARCHAR(150) NOT NULL,
    vehiculo   VARCHAR(100) NOT NULL,
    placas     VARCHAR(20)  NOT NULL,
    telefono   VARCHAR(20)  NOT NULL,
    foto_url   VARCHAR(500) NULL,
    verificado TINYINT(1)   NOT NULL DEFAULT 0,
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_repartidores_placas (placas)
);

-- Deliveries: core entity linking clients and drivers
-- Valid estado transitions: pendiente → en_camino → entregada | cancelada
CREATE TABLE entregas (
    id            INT          NOT NULL AUTO_INCREMENT,
    cliente_id    INT          NOT NULL,
    repartidor_id INT          NOT NULL,
    origen        VARCHAR(300) NOT NULL,
    destino       VARCHAR(300) NOT NULL,
    estado        ENUM('pendiente','en_camino','entregada','cancelada') NOT NULL DEFAULT 'pendiente',
    created_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    CONSTRAINT fk_entregas_cliente    FOREIGN KEY (cliente_id)    REFERENCES clientes (id),
    CONSTRAINT fk_entregas_repartidor FOREIGN KEY (repartidor_id) REFERENCES repartidores (id)
);

-- Push notification history for clients
CREATE TABLE notifications (
    id         INT          NOT NULL AUTO_INCREMENT,
    cliente_id INT          NOT NULL,
    entrega_id INT          NULL,
    titulo     VARCHAR(200) NOT NULL,
    mensaje    TEXT         NOT NULL,
    is_read    TINYINT(1)   NOT NULL DEFAULT 0,
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    CONSTRAINT fk_notifications_cliente FOREIGN KEY (cliente_id) REFERENCES clientes (id) ON DELETE CASCADE,
    CONSTRAINT fk_notifications_entrega FOREIGN KEY (entrega_id) REFERENCES entregas (id) ON DELETE SET NULL
);

-- Initial admin user (replace hash with a real BCrypt hash before use)
INSERT INTO users (name, email, password_hash)
VALUES ('Administrator', 'admin@ordermanagement.com', '$2a$11$replace_with_real_bcrypt_hash');
