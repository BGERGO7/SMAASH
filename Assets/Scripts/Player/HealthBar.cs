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

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
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
		}else{
			slider.maxValue = (int)stream.ReceiveNext();
			slider.value = (int)stream.ReceiveNext();
			fill.color = (Color)stream.ReceiveNext();
		}
    }
}