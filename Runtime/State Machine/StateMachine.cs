// jlink, 05-13-2017

public interface IState<T>
{
	void Enter(T owner);
	void Execute(T owner);
	void Exit(T owner);
}

public class StateMachine<T>
{
	public T owner { get; private set; }

    public StateMachine(T owner)
	{
		this.owner = owner;
	}

	private MachineStates _state = MachineStates.Active;
	public MachineStates state
	{
		get { return _state; }
		set
		{
			if (_state == value)
				return;

			_state = value;

			if (state == MachineStates.Inactive)
				ChangeState(null);
		}
	}

	public enum MachineStates
	{
		Inactive,
		Active,
		Paused
	}


	public IState<T> currentState
	{
		get;
		private set;
	}

	public void ChangeState(IState<T> newState)
	{
		if (state == MachineStates.Inactive)
			return;


		currentState?.Exit(owner);

		currentState = newState;

		currentState?.Enter(owner);
	}

	public void Update()
	{
		if (state != MachineStates.Active)
			return;

			
		currentState?.Execute(owner);
	}
}