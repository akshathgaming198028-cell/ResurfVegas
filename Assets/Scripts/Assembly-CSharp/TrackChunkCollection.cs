using System.Collections.Generic;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class TrackChunkCollection
{
	public static List<TrackChunk> chunks = new List<TrackChunk>();

	private List<TrackChunk> availableChunks = new List<TrackChunk>();

	public static List<TrackChunk> turboHeadstart1Chunks = new List<TrackChunk>();

	public static List<TrackChunk> turboHeadstart2Chunks = new List<TrackChunk>();

	private int lastAddedIndex = -1;

	private List<int> availableChunkIndices = new List<int>();

	public TrackChunk[] TrackChunks
	{
		get
		{
			return chunks.ToArray();
		}
	}

	public TrackChunk[] TurboHeadstart1TrackChunks
	{
		get
		{
			return turboHeadstart1Chunks.ToArray();
		}
	}

	public TrackChunk[] TurboHeadstart2TrackChunks
	{
		get
		{
			return turboHeadstart2Chunks.ToArray();
		}
	}

	public static void AddToChunks(TrackChunk newTrackChunk)
	{
		int count = chunks.Count;
		if (count == 0)
		{
			chunks.Add(newTrackChunk);
			return;
		}
		int num = 0;
		while (chunks[num].zMinimum < newTrackChunk.zMinimum)
		{
			num++;
			if (num == count)
			{
				break;
			}
		}
		chunks.Insert(num, newTrackChunk);
	}

	public static void AddToTurboHeadstart1Chunks(TrackChunk newTrackChunk)
	{
		turboHeadstart1Chunks.Add(newTrackChunk);
	}

	public static void AddToTurboHeadstart2Chunks(TrackChunk newTrackChunk)
	{
		turboHeadstart2Chunks.Add(newTrackChunk);
	}

	public void FilterAvailableInitial(float z)
	{
		availableChunks.Clear();
		lastAddedIndex = -1;
		for (int i = 0; i < chunks.Count; i++)
		{
			TrackChunk trackChunk = chunks[i];
			if (trackChunk.zMinimum <= z && z < trackChunk.zMaximum)
			{
				availableChunks.Add(trackChunk);
				lastAddedIndex = i;
			}
		}
		Recalculate();
	}

	public void FilterAvailableForward(float z)
	{
		int num = 0;
		for (int i = lastAddedIndex + 1; i < chunks.Count; i++)
		{
			TrackChunk trackChunk = chunks[i];
			if (trackChunk.zMinimum > z)
			{
				break;
			}
			availableChunks.Add(trackChunk);
			num++;
			lastAddedIndex = i;
		}
		int num2 = availableChunks.RemoveAll((TrackChunk trackChunk2) => trackChunk2.zMaximum < z);
		if (num > 0 || num2 > 0)
		{
			Recalculate();
		}
	}

	private void Recalculate()
	{
		availableChunkIndices.Clear();
		for (int i = 0; i < availableChunks.Count; i++)
		{
			TrackChunk trackChunk = availableChunks[i];
			for (int j = 0; j < trackChunk.probability; j++)
			{
				availableChunkIndices.Add(i);
			}
		}
	}

	public bool CanDeliver()
	{
		return 0 < availableChunkIndices.Count;
	}

	public TrackChunk GetRandom()
	{
		int index = Random.Range(0, availableChunkIndices.Count);
		int index2 = availableChunkIndices[index];
		if (availableChunks[index2].gameObject.name.Equals("pogoStickStart"))
		{
			PickupsMigration.Log2014PickupSpawnInNewLogger(PickupType.PogoStick);
			if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
			{
				SpawnedObjects.countUP("pogo");
			}
		}
		return availableChunks[index2];
	}

	public TrackChunk GetJetPakChunk(int index)
	{
		TrackChunk trackChunk = chunks[chunks.Count - 1 - index];
		if (!TrackChunk.IsJetpackChunk(trackChunk))
		{
			Debug.Log("Illegal TrackChunk used in jetpack mode. Index=" + index + " Name : " + trackChunk.name + " zmax : " + trackChunk.zMaximum + " zmin : " + trackChunk.zMinimum);
			Debug.Break();
		}
		return trackChunk;
	}

	public TrackChunk GetTurboHeadstart1Chunk(int index)
	{
		TrackChunk trackChunk = turboHeadstart1Chunks[turboHeadstart1Chunks.Count - 1 - index];
		if (trackChunk == null)
		{
			Debug.Log("missing TrackChunks used in headstart mode");
			Debug.Break();
		}
		return trackChunk;
	}

	public TrackChunk GetTurboHeadstart2Chunk(int index)
	{
		TrackChunk trackChunk = turboHeadstart2Chunks[turboHeadstart2Chunks.Count - 1 - index];
		if (trackChunk == null)
		{
			Debug.Log("missing TrackChunks used in headstart mode");
			Debug.Break();
		}
		return trackChunk;
	}
}
