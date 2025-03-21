DELETE FROM `weenie` WHERE `class_Id` = 30874;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (30874, 'stafffallen', 6, '2019-04-19 00:00:00') /* MeleeWeapon */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (30874,   1,          1) /* ItemType - MeleeWeapon */
     , (30874,   5,        450) /* EncumbranceVal */
     , (30874,   8,        340) /* Mass */
     , (30874,   9,    1048576) /* ValidLocations - MeleeWeapon */
     , (30874,  16,          1) /* ItemUseable - No */
     , (30874,  19,      10000) /* Value */
     , (30874,  33,          1) /* Bonded - Bonded */
     , (30874,  44,         25) /* Damage */
     , (30874,  45,          4) /* DamageType - Bludgeon */
     , (30874,  46,          2) /* DefaultCombatStyle - OneHanded */
     , (30874,  47,          6) /* AttackType - Thrust, Slash */
     , (30874,  48,         10) /* WeaponSkill - Staff */
     , (30874,  49,         25) /* WeaponTime */
     , (30874,  51,          1) /* CombatUse - Melee */
     , (30874,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (30874, 106,        250) /* ItemSpellcraft */
     , (30874, 107,       1000) /* ItemCurMana */
     , (30874, 108,       1000) /* ItemMaxMana */
     , (30874, 150,        103) /* HookPlacement - Hook */
     , (30874, 151,          2) /* HookType - Wall */
     , (30874, 158,          2) /* WieldRequirements - RawSkill */
     , (30874, 159,         10) /* WieldSkillType - Staff */
     , (30874, 160,        370) /* WieldDifficulty */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (30874,  22, True ) /* Inscribable */
     , (30874,  23, True ) /* DestroyOnSell */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (30874,   5,  -0.025) /* ManaRate */
     , (30874,  21,    1.33) /* WeaponLength */
     , (30874,  22,     0.6) /* DamageVariance */
     , (30874,  29,    1.13) /* WeaponDefense */
     , (30874,  39,       1) /* DefaultScale */
     , (30874,  62,    1.13) /* WeaponOffense */
     , (30874, 136,       2) /* CriticalMultiplier */
     , (30874, 147,    0.15) /* CriticalFrequency */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (30874,   1, 'Staff of the Fallen') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (30874,   1, 0x020012EE) /* Setup */
     , (30874,   3, 0x20000014) /* SoundTable */
     , (30874,   8, 0x06003787) /* Icon */
     , (30874,  22, 0x3400002B) /* PhysicsEffectTable */;

INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (30874,  2096,      2)  /* Infected Caress */
     , (30874,  2693,      2)  /* Moderate Staff Aptitude */;
