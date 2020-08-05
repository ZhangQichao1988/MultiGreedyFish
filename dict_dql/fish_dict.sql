DROP DATABASE IF EXISTS fish_dict
CREATE DATABASE IF NOT EXISTS fish_dict DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
USE fish_dict
DROP TABLE IF EXISTS `LanguageData`
CREATE TABLE `LanguageData`(`tw` TEXT,`en` TEXT,`jp` TEXT,`ID` INT(11),`cn` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `LanguageData`(`tw`,`en`,`jp`,`ID`,`cn`) VALUES (0.0,'江小鱼','江小魚','jiangxiaoyu','魚ちゃん'),(1.0,'第{0}名','第{0}名','No{0}','第{0}名'),(2.0,'+{0}','+{0}','+{0}','+{0}'),(3.0,'-{0}','-{0}','-{0}','-{0}'),(4.0,'躲避周圍污染的海水','躲避周圍污染的海水','躲避周圍污染的海水(en)','躲避周圍污染的海水(jp)'),(5.0,'吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！','吃掉其他玩家的魚\n成爲海底霸主！'),(6.0,'去吧！','去吧！','去吧！','去吧！'),(7.0,'生死决战！','生死决战！','生死决战！','生死决战！')
DROP TABLE IF EXISTS `FishSkillData`
CREATE TABLE `FishSkillData`(`effectId` INT(11),`ID` INT(11),`skillType` TEXT,`aryParam` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishSkillData`(`effectId`,`ID`,`skillType`,`aryParam`) VALUES (1.0,'HealLife',0.0,'0.5,0.1'),(2.0,'Suck',-1.0,'9,1')
DROP TABLE IF EXISTS `RobotData`
CREATE TABLE `RobotData`(`aiId` INT(11),`fishId` INT(11),`ID` INT(11),`name` TEXT,PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `RobotData`(`aiId`,`fishId`,`ID`,`name`) VALUES (0.0,1.0,'阿超',1),(1.0,1.0,'Ferya',1),(2.0,1.0,'HUSKY',1),(3.0,1.0,'DIAMOND DRAGON',0),(4.0,1.0,'Mr.L  - Jiayin',0),(5.0,1.0,'Asura',0),(6.0,1.0,'睡在梦里，醒在梦境',0),(7.0,1.0,'Rebecca',2),(8.0,1.0,'༄༠་Yོiིnྀgོ་༠࿐',2),(9.0,2.0,'Boss',3)
DROP TABLE IF EXISTS `FishData`
CREATE TABLE `FishData`(`prefabPath` TEXT,`life` INT(11),`name` TEXT,`atk` INT(11),`moveSpeed` DOUBLE(16,2),`ID` INT(11),`skillId` INT(11),PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
INSERT INTO `FishData`(`prefabPath`,`life`,`name`,`atk`,`moveSpeed`,`ID`,`skillId`) VALUES (0.0,'FishNpc_01',0.0,20,0.4,-1.0),(1.0,'FishPlayer_01',20.0,100,0.6,2.0),(2.0,'FishNpc_02',20.0,100,0.4,-1.0)
