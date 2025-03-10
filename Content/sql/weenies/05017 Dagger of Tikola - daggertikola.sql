DELETE FROM `weenie` WHERE `class_Id` = 5017;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (5017, 'daggertikola', 6, '2019-12-09 00:00:00') /* MeleeWeapon */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (5017,   1,          1) /* ItemType - MeleeWeapon */
     , (5017,   3,         20) /* PaletteTemplate - Silver */
     , (5017,   5,        120) /* EncumbranceVal */
     , (5017,   8,         80) /* Mass */
     , (5017,   9,    1048576) /* ValidLocations - MeleeWeapon */
     , (5017,  16,          1) /* ItemUseable - No */
     , (5017,  18,          1) /* UiEffects - Magical */
     , (5017,  19,       2500) /* Value */
     , (5017,  33,          1) /* Bonded - Bonded */
     , (5017,  44,          6) /* Damage */
     , (5017,  45,         16) /* DamageType - Fire */
     , (5017,  46,          2) /* DefaultCombatStyle - OneHanded */
     , (5017,  47,        486) /* AttackType - Thrust, Slash, DoubleSlash, TripleSlash, DoubleThrust, TripleThrust */
     , (5017,  48,          4) /* WeaponSkill - Dagger */
     , (5017,  49,         20) /* WeaponTime */
     , (5017,  51,          1) /* CombatUse - Melee */
     , (5017,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (5017, 106,        105) /* ItemSpellcraft */
     , (5017, 107,       1000) /* ItemCurMana */
     , (5017, 108,       1000) /* ItemMaxMana */
     , (5017, 114,          1) /* Attuned - Attuned */
     , (5017, 115,        125) /* ItemSkillLevelLimit */
     , (5017, 150,        103) /* HookPlacement - Hook */
     , (5017, 151,          2) /* HookType - Wall */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (5017,  22, True ) /* Inscribable */
     , (5017,  23, True ) /* DestroyOnSell */
     , (5017,  99, True ) /* Ivoryable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (5017,   5,  -0.025) /* ManaRate */
     , (5017,  21,    0.35) /* WeaponLength */
     , (5017,  22,    0.75) /* DamageVariance */
     , (5017,  29,    1.03) /* WeaponDefense */
     , (5017,  39,    1.25) /* DefaultScale */
     , (5017,  62,    1.03) /* WeaponOffense */
     , (5017, 147,    0.15) /* CriticalFrequency */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (5017,   1, 'Dagger of Tikola') /* Name */
     , (5017,  16, 'A terribly sharp, dangerous dagger that seems to be of unusual Empyrean make.') /* LongDesc */
     , (5017,  33, 'pickup_mutated_daggeroftikola') /* Quest string */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (5017,   1, 0x0200085C) /* Setup */
     , (5017,   3, 0x20000014) /* SoundTable */
     , (5017,   6, 0x04000BEF) /* PaletteBase */
     , (5017,   7, 0x10000219) /* ClothingBase */
     , (5017,   8, 0x06001607) /* Icon */
     , (5017,  22, 0x3400002B) /* PhysicsEffectTable */
     , (5017,  37,          4) /* ItemSkillLimit - Dagger */;

INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (5017,  1589,      2)  /* Heart Seeker III */
     , (5017,  1602,      2)  /* Defender III */
     , (5017,  1613,      2)  /* Blood Drinker III */
     , (5017,  1623,      2)  /* Swift Killer II */;
