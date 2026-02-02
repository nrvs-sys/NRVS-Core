using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class ModifiableValueBase<T>
{
	private T _baseValue;
	public T baseValue
	{
		get => _baseValue;
		set
		{
			_baseValue = value;

			UpdateValue();
		}
	}

	protected List<T> modifiers = new List<T>();

	private T _Value;
	public T Value
	{
		get => _Value;
		private set
		{
			_Value = value;

			OnValueChanged?.Invoke(_Value);
		}
	}

	public delegate void ValueChangedHandler(T value);
	public event ValueChangedHandler OnValueChanged;

	public void AddModifier(T modifier)
	{
		modifiers.Add(modifier);

		UpdateValue();
	}

	public void RemoveModifier(T modifier)
	{
		modifiers.Remove(modifier);

		UpdateValue();
	}

	public void ResetModifiers()
	{
		modifiers.Clear();

		UpdateValue();
	}

	private void UpdateValue() => Value = GetValue();

	protected abstract T GetValue();
}

public class ModifiableFloat : ModifiableValueBase<float>
{
	protected override float GetValue() => baseValue + modifiers.Sum();
}

public class ModifiableBool : ModifiableValueBase<bool>
{
	protected override bool GetValue() => baseValue || modifiers.Exists(b => b);
}