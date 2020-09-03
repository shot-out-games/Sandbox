using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{

    public class WeaponHoldOffset : MonoBehaviour
    {

        [System.Serializable]
        public class WeaponParams
        {
            public Vector3 leftHandPosOffset;
            public Vector3 leftHandRotOffset;
            public Vector3 triggerFingerRotOffset;
        }

        public LimbIK leftHandIK;
        public Transform rightHand;
        public Transform triggerFinger;
        public WeaponParams[] weaponParams = new WeaponParams[0];
        public int currentWeaponIndex;

        void LateUpdate()
        {
            if (weaponParams.Length == 0) return;

            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weaponParams.Length - 1);

            // Left hand position offset
            leftHandIK.solver.IKPosition = leftHandIK.solver.bone3.transform.position + rightHand.rotation * weaponParams[currentWeaponIndex].leftHandPosOffset;

            // Left hand rotation offset
            leftHandIK.solver.IKRotation = Quaternion.Euler(weaponParams[currentWeaponIndex].leftHandRotOffset) * leftHandIK.solver.bone3.transform.rotation;

            // Trigger finger rotation offset
            triggerFinger.localRotation = Quaternion.Euler(weaponParams[currentWeaponIndex].triggerFingerRotOffset) * triggerFinger.localRotation;
        }
    }
}
