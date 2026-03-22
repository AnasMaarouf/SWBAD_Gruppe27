-- Drop the database 'AarhusSpaceProgram'
-- Connect to the 'tempdb' database to run this snippet
USE tempdb
GO
-- Uncomment the ALTER DATABASE statement below to set the database to SINGLE_USER mode if the drop database command fails because the database is in use.
ALTER DATABASE AarhusSpaceProgram SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- Drop the database if it exists
IF EXISTS (
  SELECT name
   FROM sys.databases
   WHERE name = N'AarhusSpaceProgram'
)
DROP DATABASE AarhusSpaceProgram
GO