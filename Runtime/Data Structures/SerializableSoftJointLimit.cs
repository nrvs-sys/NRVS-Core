using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
    public class SerializableSoftJointLimit
    {
        private SoftJointLimit struc = new SoftJointLimit();
        public static implicit operator SoftJointLimit(SerializableSoftJointLimit c)
        {
            return new SoftJointLimit() { limit = c._limit, bounciness = c._bounciness, contactDistance = c._contactDistance };
        }
        public static explicit operator SerializableSoftJointLimit(SoftJointLimit c)
        {
            return new SerializableSoftJointLimit(c);
        }
        public SerializableSoftJointLimit() { }
        private SerializableSoftJointLimit(SoftJointLimit _data)
        {
            this.limit = _data.limit;
            this.bounciness = _data.bounciness;
            this.contactDistance = _data.contactDistance;
        }
        [SerializeField]
        private float _limit = 0;
        [SerializeField]
        private float _bounciness = 0;
        [SerializeField]
        private float _contactDistance = 0;

        public float limit { get { return struc.limit; } set { _limit = struc.limit = value; } }
        public float bounciness { get { return struc.bounciness; } set { _bounciness = struc.bounciness = value; } }
        public float contactDistance { get { return struc.contactDistance; } set { _contactDistance = struc.contactDistance = value; } }
    }
