using System;

namespace SYBO.Subway.Pickups.Pickers
{
	public interface IPicker
	{
		int ExecutionOrder { get; }

		string Name { get; }

		float SpawnDistanceMinToPrevious { get; }

		float ProbabilityToSpawn { get; }

		event Action<PickerRemoveRequestEvent> OnRemoveRequest;

		void DoAdd(PickupManager2 pickupManager2);

		bool DoUpdate();
	}
}
