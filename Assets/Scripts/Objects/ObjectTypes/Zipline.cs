using UnityEngine;

namespace Objects.ObjectTypes
{
    public class Zipline : Spawnable
    {
        [SerializeField] private Transform _otherSide;
        [SerializeField] private Transform _rope;

        public override void Place(RaycastHit hit, Vector3 playerLocation)
        {
            base.Place(hit, playerLocation);
            
            MoveOtherSide(playerLocation);

        }

        private void MoveOtherSide(Vector3 newPos)
        {
            _otherSide.position = newPos;

            _rope.position = (newPos + transform.position) / 2;
            
            _rope.LookAt(newPos);

            Vector3 newScale = _rope.transform.localScale;
            newScale.z = Vector3.Distance(newPos, transform.position)/2;
            _rope.transform.localScale = newScale;
        }
    }
}