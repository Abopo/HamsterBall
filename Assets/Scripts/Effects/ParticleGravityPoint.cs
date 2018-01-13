using UnityEngine;
using System.Collections;

public class ParticleGravityPoint : MonoBehaviour {

	ParticleSystem p;
	ParticleSystem.Particle[] particles;
	int num_Particles;

	// Use this for initialization
	void Start () {
		p = GetComponent<ParticleSystem> ();
		particles = new ParticleSystem.Particle[p.maxParticles];
		num_Particles = p.GetParticles(particles);
	}
	
	// Update is called once per frame
	void Update () {
		num_Particles = p.GetParticles(particles);

		for (int i = 0; i < num_Particles; ++i) { 
			particles[i].position = Vector3.Lerp(particles[i].position, transform.position, Time.deltaTime); 
		} 

		p.SetParticles (particles, num_Particles);
	}
}
