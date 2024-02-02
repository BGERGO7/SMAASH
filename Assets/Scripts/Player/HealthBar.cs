using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IPunObservable
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public bool isTaken = false;

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);

		isTaken = true;
	}

    public void SetHealth(int health)
	{	
		slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
		{
			stream.SendNext(slider.maxValue);
			stream.SendNext(slider.value);
			stream.SendNext(fill.color);
			Debug.Log("IsWriting: slider.maxValue: " + slider.maxValue);
			Debug.Log("IsWriting: slider.value: " + slider.value);
			Debug.Log("IsWriting: fill.color: " + fill.color.ToString());
		}else{
			slider.maxValue = (int)stream.ReceiveNext();
			slider.value = (int)stream.ReceiveNext();
			fill.color = (Color)stream.ReceiveNext();
			Debug.Log("IsReading: slider.maxValue: " + slider.maxValue);
			Debug.Log("IsReading: slider.value: " + slider.value);
			Debug.Log("IsReading: fill.color: " + fill.color.ToString());
		}
    }
}