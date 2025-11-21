-- Migration script to add AmountPaid column to Bill table
-- Run this script in your SQL Server database

ALTER TABLE [Bill]
ADD [AmountPaid] DECIMAL(18,2) NOT NULL DEFAULT 0;

