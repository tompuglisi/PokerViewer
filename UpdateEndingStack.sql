CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateEndStack`()
BEGIN
	DECLARE UpdateEnd NVARCHAR(20) DEFAULT 0;
	DECLARE HandID NVARCHAR(30);
    DECLARE tempStack decimal default 0;
    DECLARE count int default 0;
    DECLARE done INT DEFAULT FALSE;
	DECLARE Name VARCHAR(30) default "";
	DECLARE complex_cursor CURSOR FOR
			SELECT 
    plays.HandID, plays.PlayerName
FROM
    poker.plays AS plays,
    hand,
    player
WHERE PLAYER.Name = plays.PlayerName
        AND plays.HandID = hand.HandID;
 --       AND plays.EndingStack IS NULL;
        DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	
    
		BEGIN 
        
		start transaction;
            
			

				
			OPEN complex_cursor;
           
						
			myloop:LOOP
			FETCH  complex_cursor INTO HandID, Name; 
            -- SELECT 'Comment1';
            IF done THEN
            LEAVE myloop;
            
			ELSE 
           -- SELECT CONCAT('handID is ', HandID);
           -- SELECT CONCAT('Name is ', Name);
            set count=count+1;
            BEGIN 
           -- SELECT 'Comment2';

                
				-- SELECT CONCAT('ending stack is', tempStack);

					UPDATE plays 
SET 
    EndingStack = (plays.StartingStack + @tempStack)
WHERE
    handID = HandID
    and  (@tempStack :=(SELECT 
            SUM(hand_action.Amount)
        FROM
            hand_action
                INNER JOIN
            hand ON hand_action.HandID = hand.HandID
        WHERE
            hand_action.HandID = HandID
                AND hand_action.PlayerName = Name));

            --   SELECT CONCAT('handID is ', HandID,'Name is ', Name,'ending stack is',tempStack);
           
					FETCH complex_cursor INTO HandID, Name; 
					END;
			END IF;
			END LOOP;
			CLOSE complex_cursor; 
			COMMIT;
			SELECT CONCAT('myvar is ', count);
			END;
     
		END