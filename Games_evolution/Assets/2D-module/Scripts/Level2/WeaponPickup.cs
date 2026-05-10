using UnityEngine;

 
public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weaponPrefab; // ссылка на префаб оружи€, который будет на игроке
        [SerializeField] private AudioClip pickupSound;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // ѕровер€ем, есть ли уже оружие
                Weapon existingWeapon = other.GetComponentInChildren<Weapon>();
                if (existingWeapon != null) return; // уже есть
                Weapon newWeapon = Instantiate(weaponPrefab, other.transform);
                newWeapon.Equip(true);
                Destroy(gameObject);
                // звук
            }
        }
    }
 