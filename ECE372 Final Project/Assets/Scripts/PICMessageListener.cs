using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PICMessageListener : MonoBehaviour {

    public delegate void JoystickHandler(string s,float f);
    public event JoystickHandler JoystickMove;

    public delegate void ButtonHandler(int i,bool b);
    public event ButtonHandler ButtonMove;

    public delegate void TemperatureHandler(float f);
    public event TemperatureHandler TemperatureRead;

    public void OnConnectionEvent(bool connected) {
		Debug.Log ("Device " + (connected ? "connected" : "disconnected") + ".");
	}

	public void OnMessageArrived(string message) {
		//TODO: parse and execute
		char s1 = message[0];
		char s2 = message[1];
		string rest = message.Substring (2);

		switch (s1) {
            case 't':
            case 'T':
                OnTemperatureRead((float)s2);
                break;
		case 'P':
		case 'p':
			Print (rest);
			break;
		case 'j':
		case 'J':
                OnJoystickMove(s2, message[3]);
			break;
		case 'b':
		case 'B':
                OnButtonMove(s2, (message[3] = '1' ? true : false));
			break;
		default:
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

    public void OnJoystickMove(string axis, float val)
    {
        JoystickHandler handler = JoystickMove;

        if(handler != null)
        {
            handler.Invoke(axis,val);
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
