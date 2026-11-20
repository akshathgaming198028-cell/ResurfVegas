using System;
using UnityEngine;

namespace SYBO.Core.Utilities
{
	[Serializable]
	public struct ModelInfo
	{
		public Mesh Mesh;

		public Material[] Materials;

		public ModelInfo(Mesh mesh, Material[] materials)
		{
			Mesh = mesh;
			Materials = materials;
		}
	}
}
