-- ============================================================
-- V5__create_deliveries.sql
-- Records proof-of-delivery for a completed order.
-- One delivery record per order (enforced by UNIQUE on order_id).
--
-- This record is created when the driver uploads an evidence
-- image; the service layer simultaneously transitions the
-- linked order to status = 'delivered'.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS deliveries;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE deliveries (
    id           INT          NOT NULL AUTO_INCREMENT,
    order_id     INT          NOT NULL               COMMENT 'The order being confirmed as delivered',
    driver_id    INT          NOT NULL               COMMENT 'Driver who completed the delivery',
    image_url    VARCHAR(500) NOT NULL               COMMENT 'Cloudinary URL of the evidence photo',
    notes        TEXT         NULL                   COMMENT 'Optional driver notes about the delivery',
    delivered_at DATETIME     NOT NULL               COMMENT 'Moment the driver marked the delivery as done',
    created_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_deliveries_order_id (order_id),

    -- Index to look up all deliveries made by a driver
    KEY idx_deliveries_driver_id (driver_id),

    CONSTRAINT fk_deliveries_order
        FOREIGN KEY (order_id)
        REFERENCES orders (id)
        ON DELETE CASCADE,

    CONSTRAINT fk_deliveries_driver
        FOREIGN KEY (driver_id)
        REFERENCES drivers (id)
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Proof-of-delivery records attached to completed orders';
