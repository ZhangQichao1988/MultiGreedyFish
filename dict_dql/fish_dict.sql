DROP DATABASE IF EXISTS fish_dict;
CREATE DATABASE IF NOT EXISTS fish_dict DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
USE fish_dict;
DROP TABLE IF EXISTS `Config`;
CREATE TABLE `Config`(`ID` INT(11),`key` TEXT,`floatValue` DOUBLE(16,2),`intValue` INT(11),`stringValue` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `Config`(`ID`,`key`,`floatValue`,`intValue`,`stringValue`) VALUES (0,'PoisonRingScaleSpeed',0.6,0,''),(1,'PoisonRingRadiusMin',5.0,0,''),(2,'PoisonRingRadiusMax',120.0,0,''),(3,'PoisonRingDmg',10.0,0,''),(4,'PoisonRingDmgCoolTime',1.0,0,''),(5,'BgBound',72.0,0,''),(6,'AquaticHeal',40.0,0,''),(7,'AquaticHealCoolTime',0.5,0,''),(8,'PlayerLevelUpRate',0.1,0,''),(9,'PlayerSizeUpRate',0.3,0,''),(10,'FishMaxScale',3.0,0,''),(11,'HealLifeFromEatRate',1.0,0,''),(12,'EatFishTime',0.2,0,''),(13,'AttackHardTime',0.35,0,''),(14,'RobotVisionX',20.0,0,''),(15,'RobotVisionY',10.0,0,''),(16,'RobotVisionRange',550.0,0,''),(17,'EnemyResurrectionRemainingTime',3.0,0,''),(18,'AquaticRange',3.0,0,''),(19,'CanStealthTimeFromDmg',2.0,0,''),(20,'EatPearlRange',3.0,0,''),(21,'OpenShellRemainingTime',5.0,0,''),(22,'ShellOpenningTime',2.0,0,''),(23,'PearlRecoverLevel',10.0,0,''),(24,'ShellCloseDmg',100.0,0,''),(25,'ShellPearlResetRate',0.2,0,''),(1000,'BattleRewardGoldRate',0,10.0,''),(1001,'BattleRewardGoldAdvertRate',0,2.0,''),(1002,'InitialFishId',0,1.0,''),(1003,'FishLevelMax',0,10.0,''),(1004,'InitialHeadId',0,1.0,''),(1005,'InitialGold',0,10.0,''),(1006,'InitialDiamond',0,10.0,''),(2000,'FishAtkMax',0,100.0,''),(2001,'FishHpMax',0,500.0,''),(2002,'FishSpdMax',1.0,0,''),(2010,'FishLevelUpRate',0.1,0,'');
DROP TABLE IF EXISTS `FishData`;
CREATE TABLE `FishData`(`ID` INT(11),`name` INT(11),`prefabPath` TEXT,`atk` INT(11),`life` INT(11),`moveSpeed` DOUBLE(16,2),`skillId` INT(11),`isPlayerFish` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishData`(`ID`,`name`,`prefabPath`,`atk`,`life`,`moveSpeed`,`skillId`,`isPlayerFish`) VALUES (0,10000,'FishNpc_01',0.0,1,0.6,-1.0,0.0),(1,10010,'FishPlayer_01',10.0,100,0.8,1.0,1.0),(2,10020,'FishNpc_02',20.0,5000,0.5,-1.0,0.0),(3,10030,'FishPlayer_02',10.0,100,0.8,3.0,1.0);
DROP TABLE IF EXISTS `FishLevelUpData`;
CREATE TABLE `FishLevelUpData`(`ID` INT(11),`useChip` INT(11),`useGold` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishLevelUpData`(`ID`,`useChip`,`useGold`) VALUES (0,10.0,10),(1,10.0,20),(2,20.0,50),(3,50.0,100),(4,100.0,200),(5,200.0,500),(6,500.0,1000),(7,1000.0,2000),(8,2000.0,5000),(9,5000.0,10000),(10,-1.0,-1);
DROP TABLE IF EXISTS `FishRankLevelData`;
CREATE TABLE `FishRankLevelData`(`ID` INT(11),`rankLevel` INT(11),`getGold` INT(11),`rankIcon` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishRankLevelData`(`ID`,`rankLevel`,`getGold`,`rankIcon`) VALUES (1,0.0,0,'UI_RankIcon_1'),(2,40.0,20,'UI_RankIcon_1'),(3,60.0,30,'UI_RankIcon_1'),(4,80.0,40,'UI_RankIcon_1'),(5,100.0,50,'UI_RankIcon_2'),(6,120.0,60,'UI_RankIcon_2'),(7,140.0,70,'UI_RankIcon_2'),(8,160.0,80,'UI_RankIcon_2'),(9,180.0,90,'UI_RankIcon_3'),(10,200.0,100,'UI_RankIcon_3'),(11,220.0,110,'UI_RankIcon_3'),(12,240.0,120,'UI_RankIcon_3'),(13,260.0,130,'UI_RankIcon_4'),(14,280.0,140,'UI_RankIcon_4'),(15,300.0,150,'UI_RankIcon_4'),(16,320.0,160,'UI_RankIcon_4'),(17,340.0,170,'UI_RankIcon_5'),(18,360.0,180,'UI_RankIcon_5'),(19,380.0,190,'UI_RankIcon_5'),(20,400.0,200,'UI_RankIcon_5');
DROP TABLE IF EXISTS `FishSkillData`;
CREATE TABLE `FishSkillData`(`ID` INT(11),`name` INT(11),`skillType` TEXT,`effectId` INT(11),`aryParam` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishSkillData`(`ID`,`name`,`skillType`,`effectId`,`aryParam`) VALUES (1,10012.0,'FishSkillHealLife',0.0,'0.1,5'),(2,0,'FishSkillSuck',-1.0,'1,9'),(3,10032.0,'FishSkillSwelling',-1.0,'0.2,2,2');
DROP TABLE IF EXISTS `LanguageData`;
CREATE TABLE `LanguageData`(`ID` INT(11),`cn` TEXT,`tw` TEXT,`en` TEXT,`jp` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `LanguageData`(`ID`,`cn`,`tw`,`en`,`jp`) VALUES (1,'第{0}名','第{0}名','No{0}','第{0}名'),(2,'+{0}','+{0}','+{0}','+{0}'),(3,'-{0}','-{0}','-{0}','-{0}'),(4,'躲避周圍污染的海水','躲避周圍污染的海水','躲避周圍污染的海水(en)','躲避周圍污染的海水(jp)'),(5,'吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！'),(6,'去吧！','去吧！','去吧！','去吧！'),(7,'生死决战！','生死决战！','生死决战！','生死决战！'),(8,'获得金币\n{0}','获得金币\n{0}','获得金币\n{0}','获得金币\n{0}'),(9,'RankUp\n+{0}','RankUp\n+{0}','RankUp\n+{0}','RankUp\n+{0}'),(10,'战力等级{0}','战力等级{0}','战力等级{0}','战力等级{0}'),(11,'攻击力','Atk','Atk','Atk'),(12,'生命值','Hp','Hp','Hp'),(13,'速度','Spd','Spd','Spd'),(14,'技能','Skill','Skill','Skill'),(15,'升级','LevelUp','LevelUp','LevelUp'),(16,'选择','Select','Select','Select'),(50,'是否将等级提升到{0}？','是否将等级提升到{0}？','是否将等级提升到{0}？','是否将等级提升到{0}？'),(10000,'宝宝鱼','宝宝鱼','宝宝鱼','宝宝鱼'),(10010,'贪吃鱼','贪吃鱼','贪吃鱼','贪吃鱼'),(10011,'宝宝鱼长大形态，通过技能可以恢复大量体力','宝宝鱼长大形态，通过技能可以恢复大量体力','宝宝鱼长大形态，通过技能可以恢复大量体力','宝宝鱼长大形态，通过技能可以恢复大量体力'),(10012,'打嗝','打嗝','打嗝','打嗝'),(10020,'鲨鱼','鲨鱼','鲨鱼','鲨鱼'),(10030,'河豚鱼','河豚鱼','河豚鱼','河豚鱼'),(10031,'看上去温柔可爱，可是如果你主动攻击它，它可不好惹','看上去温柔可爱，可是如果你主动攻击它，它可不好惹','看上去温柔可爱，可是如果你主动攻击它，它可不好惹','看上去温柔可爱，可是如果你主动攻击它，它可不好惹'),(10032,'肿胀','肿胀','肿胀','肿胀'),(20000,'欢乐的','欢乐的','欢乐的','欢乐的'),(20001,'威严的','威严的','威严的','威严的'),(20002,'聪明的','聪明的','聪明的','聪明的'),(20003,'美丽的','美丽的','美丽的','美丽的'),(20004,'苗条的','苗条的','苗条的','苗条的'),(20005,'肥胖的','肥胖的','肥胖的','肥胖的'),(20006,'宏伟的','宏伟的','宏伟的','宏伟的'),(20007,'壮丽的','壮丽的','壮丽的','壮丽的'),(20008,'高大的','高大的','高大的','高大的'),(20009,'矮小的','矮小的','矮小的','矮小的'),(21000,'泡泡鱼','泡泡鱼','泡泡鱼','泡泡鱼'),(21001,'胖胖鱼','胖胖鱼','胖胖鱼','胖胖鱼'),(21002,'沸腾鱼','沸腾鱼','沸腾鱼','沸腾鱼'),(21003,'水煮鱼','水煮鱼','水煮鱼','水煮鱼'),(21004,'青花鱼','青花鱼','青花鱼','青花鱼'),(21005,'跳跳鱼','跳跳鱼','跳跳鱼','跳跳鱼'),(21006,'食人鱼','食人鱼','食人鱼','食人鱼'),(21007,'小尾巴鱼','小尾巴鱼','小尾巴鱼','小尾巴鱼'),(21008,'鲸鱼','鲸鱼','鲸鱼','鲸鱼'),(21009,'鲨鱼','鲨鱼','鲨鱼','鲨鱼');
DROP TABLE IF EXISTS `RankBonusData`;
CREATE TABLE `RankBonusData`(`ID` INT(11),`rankLevel` INT(11),`itemId` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `RankBonusData`(`ID`,`rankLevel`,`itemId`) VALUES (1,50.0,1),(2,100.0,1),(3,150.0,1),(4,200.0,1),(5,250.0,1),(6,300.0,1),(7,350.0,1),(8,400.0,1),(9,450.0,1),(10,500.0,1),(11,550.0,1),(12,600.0,1),(13,650.0,1),(14,700.0,1),(15,750.0,1),(16,800.0,1),(17,850.0,1),(18,900.0,1),(19,950.0,1),(20,1000.0,1);
DROP TABLE IF EXISTS `EnemyData`;
CREATE TABLE `EnemyData`(`ID` INT(11),`fishId` INT(11),`fishCountMin` INT(11),`fishCountMax` INT(11),`groupId` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `EnemyData`(`ID`,`fishId`,`fishCountMin`,`fishCountMax`,`groupId`) VALUES (0,0.0,5.0,100.0,0.0),(1,0.0,5.0,100.0,1.0),(2,0.0,5.0,100.0,2.0),(3,0.0,5.0,100.0,3.0),(4,0.0,5.0,100.0,4.0),(5,0.0,5.0,100.0,5.0),(6,0.0,5.0,100.0,6.0),(7,0.0,5.0,100.0,7.0),(8,0.0,5.0,100.0,8.0),(9,0.0,5.0,100.0,9.0);
DROP TABLE IF EXISTS `RobotData`;
CREATE TABLE `RobotData`(`ID` INT(11),`fishId` INT(11),`level` INT(11),`aiId` INT(11),`groupId` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `RobotData`(`ID`,`fishId`,`level`,`aiId`,`groupId`) VALUES (1,1.0,1.0,1.0,0.0),(2,1.0,1.0,1.0,0.0),(3,3.0,1.0,1.0,0.0),(4,1.0,1.0,0.0,0.0),(5,1.0,1.0,0.0,0.0),(6,3.0,1.0,0.0,0.0),(7,3.0,1.0,0.0,0.0),(8,1.0,1.0,2.0,0.0),(9,3.0,1.0,2.0,0.0),(10,1.0,2.0,0.0,1.0),(11,1.0,2.0,0.0,1.0),(12,1.0,2.0,0.0,1.0),(13,1.0,2.0,0.0,1.0),(14,1.0,2.0,0.0,1.0),(15,1.0,2.0,0.0,1.0),(16,1.0,2.0,0.0,1.0),(17,1.0,2.0,0.0,1.0),(18,1.0,2.0,0.0,1.0),(19,1.0,3.0,0.0,2.0),(20,1.0,3.0,0.0,2.0),(21,1.0,3.0,0.0,2.0),(22,1.0,3.0,0.0,2.0),(23,1.0,3.0,0.0,2.0),(24,1.0,3.0,0.0,2.0),(25,1.0,3.0,0.0,2.0),(26,1.0,3.0,0.0,2.0),(27,1.0,3.0,0.0,2.0),(28,1.0,4.0,0.0,3.0),(29,1.0,4.0,0.0,3.0),(30,1.0,4.0,0.0,3.0),(31,1.0,4.0,0.0,3.0),(32,1.0,4.0,0.0,3.0),(33,1.0,4.0,0.0,3.0),(34,1.0,4.0,0.0,3.0),(35,1.0,4.0,0.0,3.0),(36,1.0,4.0,0.0,3.0),(37,1.0,5.0,0.0,4.0),(38,1.0,5.0,0.0,4.0),(39,1.0,5.0,0.0,4.0),(40,1.0,5.0,0.0,4.0),(41,1.0,5.0,0.0,4.0),(42,1.0,5.0,0.0,4.0),(43,1.0,5.0,0.0,4.0),(44,1.0,5.0,0.0,4.0),(45,1.0,5.0,0.0,4.0),(46,1.0,6.0,0.0,5.0),(47,1.0,6.0,0.0,5.0),(48,1.0,6.0,0.0,5.0),(49,1.0,6.0,0.0,5.0),(50,1.0,6.0,0.0,5.0),(51,1.0,6.0,0.0,5.0),(52,1.0,6.0,0.0,5.0),(53,1.0,6.0,0.0,5.0),(54,1.0,6.0,0.0,5.0),(55,1.0,6.0,0.0,6.0),(56,1.0,7.0,0.0,6.0),(57,1.0,7.0,0.0,6.0),(58,1.0,7.0,0.0,6.0),(59,1.0,7.0,0.0,6.0),(60,1.0,7.0,0.0,6.0),(61,1.0,7.0,0.0,6.0),(62,1.0,7.0,0.0,6.0),(63,1.0,7.0,0.0,6.0),(64,1.0,7.0,0.0,6.0),(65,1.0,8.0,0.0,7.0),(66,1.0,8.0,0.0,7.0),(67,1.0,8.0,0.0,7.0),(68,1.0,8.0,0.0,7.0),(69,1.0,8.0,0.0,7.0),(70,1.0,8.0,0.0,7.0),(71,1.0,8.0,0.0,7.0),(72,1.0,8.0,0.0,7.0),(73,1.0,8.0,0.0,7.0),(74,1.0,9.0,0.0,8.0),(75,1.0,9.0,0.0,8.0),(76,1.0,9.0,0.0,8.0),(77,1.0,9.0,0.0,8.0),(78,1.0,9.0,0.0,8.0),(79,1.0,9.0,0.0,8.0),(80,1.0,9.0,0.0,8.0),(81,1.0,9.0,0.0,8.0),(82,1.0,9.0,0.0,8.0),(83,1.0,9.0,0.0,9.0),(84,1.0,9.0,0.0,9.0),(85,1.0,9.0,0.0,9.0),(86,1.0,9.0,0.0,9.0),(87,1.0,9.0,0.0,9.0),(88,1.0,9.0,0.0,9.0),(89,1.0,9.0,0.0,9.0),(90,1.0,9.0,0.0,9.0),(91,1.0,9.0,0.0,9.0);
DROP TABLE IF EXISTS `RobotGroupData`;
CREATE TABLE `RobotGroupData`(`ID` INT(11),`groupId` INT(11),`rankMin` INT(11),`rankMax` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `RobotGroupData`(`ID`,`groupId`,`rankMin`,`rankMax`) VALUES (0,0.0,0.0,100.0),(1,1.0,100.0,200.0),(2,2.0,200.0,300.0),(3,3.0,300.0,400.0),(4,4.0,400.0,500.0),(5,5.0,500.0,600.0),(6,6.0,600.0,700.0),(7,7.0,700.0,800.0),(8,8.0,800.0,900.0),(9,9.0,900.0,-1.0);
DROP TABLE IF EXISTS `UserNick`;
CREATE TABLE `UserNick`(`ID` INT(11),`value` TEXT,`type` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `UserNick`(`ID`,`value`,`type`) VALUES (1,'欢乐的',0),(2,'威严的',0),(3,'聪明的',0),(4,'美丽的',0),(5,'苗条的',0),(6,'肥胖的',0),(7,'宏伟的',0),(8,'壮丽的',0),(9,'高大的',0),(10,'矮小的',0),(11,'泡泡鱼',1.0),(12,'胖胖鱼',1.0),(13,'沸腾鱼',1.0),(14,'水煮鱼',1.0),(15,'青花鱼',1.0),(16,'跳跳鱼',1.0),(17,'食人鱼',1.0),(18,'小尾巴鱼',1.0),(19,'鲸鱼',1.0),(20,'鲨鱼',1.0);
