-- ============================================================
-- V3__create_drivers.sql
-- Delivery drivers who fulfill deliveries.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS drivers;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE drivers (
    id         INT          NOT NULL AUTO_INCREMENT,
    name       VARCHAR(150) NOT NULL,
    vehicle    VARCHAR(100) NOT NULL,
    license_plate VARCHAR(20)  NOT NULL,
    phone      VARCHAR(20)  NOT NULL,
    photo_url  VARCHAR(500) NULL,
    is_verified TINYINT(1)  NOT NULL DEFAULT 0  COMMENT '0 = not verified, 1 = verified',
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                     ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_drivers_license_plate (license_plate)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Delivery drivers who fulfill deliveries';
