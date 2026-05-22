-- ============================================================
-- V1__create_users.sql
-- Stores system accounts (admins and drivers).
-- Customers are managed in the `customers` table and do NOT
-- require a login account.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS users;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE users (
    id            INT          NOT NULL AUTO_INCREMENT,
    name          VARCHAR(150) NOT NULL,
    email         VARCHAR(200) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    fcm_token     VARCHAR(500) NULL     COMMENT 'Firebase Cloud Messaging token for push notifications',
    role          ENUM(
                      'admin',
                      'driver'
                  )            NOT NULL,
    created_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                        ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_users_email (email)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'System user accounts (admins and delivery drivers)';
