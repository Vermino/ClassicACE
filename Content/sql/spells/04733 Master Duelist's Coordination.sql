DELETE FROM `spell` WHERE `id` = 4733;

INSERT INTO `spell` (`id`, `name`, `stat_Mod_Type`, `stat_Mod_Key`, `stat_Mod_Val`, `last_Modified`)
VALUES (4733, 'Master Duelist''s Coordination', 33591297 /* Attribute, SingleStat, Additive, Beneficial */, 4 /* Coordination */, 20, '2021-11-01 00:00:00');
