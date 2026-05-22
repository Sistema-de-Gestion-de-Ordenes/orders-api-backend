-- ============================================================
-- V2__create_customers.sql
-- Stores end customers who place delivery orders.
-- A customer cannot be deleted while they have active orders
-- (enforced at the service layer).
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS customers;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE customers (
    id         INT          NOT NULL AUTO_INCREMENT,
    name       VARCHAR(150) NOT NULL,
    phone      VARCHAR(20)  NOT NULL DEFAULT '',
    address    VARCHAR(300) NOT NULL,
    active     TINYINT(1)   NOT NULL DEFAULT 1  COMMENT '1 = active, 0 = soft-deleted',
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                     ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    KEY idx_customers_active (active)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'End customers who place delivery orders';
