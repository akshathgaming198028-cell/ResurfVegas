using UnityEngine;

public class UISlideInSpinner : UISlideIn
{
	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UISprite spinner;

	private Transform _cachedSpinnerTransform;

	private Vector3 _singleStepRotation = new Vector3(0f, 0f, -45f);

	private float _singleStepThreshold = 0.1f;

	private int _currentStep;

	private float _currentDuration;

	private int numberofSteps = 8;

	private void SetupSlidein(string message)
	{
		base.gameObject.SetActive(true);
		messageLabel.text = message;
		SlideIn();
	}

	private void Update()
	{
		if (_cachedSpinnerTransform == null)
		{
			_cachedSpinnerTransform = spinner.cachedTransform;
		}
		_currentDuration += Time.deltaTime;
		if (_currentDuration > _singleStepThreshold)
		{
			_currentStep++;
			_currentStep %= numberofSteps;
			Vector3 singleStepRotation = _singleStepRotation;
			_cachedSpinnerTransform.Rotate(singleStepRotation);
			_currentDuration -= _singleStepThreshold;
		}
	}
}
