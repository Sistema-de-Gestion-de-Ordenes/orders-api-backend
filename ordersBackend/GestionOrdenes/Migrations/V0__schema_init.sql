-- ============================================================
-- V0__schema_init.sql
-- Creates the database if it does not already exist.
-- Run this file once before applying any other migration.
-- ============================================================

CREATE DATABASE IF NOT EXISTS order_management
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE order_management;
