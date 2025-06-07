create database EMSdb;
use EMSdb;

CREATE TABLE Departments (
    DepartmentId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    ParentDepartmentId INT NULL,
    FOREIGN KEY (ParentDepartmentId) REFERENCES Departments(DepartmentId)
);

CREATE TABLE Employees (
    EmployeeId INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Email NVARCHAR(100),
    Position NVARCHAR(50),
    Salary DECIMAL(18,2),
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    DateOfBirth DATE,
    CreatedBy NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Projects (
    ProjectId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Description NVARCHAR(MAX)
);

CREATE TABLE EmployeeProjects (
    EmployeeId INT,
    ProjectId INT,
    AssignedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (EmployeeId, ProjectId),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId)
);

ALTER TABLE AuditLogs
ADD CreatedAt DATETIME DEFAULT GETDATE(),
  CreatedBy NVARCHAR(50),
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(50);

CREATE TABLE AuditLogs (
    AuditLogId INT PRIMARY KEY IDENTITY,
    EntityName NVARCHAR(50),
    EntityId INT,
    Action NVARCHAR(50),
    PerformedBy NVARCHAR(50),
    PerformedAt DATETIME DEFAULT GETDATE(),
    Changes NVARCHAR(MAX)
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE,
    PasswordHash NVARCHAR(256),
    Role NVARCHAR(20)
);

INSERT INTO Departments (Name, ParentDepartmentId) VALUES
('Management', NULL),
('Development', 1),
('HR', 1),
('Finance', 1),
('QA', 2);

INSERT INTO Employees (FirstName, LastName, Email, Position, Salary, DepartmentId, DateOfBirth, CreatedBy)
VALUES
('Alice', 'Johnson', 'alice.johnson@example.com', 'HR Manager', 72000, 3, '1985-06-12', 'admin'),
('Bob', 'Smith', 'bob.smith@example.com', 'Software Engineer', 85000, 2, '1990-09-01', 'admin'),
('Charlie', 'Lee', 'charlie.lee@example.com', 'QA Analyst', 65000, 5, '1988-03-15', 'admin'),
('Diana', 'Morris', 'diana.morris@example.com', 'Finance Officer', 70000, 4, '1987-11-30', 'admin'),
('Ethan', 'Clark', 'ethan.clark@example.com', 'Senior Developer', 95000, 2, '1984-01-20', 'admin'),
('Fiona', 'Davis', 'fiona.davis@example.com', 'Intern', 40000, 2, '1998-04-10', 'admin');

INSERT INTO Projects (Name, Description) VALUES
('Onboarding System', 'Build a new employee onboarding system'),
('Payroll Integration', 'Integrate third-party payroll service'),
('Internal Audit Tool', 'Tool for financial and compliance audits');

INSERT INTO EmployeeProjects (EmployeeId, ProjectId) VALUES
(2, 1), -- Bob in Onboarding
(2, 2), -- Bob in Payroll
(3, 1), -- Charlie in Onboarding
(4, 3), -- Diana in Audit
(5, 1), -- Ethan in Onboarding
(5, 2); -- Ethan in Payroll

INSERT INTO Users (Username, PasswordHash, Role) VALUES
('admin', 'HASHED_ADMIN_PASS', 'Admin'),
('manager1', 'HASHED_MANAGER_PASS', 'Manager'),
('user1', 'HASHED_USER_PASS', 'User');
    