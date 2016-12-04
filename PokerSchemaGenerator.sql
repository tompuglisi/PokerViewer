-- MySQL Script generated by MySQL Workbench
-- 12/04/16 11:43:54
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema poker
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema poker
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `poker` DEFAULT CHARACTER SET utf8 ;
USE `poker` ;

-- -----------------------------------------------------
-- Table `poker`.`player`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`player` (
  `PlayerID` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(25) NOT NULL,
  PRIMARY KEY (`PlayerID`),
  UNIQUE INDEX `Name` (`Name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`table`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`table` (
  `TableID` BIGINT NOT NULL AUTO_INCREMENT,
  `TableName` VARCHAR(50) NOT NULL,
  `MaxPlayers` INT NULL,
  `Stakes` VARCHAR(20) NULL,
  `Site` VARCHAR(45) NULL,
  PRIMARY KEY (`TableID`),
  UNIQUE INDEX `TableName` (`TableName` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`hand`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`hand` (
  `HandID` BIGINT NOT NULL,
  `TableID` BIGINT NULL,
  `NumPlayers` INT NULL,
  `StartTime` DATETIME NULL,
  `ButtonPosition` INT NULL,
  `PotSize` DECIMAL NULL,
  `FlopCard1` CHAR(2) NULL,
  `FlopCard2` CHAR(2) NULL,
  `FlopCard3` CHAR(2) NULL,
  `TurnCard` CHAR(2) NULL,
  `RiverCard` CHAR(2) NULL,
  PRIMARY KEY (`HandID`),
  INDEX `TableID_idx` (`TableID` ASC),
  CONSTRAINT `TableID`
    FOREIGN KEY (`TableID`)
    REFERENCES `poker`.`table` (`TableID`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`plays`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`plays` (
  `PlayerID` BIGINT NOT NULL,
  `HandID` BIGINT NOT NULL,
  `StartingStack` DECIMAL NULL,
  `EndingStack` DECIMAL NULL,
  `SeatPosition` INT NULL,
  `HoleCard1` CHAR(2) NULL,
  `HoleCard2` CHAR(2) NULL,
  PRIMARY KEY (`PlayerID`, `HandID`),
  INDEX `HandID_idx` (`HandID` ASC),
  CONSTRAINT `PlayerID`
    FOREIGN KEY (`PlayerID`)
    REFERENCES `poker`.`player` (`PlayerID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `HandID`
    FOREIGN KEY (`HandID`)
    REFERENCES `poker`.`hand` (`HandID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`hand_action`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`hand_action` (
  `HandID` BIGINT NOT NULL,
  `ActionID` INT NOT NULL,
  `PlayerID` BIGINT NULL,
  `ActionName` VARCHAR(45) NULL,
  `Street` VARCHAR(10) NULL,
  `Amount` DECIMAL NULL,
  `IsPFR` BIT NULL,
  `IsVPIP` BIT NULL,
  `Is3Bet` BIT NULL,
  `Is4Bet` BIT NULL,
  PRIMARY KEY (`HandID`, `ActionID`),
  INDEX `PlayerName_idx` (`PlayerID` ASC),
  CONSTRAINT `HandID_Action`
    FOREIGN KEY (`HandID`)
    REFERENCES `poker`.`hand` (`HandID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `PlayerID_Action`
    FOREIGN KEY (`PlayerID`)
    REFERENCES `poker`.`player` (`PlayerID`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;

USE `poker` ;

-- -----------------------------------------------------
-- Placeholder table for view `poker`.`player_stats`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`player_stats` (`Name` INT, `HandsPlayed` INT, `Winnings` INT, `VPIP` INT, `PFR` INT, `ThreeBet` INT, `FourBet` INT, `PFAF` INT);

-- -----------------------------------------------------
-- View `poker`.`player_stats`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `poker`.`player_stats`;
USE `poker`;
CREATE  OR REPLACE VIEW `player_stats` AS
SELECT
player.Name,
COUNT(DISTINCT ha1.HandID) AS HandsPlayed,
SUM(ha1.Amount) AS Winnings,
(SELECT COUNT(DISTINCT ha2.HandID) 
	FROM hand_action ha2 
    WHERE ha1.PlayerID = ha2.PlayerID 
		AND ha2.Street = 'Preflop' 
		AND ha2.ActionName IN ('RAISE', 'CALL', 'ALL_IN', 'BET')
)/COUNT(DISTINCT ha1.HandID) AS VPIP,
SUM(ha1.IsPFR)/COUNT(DISTINCT ha1.HandID) AS PFR,
SUM(ha1.Is3Bet)/COUNT(DISTINCT ha1.HandID) AS ThreeBet,
SUM(ha1.Is4Bet)/COUNT(DISTINCT ha1.HandID) AS FourBet,
(SELECT COUNT(*) 
	FROM hand_action ha3 
    WHERE ha3.ActionName IN ('BET','RAISE','ALL_IN') 
    AND ha3.Street <> 'Preflop'
    AND ha1.PlayerID = ha3.PlayerID
)/(SELECT COUNT(*) 
	FROM hand_action ha4 
    WHERE ha1.PlayerID = ha4.PlayerID 
    AND ha4.ActionName = 'CALL'
    AND ha4.Street <> 'Preflop'
) AS PFAF
FROM hand_action ha1
INNER JOIN player ON ha1.PlayerID = player.PlayerID
GROUP BY ha1.PlayerID;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
