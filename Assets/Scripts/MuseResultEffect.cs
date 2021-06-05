using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseResultEffect : MonoBehaviour
{
    [SerializeField]
    Transform[] notes;

    [SerializeField]
    new ParticleSystem particleSystem;

    void RandownNotes()
    {
        var count = notes.Length;
        var activeIndex = Random.Range(0, count);
        for (int i = 0; i < notes.Length; i++)
        {
            notes[i].gameObject.SetActive(activeIndex == i);
        }
    }

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        RandownNotes();
        particleSystem.Play();
    }

    public void Stop()
    {
        particleSystem.Stop();
    }
}
