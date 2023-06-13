# Neon Classroom

[![License](https://img.shields.io/badge/License-GNU%20GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

Neon Classroom is a desktop application built using C# and WinForms that aims to provide similar functionality to Google Classroom. It allows teachers to create courses, post announcements, assign and grade assignments, and facilitate communication with students.

## Features

- **Course Management**: Teachers can create and manage multiple courses, including course details, such as name, description, and start/end dates.
- **Announcements**: Teachers can post announcements in each course to provide updates and important information to students.
- **Assignments**: Teachers can create assignments, specify due dates, attach files, and provide instructions for students.
- **Grading**: Teachers can grade student assignments and provide feedback.
- **Discussion Board**: Students can participate in course-specific discussion boards to ask questions and engage in class discussions.
- **File Sharing**: Teachers can share files with students, such as lecture notes, presentations, or additional resources.
- **Notifications**: Students and teachers receive notifications for new announcements, assignments, or graded work.

## Screenshots
**Teacher Panel**
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/a0032b06-e0b0-470b-9425-39704e0f6883)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/bcfa1d2f-2704-40a9-99bb-71f42cbfa475)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/861ad3a2-f76f-4b15-8d93-19ba9101cf42)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/b24d510e-8271-49d5-87e1-0c0350a31271)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/764acef2-5e21-49fd-9b65-f759699ff810)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/07c31db4-5fe4-465f-8bc5-bf6543e6b652)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/d729bf42-3faf-47f7-84ca-e912159ac09f)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/e48023ab-1449-43e8-8a0d-64277d5f2a76)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/7e3fa936-93a7-4f74-9811-e2b0b0a40586)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/7c5be0db-d880-4b7e-aefc-05e96c45747e)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/2408622d-ebf9-4e75-9c2c-0822a0d75ac7)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/b87bad30-73d7-4193-b19a-19bec239d98e)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/38ed1e62-c172-4766-8b71-e17a3b9ca4c4)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/17ec879d-33a9-47f6-abff-551d41ad5b84)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/83580c8d-eb56-42e1-8b26-7eb3bda33818)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/909e1548-2397-4222-9667-0f37a7231233)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/7c3087a7-19d4-4168-9c43-4904763f61e6)

**Student Panel**
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/3f481a89-685e-4b10-bc5f-08336784a8d6)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/8597474b-c4ce-4f3a-ad21-e45abfdcd044)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/65de6365-18a0-40be-9a02-042ec21b48f3)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/c0636e66-7e44-410c-81d7-ea9443741e65)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/d46e81e3-92a6-4674-a5db-6a6b12363b70)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/566179aa-ac69-4cae-86b4-789ad7d3e306)
![image](https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS/assets/123104663/6bf6ca28-ed12-45b0-a519-729a779a8844)


## Prerequisites

To run this application, you need to have the following software installed on your system:

- .NET Framework (version 3.0 or higher)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server) for database storage

## Getting Started

To get a local copy of this project up and running, follow these steps:

1. Clone the repository:

```bash
git clone https://github.com/basit-nazir-memon/Neon-Classroom-Google-Classroom-LMS.git
```

2. Navigate to the project directory:

```bash
cd NeonClassroom
```

3. Open the solution file (`NeonClassroom.sln`) using Visual Studio.

4. Create the SQL Server Database using the SQL sript in 'NeonClassroomScript.sql'.

5. Modify the connection string in the `Students.cs` file to point to your SQL Server database.

6. Build the solution in Visual Studio to restore NuGet packages and compile the project.

7. Run the application and start using Neon Classroom.

## Contributing

Contributions are welcome! If you find any bugs or want to add new features, please open an issue or submit a pull request. Make sure to follow the existing coding style and conventions.

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for more information.

## Contact

For any questions or inquiries, please contact me at basitnazir585@gmail.com.
