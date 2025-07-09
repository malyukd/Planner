DROP DATABASE Planner

CREATE DATABASE Planner
GO
USE Planner

CREATE TABLE Groups(
    group_id VARCHAR(10) PRIMARY KEY,

	CONSTRAINT CK_Groups_GroupId CHECK(group_id!='')
)

CREATE TABLE Administrators(
	 admin_id INT IDENTITY PRIMARY KEY,
	 [name] VARCHAR(100) NOT NULL,
	 email VARCHAR(100) NOT NULL,
	 pass VARCHAR(50) NOT NULL,

	 CONSTRAINT UQ_Administrators_Email UNIQUE(email),
	 CONSTRAINT CK_Administrators_Email CHECK([email]!=''),
	 CONSTRAINT CK_Administrators_Name CHECK([name]!=''),
	 CONSTRAINT CK_Administrators_Pass CHECK([pass]!='')
)

CREATE TABLE Students(
     student_id INT IDENTITY PRIMARY KEY,
	 [name] VARCHAR(100) NOT NULL,
	 email VARCHAR(100) NOT NULL,
	 pass VARCHAR(50) NOT NULL,
	 group_id VARCHAR(10) NOT NULL,

	 CONSTRAINT FK_Students_GroupId FOREIGN KEY (group_id) REFERENCES Groups(group_id),
	 CONSTRAINT UQ_Students_Email UNIQUE(email),
	 CONSTRAINT CK_Students_Email CHECK([email]!=''),
	 CONSTRAINT CK_Students_Name CHECK([name]!=''),
	 CONSTRAINT CK_Students_Pass CHECK([pass]!='')
)

CREATE TABLE Teachers(
     teacher_id INT IDENTITY PRIMARY KEY,
     [name] VARCHAR(100) NOT NULL,
	 email VARCHAR(100) NOT NULL,
	 pass VARCHAR(50) NOT NULL,

	 CONSTRAINT UQ_Teachers_Email UNIQUE(email),
	 CONSTRAINT CK_Teachers_Email CHECK([email]!=''),
	 CONSTRAINT CK_Teachers_Name CHECK([name]!=''),
	 CONSTRAINT CK_Teachers_Pass CHECK([pass]!='')
)

CREATE TABLE Subjects( 
     subject_id INT IDENTITY PRIMARY KEY,
     title VARCHAR(50) NOT NULL,
	 duration INT NOT NULL,

	 CONSTRAINT UQ_Subjects_Title UNIQUE(title),
	 CONSTRAINT CK_Subjects_Title CHECK(title!=''),
	 CONSTRAINT CK_Subjects_Duration CHECK(duration>0)
)

CREATE TABLE TeacherSubject(
     teacher_id INT NOT NULL,
	 subject_id INT NOT NULL,
	 PRIMARY KEY (teacher_id, subject_id),

	 CONSTRAINT FK_TeacherSubject_TeacherId FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id),
	 CONSTRAINT FK_TeacherSubject_SubjectId FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
)

CREATE TABLE TeacherGroup(
     teacher_id INT NOT NULL,
	 group_id  VARCHAR(10) NOT NULL,
	 PRIMARY KEY (teacher_id, group_id),

	 CONSTRAINT FK_TeacherGroup_TeacherId FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id),
	 CONSTRAINT FK_TeacherGroup_GroupId FOREIGN KEY (group_id) REFERENCES Groups(group_id)
)

CREATE TABLE Notes( 
     note_id INT IDENTITY PRIMARY KEY,
     title VARCHAR(50) NOT NULL,
	 [text] VARCHAR(1000) NOT NULL,
	 [date] DATE NOT NULL,
	 student_id INT NOT NULL,
	 subject_id INT,


	 CONSTRAINT CK_Notes_Title CHECK(title!=''),
	 CONSTRAINT FK_Notes_StudentId FOREIGN KEY (student_id) REFERENCES Students(student_id),
	 CONSTRAINT FK_Notes_SubjectId FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
)

CREATE TABLE Assignments( 
     assignment_id INT IDENTITY PRIMARY KEY,
     title VARCHAR(50) NOT NULL,
	 [description] VARCHAR(300),
	 [date] DATE NOT NULL,
	 due_date DATE NOT NULL,
	 teacher_id INT NOT NULL,
	 subject_id INT NOT NULL,
	 group_id VARCHAR(10) NOT NULL,


	 CONSTRAINT CK_Assignments_Title CHECK(title!=''),
	 CONSTRAINT CK_Assignments_Date CHECK(due_date>=[date]),
	 CONSTRAINT FK_Assignments_TeacherId FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id),
	 CONSTRAINT FK_Assignments_SubjectId FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id),
	 CONSTRAINT FK_Assignments_GroupId FOREIGN KEY (group_id) REFERENCES Groups(group_id)
)

CREATE TABLE AssignmentStatus( 
     status_id INT IDENTITY PRIMARY KEY,
     [name] VARCHAR(20) NOT NULL,

	 CONSTRAINT UQ_AssignmentStatus_Name UNIQUE([name]),
	 CONSTRAINT CK_AssignmentStatus_Name CHECK([name]!='')
)

CREATE TABLE StudentAssignment( 
     student_id INT NOT NULL,
     assignment_id INT NOT NULL,
	 status_id INT NOT NULL,
	 PRIMARY KEY (student_id, assignment_id),

	 CONSTRAINT FK_StudentAssignment_StudentId FOREIGN KEY (student_id) REFERENCES Students(student_id),
	 CONSTRAINT FK_StudentAssignment_AssignmentId FOREIGN KEY (assignment_id) REFERENCES Assignments(assignment_id),
	 CONSTRAINT FK_StudentAssignment_StatusId FOREIGN KEY (status_id) REFERENCES AssignmentStatus(status_id)
)

CREATE TABLE ExamTypes( 
     [type_id] INT IDENTITY PRIMARY KEY,
     [name] VARCHAR(20) NOT NULL,

	 CONSTRAINT UQ_ExamTypes_Name UNIQUE([name]),
	 CONSTRAINT CK_ExamTypes_Name CHECK([name]!='')
)

CREATE TABLE Buildings( 
     building_id INT IDENTITY PRIMARY KEY,
     [address] VARCHAR(50) NOT NULL,

	 CONSTRAINT UQ_Buildings_Address UNIQUE([address]),
	 CONSTRAINT CK_Buildings_Address CHECK([address]!='')
)

CREATE TABLE Exams( 
     exam_id INT IDENTITY PRIMARY KEY,
	 subject_id INT NOT NULL,
     title VARCHAR(50) NOT NULL,
	 [date] DATE NOT NULL,
	 [time] TIME NOT NULL,
	 [type_id] INT NOT NULL,
	 teacher_id INT NOT NULL,
	 building_id INT NOT NULL,
	 group_id VARCHAR(10) NOT NULL,

	 CONSTRAINT FK_Exams_SubjectId FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id),
	 CONSTRAINT FK_Exams_TypeId FOREIGN KEY ([type_id]) REFERENCES ExamTypes([type_id]),
	 CONSTRAINT FK_Exams_TeacherId FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id),
	 CONSTRAINT FK_Exams_BuildingId FOREIGN KEY (building_id) REFERENCES Buildings(building_id),
	 CONSTRAINT FK_Exams_GroupId FOREIGN KEY (group_id) REFERENCES Groups(group_id)
)