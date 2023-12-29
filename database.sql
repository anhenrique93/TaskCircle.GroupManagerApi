--Create tables
CREATE TABLE "GROUP" (Id SERIAL NOT NULL, Name VARCHAR NOT NULL, AdminId int4 NOT NULL, PRIMARY KEY (Id));
CREATE TABLE USERS (Id SERIAL NOT NULL, USERId int4 NOT NULL, GroupId int4 NOT NULL, PRIMARY KEY (Id));
ALTER TABLE USERS ADD CONSTRAINT FKUSERS218327 FOREIGN KEY (GroupId) REFERENCES "GROUP" (Id);
ALTER TABLE "GROUP" ADD CONSTRAINT unique_group_name UNIQUE (Name);


--Drop tables
ALTER TABLE USERS DROP CONSTRAINT FKUSERS218327;
DROP TABLE IF EXISTS "GROUP" CASCADE;
DROP TABLE IF EXISTS USERS CASCADE;


--Select
SELECT Id, Name, AdminId FROM "GROUP";
SELECT Id, USERId, GroupId FROM USERS;


--Insert
INSERT INTO "GROUP"(Id, Name, AdminId) VALUES (?, ?, ?);
INSERT INTO USERS(Id, USERId, GroupId) VALUES (?, ?, ?);


--Update
UPDATE "GROUP" SET Name = ?, AdminId = ? WHERE Id = ?;
UPDATE USERS SET USERId = ?, GroupId = ? WHERE Id = ?;


--Delete
DELETE FROM "GROUP" WHERE Id = ?;
DELETE FROM USERS WHERE Id = ?;



--Stored Procedures / Functions (PostgreSQL)--
--AddGroup
CREATE OR REPLACE PROCEDURE AddGroup(p_name VARCHAR, p_adminId INT)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO "GROUP"(Name, AdminId) VALUES (p_name, p_adminId);
END;
$$;

--GetGroupById
CREATE OR REPLACE FUNCTION GetGroupById(p_id INT)
RETURNS TABLE(Id INT, Name VARCHAR, AdminId INT)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT "GROUP".* FROM "GROUP" WHERE "GROUP".Id = p_id;
END;
$$;


--AddUserToGroup
CREATE OR REPLACE FUNCTION AddUserToGroup(p_userId INT, p_groupId INT)
RETURNS TABLE(rows_affected INT)
LANGUAGE plpgsql
AS $$
DECLARE
    rows INTEGER;
BEGIN
    INSERT INTO USERS(USERId, GroupId) VALUES (p_userId, p_groupId);
    GET DIAGNOSTICS rows = ROW_COUNT;
    rows_affected := rows;
    RETURN NEXT;
END;
$$;

--GetUserInGroupById
CREATE OR REPLACE FUNCTION GetUserInGroupById(p_userId INT, p_groupId INT)
RETURNS TABLE(USERId INT, GroupId INT)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT USERS.USERId, USERS.GroupId FROM USERS WHERE USERS.USERId = p_userId AND USERS.GroupId = p_groupId;
END;
$$;


--DeleteGroupById
CREATE OR REPLACE PROCEDURE DeleteGroupById(p_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Deleta todos os usuários que estão no grupo
    DELETE FROM USERS WHERE GroupId = p_id;

    -- Deleta o grupo
    DELETE FROM "GROUP" WHERE Id = p_id;
END;
$$;

--RemoveUserFromGroup
CREATE OR REPLACE FUNCTION RemoveUserFromGroup(p_userId INT, p_groupId INT)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
    rows_affected INT;
BEGIN
    DELETE FROM USERS WHERE USERId = p_userId AND GroupId = p_groupId;
    GET DIAGNOSTICS rows_affected = ROW_COUNT;
    RETURN rows_affected;
END;
$$;

--GetGroupByName
CREATE OR REPLACE FUNCTION GetGroupByName(p_name VARCHAR)
RETURNS TABLE(IdGroup INT, Name VARCHAR, AdminId INT)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM "GROUP" WHERE "GROUP".Name = p_name;
END;
$$;


--RemoveAdminGroups
CREATE OR REPLACE PROCEDURE RemoveGroupsAndUsersByAdmin(p_adminId INT)
LANGUAGE plpgsql
AS $$
DECLARE
    group_ids INT[];
BEGIN
    -- Obtém os IDs dos grupos pertencentes ao admin
    SELECT array_agg(Id) INTO group_ids FROM "GROUP" WHERE AdminId = p_adminId;

    -- Remove todos os usuários desses grupos
    DELETE FROM USERS WHERE GroupId = ANY(group_ids);

    -- Remove todos os grupos pertencentes ao admin
    DELETE FROM "GROUP" WHERE AdminId = p_adminId;
END;
$$;

--GetAllUsersFromGroup
CREATE OR REPLACE FUNCTION GetAllUsersFromGroup(p_groupId INT)
RETURNS TABLE(UserId INT) AS $$
BEGIN
    RETURN QUERY 
    SELECT Id FROM USERS WHERE GroupId = p_groupId;
END; 
$$ LANGUAGE plpgsql;

--GetAllGroupsFromUserId
CREATE OR REPLACE FUNCTION GetAllGroupsFromUserId(p_userId INT)
RETURNS TABLE(GroupId INT, GroupName VARCHAR(255), AdminId INT) AS $$
BEGIN
    RETURN QUERY 
    SELECT "GROUP".Id, "GROUP".Name, "GROUP".adminid  FROM "GROUP"
    INNER JOIN USERS ON "GROUP".Id = USERS.GroupId
    WHERE USERS.UserId = p_userId;
END; 
$$ LANGUAGE plpgsql;

--GetAllGroupsFromAdminId
CREATE OR REPLACE FUNCTION GetAllGroupsFromAdminId(p_adminId INT)
RETURNS TABLE(GroupId INT, GroupName VARCHAR(255)) AS $$
BEGIN
    RETURN QUERY 
    SELECT Id, Name FROM "GROUP" WHERE AdminId = p_adminId;
END; 
$$ LANGUAGE plpgsql;
