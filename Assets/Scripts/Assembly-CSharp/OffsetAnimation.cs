using UnityEngine;

public class OffsetAnimation : MonoBehaviour
{
	private Animator animator;

	private void Start()
	{
		if (animator == null)
		{
			animator = base.gameObject.GetComponent<Animator>();
			float value = Random.Range(0f, 1f);
			animator.SetFloat("offset", value);
		}
	}
}
