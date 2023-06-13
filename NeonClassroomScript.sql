CREATE DATABASE NeonClassroom;
USE NeonClassroom;

DROP TABLE IF EXISTS LoginHistory;
DROP TABLE IF EXISTS Enrollment;
DROP TABLE IF EXISTS Submission;
DROP TABLE IF EXISTS Assignment;
DROP TABLE IF EXISTS Comment;
DROP TABLE IF EXISTS Announcement;
DROP TABLE IF EXISTS Material;
DROP TABLE IF EXISTS AttachmentFiles;
DROP TABLE IF EXISTS ClassItems;
DROP TABLE IF EXISTS Class
DROP TABLE IF EXISTS Student;
DROP TABLE IF EXISTS Teacher;
DROP TABLE IF EXISTS UserInfo;
DROP TABLE IF EXISTS [User];

--DROP TABLE [User];

CREATE TABLE [User] (
  id INT IDENTITY(1,1) PRIMARY KEY,
  username VARCHAR(255) NOT NULL UNIQUE,
  [password] VARCHAR(255) NOT NULL,
  email VARCHAR(255) NOT NULL,
  role VARCHAR(255) CHECK (role IN ('Student', 'Teacher')),
  profilePic VARCHAR(255),
  tAndC BIT,
  isActive BIT
);

CREATE TABLE UserInfo (
  userId INT PRIMARY KEY,
  first_name VARCHAR(255),
  last_name VARCHAR(255),
  dob DATE,
  gender VARCHAR(255) CHECK (gender IN ('Male', 'Female', 'Other')),
  FOREIGN KEY (userId) REFERENCES [User](id)  ON UPDATE CASCADE ON DELETE CASCADE
);


CREATE TABLE Teacher (
  id INT IDENTITY(1,1) PRIMARY KEY,
  userId INT NOT NULL,
  FOREIGN KEY (userId) REFERENCES [User](id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Student (
  id INT IDENTITY(1,1) PRIMARY KEY,
  userId INT NOT NULL,
  FOREIGN KEY (userId) REFERENCES [User](id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Class (
  id INT IDENTITY(1,1) PRIMARY KEY,
  className VARCHAR(255),
  subject VARCHAR(255),
  code VARCHAR(255) NOT NULL UNIQUE,
  section VARCHAR(255),
  room VARCHAR(255),
  isActive BIT,
  teacherId INT NOT NULL,
  FOREIGN KEY (teacherId) REFERENCES Teacher(id) ON UPDATE CASCADE ON DELETE CASCADE
);


CREATE TABLE ClassItems (
  id INT IDENTITY(1,1) PRIMARY KEY,
  classId INT NOT NULL,
  type VARCHAR(255) CHECK (type IN ('Announcement', 'Assignment', 'Material', 'Submission')),
  publishTime DATETIME,
  FOREIGN KEY (classId) REFERENCES Class(id) ON DELETE CASCADE 
);

CREATE TABLE AttachmentFiles (
  id INT IDENTITY(1,1) PRIMARY KEY,
  fileName VARCHAR(255),
  path VARCHAR(255) NOT NULL,
  classItemId INT NOT NULL,
  FOREIGN KEY (classItemId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Material (
  id INT IDENTITY(1,1) PRIMARY KEY,
  classItemId INT NOT NULL,
  title VARCHAR(255),
  description VARCHAR(255),
  FOREIGN KEY (classItemId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Announcement (
  id INT IDENTITY(1,1) PRIMARY KEY,
  classItemId INT NOT NULL,
  announcementText TEXT,
  userId INT NOT NULL,
  FOREIGN KEY (classItemId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE,
  FOREIGN KEY (userId) REFERENCES [User](id)
);

CREATE TABLE Comment (
  userId INT,
  classItemId INT,
  time DATETIME,
  comments VARCHAR(255),
  PRIMARY KEY (userId, classItemId, time),
  FOREIGN KEY (userId) REFERENCES [User](id),
  FOREIGN KEY (classItemId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Assignment (
  id INT IDENTITY(1,1) PRIMARY KEY,
  classItemId INT NOT NULL,
  points INT,
  title VARCHAR(255),
  instructions VARCHAR(255),
  dueDateTime DATETIME,
  FOREIGN KEY (classItemId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Submission (
  id INT IDENTITY(1,1) PRIMARY KEY,
  classItemsId INT NOT NULL,
  submissionDateTime DATETIME,
  isTurnedIn BIT,
  assignmentId INT NOT NULL,
  obtainedMarks INT,
  studentId INT NOT NULL,
  FOREIGN KEY (classItemsId) REFERENCES ClassItems(id) ON UPDATE CASCADE ON DELETE CASCADE,
  FOREIGN KEY (studentId) REFERENCES Student(id)
);

CREATE TABLE Enrollment (
classId INT,
studentId INT,
dateTime DATETIME,
PRIMARY KEY (classId, studentId),
FOREIGN KEY (classId) REFERENCES Class(id),
FOREIGN KEY (studentId) REFERENCES Student(id) ON UPDATE CASCADE ON DELETE CASCADE
);


CREATE TABLE LoginHistory (
userId INT,
dateTime DATETIME,
FOREIGN KEY (userId) REFERENCES [User](id) ON UPDATE CASCADE ON DELETE CASCADE,
);

CREATE TABLE classLog (
actionName VARCHAR(255),
dateTime DATETIME,
);


-- Insert into User table
INSERT INTO [User] (username, [password], email, role, profilePic, tAndC, isActive) VALUES 
('teacher', 'teacher', 'teacher@example.com', 'Teacher', NULL, 1, 1),
('jack_doe', 'password123', 'jack@example.com', 'Student', NULL, 1, 1);

-- Insert into UserInfo table
INSERT INTO UserInfo (userId, first_name, last_name, dob, gender)
VALUES 
(1, 'John', 'Doe', '2023-03-12', 'Male'),
(2, 'Jack', 'Doe', '2023-03-12', 'Male');


-- Insert into Teacher table
INSERT INTO Teacher (userId)
VALUES (1);

-- Insert into Student table
INSERT INTO Student (userId)
VALUES (2);


--------------------------------------------------------------
--------------------------------------------------------------

CREATE FUNCTION CheckUserCredentials (
  @usernameOrEmail VARCHAR(255),
  @password VARCHAR(255)
)
RETURNS BIT
AS
BEGIN
  DECLARE @found BIT;

  -- Check if the username or email and password match a record in the User table
  SELECT @found = CASE WHEN EXISTS (
    SELECT 1 FROM [User]
    WHERE (username = @usernameOrEmail OR email = @usernameOrEmail) AND [password] = @password
  ) THEN 1 ELSE 0 END;

  RETURN @found;
END;


--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE SelectUserByCredentials
  @usernameOrEmail VARCHAR(255),
  @password VARCHAR(255)
AS
BEGIN
  SELECT *
  FROM [User]
  WHERE (username = @usernameOrEmail OR email = @usernameOrEmail) AND [password] = @password;
END;

--------------------------------------------------------------
--------------------------------------------------------------

 -- for user registration
CREATE PROCEDURE RegisterUser
  @username VARCHAR(255),
  @password VARCHAR(255),
  @email VARCHAR(255),
  @role VARCHAR(255),
  @tAndC BIT,
  @firstName VARCHAR(255),
  @lastName VARCHAR(255),
  @dob DATE,
  @gender VARCHAR(255)
AS
BEGIN
	INSERT INTO [User] (username, [password], email, role, profilePic, tAndC, isActive) 
		VALUES (@username, @password, @email, @role,NULL, @tAndC, 1);

	DECLARE @id INT = (SELECT id from [User] where username = @username);

	INSERT INTO UserInfo (userId, first_name, last_name, dob, gender) 
		VALUES (@id, @firstName, @lastName, @dob, @gender);

	IF @role = 'Student'
	BEGIN
		INSERT INTO Student(userId) VALUES (@id);
	END
	ELSE
	BEGIN
		INSERT INTO Teacher(userId) VALUES (@id);
	END
END;

--------------------------------------------------------------
--------------------------------------------------------------
-- for class creation
CREATE PROCEDURE CreateClass
  @subject VARCHAR(255),
  @className VARCHAR(255),
  @room VARCHAR(255),
  @section VARCHAR(255),
  @userId INT
AS
BEGIN
	DECLARE @code VARCHAR(7);
	DECLARE @isCodeUnique BIT;

	SET @isCodeUnique = 0;

	WHILE @isCodeUnique = 0
	BEGIN
	  SET @code = LEFT(CAST(NEWID() AS VARCHAR(36)), 7);
	  IF NOT EXISTS (SELECT 1 FROM Class WHERE code = @code)
	  BEGIN
		SET @isCodeUnique = 1; -- Code is unique
	  END
	END

	DECLARE @techId INT = (SELECT id from Teacher where Teacher.userId = @userId);

	INSERT INTO Class (className, subject, code, section, room, isActive, teacherId) 
		VALUES (@className, @subject, @code, @section, @room, 1, @techId);
END;

--------------------------------------------------------------
--------------------------------------------------------------
-- for teacher classess
CREATE PROCEDURE ClassForTeacher
  @userId INT,
  @isActive BIT
AS
BEGIN
	DECLARE @techId INT = (SELECT id from Teacher where Teacher.userId = @userId);
	SELECT * FROM Class where teacherId = @techId AND isActive = @isActive;
END;



--------------------------------------------------------------
--------------------------------------------------------------
-- for teacher classess
CREATE PROCEDURE getName
  @userId INT
AS
BEGIN
	SELECT CONCAT(first_name, ' ', last_name) as name from UserInfo where userId = @userId
END;

--------------------------------------------------------------
--------------------------------------------------------------

-- for teacher classess
CREATE PROCEDURE getClass
  @classId INT
AS
BEGIN
	SELECT * from Class where id = @classId
END;


--------------------------------------------------------------
--------------------------------------------------------------
CREATE PROCEDURE getClassItems
  @classId INT
AS
BEGIN
	SELECT * from ClassItems where classId = @classId ORDER BY id DESC;
END;


--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getCommentCount 
	@classItemId INT
AS
BEGIN
	select COUNT(*) FROM Comment WHERE classItemId = @classItemId;
END;


--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getComments
  @classItemId INT
AS
BEGIN
	SELECT cm.classItemId, cm.time, cm.comments, concat(ui.first_name, ' ' , ui.last_name) as name , cm.userId
	from Comment cm 
	LEFT JOIN UserInfo ui 
	ON ui.userId = cm.userId 
	where cm.classItemId = @classItemId ORDER BY cm.time DESC;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE insertFile
  @fileName VARCHAR(255),
  @classItemId INT
AS
BEGIN
	INSERT INTO AttachmentFiles(fileName, path, classItemId)
		VALUES (@fileName, 'E:\\Files Upload\\', @classItemId);
END;


--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE createAnnouncement
@type VARCHAR(50),
@annText TEXT,
@userId INT,
@classId INT
AS
BEGIN
	
	DECLARE @pubTime DateTime = GETDATE();


	INSERT INTO ClassItems (classId, type, publishTime) 
		VALUES (@classId, @type, @pubTime);

	DECLARE @itemId INT = (SELECT id FROM ClassItems WHERE publishTime = @pubTime AND classId = @classId);

	INSERT INTO Announcement (classItemId, announcementText, userId) 
			VALUES (@itemId, @annText, @userId);
	SELECT @itemId as id;
END;



--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE createAssignment
@classId INT, 
@points INT,
@title VARCHAR(255),
@instructions VARCHAR(255),
@dueDateTime DATE
AS
BEGIN
	
	DECLARE @pubTime DateTime = GETDATE();


	INSERT INTO ClassItems (classId, type, publishTime) 
		VALUES (@classId, 'Assignment', @pubTime);
	
	DECLARE @classItemId INT = (SELECT id FROM ClassItems WHERE publishTime = @pubTime AND classId = @classId);


	INSERT INTO Assignment (classItemId, points, title, instructions, dueDateTime) 
			VALUES ( @classItemId, @points, @title, @instructions, @dueDateTime);

	SELECT @classItemId as id;
END;

--------------------------------------------------------------
--------------------------------------------------------------



CREATE PROCEDURE updateAssignment
@classItemId INT, 
@points INT,
@title VARCHAR(255),
@instructions VARCHAR(255),
@dueDateTime DATE
AS
BEGIN
	UPDATE Assignment 
	SET points = @points, title = @title, instructions = @instructions, dueDateTime = @dueDateTime
	WHERE classItemId = @classItemId
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE updateAnnouncement
@classItemId INT, 
@title VARCHAR(255)
AS
BEGIN
	UPDATE Announcement 
	SET announcementText = @title
	WHERE classItemId = @classItemId;
END;

--------------------------------------------------------------
--------------------------------------------------------------


CREATE PROCEDURE createMaterial
@classId INT, 
@title VARCHAR(255),
@desc VARCHAR(255)
AS
BEGIN
	
	DECLARE @pubTime DateTime = GETDATE();

	INSERT INTO ClassItems (classId, type, publishTime) 
		VALUES (@classId, 'Material', @pubTime);

	DECLARE @classItemId INT = (SELECT id FROM ClassItems WHERE publishTime = @pubTime AND classId = @classId);

	INSERT INTO Material (classItemId,title, description) 
			VALUES ( @classItemId, @title, @desc);

	SELECT @classItemId as id;
END;



--------------------------------------------------------------
--------------------------------------------------------------


CREATE PROCEDURE updateMaterial
@classItemId INT, 
@title VARCHAR(255),
@desc VARCHAR(255)
AS
BEGIN
	UPDATE Material 
	SET title = @title, description = @desc
	WHERE classItemId = @classItemId;
END;

--------------------------------------------------------------
--------------------------------------------------------------


CREATE PROCEDURE getAssignments @classId INT 
AS
BEGIN
	SELECT ass.id, ass.classItemId, ass.points, ass.title, ass.instructions, ass.dueDateTime, ci.publishTime 
	FROM Assignment ass 
	INNER JOIN ClassItems ci 
	ON ci.id = ass.classItemId 
	WHERE ci.classId = @classId ORDER BY ci.publishTime DESC;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getAssignment @classItemId INT 
AS
BEGIN
	SELECT a.id, a.classItemId, a.points, a.title, a.instructions, a.dueDateTime, ci.publishTime, CONCAT(first_name, ' ', last_name) as name
	FROM ClassItems ci 
	INNER JOIN Assignment a 
	ON a.classItemId = ci.id 
	INNER JOIN Class c 
	ON c.id = ci.classId 
	INNER JOIN Teacher t 
	ON t.id = c.teacherId 
	INNER JOIn UserInfo ui
	ON ui.userId = t.userId
	WHERE ci.id = @classItemId;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getMaterial @classItemId INT 
AS
BEGIN
	SELECT m.id, m.title, m.description, ci.publishTime, CONCAT(first_name, ' ', last_name) as name
	FROM ClassItems ci 
	INNER JOIN Material m 
	ON m.classItemId = ci.id 
	INNER JOIN Class c 
	ON c.id = ci.classId 
	INNER JOIN Teacher t 
	ON t.id = c.teacherId 
	INNER JOIn UserInfo ui
	ON ui.userId = t.userId
	WHERE ci.id = @classItemId;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getAnnouncement @classItemId INT 
AS
BEGIN
	SELECT a.id, a.classItemId, a.announcementText, ci.publishTime, CONCAT(first_name, ' ', last_name) as name
	FROM ClassItems ci 
	INNER JOIN Announcement a 
	ON a.classItemId = ci.id 
	INNER JOIN Class c 
	ON c.id = ci.classId 
	INNER JOIN Teacher t 
	ON t.id = c.teacherId 
	INNER JOIn UserInfo ui
	ON ui.userId = t.userId
	WHERE ci.id = @classItemId;
END;

--------------------------------------------------------------
--------------------------------------------------------------


CREATE PROCEDURE getMaterials @classId INT 
AS
BEGIN
	SELECT m.id, m.classItemId,m.title, m.description, ci.publishTime 
	FROM Material m 
	INNER JOIN ClassItems ci 
	ON ci.id = m.classItemId 
	WHERE ci.classId = @classId ORDER BY ci.publishTime DESC;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getTeacher @classId INT 
AS
BEGIN
	SELECT CONCAT (first_name, ' ', last_name) as name, t.userId
	FROM Class c  
	INNER JOIN Teacher t 
	ON c.teacherId = t.id 
	INNER JOIN UserInfo ui
	ON ui.userId = t.userId
	WHERE c.id = @classId;
END;



--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getUserInfo @userId INT 
AS
BEGIN
	SELECT *
	FROM [User] u  
	INNER JOIN UserInfo ui 
	ON u.id = ui.userId
	WHERE u.id = @userId;
END;


--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getStudents @classId INT 
AS
BEGIN
	SELECT CONCAT (first_name, ' ', last_name) as name, s.userId
	FROM Enrollment e
	INNER JOIN Student s 
	ON e.studentId= s.id 
	INNER JOIN UserInfo ui
	ON ui.userId = s.userId
	WHERE e.classId = @classId;
END;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getGrades @assignmentId INT , @classId INT
AS
BEGIN
	SELECT CONCAT(first_name, ' ', last_name) as name, isTurnedIn, obtainedMarks
	FROM Submission s
	RIGHT JOIN (SELECT * FROM Enrollment WHERE classId = @classId) stds
	ON s.studentId= stds.studentId
	INNER JOIN Student std
	ON stds.studentId = std.id
	INNER JOIN UserInfo ui
	ON ui.userId = std.userId
	WHERE s.assignmentId = @assignmentId;
END;



--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE getGrades @assignmentId INT , @classId INT
AS
BEGIN
	SELECT CONCAT(first_name, ' ', last_name) as name, isTurnedIn, obtainedMarks, s.id
	FROM Submission s
	RIGHT JOIN (SELECT * FROM Enrollment WHERE classId = @classId) stds
	ON s.studentId= stds.studentId
	INNER JOIN Student std
	ON stds.studentId = std.id
	INNER JOIN UserInfo ui
	ON ui.userId = std.userId
	WHERE s.assignmentId = @assignmentId;
END;

--------------------------------------------------------------
--------------------------------------------------------------




CREATE PROCEDURE createSubmission
@classId INT, 
@classItemId INT,
@studentId INT
AS
BEGIN
	
	DECLARE @pubTime DATETIME = GETDATE();

	INSERT INTO ClassItems (classId, type, publishTime) 
		VALUES (@classId, 'Submission', @pubTime);

	DECLARE @classItemIdSub INT = (SELECT id FROM ClassItems WHERE publishTime = @pubTime AND classId = @classId);

	DECLARE @ass_id INT =  (SELECT id FROM Assignment WHERE classItemId = @classItemId);

	INSERT INTO Submission (classItemsId, submissionDateTime, isTurnedIn, assignmentId, obtainedMarks, studentId) 
			VALUES ( @classItemIdSub, NULL, 0, @ass_id, NULL, @studentId);

	SELECT @classItemIdSub as id;
END;


--------------------------------------------------------------
--------------------------------------------------------------



CREATE PROCEDURE getSubmissions
@classItemId INT
AS
BEGIN
	SELECT CONCAT(first_name, ' ', last_name) as name, isTurnedIn, obtainedMarks
	FROM Assignment a 
	INNER JOIN Submission s	
	ON a.id = s.assignmentId 
	INNER JOIN Student std
	ON std.id = s.studentId
	INNER JOIN UserInfo ui
	ON ui.userId = std.userId
	WHERE isTurnedIn = 1 AND a.classItemId = @classItemId;
END;


--------------------------------------------------------------
--------------------------------------------------------------




CREATE PROCEDURE joinClass
@classCode VARCHAR(255),
@userId INT
AS
BEGIN
	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	DECLARE @classId INT = (SELECT id FROM Class WHERE code = @classCode);

	INSERT INTO Enrollment (classId, studentId, dateTime)
	VALUES (@classId, @stdId, GETDATE());
END;



--------------------------------------------------------------
--------------------------------------------------------------





CREATE PROCEDURE getSubmission
@classItemId INT,
@userId INT
AS
BEGIN

	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);
	DECLARE @stdName VARCHAR(255) = (SELECT CONCAT(first_name, ' ', last_name) AS name FROM UserInfo WHERE userId = @userId);

	SELECT *, @stdName as name  FROM Submission s INNER JOIN Assignment a on a.id = s.assignmentId WHERE studentId = @stdId AND a.classItemId = @classItemId;

END;





--------------------------------------------------------------
--------------------------------------------------------------






CREATE PROCEDURE assignmentSubmission
@subId INT,
@userId INT
AS
BEGIN

	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	DECLARE @stdName VARCHAR(255) = (SELECT CONCAT(first_name, ' ', last_name) AS name FROM UserInfo WHERE userId = @userId);


	SELECT * , @stdName as name FROM Submission WHERE id = @subId AND studentId = @stdId; 

END;





--------------------------------------------------------------
--------------------------------------------------------------





CREATE PROCEDURE getSubmittedFiles
@classItemId INT,
@userId INT
AS
BEGIN
	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	DECLARE @submissionCID INT = (SELECT s.classItemsId FROM Assignment a 
	INNER JOIN Submission s
	ON a.id = s.assignmentId 
	INNER JOIN Student std
	ON std.id = s.studentId
	INNER JOIN  UserInfo ui
	ON std.userId = ui.userId
	WHERE a.classItemId = @classItemId AND s.studentId = @stdId);

	SELECT * FROM AttachmentFiles WHERE classItemId = @submissionCID;
END;




--------------------------------------------------------------
--------------------------------------------------------------


CREATE PROCEDURE getClassForStudent
@userId INT,
@isActive BIT
AS
BEGIN
	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	SELECT classId, dateTime, c.id, className, subject, code, section, room, isActive,CONCAT(first_name, ' ', last_name) AS name 
	FROM Enrollment e 
	INNER JOIN Class c 
	ON c.id = e.classId 
	INNER JOIN Teacher t
	ON t.id = c.teacherId
	INNER JOIN UserInfo ui
	ON ui.userId = t.userId
	WHERE studentId = @stdId AND isActive = @isActive;
END;






--------------------------------------------------------------
--------------------------------------------------------------





CREATE PROCEDURE turnInSubmission
@classItemId INT,
@userId INT
AS
BEGIN
	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	DECLARE @submissionCID INT = (SELECT s.classItemsId FROM Assignment a 
	INNER JOIN Submission s
	ON a.id = s.assignmentId 
	INNER JOIN Student std
	ON std.id = s.studentId
	INNER JOIN  UserInfo ui
	ON std.userId = ui.userId
	WHERE a.classItemId = @classItemId AND s.studentId = @stdId);

	UPDATE Submission SET isTurnedIn = 1, submissionDateTime = GETDATE() WHERE classItemsId = @submissionCID AND studentId = @stdId;
	
	SELECT @submissionCID as id;
END;



--------------------------------------------------------------
--------------------------------------------------------------







CREATE PROCEDURE unSubmitSubmission
@classItemId INT,
@userId INT
AS
BEGIN
	DECLARE @stdId INT = (SELECT id FROM Student WHERE userId = @userId);

	DECLARE @submissionCID INT = (SELECT s.classItemsId FROM Assignment a 
	INNER JOIN Submission s
	ON a.id = s.assignmentId 
	INNER JOIN Student std
	ON std.id = s.studentId
	INNER JOIN  UserInfo ui
	ON std.userId = ui.userId
	WHERE a.classItemId = @classItemId AND s.studentId = @stdId);

	UPDATE Submission SET isTurnedIn = 0, submissionDateTime = NULL WHERE classItemsId = @submissionCID AND studentId = @stdId;
	
	SELECT @submissionCID as id;
END;



--------------------------------------------------------------
--------------------------------------------------------------





CREATE VIEW StudentMarksInClassess AS
SELECT CONCAT(first_name, ' ', last_name) as name, className, obtainedMarks FROM Enrollment e 
INNER JOIN Student std
ON e.studentId = std.id
INNER JOIN UserInfo ui
ON ui.userId = std.userId
INNER JOIN Class c 
ON c.id = e.classId
INNER JOIN ClassItems ci
ON ci.classId = c.id
INNER JOIN Assignment a
ON a.classItemId = ci.id
INNER JOIN Submission s
ON s.assignmentId = a.id
WHERE s.studentId = std.id




--------------------------------------------------------------
--------------------------------------------------------------



CREATE TRIGGER updateClassLog 
ON Class
AFTER INSERT
AS
BEGIN
INSERT INTO classLog (actionName, dateTime)
VALUES('Class Created', GETDATE());
END;



--------------------------------------------------------------
--------------------------------------------------------------




CREATE VIEW studentReport AS
SELECT CONCAT(first_name, ' ', last_name) as name,
(SELECT COUNT(*) as Announcements FROM Announcement WHERE userId = std.userId) as announcements, 
(SELECT COUNT(*) as Comments FROM Comment WHERE userId = std.userId) as comments,
(SELECT COUNT(*) as AssignmentsSub FROM Assignment ass INNER JOIN Submission s ON ass.id = s.assignmentId WHERE studentId = std.id AND isTurnedIn = 1) as assignmentsSubmitted,
(SELECT COUNT(*) as AssignmentsSub FROM Assignment ass INNER JOIN Submission s ON ass.id = s.assignmentId WHERE studentId = std.id AND isTurnedIn = 0) as assignmentsMissing,
dob
FROM Class c 
INNER JOIN Enrollment e 
ON e.classId = c.id 
INNER JOIN Student std 
ON e.studentId = std.id 
INNER JOIN UserInfo ui 
ON std.userId = ui.userId;

--------------------------------------------------------------
--------------------------------------------------------------

CREATE PROCEDURE assignmentReport 
@classId INT
AS
BEGIN

SELECT 
a.id as AssignmentId, 
a.title as Title,
(SELECT COUNT(*) as AssignmentsSub FROM Submission s WHERE assignmentId = a.id AND isTurnedIn = 1) as Submitted,
(SELECT COUNT(*) as AssignmentsSub FROM Submission s WHERE assignmentId = a.id AND isTurnedIn = 0) as Pending,
(SELECT MAX(obtainedMarks) as MaxMarks FROM Submission s WHERE assignmentId = a.id AND isTurnedIn = 1) as MaxMarks,
(SELECT MIN(obtainedMarks) as MinMarks FROM Submission s WHERE assignmentId = a.id AND isTurnedIn = 1) as MinMarks,
(SELECT  AVG(obtainedMarks) as averageMarks FROM Submission s WHERE  assignmentId = a.id AND isTurnedIn = 1) as Avgmarks,
(SELECT STDEV(obtainedMarks) FROM Submission s WHERE  assignmentId = a.id AND isTurnedIn = 1) as StdDev
FROM ClassItems ci
INNER JOIN Assignment a
ON a.classItemId = ci.id
WHERE ci.classId = @classId

END;


--------------------------------------------------------------
--------------------------------------------------------------






