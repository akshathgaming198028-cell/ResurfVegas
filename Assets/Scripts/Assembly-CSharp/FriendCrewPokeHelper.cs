using UnityEngine;

public class FriendCrewPokeHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite pokeIcon;

	[SerializeField]
	private BoxCollider pokeCollider;

	private Friend _friend;

	public void ActivatePoke(Friend friend)
	{
		_friend = friend;
		pokeCollider.enabled = true;
		pokeIcon.enabled = true;
	}

	public void DeactivatePoke()
	{
		pokeCollider.enabled = false;
		pokeIcon.enabled = false;
	}

	private void OnClick()
	{
		DeactivatePoke();
		Statistics stats;
		Statistics statistics = (stats = PlayerInfo.Instance.stats);
		Stat stat2;
		Stat stat = (stat2 = Stat.PokeFriend);
		int num = stats[stat2];
		statistics[stat] = num + 1;
		if (!Application.isEditor)
		{
			SocialManager.instance.Poke(_friend, SocialManager.PokeType.Poke);
		}
	}
}
