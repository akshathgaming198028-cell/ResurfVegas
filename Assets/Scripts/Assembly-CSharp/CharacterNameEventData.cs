using System.Collections.Generic;
using SYBO.Subway.Info;

public class CharacterNameEventData
{
	public List<Characters.CharacterType> charactersList { get; set; }

	public Characters.CharacterType currentCharacter { get; set; }

	public int collectedLetterIndex { get; set; }

	public int completedCharacters { get; set; }

	public int skippedCharacters { get; set; }

	public bool wasLastCharacterCompleted { get; set; }

	public long eventID { get; set; }

	public CharacterNameEventData(List<Characters.CharacterType> charactersList, Characters.CharacterType currentCharacter, int collectedLetterIndex, int completedCharacters, int skippedCharacters, bool wasLastCharacterCompleted, long eventID)
	{
		this.charactersList = charactersList;
		this.currentCharacter = currentCharacter;
		this.collectedLetterIndex = collectedLetterIndex;
		this.completedCharacters = completedCharacters;
		this.skippedCharacters = skippedCharacters;
		this.wasLastCharacterCompleted = wasLastCharacterCompleted;
		this.eventID = eventID;
	}
}
