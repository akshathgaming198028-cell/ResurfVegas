using UnityEngine;

public class Layers
{
	public readonly int Default = FindLayer("Default");

	public readonly int Hit = FindLayer("Hit");

	public readonly int HitBounceOnly = FindLayer("HitBounceOnly");

	public readonly int KeepOnHoverboard = FindLayer("KeepOnHoverboard");

	public readonly int Character = FindLayer("Character");

	public readonly int Guard = FindLayer("Guard");

	public readonly int Pickup = FindLayer("Pickup");

	public readonly int Background = FindLayer("Background");

	public readonly int TutorialTrigger = FindLayer("TutorialTrigger");

	public readonly int TrackEvent = FindLayer("TrackEvent");

	public readonly int TrackEventTrigger = FindLayer("TrackEventTrigger");

	public readonly int _3DGUI = FindLayer("3DGUI");

	public readonly int CELEBRATION = FindLayer("CELEBRATION");

	public readonly int _2DGUI = FindLayer("2DGUI");

	public readonly int _2DOVERLAY = FindLayer("2DOVERLAY");

	private static Layers _instance;

	public static Layers Instance
	{
		get
		{
			if (_instance == null)
			{
				Init();
			}
			return _instance;
		}
	}

	private Layers()
	{
	}

	private static int FindLayer(string name)
	{
		int num = LayerMask.NameToLayer(name);
		if (num == -1)
		{
			Debug.LogError("Could not find layer '" + name + "'.");
		}
		return num;
	}

	public static void Init()
	{
		_instance = new Layers();
	}
}
