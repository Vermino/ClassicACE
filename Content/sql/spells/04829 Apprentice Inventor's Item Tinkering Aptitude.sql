DELETE FROM `spell` WHERE `id` = 4829;

INSERT INTO `spell` (`id`, `name`, `stat_Mod_Type`, `stat_Mod_Key`, `stat_Mod_Val`, `last_Modified`)
VALUES (4829, 'Apprentice Inventor''s Item Tinkering Aptitude', 33591312 /* Skill, SingleStat, Additive, Beneficial */, 18 /* ItemTinkering */, 5, '2021-11-01 00:00:00');
