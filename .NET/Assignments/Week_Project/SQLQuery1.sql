CREATE DATABASE CollegeDB;

USE CollegeDB;

-- COURSE TABLE
CREATE TABLE Course (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode NVARCHAR(20) NOT NULL UNIQUE,
    CourseName NVARCHAR(100) NOT NULL,
    Department NVARCHAR(50),
    Semester INT
);

-- STUDENT TABLE
CREATE TABLE Student (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    RollNumber NVARCHAR(20) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(15),
    Address NVARCHAR(200),
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    CourseId INT,
    FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
);


SELECT * FROM Users

SELECT * FROM Course

SELECT * FROM Student

INSERT INTO Course (CourseCode, CourseName, Department, Semester)
VALUES
('CSE201', 'Web Technologies', 'CSE', 5),
('CSE202', 'Artificial Intelligence', 'CSE', 6),
('CSE203', 'Cloud Computing', 'CSE', 7),
('ECE301', 'Microprocessors', 'ECE', 5),
('ME401', 'Machine Design', 'Mechanical', 7),
('CIV501', 'Structural Engineering', 'Civil', 8),
('EEE101', 'Power Systems', 'EEE', 4),
('IT301', 'Cyber Security', 'IT', 6);

INSERT INTO Student (RollNumber, Name, Email, Phone, Address, DateOfBirth, Gender, CourseId)
VALUES
('CSE010', 'Riya Fernandes', 'riya.fernandes@example.com', '9876123450', 'Derebail, Mangalore', '2003-01-12', 'Female', 1),
('CSE011', 'Aditya Rao', 'aditya.rao@example.com', '9890023345', 'Kadri, Mangalore', '2002-10-05', 'Male', 2),
('CSE012', 'Harini Shetty', 'harini.shetty@example.com', '9812345678', 'Surathkal, Mangalore', '2003-04-20', 'Female', 3),
('ECE013', 'Manoj Bhat', 'manoj.bhat@example.com', '9823412345', 'Padil, Mangalore', '2002-07-16', 'Male', 4),
('ME014', 'Sneha Dsouza', 'sneha.dsouza@example.com', '9845032190', 'Kulshekar, Mangalore', '2003-02-22', 'Female', 5),
('CIV015', 'Naveen Kumar', 'naveen.kumar@example.com', '9831122334', 'Bajpe, Mangalore', '2002-11-09', 'Male', 6),
('EEE016', 'Lavanya Jain', 'lavanya.jain@example.com', '9877896543', 'Puttur, DK', '2003-06-28', 'Female', 7),
('IT017', 'Rakesh Naik', 'rakesh.naik@example.com', '9865321470', 'Bantwal, DK', '2002-09-03', 'Male', 8),
('CSE018', 'Priya Shenoy', 'priya.shenoy@example.com', '9845071234', 'Bejai, Mangalore', '2003-12-25', 'Female', 1),
('CSE019', 'Karthik Nayak', 'karthik.nayak@example.com', '9856043217', 'Thokkottu, Mangalore', '2003-03-09', 'Male', 3);

INSERT INTO Users (Username, Password, Role)
VALUES 
('admin', 'admin123', 'Admin'),
('faculty', 'faculty123', 'Faculty');



CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);


