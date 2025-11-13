CREATE DATABASE Blazor_tutorial

USE Blazor_tutorial


CREATE TABLE student(
ID INT PRIMARY KEY IDENTITY(1,1),
Name VARCHAR(30),
AGE INT,
Birthday DATETIME
);

INSERT INTO student VALUES('Nami', 20, '2003-12-21');

SELECT * FROM student


