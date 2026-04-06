using Assets.Components.InGame.Player.Inputs;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer
{
	private readonly Dictionary<string, BufferedInput> _buffer = new();

	public void Buffer(string inputName, int inputDuration = 15)
	{
		_buffer[inputName] = new BufferedInput(Time.frameCount, inputDuration);
	}

	public bool TryConsume(string inputName)
	{
		if (!_buffer.TryGetValue(inputName, out var bufferedInput))
		{
			Debug.Log($"No {inputName} input in the buffer");
			return false;
		}

		if (!bufferedInput.IsAlive(Time.frameCount))
		{
			_buffer.Remove(inputName);
			Debug.Log($"{inputName} input is expired");
			return false;
		}

		_buffer.Remove(inputName);
		return true;
	}
}