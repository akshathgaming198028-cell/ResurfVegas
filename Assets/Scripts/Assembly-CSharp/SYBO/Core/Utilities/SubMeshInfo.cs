using UnityEngine;

namespace SYBO.Core.Utilities
{
	public struct SubMeshInfo
	{
		public MeshFilter MeshFilter;

		public int SubmeshIndex;

		public Material Material;

		public SubMeshInfo(MeshFilter meshFilter, int submeshIndex, Material material)
		{
			MeshFilter = meshFilter;
			SubmeshIndex = submeshIndex;
			Material = material;
		}
	}
}
