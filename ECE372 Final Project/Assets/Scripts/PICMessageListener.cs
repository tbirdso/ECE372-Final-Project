using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PICMessageListener : MonoBehaviour {

	public delegate void JoystickHandler(int j, Vector2 v);
    public event JoystickHandler JoystickMove;

    public delegate void ButtonHandler(int i,bool b);
    public event ButtonHandler ButtonMove;

    public delegate void TemperatureHandler(float f);
    public event TemperatureHandler TemperatureRead;

    public void OnConnectionEvent(bool connected) {
		Debug.Log ("Device " + (connected ? "connected" : "disconnected") + ".");
	}

	public void OnMessageArrived(string message) {

		Debug.Log ("Received message: " + message);

		//TODO: parse and execute
		char s1 = message[0];
		char s2 = message[1];
		string rest = message.Substring (2);

		switch (s1) {
            case 't':
            case 'T':
                OnTemperatureRead(s2);
                break;
		case 'j':
		case 'J':
			OnJoystickMove(s2, new Vector2((float)rest[0],(float)rest[1]));
			break;
		case 'b':
		case 'B':
			Debug.Log ("button rest is " + rest [0]);
                OnButtonMove(s2, (rest[0] == '1' ? true : false));
			break;
		case 'P':
		case 'p':
			Print (rest);
			break;
		default:
			Print (rest);
			break;
		}

	}

	public void Print(string val) {
		Debug.Log("Received: " + val);
	}

    #region Event Handlers
    public void OnTemperatureRead(int t)
    {
        TemperatureHandler handler = TemperatureRead;

        float temp = (float)t;

        if(handler != null)
        {
            handler.Invoke(temp);
        }
    }

	public void OnJoystickMove(int joystick, Vector2 vals)
    {
        JoystickHandler handler = JoystickMove;

        if(handler != null)
        {
			handler.Invoke(joystick,vals);
        }
    }

    public void OnButtonMove(int id, bool pressed)
    {
        ButtonHandler handler = ButtonMove;

        if(handler != null)
        {
            handler.Invoke(id, pressed);
        }
    }

    #endregion

}
