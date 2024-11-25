using System;
using MelonLoader;
using UnityEngine;
using WYDClassicMod.Managers;
using WYDClassicMod.Networking;

namespace WYDClassicMod.Items;

public class Gun: PickupableItem
{
	public int maxAmmo = 25;
	public int ammo = 25;
	
	public override void Start()
	{
		base.Start();
		leftHandPickup = true;
	}
	
	public override void ButtonDown()
	{
		if(!held)
			return;
		if(ammo <= 0)
			return;
		
		MelonLogger.Msg("ButtonDown!");
		
		netView.RPC("Fire", PhotonTargets.All);
	}
	
	[PunRPC]
	public void Fire()
	{
		if(!held)
			return;
		if(ammo <= 0)
			return;
		
		ammo--;

		MelonLogger.Msg("Firing gun!");
		// var bullet = PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation, 0);
		// bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
	}

	public void OnGUI()
	{
		if(held && netView.isMine)
			GUI.Label(new Rect(20, Screen.height - 40, 200, 40), $"<b><size=32>{ammo}/{maxAmmo}</size></b>");
	}
}