using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectTo : MonoBehaviour
{
    
    static public string input;
	public Text textInput;

	void Start() {
		if (input != null) {
			textInput.text = input;
		}
	}

    public void ReadInput(string ip){
		input=ip;
		Debug.Log(input);
		//ligar ao endereço ip passado
	}

    
}
