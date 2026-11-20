using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;

namespace SYBO.Core.Templates
{
	public class TemplateManager : SingletonMonobehavior<TemplateManager>
	{
		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}
	}
}
