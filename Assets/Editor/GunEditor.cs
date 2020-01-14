using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Gun))]

[System.SerializableAttribute]
public class GunEditor : MyCustomEditor {
	
#region Initialize parameters
	private SerializedProperty pickup;
	private SerializedProperty gunName;
	private SerializedProperty fireMode;
	private SerializedProperty rpm;
	private SerializedProperty magSize;
	private SerializedProperty startingAmmo;
	private SerializedProperty burstCount;
	private SerializedProperty delayBetweenBursts;
	private SerializedProperty projectileType;
	private SerializedProperty impactDamage;
	private SerializedProperty rayProjectileRange;
	private SerializedProperty physicalProjectileVelocity;
	private SerializedProperty collisionMask;
	private SerializedProperty projectileSpawns;
	private SerializedProperty physicalProjectile;
	private SerializedProperty useShells;
	private SerializedProperty shellEjectPoint;
	private SerializedProperty shell;
	private SerializedProperty useAudio;
	private SerializedProperty shootAudio;
	private SerializedProperty useMuzzleflashTextures;
	private SerializedProperty muzzleFlashTextures;
	private SerializedProperty reloadTime;
	private SerializedProperty reloadAudio;
	private SerializedProperty outOfAmmoAudio;

	protected override void Initialize () {
		reloadAudio = serializedObject.FindProperty("reloadAudio");
		outOfAmmoAudio = serializedObject.FindProperty("outOfAmmoAudio");
		pickup = serializedObject.FindProperty("pickup");
		reloadTime = serializedObject.FindProperty("reloadTime");
		gunName = serializedObject.FindProperty("gunName");
		fireMode = serializedObject.FindProperty("fireMode");
		rpm = serializedObject.FindProperty("rpm");
		magSize = serializedObject.FindProperty("magSize");
		startingAmmo = serializedObject.FindProperty("startingAmmo");
		burstCount = serializedObject.FindProperty("burstCount");
		delayBetweenBursts = serializedObject.FindProperty("delayBetweenBursts");
		projectileType = serializedObject.FindProperty("projectileType");
		impactDamage = serializedObject.FindProperty("impactDamage");
		rayProjectileRange = serializedObject.FindProperty("rayProjectileRange");
		physicalProjectileVelocity = serializedObject.FindProperty("physicalProjectileVelocity");
		collisionMask = serializedObject.FindProperty("collisionMask");
		projectileSpawns = serializedObject.FindProperty("projectileSpawns");
		physicalProjectile = serializedObject.FindProperty("physicalProjectile");
		useShells = serializedObject.FindProperty("useShells");
		shellEjectPoint = serializedObject.FindProperty("shellEjectPoint");
		shell = serializedObject.FindProperty("shell");
		useAudio = serializedObject.FindProperty("useAudio");
		shootAudio = serializedObject.FindProperty("shootAudio");
		useMuzzleflashTextures = serializedObject.FindProperty("useMuzzleflashTextures");
		muzzleFlashTextures = serializedObject.FindProperty("muzzleFlashTextures");
	}
#endregion


	public override void OnInspectorGUI () {

		Gun gun = (Gun)target;

		StartEdit();
		if (Section("Gun Attributes")) {
			PropertyField("Name", "", gunName);
			PropertyField("Mode", "", fireMode);
			PropertyField("RPM", "", rpm,PropertyBehaviour.ClampGreaterThan,1);
			PropertyField("Mag Size", "", magSize);
			PropertyField("Start Ammo", "", startingAmmo);
			PropertyField("Reload Time", "", reloadTime);
			if (fireMode.enumValueIndex == (int)Gun.FireMode.Burst) {
				PropertyField("Burst Count", "", burstCount);
				PropertyField("Burst Cooldown", "Time between bursts", delayBetweenBursts);
			}
		}
		if (Section("Projectile")) {
			PropertyField("Type", "", projectileType);
			PropertyField("Spawns", "", projectileSpawns);
			if (projectileType.enumValueIndex == (int)Gun.ProjectileType.Raycast)
				PropertyField("Range", "", rayProjectileRange,PropertyBehaviour.ClampGreaterThan,1);
			else {
				PropertyField("Projectile", "", physicalProjectile);
				PropertyField("Velocity", "", physicalProjectileVelocity);
			}
			PropertyField("Impact Damage", "", impactDamage);
			PropertyField("Collision Mask", "", collisionMask);
		}
		if (Section("Effects")) {
			PropertyField("Use Shells", "", useShells);
			if (useShells.boolValue) {
				if (Subsection("Shell Variables")) {
					PropertyField("Eject Point", "", shellEjectPoint);
					PropertyField("Shell", "", shell);
					EndSubsection();
				}
			}
			
			PropertyField("Use Audio", "", useAudio);
			if (useAudio.boolValue) {
				if (gun.gameObject.GetComponent<AudioSource>() == null) {
					gun.gameObject.AddComponent<AudioSource>();
				}
				if (Subsection("Audio Variables")) {
					PropertyField("Shoot", "", shootAudio);
					PropertyField("Empty", "", outOfAmmoAudio);
					PropertyField("Reload", "", reloadAudio);
					EndSubsection();
				}
			}
			PropertyField("Use Muzzleflash Planes", "", useMuzzleflashTextures);
			if (useMuzzleflashTextures.boolValue) {
				if (Subsection("Muzzleflash Variables")) {
					NoteField("Muzzleflash planes must be children of their respective projectile spawn transforms. They must be named 'Muzzleflash'.");
					PropertyField("Textures", "", muzzleFlashTextures);
					EndSubsection();
				}
			}
		}
		PropertyField("Pickup","",pickup);
		EndEdit();
	}
}
