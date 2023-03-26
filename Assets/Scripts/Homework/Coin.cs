using UnityEngine;

namespace Homework
{
	public class Coin : MonoBehaviour {

		public float rotationSpeed = 100.0f;
		
		private void Update () {
			transform.Rotate(new Vector3(0,0,rotationSpeed * Time.deltaTime));
		}

		private void OnTriggerEnter(Collider col)
		{
			if (!col.CompareTag("Player")) return;
			col.gameObject.SendMessage("CellPickup");
			Destroy(gameObject);
		}
	}
}
