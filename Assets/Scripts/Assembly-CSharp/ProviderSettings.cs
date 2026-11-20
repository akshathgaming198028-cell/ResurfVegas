public class ProviderSettings
{
	private string _id;

	private int _weight;

	private int _priority;

	public string id
	{
		get
		{
			return _id;
		}
	}

	public int weight
	{
		get
		{
			return _weight;
		}
	}

	public int priority
	{
		get
		{
			return _priority;
		}
	}

	public ProviderSettings(string id, int priority, int weight)
	{
		_id = id;
		_weight = weight;
		_priority = priority;
	}
}
