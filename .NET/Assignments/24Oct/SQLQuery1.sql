CREATE DATABASE ProductInventoryAPI

USE ProductInventoryAPI

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(150) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId)
);


Scaffold-DbContext "Data Source=LAPTOP-VPPT2LSP;Initial Catalog=ProductInventoryAPI;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models