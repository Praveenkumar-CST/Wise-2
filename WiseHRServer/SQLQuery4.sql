SELECT table_name
FROM information_schema.tables
WHERE table_type = 'BASE TABLE';
select * from BankingInformation;
select * from Experience;
select * from EmployeeDetails;
DELETE FROM  BankingInformation WHERE EmployeeID = '4';
DELETE FROM   EmployeeDetails WHERE EmployeeID = '4';
DELETE FROM Experience WHERE EmployeeID = '4';
