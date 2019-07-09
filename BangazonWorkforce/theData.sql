--Drop Database
DROP DATABASE BangazonAPI;

-- Create Database
USE MASTER
GO

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases
    WHERE [name] = N'BangazonAPI'
)
CREATE DATABASE BangazonAPI
GO

USE BangazonAPI
GO


CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL
);

CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);


CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price MONEY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber INTEGER NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

-- Insert actual Data into the newly created database
-- NOTE: this is onlye for the tables that we are working on for this sprint
INSERT INTO Department ([Name], Budget) VALUES ('Customer Experience', 450000);
INSERT INTO Department ([Name], Budget) VALUES ('IT', 890000);
INSERT INTO Department ([Name], Budget) VALUES ('Human Resources', 500000);
INSERT INTO Department ([Name], Budget) VALUES ('Marketing', 550000);
INSERT INTO Department ([Name], Budget) VALUES ('Analytics', 880000);

INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (1/23/2015, 9/23/2018, 'Inspiron 1500', 'Dell');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (2/15/2017, null,'Aspire E 15', 'Acer');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (10/5/2016, null, 'Chromebook C434', 'Asus');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (12/24/2013, 4/21/2019, 'Yogo C930', 'Lenovo');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (7/2/2012, 5/08/2019, 'Area-51m', 'Alienware');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (6/12/2016, null, 'XPS 30', 'Dell');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (9/1/2015, null, 'ThinkPad X1', 'Lenovo');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (11/04/2018, null, 'MacBook Pro 13in', 'Apple');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES (3/11/2017, null, 'MateBook X', 'Huawei');

INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Andy', 'Collins', 1, 1);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Jisie', 'Davis', 2, 1);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Steve', 'Brownlee', 3, 1);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Leah', 'Hoefling', 2, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Kristen', 'Norris', 1, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Madi', 'Peper', 3, 0);

INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES('Diversity & Inclusion', 4/25/2019, 6/25/2019, 150);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES('Customer Complaint', 12/02/2019, 1/6/2020, 200);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES('Safety in the Workplace', 8/02/2019, 9/09/2019, 100);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES('GitHub Challenges', 7/02/2019, 7/06/2019, 100);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES('Harrassment', 2/20/2019, 3/01/2019, 250);

INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (1,2, 12/24/2018, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (2,3, 9/01/2017, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (3,6, 8/12/2017, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (4,7, 1/20/2017, 10/02/2019);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (5,8, 12/10/2018, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (6,9, 3/15/2018, null);

INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2,2);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1,3);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (6,1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5,1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (3,5);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (4,4);
