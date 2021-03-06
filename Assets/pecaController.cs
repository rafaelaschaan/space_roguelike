﻿using UnityEngine;
using System.Collections;

public class pecaController : MonoBehaviour {

	public float m_health;
	float m_blinkingTime = 0.0f ;
	float m_blinkingTimeMax = 0.25f;
	bool isTakingDamage = false;
	Color m_materialColor;

	// Use this for initialization
	void Awake () {
		m_materialColor = gameObject.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		DamageFeedback ();
		if (IsDead ()) {
			gameController.control.UpdateDestroyablesCount ();
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter(Collision col){
		if (col.transform.tag.Equals ("Projectile")) {
			TakeDamage(col.transform.gameObject.GetComponent<projectileController>().m_damage);
			Destroy(col.transform.gameObject);
		}
	}

	void DamageFeedback(){
		if(isTakingDamage){
			gameObject.renderer.material.color = Color.red;
			m_blinkingTime += Time.deltaTime;
			if(m_blinkingTime > m_blinkingTimeMax)
				isTakingDamage = false;
		}
		if (!isTakingDamage) {
			m_blinkingTime = 0.0f;
			gameObject.renderer.material.color = m_materialColor;
		}
	}

	public void TakeDamage(float damageTaken){
		m_health -= damageTaken;
		isTakingDamage = true;
		
	}
	
	public bool IsDead(){
		if (m_health <= 0.0f)
			return true;
		else
			return false;
	}
}
