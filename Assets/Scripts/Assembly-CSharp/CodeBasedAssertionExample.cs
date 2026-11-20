using UnityEngine;
using UnityTest;

[IntegrationTest.DynamicTest("ExampleIntegrationTests")]
[IntegrationTest.SucceedWithAssertions]
public class CodeBasedAssertionExample : MonoBehaviour
{
	public float FloatField = 3f;

	public GameObject goReference;

	public void Awake()
	{
		IAssertionComponentConfigurator configurator;
		FloatComparer floatComparer = AssertionComponent.Create<FloatComparer>(out configurator, CheckMethod.Start | CheckMethod.Update, base.gameObject, "CodeBasedAssertionExample.FloatField", 3f);
		configurator.UpdateCheckRepeatFrequency = 5;
		floatComparer.floatingPointError = 0.1;
		floatComparer.compareTypes = FloatComparer.CompareTypes.Equal;
		AssertionComponent.Create<ValueDoesNotChange>(CheckMethod.Start | CheckMethod.Update, base.gameObject, "CodeBasedAssertionExample.FloatField");
		base.transform.position = new Vector3(0f, 3f, 0f);
		AssertionComponent.Create<FloatComparer>(CheckMethod.Update, base.gameObject, "CodeBasedAssertionExample.FloatField", base.gameObject, "transform.position.y");
		goReference = base.gameObject;
		GeneralComparer generalComparer = AssertionComponent.Create<GeneralComparer>(CheckMethod.Update, base.gameObject, "CodeBasedAssertionExample.goReference", null);
		generalComparer.compareType = GeneralComparer.CompareType.ANotEqualsB;
	}
}
