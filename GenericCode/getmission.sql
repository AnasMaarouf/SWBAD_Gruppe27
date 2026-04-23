USE AarhusSpaceProgram
/* =========================
   1. LIST ALL TABLES
========================= */
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;


/* =========================
   2. ALL MISSIONS
========================= */
SELECT *
FROM Missions;


/* =========================
   3. MISSION IDS ONLY
========================= */
SELECT ID
FROM Missions;


/* =========================
   4. MISSIONS + FOREIGN KEYS
========================= */
SELECT 
    ID,
    Name,
    Duration,
    CurrentStatus,
    Type,
    LaunchDate,
    FK_RocketID,
    FK_launchpadID,
    FK_CrewID,
    FK_ManagerID,
    FK_CelestialID
FROM Missions;


/* =========================
   5. FULL JOIN VIEW
   (MISSION + RELATED TABLES)
========================= */
SELECT 
    m.ID AS MissionId,
    m.Name,
    m.Duration,
    m.CurrentStatus,
    m.Type,
    m.LaunchDate,

    r.ID AS RocketId,
    l.ID AS LaunchpadId,
    c.ID AS CrewId,
    mg.ID AS ManagerId,
    cb.ID AS CelestialBodyId

FROM Missions m
LEFT JOIN Rockets r ON m.FK_RocketID = r.ID
LEFT JOIN Launchpads l ON m.FK_launchpadID = l.ID
LEFT JOIN Crews c ON m.FK_CrewID = c.ID
LEFT JOIN Managers mg ON m.FK_ManagerID = mg.ID
LEFT JOIN CelestialBodies cb ON m.FK_CelestialID = cb.ID;


/* =========================
   6. ACTIVE MISSIONS ONLY
   (Active = 4 in enum)
========================= */
SELECT *
FROM Missions
WHERE CurrentStatus = 4;