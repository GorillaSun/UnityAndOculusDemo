using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraScriptBK : MonoBehaviour {

	public Text myText;

	public GameObject perfab01;
	private Vector3 pos;


	private GameObject rotateObj;

	int serverNum = 1;

	int showMode = 1; // 1 freemode, 2 objectSeleced, 3 observeMode

	GameObject selectedObject;
	Vector3 cameraOriginalPos;

	// Use this for initialization
	void Start () {
		Cursor.visible = true;

		pos = new Vector3 (-10, 30, 0);

		rotateObj = GameObject.Find("server_rack-1");
		rotateObj.SetActive (false);
		GameObject btnObj1 = GameObject.Find ("Canvas/AddRack");
		Button btn1 = (Button) btnObj1.GetComponent<Button>();
		btn1.onClick.AddListener (delegate() {
			onClick_addRack(btnObj1);
			});
		GameObject btnObj2 = GameObject.Find ("Canvas/AddServer");
		Button btn2 = (Button) btnObj2.GetComponent<Button>();
		btn2.onClick.AddListener (delegate() {
			onClick_addServer(btnObj2);
		});
	}

	void onClick_addRack (GameObject obj) {
		rotateObj.SetActive (true);
	}

	void onClick_addServer (GameObject obj) {
		Object server = Instantiate (perfab01, pos, Quaternion.identity);
		server.name = "server" + serverNum;
		serverNum++;
	}

	// Update is called once per frame
	void Update () {
		if (showMode == 1) {
			if (Input.GetKey ("up")) {
				transform.Translate (0, 1, 0);
			}
			if (Input.GetKey ("down")) {
				transform.Translate (0, -1, 0);
			}
			if (Input.GetKey ("left")) {
				//transform.Rotate (0, 10, 0);
				//transform.Translate (-10, 0, 0);
				transform.RotateAround(rotateObj.transform.position, Vector3.up, 10	);
			}
			if (Input.GetKey ("right")) {
				transform.RotateAround(rotateObj.transform.position, Vector3.down, 10);
			}
			//GameObject.Find("Event").transform
			//transform.position = player.transform.position + offset;

			if (Input.GetKeyDown (KeyCode.S)) {
				Object server = Instantiate (perfab01, pos, Quaternion.identity);
				server.name = "server" + serverNum;
				serverNum++;
			}
			if (Input.GetMouseButton (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					this.myText.text = hit.collider.name;
					selectedObject = GameObject.Find(this.myText.text);
					showMode = 2;
				}
			}

		} else if (showMode == 2) {
			float h = Input.GetAxis ("Mouse X");
			float v = Input.GetAxis ("Mouse Y");
			if (Input.GetMouseButton(0) == false) {
				selectedObject.transform.Translate (h, v, 0);
			} else {
				selectedObject.transform.Translate (h, 0, v);
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				cameraOriginalPos = transform.position;
				Vector3 cameraPos = new Vector3(selectedObject.transform.position.x,
												selectedObject.transform.position.y,
												selectedObject.transform.position.z-20);
				transform.position = cameraPos;
				showMode = 3;
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				this.myText.text = "mode1";
				showMode = 1;
			}

		} else if (showMode == 3) {
			float h = 5 * Input.GetAxis ("Mouse X");
			float v = 5 * Input.GetAxis ("Mouse Y");

			selectedObject.transform.Rotate (0, v, 0);

			if (Input.GetKeyDown (KeyCode.Space)) {
				transform.position = cameraOriginalPos;
				showMode = 2;
			}
		}
	}
}
