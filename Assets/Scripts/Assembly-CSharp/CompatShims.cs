using UnityEngine;

// Compatibility stubs for Unity 5.3 particle components that were removed in newer Unity versions.
// The old particle effects no longer play (they become no-ops), but the code still compiles.
public class ParticleEmitter : MonoBehaviour
{
	public float minEnergy;
	public float maxEnergy;
	public float minEmission;
	public float maxEmission;
	public float delay;
	public bool emit;
	public int particleCount;

	public bool IsAlive(bool includeChildren)
	{
		return false;
	}

	public void Emit()
	{
	}

	public void ClearParticles()
	{
	}
}

public class ParticleAnimator : MonoBehaviour
{
	public bool autodestruct;
	public Vector3 force;
}

public class ParticleRenderer : MonoBehaviour
{
}
