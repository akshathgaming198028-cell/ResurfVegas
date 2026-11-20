using System;

namespace SYBO.Subway.Pickups
{
	[Flags]
	public enum PickerType
	{
		Default = 1,
		LetterHuntDaily = 2,
		TokenHunt = 4,
		SuperMysteryBox = 8,
		MysteryPowerUp = 0x10,
		XTest = 0x20
	}
}
