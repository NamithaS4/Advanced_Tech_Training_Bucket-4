CREATE DATABASE EmployeeDB

USE EmployeeDB

CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Department NVARCHAR(50) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL,
    Email NVARCHAR(100) NULL UNIQUE
);


CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NULL
);

INSERT INTO Users (UserName, Password, Role)
VALUES ('admin', 'adminpass', 'Admin');

select * from Users