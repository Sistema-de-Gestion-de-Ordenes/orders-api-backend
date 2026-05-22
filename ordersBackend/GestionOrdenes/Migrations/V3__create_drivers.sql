-- ============================================================
-- V3__create_drivers.sql
-- Stores delivery drivers available to fulfill orders.
-- Business rules (enforced at the service layer):
--   - `available` is set to FALSE when an order is assigned.
--   - A driver cannot be deleted while they have active orders.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS drivers;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE drivers (
    id         INT          NOT NULL AUTO_INCREMENT,
    name       VARCHAR(150) NOT NULL,
    phone      VARCHAR(20)  NOT NULL DEFAULT '',
    available  TINYINT(1)   NOT NULL DEFAULT 1  COMMENT '1 = available to accept new orders',
    active     TINYINT(1)   NOT NULL DEFAULT 1  COMMENT '1 = active, 0 = soft-deleted',
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                     ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    KEY idx_drivers_available (available),
    KEY idx_drivers_active    (active)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Delivery drivers who fulfill orders';
