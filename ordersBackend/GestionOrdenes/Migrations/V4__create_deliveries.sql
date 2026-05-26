-- ============================================================
-- V4__create_deliveries.sql
-- Core entity of the system. Tracks every delivery from
-- creation to completion.
--
-- Valid status transitions (enforced at the service layer):
--   pending -> on_the_way -> delivered
--                     -> canceled
--
-- Edit rules:
--   - origin, destination and driver_id are editable while
--     status = 'pending' or 'on_the_way'.
--   - Deletes are only allowed when status = 'pending'.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS deliveries;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE deliveries (
    id            INT          NOT NULL AUTO_INCREMENT,
    customer_id   INT          NOT NULL,
    driver_id     INT          NOT NULL,
    origin        VARCHAR(300) NOT NULL,
    destination   VARCHAR(300) NOT NULL,
    status        ENUM(
                      'pending',
                      'on_the_way',
                      'delivered',
                      'canceled'
                  )            NOT NULL DEFAULT 'pending',
    created_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                        ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    KEY idx_deliveries_status      (status),
    KEY idx_deliveries_customer_id (customer_id),
    KEY idx_deliveries_driver_id   (driver_id),

    CONSTRAINT fk_deliveries_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers (id),

    CONSTRAINT fk_deliveries_driver
        FOREIGN KEY (driver_id)
        REFERENCES drivers (id)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Delivery assignments linking customers and drivers';
