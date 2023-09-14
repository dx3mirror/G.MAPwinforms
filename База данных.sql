Create database [Map]


CREATE TABLE MAPKOORDINAT (
ID INT NOT NULL PRIMARY KEY IDENTITY (1,1),
Latitude INT,
Longitude INT)

INSERT INTO [dbo].MAPKOORDINAT
           ([Latitude]
           ,[Longitude])
     VALUES
           (43.751244, 32.6125423)

		   INSERT INTO [dbo].MAPKOORDINAT
           ([Latitude]
           ,[Longitude])
     VALUES
           (123.43144, 132.6125423)

		    INSERT INTO [dbo].MAPKOORDINAT
           ([Latitude]
           ,[Longitude])
     VALUES
           (433.43144, 132.6125423)
