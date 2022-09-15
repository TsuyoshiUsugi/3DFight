using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirstGun : GunBase
{
    [PunRPC]
    protected override void FireBullet(Vector3 playerLook, Vector3 muzzle)
    {
        base.FireBullet(playerLook, muzzle);
    }


}
