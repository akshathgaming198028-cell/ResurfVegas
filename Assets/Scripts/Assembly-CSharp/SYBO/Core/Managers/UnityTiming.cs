namespace SYBO.Core.Managers
{
	public enum UnityTiming
	{
		None = 0,
		EditorAfterCompilation = 1,
		EditorInitializeOnLoad = 2,
		EditorUTMEConstructor = 3,
		EditorEnabled = 4,
		EditorDisable = 5,
		EditorBeforeChangeToRuntime = 6,
		RuntimeEnabled = 7,
		RuntimeAwake = 8,
		RuntimeStart = 9,
		RuntimeTBD_SeemsToBeAfter2UpdateCalls = 10,
		RuntimeUpdate = 11,
		RuntimeBeforeDisable = 12,
		RuntimeDisable = 13,
		RuntimeOnDestroy = 14,
		EditorAfterRuntimeOnDestroy = 15
	}
}
