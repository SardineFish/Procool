using System;
using System.Collections.Generic;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class DamageEntity : MonoBehaviour
    {
        public List<ContactPoint2D> Contacts = new List<ContactPoint2D>(16);
        private void OnCollisionEnter2D(Collision2D other)
        {
            other.GetContacts(Contacts);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            other.GetContacts(Contacts);
        }

        private void FixedUpdate()
        {
            Contacts.Clear();
        }
        
    }
}