using Unity.Hierarchy;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Model")]
    public GameObject Model;
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;

    [Header("Shooting")]
    public bool IsAuto;
    public float FireCooldown;
    public int Damage;
    public float Range;
    public float MaxSpread;

    [Header("Ammo")]
    public int MagazineCapacity;
    public int DefaultTotalAmmo;
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public int TotalAmmo;
    public float ReloadTime;

    [Header("Recoil")]
    public float MinPitchRecoil;
    public float MaxPitchRecoil;
    public float MinYawRecoil;
    public float MaxYawRecoil;
    public float RecoilKickSpeed;
    public float RecoilRecoverySpeed;

    [Header("Sound")]
    public AudioClip FireSound;

    public void Init()
    {
        CurrentAmmo = MagazineCapacity;
        TotalAmmo = DefaultTotalAmmo;
    }
}
