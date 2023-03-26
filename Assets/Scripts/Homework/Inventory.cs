using UnityEngine;
using UnityEngine.UI;

namespace Homework
{
	public class Inventory : MonoBehaviour {

		public static int Num;
		public Text text;
		
		private void Start () {
			Num = 0;
		}
		
		private void CellPickup(){
			Num++;
			text.text = Num.ToString ();
			text.fontSize = Num * 20;
		}
		
	}
}
