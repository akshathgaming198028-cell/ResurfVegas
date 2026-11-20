using System;
using SYBO.Subway.Characters.States;

namespace SYBO.Subway
{
	public class CharacterStateChangedEventArgs : EventArgs
	{
		private CharacterState _oldCharacterState;

		private CharacterState _newCharacterState;

		public CharacterState OldCharacterState
		{
			get
			{
				return _oldCharacterState;
			}
		}

		public CharacterState NewCharacterState
		{
			get
			{
				return _newCharacterState;
			}
		}

		public CharacterStateChangedEventArgs(CharacterState oldCharacterState, CharacterState newCharacterState)
		{
			_oldCharacterState = oldCharacterState;
			_newCharacterState = newCharacterState;
		}
	}
}
