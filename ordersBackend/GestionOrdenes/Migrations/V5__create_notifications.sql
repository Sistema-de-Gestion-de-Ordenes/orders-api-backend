-- ============================================================
-- V5__create_notifications.sql
-- Push notification history sent to customers when a delivery
-- changes status.
-- Actual push delivery via FCM is handled at the service layer;
-- this table persists notification history.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS notifications;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE notifications (
    id           INT          NOT NULL AUTO_INCREMENT,
    customer_id  INT          NOT NULL,
    delivery_id  INT          NULL     COMMENT 'Related delivery, if applicable',
    title        VARCHAR(200) NOT NULL,
    message      TEXT         NOT NULL,
    is_read      TINYINT(1)   NOT NULL DEFAULT 0  COMMENT '0 = unread, 1 = read',
    created_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    KEY idx_notifications_customer_id (customer_id),
    KEY idx_notifications_is_read     (is_read),

    CONSTRAINT fk_notifications_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers (id)
        ON DELETE CASCADE,

    CONSTRAINT fk_notifications_delivery
        FOREIGN KEY (delivery_id)
        REFERENCES deliveries (id)
        ON DELETE SET NULL
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Push notification history for customers';
