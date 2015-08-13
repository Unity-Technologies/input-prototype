using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class KeyboardInputToEvents
	: MonoBehaviour
{
	public void Update()
	{
		HandleKey(KeyCode.A, KeyControl.A);
		HandleKey(KeyCode.B, KeyControl.B);
		HandleKey(KeyCode.C, KeyControl.C);
		HandleKey(KeyCode.D, KeyControl.D);
		HandleKey(KeyCode.E, KeyControl.E);
		HandleKey(KeyCode.F, KeyControl.F);
		HandleKey(KeyCode.G, KeyControl.G);
		HandleKey(KeyCode.H, KeyControl.H);
		HandleKey(KeyCode.I, KeyControl.I);
		HandleKey(KeyCode.J, KeyControl.J);
		HandleKey(KeyCode.K, KeyControl.K);
		HandleKey(KeyCode.L, KeyControl.L);
		HandleKey(KeyCode.M, KeyControl.M);
		HandleKey(KeyCode.N, KeyControl.N);
		HandleKey(KeyCode.O, KeyControl.O);
		HandleKey(KeyCode.P, KeyControl.P);
		HandleKey(KeyCode.Q, KeyControl.Q);
		HandleKey(KeyCode.R, KeyControl.R);
		HandleKey(KeyCode.S, KeyControl.S);
		HandleKey(KeyCode.T, KeyControl.T);
		HandleKey(KeyCode.U, KeyControl.U);
		HandleKey(KeyCode.V, KeyControl.V);
		HandleKey(KeyCode.W, KeyControl.W);
		HandleKey(KeyCode.X, KeyControl.X);
		HandleKey(KeyCode.Y, KeyControl.Y);
		HandleKey(KeyCode.Z, KeyControl.Z);
		HandleKey(KeyCode.Alpha0, KeyControl.Alpha0);
		HandleKey(KeyCode.Alpha1, KeyControl.Alpha1);
		HandleKey(KeyCode.Alpha2, KeyControl.Alpha2);
		HandleKey(KeyCode.Alpha3, KeyControl.Alpha3);
		HandleKey(KeyCode.Alpha4, KeyControl.Alpha4);
		HandleKey(KeyCode.Alpha5, KeyControl.Alpha5);
		HandleKey(KeyCode.Alpha6, KeyControl.Alpha6);
		HandleKey(KeyCode.Alpha7, KeyControl.Alpha7);
		HandleKey(KeyCode.Alpha8, KeyControl.Alpha8);
		HandleKey(KeyCode.Alpha9, KeyControl.Alpha9);
		HandleKey(KeyCode.Tab, KeyControl.Tab);
		HandleKey(KeyCode.Space, KeyControl.Space);
		HandleKey(KeyCode.Return, KeyControl.Return);
	}

	void HandleKey(KeyCode keyCode, KeyControl keyControl)
	{
		if (Input.GetKeyDown(keyCode))
			SendKeyboardEvent(keyControl, true);
		if (Input.GetKeyUp(keyCode))
			SendKeyboardEvent(keyControl, false);
	}

	void SendKeyboardEvent(KeyControl key, bool isDown)
	{
		var inputEvent = InputSystem.CreateEvent<KeyboardEvent>();

		inputEvent.deviceType = typeof(Keyboard);
		inputEvent.deviceIndex = 0;
		inputEvent.key = key;
		inputEvent.isDown = isDown;
		////TODO: modifiers

		InputSystem.QueueEvent(inputEvent);
	}
}
