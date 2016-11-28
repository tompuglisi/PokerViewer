-- MySQL Script generated by MySQL Workbench
-- 11/28/16 16:53:14
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
  `Name` VARCHAR(25) NOT NULL,
  `Winnings` DECIMAL NULL,
  PRIMARY KEY (`Name`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`table`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`table` (
  `TableID` VARCHAR(100) NOT NULL,
  `MaxPlayers` INT NULL,
  `Stakes` VARCHAR(20) NULL,
  `Site` VARCHAR(45) NULL,
  PRIMARY KEY (`TableID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `poker`.`hand`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`hand` (
  `HandID` BIGINT NOT NULL,
  `TableID` VARCHAR(100) NULL,
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
  `PlayerName` VARCHAR(25) NOT NULL,
  `HandID` BIGINT NOT NULL,
  `StartingStack` DECIMAL NULL,
  `EndingStack` DECIMAL NULL,
  `SeatPosition` INT NULL,
  `HoleCard1` CHAR(2) NULL,
  `HoleCard2` CHAR(2) NULL,
  PRIMARY KEY (`PlayerName`, `HandID`),
  INDEX `HandID_idx` (`HandID` ASC),
  CONSTRAINT `PlayerName`
    FOREIGN KEY (`PlayerName`)
    REFERENCES `poker`.`player` (`Name`)
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
  `PlayerName` VARCHAR(25) NULL,
  `ActionName` VARCHAR(45) NULL,
  `Street` VARCHAR(10) NULL,
  `Amount` DECIMAL NULL,
  `IsPFR` BIT NULL,
  `IsVPIP` BIT NULL,
  `Is3Bet` BIT NULL,
  `Is4Bet` BIT NULL,
  PRIMARY KEY (`HandID`, `ActionID`),
  INDEX `PlayerName_idx` (`PlayerName` ASC),
  CONSTRAINT `HandID_Action`
    FOREIGN KEY (`HandID`)
    REFERENCES `poker`.`hand` (`HandID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `PlayerName_Action`
    FOREIGN KEY (`PlayerName`)
    REFERENCES `poker`.`player` (`Name`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;

USE `poker` ;

-- -----------------------------------------------------
-- Placeholder table for view `poker`.`player_stats`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `poker`.`player_stats` (`PlayerName` INT, `HandsPlayed` INT, `Winnings` INT, `VPIP` INT, `PFR` INT, `ThreeBet` INT, `FourBet` INT, `PFAF` INT);

-- -----------------------------------------------------
-- View `poker`.`player_stats`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `poker`.`player_stats`;
USE `poker`;
CREATE 
     OR REPLACE ALGORITHM = UNDEFINED 
    DEFINER = `root`@`localhost` 
    SQL SECURITY DEFINER
VIEW `player_stats` AS
    SELECT 
        `ha1`.`PlayerName` AS `PlayerName`,
        COUNT(DISTINCT `ha1`.`HandID`) AS `HandsPlayed`,
        SUM(`ha1`.`Amount`) AS `Winnings`,
        ((SELECT 
                COUNT(DISTINCT `ha2`.`HandID`)
            FROM
                `hand_action` `ha2`
            WHERE
                ((`ha1`.`PlayerName` = `ha2`.`PlayerName`)
                    AND (`ha2`.`Street` = 'Preflop')
                    AND (`ha2`.`ActionName` IN ('RAISE' , 'CALL', 'ALL_IN', 'BET')))) / COUNT(DISTINCT `ha1`.`HandID`)) AS `VPIP`,
        (SUM(`ha1`.`IsPFR`) / COUNT(DISTINCT `ha1`.`HandID`)) AS `PFR`,
        (SUM(`ha1`.`Is3Bet`) / COUNT(DISTINCT `ha1`.`HandID`)) AS `ThreeBet`,
        (SUM(`ha1`.`Is4Bet`) / COUNT(DISTINCT `ha1`.`HandID`)) AS `FourBet`,
        ((SELECT 
                COUNT(0)
            FROM
                `hand_action` `ha3`
            WHERE
                ((`ha3`.`ActionName` IN ('BET' , 'RAISE', 'ALL_IN'))
                    AND (`ha3`.`Street` <> 'Preflop')
                    AND (`ha1`.`PlayerName` = `ha3`.`PlayerName`))) / (SELECT 
                COUNT(0)
            FROM
                `hand_action` `ha4`
            WHERE
                ((`ha1`.`PlayerName` = `ha4`.`PlayerName`)
                    AND (`ha4`.`ActionName` = 'CALL')
                    AND (`ha4`.`Street` <> 'Preflop')))) AS `PFAF`
    FROM
        `hand_action` `ha1`
    GROUP BY `ha1`.`PlayerName`;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
