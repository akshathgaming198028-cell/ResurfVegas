using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Core.Visuals
{
	public class OnStartMeshCombineLauncher : MonoBehaviour
	{
		private MeshCombiner _meshCombiner;

		private void Start()
		{
			_meshCombiner = this.AddComponentIfMissing(typeof(MeshCombiner)) as MeshCombiner;
			_meshCombiner.CombineInactive();
		}
	}
}
