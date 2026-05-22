-- ============================================================
-- V4__create_orders.sql
-- Central table of the system. Tracks every delivery order
-- from creation to completion.
--
-- Valid status transitions (enforced at the service layer):
--   pending → in_progress → delivered
--                         → cancelled
--
-- Edit rules:
--   - Updates are blocked when status = 'delivered' or 'cancelled'.
--   - Deletes are only allowed when status = 'pending'.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS orders;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE orders (
    id               INT            NOT NULL AUTO_INCREMENT,
    order_number     VARCHAR(20)    NOT NULL               COMMENT 'Human-readable identifier, e.g. ORD-001',
    customer_id      INT            NOT NULL,
    driver_id        INT            NULL                   COMMENT 'Assigned after order is accepted',
    description      VARCHAR(500)   NOT NULL,
    total            DECIMAL(10, 2) NOT NULL,
    status           ENUM(
                         'pending',
                         'in_progress',
                         'delivered',
                         'cancelled'
                     )              NOT NULL DEFAULT 'pending',
    created_at       DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at       DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP
                                             ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_orders_order_number (order_number),

    -- Indexes for common filter queries
    KEY idx_orders_status      (status),
    KEY idx_orders_customer_id (customer_id),
    KEY idx_orders_driver_id   (driver_id),

    CONSTRAINT fk_orders_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers (id),

    CONSTRAINT fk_orders_driver
        FOREIGN KEY (driver_id)
        REFERENCES drivers (id)
        ON DELETE SET NULL
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Delivery orders placed by customers';
