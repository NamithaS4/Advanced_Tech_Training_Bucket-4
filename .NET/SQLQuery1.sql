USE CollegeDB

GO

SELECT * FROM Student

CREATE OR ALTER PROCEDURE GetStudentInfo
AS
BEGIN
	DECLARE @TotalStudents INT;
	SELECT @TotalStudents = COUNT(*) FROM Student;
	PRINT 'Total Number of Students: ' + CAST(@TotalStudents AS VARCHAR(100));
	SELECT * FROM Student
END;

EXEC GetStudentInfo

CREATE PROCEDURE GetStudentById
	@StudentId INT
AS
BEGIN
	DECLARE @StudentName VARCHAR(50);

	SELECT @StudentName = Name
	FROM Student
	WHERE StudentId = @StudentId;

	PRINT @StudentName;
END;

EXEC GetStudentById @StudentId = 3