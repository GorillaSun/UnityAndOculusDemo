using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraScript : MonoBehaviour {

	public Text myText;

	public GameObject rackPerfab1;
	public GameObject rackPerfab2;
	public GameObject serverPerfab1;
	public GameObject serverPerfab2;
	public GameObject cableRowRedPerfab;
	public GameObject cableRowYellowPerfab;
	public GameObject cableColRedPerfab;
	public GameObject cableColYellowPerfab;
	public Material normalWall;
	public Material normalRoof;
	public Material nightWall;
	public Material nightRoof;

	public GirlScript girlScript;
    public birdfly1 birdfly1;
    public birdfly2 birdfly2;

    public AudioClip[] bgSoundArray;
    public AudioSource bgSound;

    int rackNum = 4;
	int serverNum = 7;
	int cableNum = 1;

	int clickedButton = 0; //0 no select, 1 addRack1, 2 addRack2, 3 addServer1, 4 addServer2 5 observe 6 delete 7 clone
	int showMode = 6; // 1 freemode, 2 addRack, 3 addServer, 4 observeObject 5 girl 6 start

	GameObject selectedObject;

	Vector3 selectedObjectOriginalPos;
	Vector3 selectedObjectOriginalRotate;

	int envTemplate = 1; // 1 normal, 2 night
	int ifClone1 = 0;
	int ifClone2 = 0;
	int ifClone3 = 0;
	int totalCableLength = 5;

    int simulationProcess = 0;

	// Use this for initialization
	void Start () {

		Cursor.visible = true;

		GameObject addRackObj1 = GameObject.Find ("Canvas/AddRack1");
		Button addRackButton1 = (Button) addRackObj1.GetComponent<Button>();
		addRackButton1.onClick.AddListener (delegate() {
			onClick_addRack1(addRackObj1);
			});
		GameObject addRackObj2 = GameObject.Find ("Canvas/AddRack2");
		Button addRackButton2 = (Button) addRackObj2.GetComponent<Button>();
		addRackButton2.onClick.AddListener (delegate() {
			onClick_addRack2(addRackObj2);
		});
		GameObject addServerObj1 = GameObject.Find ("Canvas/AddServer1");
		Button addServerButton1 = (Button) addServerObj1.GetComponent<Button>();
		addServerButton1.onClick.AddListener (delegate() {
			onClick_addServer1(addServerObj1);
		});
		GameObject addServerObj2 = GameObject.Find ("Canvas/AddServer2");
		Button addServerButton2 = (Button) addServerObj2.GetComponent<Button>();
		addServerButton2.onClick.AddListener (delegate() {
			onClick_addServer2(addServerObj2);
		});
		GameObject observeObj = GameObject.Find ("Canvas/Observe");
		Button observeButton = (Button) observeObj.GetComponent<Button>();
		observeButton.onClick.AddListener (delegate() {
			onClick_observe(observeObj);
		});
		GameObject deleteObj = GameObject.Find ("Canvas/Delete");
		Button deleteButton = (Button) deleteObj.GetComponent<Button>();
		deleteButton.onClick.AddListener (delegate() {
			onClick_delete(deleteObj);
		});
		GameObject cloneObj = GameObject.Find ("Canvas/Clone");
		Button cloneButton = (Button) cloneObj.GetComponent<Button>();
		cloneButton.onClick.AddListener (delegate() {
			onClick_clone(cloneObj);
		});
		GameObject cablingObj = GameObject.Find ("Canvas/Cabling");
		Button cablingButton = (Button) cablingObj.GetComponent<Button>();
		cablingButton.onClick.AddListener (delegate() {
			onClick_cabling(cablingObj);
		});
        MeshRenderer[] marr = GameObject.Find("Trees").GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer m in marr)
        {
            m.enabled = false;
        }
        birdfly1.hide();
        birdfly2.hide();
    }

    void onClick_addRack1 (GameObject obj) {
		clickedButton = 1;
		showMode = 2;
	}
	void onClick_addRack2 (GameObject obj) {
		clickedButton = 2;
		showMode = 2;
	}
	void onClick_addServer1 (GameObject obj) {
		clickedButton = 3;
		showMode = 3;
		createObject (clickedButton, 1);
	}
	void onClick_addServer2 (GameObject obj) {
		clickedButton = 4;
		showMode = 3;
		createObject (clickedButton, 1);
	}
	void onClick_observe (GameObject obj) {
		selectedObjectOriginalRotate = selectedObject.transform.localEulerAngles;
		selectedObjectOriginalPos = selectedObject.transform.position;
		Vector3 selectObjectPos = new Vector3 (transform.position.x,
			transform.position.y,
			transform.position.z + 25);
		selectedObject.transform.position = selectObjectPos;
		showMode = 4;
	}
	void onClick_delete (GameObject obj) {
		Destroy (selectedObject);
		showMode = 1;
	}
	void onClick_clone (GameObject obj) {
		createCloneObject ();
		showMode = 1;
	}
	void onClick_cabling (GameObject obj) {
		createCableObject ();
	}


    void Update () {
        OVRInput.Update();

        if (envTemplate == 2) {
            bgSound.clip = bgSoundArray[Random.Range(0, bgSoundArray.Length)];
            bgSound.Play();
        }
        if (showMode == 1) {
			if (Input.GetMouseButton (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					this.myText.text = hit.collider.name;
					if (hit.collider.name == "RackSphere9307RC1") {
						onClick_addRack1 (null);
					} else if (hit.collider.name == "RackSphere9307RC2") {
						onClick_addRack2 (null);
					} else if (hit.collider.name == "ServerSphere8871AC1") {
						onClick_addServer1 (null);
					} else if (hit.collider.name == "ServerSphere5462AC1") {
						onClick_addServer2 (null);
					} else if (hit.collider.name.StartsWith ("Rack") || hit.collider.name.StartsWith ("Server")) {
						selectedObject = GameObject.Find (hit.collider.name);
					} else if (hit.collider.name == "unitychan") {
						selectedObject = GameObject.Find (hit.collider.name);
						showMode = 5;
					} else if (hit.collider.name == "Gorilla") {
                        changeEnv();
					}
				}
			}
		}
		if (showMode == 1 || showMode == 2 || showMode == 3) {

            //Vector2 touchAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * Time.deltaTime;
            //transform.position += new Vector3(touchAxis.x, 0, touchAxis.y);

            //if (OVRInput.Get(OVRInput.Button.Any) || Input.GetKeyDown(KeyCode.D)) {

            if (OVRInput.Get(OVRInput.Button.Three)) {
                changeEnv();
            }
            if (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.D)) {
                    
                if (simulationProcess == 0) {
                    // create Rack
                    createObject(1, 1);
                    createObject(2, 2);
                    createObject(2, 3);
                    simulationProcess = 1;
                } else if (simulationProcess == 1) { 
                    // create Server
                    createObject(3, 1);
                    selectedObject.transform.position = new Vector3(0, 13, -1);
                    selectedObject.name = "Server2";
                    createObject(3, 1);
                    selectedObject.transform.position = new Vector3(30, 13, -1);
                    selectedObject.name = "Server5";
                    createObject(4, 1);
                    selectedObject.transform.position = new Vector3(0, 10, -1);
                    selectedObject.name = "Server3";
                    createObject(4, 1);
                    selectedObject.transform.position = new Vector3(30, 10, -1);
                    selectedObject.name = "Server6";
                    simulationProcess = 2;
                } else if (simulationProcess == 2) {   
                    // clone
                    instantiateCloneRack(0, 10, rackPerfab2);
                    instantiateCloneServer(2, 13, 14, serverPerfab1);
                    instantiateCloneServer(2, 10, 14, serverPerfab2);
                    ifClone2 = 1;
                    instantiateCloneRack(30, 10, rackPerfab2);
                    instantiateCloneServer(32, 13, 14, serverPerfab1);
                    instantiateCloneServer(32, 10, 14, serverPerfab2);
                    ifClone3 = 1;
                    simulationProcess = 3;
                } else if (simulationProcess == 3) {
                    // cable
                    createCableObject();
                    simulationProcess = 4;
                } else if (simulationProcess == 4) {
                    // observe
                    selectedObject = GameObject.Find("Server2");
                    selectedObjectOriginalRotate = selectedObject.transform.localEulerAngles;
                    selectedObjectOriginalPos = selectedObject.transform.position;
                    Vector3 selectObjectPos = new Vector3(transform.position.x,
                        transform.position.y,
                        transform.position.z + 25);
                    selectedObject.transform.position = selectObjectPos;
                    simulationProcess = 5;
                }
            }

            Vector2 primaryStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector2 secondaryStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (simulationProcess != 5)
            {

                if (primaryStick.x != 0.0f)
                {
                    transform.Translate(Vector3.forward * primaryStick.x / 10);
                }
                if (primaryStick.y != 0.0)
                {
                    transform.Translate(Vector3.left * primaryStick.y / 10);
                }
                if (secondaryStick.x < 0.0)
                {
                    transform.Rotate(0, -0.1f, 0);
                }
                if (secondaryStick.x > 0.0)
                {
                    transform.Rotate(0, 0.1f, 0);
                }
            }
            else
            {
                float h = 5 * Input.GetAxis("Mouse X");
                float v = 5 * Input.GetAxis("Mouse Y");
                selectedObject.transform.Rotate(0, primaryStick.x, primaryStick.y);
                if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.F))
                //if (OVRInput.Get(OVRInput.Button.Any) || Input.GetKeyDown(KeyCode.F))
                    {
                        selectedObject.transform.localEulerAngles = selectedObjectOriginalRotate;
                    selectedObject.transform.position = selectedObjectOriginalPos;
                    simulationProcess = 4;
                }
            }


            if (Input.GetKey ("up")) {
				transform.Translate (Vector3.forward);
			}
			if (Input.GetKey ("down")) {
				transform.Translate (Vector3.back);
			}
			if (Input.GetKey ("left")) {
				transform.Rotate (0, -5, 0);
			}
			if (Input.GetKey ("right")) {
				transform.Rotate (0, 5, 0);
			}
		}

		if (showMode == 2) {
			if (Input.GetMouseButton (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					if ("RackPosition1" == hit.collider.name) {
						createObject (clickedButton, 1);
						showMode = 1;
					} else if ("RackPosition2" == hit.collider.name) {
						createObject (clickedButton, 2);
						showMode = 1;
					} else if ("RackPosition3" == hit.collider.name) {
						createObject (clickedButton, 3);
						showMode = 1;
					}
				}
			}
		} else if (showMode == 3) {
			mouseFlow ();
			if (Input.GetMouseButton (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					if ("Rack1" == hit.collider.name) {
						if (clickedButton == 3) {
							selectedObject.transform.position = new Vector3 (-30, 13, -1);
							selectedObject.name = "Server1";
						} else {
							selectedObject.transform.position = new Vector3 (-30, 10, -1);
							selectedObject.name = "Server4";
						}
						this.myText.text = selectedObject.name;
						showMode = 1;
					} else if ("Rack2" == hit.collider.name) {
						if (clickedButton == 3) {
							selectedObject.transform.position = new Vector3 (0, 13, -1);
							selectedObject.name = "Server2";
						} else {
							selectedObject.transform.position = new Vector3 (0, 10, -1);
							selectedObject.name = "Server5";
						}
						this.myText.text = selectedObject.name;
						showMode = 1;
					} else if ("Rack3" == hit.collider.name) {
						if (clickedButton == 3) {
							selectedObject.transform.position = new Vector3 (30, 13, -1);
							selectedObject.name = "Server3";
						} else {
							selectedObject.transform.position = new Vector3 (30, 10, -1);
							selectedObject.name = "Server6";
						}
						this.myText.text = selectedObject.name;
						showMode = 1;
					}
				}
			}
		} else if (showMode == 4) {
			float h = 5 * Input.GetAxis ("Mouse X");
			float v = 5 * Input.GetAxis ("Mouse Y");
			selectedObject.transform.Rotate (0, h, v);
			if (Input.GetKeyDown (KeyCode.Escape)) {
				selectedObject.transform.localEulerAngles = selectedObjectOriginalRotate;
				selectedObject.transform.position = selectedObjectOriginalPos;
				showMode = 1;
			}
		} else if (showMode == 5) {
			if (Input.GetKeyDown ("up")) {
				girlScript.run ();
			}
			if (Input.GetKeyDown ("down")) {
				girlScript.samk ();
			}
			if (Input.GetKeyDown ("left")) {
				girlScript.hikick ();
			}
			if (Input.GetKeyDown ("right")) {
				girlScript.jab ();
			}
            if (Input.GetKeyDown (KeyCode.Escape)) {
				showMode = 1;
			}
			if (Input.GetKeyUp ("up")) {
				girlScript.runReset ();
				girlScript.idle ();
			}
			if (Input.GetKeyUp ("down")) {
				girlScript.samkReset ();
				girlScript.idle ();
			}
			if (Input.GetKeyUp ("left")) {
				girlScript.hikickReset ();
				girlScript.idle ();
			}
			if (Input.GetKeyUp ("right")) {
				girlScript.jabReset ();
				girlScript.idle ();
			}
		} else {
            if (Input.GetKeyDown (KeyCode.Escape)
                || OVRInput.Get(OVRInput.Button.Any)) {
				GameObject.Find ("Start").gameObject.SetActive(false);
				showMode = 1;
			}
        }
	}

	void createObject(int objectType, int position) {
		if (objectType == 1) {
			Vector3 pos = createRackVector (position);
			pos = new Vector3 (pos.x + 4, pos.y + 8, pos.z + 4);
			Object rack = Instantiate (rackPerfab1, pos, Quaternion.identity);
			rack.name = "Rack" + position;
			selectedObject = GameObject.Find(rack.name);
			this.myText.text = selectedObject.name;
		} else if (objectType == 2) {
			Vector3 pos = createRackVector (position);
			Object rack = Instantiate (rackPerfab2, pos, Quaternion.identity);
			rack.name = "Rack" + position;
			selectedObject = GameObject.Find(rack.name);
			this.myText.text = selectedObject.name;
			selectedObject.transform.Rotate (0, 90, 0);
		} else if (objectType == 3) {
			Vector3 pos = new Vector3 (0, 0, 0);
			Object server = Instantiate (serverPerfab1, pos, Quaternion.identity);
			selectedObject = GameObject.Find(server.name);
			selectedObject.transform.Rotate (0, 180, 0);
		} else if (objectType == 4) {
			Vector3 pos = new Vector3 (0, 0, 0);
			Object server = Instantiate (serverPerfab2, pos, Quaternion.identity);
			selectedObject = GameObject.Find(server.name);
			selectedObject.transform.Rotate (0, 180, 0);
		}
	}

	Vector3 createRackVector(int position){
		Vector3 pos = new Vector3 (26, 12, 0);
		if (position == 1) {
			pos = new Vector3 (-34, 12, 0);
		} else if (position == 2) {
			pos = new Vector3 (-4, 12, 0);
		}
		return pos;
	}

	void createCloneObject() {
		if (selectedObject.name == "Rack1") {
			instantiateCloneRack (-30, 10, rackPerfab2);
			if (GameObject.Find("Server1") != null) {
				instantiateCloneServer (-28, 13, 14, serverPerfab1);
			}
			if (GameObject.Find("Server4") != null) {
				instantiateCloneServer (-28, 10, 14, serverPerfab2);
			}
			ifClone1 = 1;
		} else if (selectedObject.name == "Rack2") {
			instantiateCloneRack (0, 10, rackPerfab2);
			if (GameObject.Find("Server2") != null) {
				instantiateCloneServer (2, 13, 14, serverPerfab1);
			}
			if (GameObject.Find("Server5") != null) {
				instantiateCloneServer (2, 10, 14, serverPerfab2);
			}
			ifClone2 = 1;
		} else if (selectedObject.name == "Rack3") {
			instantiateCloneRack (30, 10, rackPerfab2);
			if (GameObject.Find("Server3") != null) {
				instantiateCloneServer (32, 13, 14, serverPerfab1);
			}
			if (GameObject.Find("Server6") != null) {
				instantiateCloneServer (32, 10, 14, serverPerfab2);
			}
			ifClone3 = 1;
		}
	}

	void createCableObject() {
		if (ifClone1 == 1) {
			instantiateCableRow (1);
			totalCableLength += 10;
			if (GameObject.Find("Server1") != null) {
				instantiateCableColumn (1);
				totalCableLength += 16;
			}
			if (GameObject.Find("Server4") != null) {
				instantiateCableColumn (4);
				totalCableLength += 12;
			}
		}
		if (ifClone2 == 1) {
			instantiateCableRow (2);
			totalCableLength += 10;
			if (GameObject.Find("Server2") != null) {
				instantiateCableColumn (2);
				totalCableLength += 16;
			}
			if (GameObject.Find("Server5") != null) {
				instantiateCableColumn (5);
				totalCableLength += 12;
			}
		}
		if (ifClone3 == 1) {
			instantiateCableRow (3);
			totalCableLength += 10;
			if (GameObject.Find("Server3") != null) {
				instantiateCableColumn (3);
				totalCableLength += 16;
			}
			if (GameObject.Find("Server6") != null) {
				instantiateCableColumn (6);
				totalCableLength += 12;
			}
		}
		this.myText.text = "Total cable lenght : " + totalCableLength + " M.";
	}

	void instantiateCableRow(int position) {
		float x = -36f;
		if (position == 2) {
			x = -6f;
		} else if (position == 3) {
			x = 24f;
		}
		Object cableObj = Instantiate (cableRowRedPerfab, new Vector3(x, 1, 56), Quaternion.identity);
		cableObj.name = "Cable" + cableNum;
		GameObject tempObject = GameObject.Find(cableObj.name);
		tempObject.transform.Rotate (90, 0, 0);
		cableNum++;
		cableObj = Instantiate (cableRowYellowPerfab, new Vector3(x + 0.5f, 1, 56), Quaternion.identity);
		cableObj.name = "Cable" + cableNum;
		tempObject = GameObject.Find(cableObj.name);
		tempObject.transform.Rotate (90, 0, 0);
		cableNum++;
	}

	void instantiateCableColumn(int position) {
		GameObject perfabObj = cableColRedPerfab;
		float x = -36f;
		float y = 11f;
		float z = 11f;
		if (position == 2) {
			x = -6f;
			y = 11f;
			z = 11f;
		} else if (position == 3) {
			x = 24f;
			y = 11f;
			z = 11f;
		} else if (position == 4) {
			x = -35.5f;
			y = 7f;
			z = 17f;
			perfabObj = cableColYellowPerfab;
		} else if (position == 5) {
			x = -5.5f;
			y = 7f;
			z = 17f;
			perfabObj = cableColYellowPerfab;
		} else if (position == 6) {
			x = 24.5f;
			y = 7f;
			z = 17f;
			perfabObj = cableColYellowPerfab;
		}
		int count = 0;
		while (count < 10) {
			Instantiate (perfabObj, new Vector3 (x, y, z + count * 10), Quaternion.identity);
			count++;
		}
	}

	void instantiateCloneServer(int x, int y, int z, GameObject perfeb) {
		int count = 0;
		while (count < 10) {
			Object server = Instantiate (perfeb, new Vector3(x, y, z + count * 10), Quaternion.identity);
			server.name = "Server" + serverNum;
			GameObject tempObject = GameObject.Find(server.name);
			tempObject.transform.Rotate (0, -90, 0);
			serverNum++;
			count++;
		}

	}

	void instantiateCloneRack(int x, int z, GameObject perfeb) {
		int count = 0;
		while (count < 10) {
			Object rack = Instantiate (perfeb, new Vector3(x, 12, z + count * 10), Quaternion.identity);
			rack.name = "Rack" + rackNum;
			rackNum++;
			count++;
		}
	}

    void changeEnv() {
        if (envTemplate == 1)
        {
            MeshRenderer[] marr = GameObject.Find("Trees").GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer m in marr)
            {
                m.enabled = true;
            }
            birdfly1.show();
            birdfly2.show();

            selectedObject = GameObject.Find("StaticObject/Wall1");
            selectedObject.GetComponent<Renderer>().material = nightWall;
            selectedObject = GameObject.Find("StaticObject/Wall2");
            selectedObject.GetComponent<Renderer>().material = nightWall;
            selectedObject = GameObject.Find("StaticObject/Wall3");
            selectedObject.GetComponent<Renderer>().material = nightWall;
            selectedObject = GameObject.Find("StaticObject/Roof");
            selectedObject.GetComponent<Renderer>().material = nightRoof;
            envTemplate = 2;
        }
        else
        {
            MeshRenderer[] marr = GameObject.Find("Trees").GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer m in marr)
            {
                m.enabled = false;
            }
            birdfly1.hide();
            birdfly2.hide();

            selectedObject = GameObject.Find("StaticObject/Wall1");
            selectedObject.GetComponent<Renderer>().material = normalWall;
            selectedObject = GameObject.Find("StaticObject/Wall2");
            selectedObject.GetComponent<Renderer>().material = normalWall;
            selectedObject = GameObject.Find("StaticObject/Wall3");
            selectedObject.GetComponent<Renderer>().material = normalWall;
            selectedObject = GameObject.Find("StaticObject/Roof");
            selectedObject.GetComponent<Renderer>().material = normalRoof;
            envTemplate = 1;
        }
    }

    void mouseFlow() {
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
		//获取鼠标在场景中坐标
		Vector3 mousePositionOnScreen = Input.mousePosition;
		//让场景中的Z=鼠标坐标的Z
		mousePositionOnScreen.z = screenPosition.z;
		//将相机中的坐标转化为世界坐标
		Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
		//物体跟随鼠标移动
		selectedObject.transform.position = mousePositionInWorld;
		selectedObject.transform.position = new Vector3 (selectedObject.transform.position.x, selectedObject.transform.position.y + 10, selectedObject.transform.position.z);
		//物体跟随鼠标X轴移动
		//selectedObject.transform.position = new Vector3(mousePositionInWorld.x,transform.position.y,transform.position.z);
	}
}
