-- Database schema: order_management
-- Compatible with MySQL 8.0+

CREATE DATABASE IF NOT EXISTS order_management CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE order_management;

CREATE TABLE users (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    name          VARCHAR(150) NOT NULL,
    email         VARCHAR(200) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role          ENUM('Admin','Customer','Driver') NOT NULL DEFAULT 'Customer',
    fcm_token     VARCHAR(500) NULL,
    created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE customers (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    user_id    INT NOT NULL,
    phone      VARCHAR(20) NOT NULL DEFAULT '',
    address    VARCHAR(300) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_customers_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE drivers (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    user_id    INT NOT NULL,
    phone      VARCHAR(20) NOT NULL DEFAULT '',
    vehicle    VARCHAR(100) NOT NULL,
    available  TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_drivers_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE orders (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    order_number     VARCHAR(20) NOT NULL UNIQUE,
    customer_id      INT NOT NULL,
    driver_id        INT NULL,
    description      VARCHAR(500) NOT NULL,
    delivery_address VARCHAR(300) NOT NULL,
    total            DECIMAL(10,2) NOT NULL,
    status           VARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at       DATETIME NULL,
    CONSTRAINT fk_orders_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
    CONSTRAINT fk_orders_driver   FOREIGN KEY (driver_id)   REFERENCES drivers(id) ON DELETE SET NULL
);

CREATE TABLE deliveries (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    order_id     INT NOT NULL UNIQUE,
    evidence_url VARCHAR(500) NOT NULL,
    notes        TEXT NULL,
    delivered_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_deliveries_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
);

CREATE TABLE notifications (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    user_id    INT NOT NULL,
    order_id   INT NULL,
    title      VARCHAR(200) NOT NULL,
    message    TEXT NOT NULL,
    is_read    TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_notifications_user  FOREIGN KEY (user_id)  REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_notifications_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE SET NULL
);

-- Initial admin user (password: Admin1234! — replace hash with a real BCrypt hash)
INSERT INTO users (name, email, password_hash, role)
VALUES ('Administrator', 'admin@ordermanagement.com',
        '$2a$11$replace_with_real_bcrypt_hash', 'Admin');
