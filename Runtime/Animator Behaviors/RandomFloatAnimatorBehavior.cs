using UnityEngine;
public class SteppedRandomFloatAnimatorBehavior : StateMachineBehaviour
{
    [Tooltip("The name of the parameter to set on the Animator.")]
    public string parameterName;
    [Tooltip("The minimum random value (inclusive)."), Range(0f, 1f)]
    public float minValue = 0f;
    [Tooltip("The maximum random value (inclusive)."), Range(0f, 1f)]
    public float maxValue = 1f;
    [Tooltip("The step size for the random values."), Range(0.001f, 1f)]
    public float stepSize = 0.125f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (parameterName == string.Empty)
            return;

        // Calculate the minimum step index based on minValue
        int minStepIndex = Mathf.CeilToInt(minValue / stepSize);

        // Calculate the maximum step index based on maxValue
        int maxStepIndex = Mathf.FloorToInt(maxValue / stepSize);

        // Initialize the random step index
        int randomStepIndex;

        // Check if we have valid steps in our range
        if (minStepIndex > maxStepIndex)
        {
            Debug.LogWarning($"SteppedRandomFloatAnimatorBehavior: No valid steps in range [{minValue}, {maxValue}] with step size {stepSize}. Using closest valid step.");
            // Use the closest valid step to minValue
            randomStepIndex = minStepIndex;
        }
        else
        {
            // Pick a random step index within our range
            randomStepIndex = Random.Range(minStepIndex, maxStepIndex + 1);
        }

        // Calculate the discrete float value based on the step index
        float steppedValue = randomStepIndex * stepSize;

        animator.SetFloat(parameterName, steppedValue);
    }
}