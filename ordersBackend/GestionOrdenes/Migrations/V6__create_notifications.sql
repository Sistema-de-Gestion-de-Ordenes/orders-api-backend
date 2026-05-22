-- ============================================================
-- V6__create_notifications.sql
-- In-app notifications sent to system users (admins and drivers)
-- whenever a relevant order event occurs.
-- Push delivery via FCM is handled at the service layer;
-- this table persists the notification history.
-- ============================================================

USE order_management;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS notifications;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE notifications (
    id         INT          NOT NULL AUTO_INCREMENT,
    user_id    INT          NOT NULL,
    order_id   INT          NULL                   COMMENT 'Related order, if applicable',
    title      VARCHAR(200) NOT NULL,
    message    TEXT         NOT NULL,
    is_read    TINYINT(1)   NOT NULL DEFAULT 0     COMMENT '0 = unread, 1 = read',
    created_at DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    -- Indexes for common filter queries (unread count, user inbox)
    KEY idx_notifications_user_id (user_id),
    KEY idx_notifications_is_read (is_read),

    CONSTRAINT fk_notifications_user
        FOREIGN KEY (user_id)
        REFERENCES users (id)
        ON DELETE CASCADE,

    CONSTRAINT fk_notifications_order
        FOREIGN KEY (order_id)
        REFERENCES orders (id)
        ON DELETE SET NULL
)
ENGINE  = InnoDB
CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'In-app notification history for system users';
