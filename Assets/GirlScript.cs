using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlScript : MonoBehaviour {

	private Animator m_animator;

	// Use this for initialization
	void Start () {
		m_animator = GetComponent<Animator> ();
		m_animator.SetTrigger ("Idle");
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void run() {
		m_animator.SetTrigger ("Run");
	}

	public void hikick() {
		m_animator.SetTrigger ("Hikick");
	}

	public void samk() {
		m_animator.SetTrigger ("SAMK");
	}

	public void jab() {
		m_animator.SetTrigger ("Jab");
	}

	public void runReset() {
		m_animator.ResetTrigger ("Run");
	}

	public void hikickReset() {
		m_animator.ResetTrigger ("Hikick");
	}

	public void samkReset() {
		m_animator.ResetTrigger ("SAMK");
	}

	public void jabReset() {
		m_animator.ResetTrigger ("Jab");
	}

	public void idle() {
		m_animator.SetTrigger ("Idle");
	}


}
