using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody rigidbody;
    public float footstepThreshold;
    public float footstepRate;
    private float lastFootstepTime;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Mathf.Abs(rigidbody.velocity.y) < 0.1f)
        {
            if(rigidbody.velocity.magnitude > footstepThreshold)
            {
                if(Time.time - lastFootstepTime > footstepRate)
                {
                    lastFootstepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); ;
                }
            }
        }
    }
}
