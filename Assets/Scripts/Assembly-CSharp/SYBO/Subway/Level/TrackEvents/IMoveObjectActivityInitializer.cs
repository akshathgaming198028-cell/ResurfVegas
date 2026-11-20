using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public interface IMoveObjectActivityInitializer : IActivityInitializer
	{
		Transform Transform { get; }

		float Speed { get; }

		TrackChunk TrackChunk { get; }
	}
}
