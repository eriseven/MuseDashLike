using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class NotesManager : MonoBehaviour
{
    public TimelineAsset gameLevel;

    [SerializeField]
    GameObject clickNotePrefab;

    [SerializeField]
    Transform trackRoot;

    [SerializeField]
    Transform leftTrack;

    [SerializeField]
    Transform rightTrack;


    public enum InputEvent
    {
        PRESSED,
        RELEASED,
    }

    public enum InputResult
    {
        NONE,
        PERFECT,
        FAILED,
    }

    class Note
    {
        public GameObject noteObject;
        public string id;
        public float time;
        public float offsetTime = 0.5f;

        InputResult result = InputResult.NONE;

        public InputResult Update()
        {
            if (noteObject.transform.position.x < 0)
            {
                result = InputResult.FAILED;
                Debug.Log($"Note Result FAILED: {time}, {id}");
            }

            return result;
        }

        public InputResult OnInput(InputEvent ev)
        {
            if (ev == InputEvent.PRESSED)
            {
                if (Mathf.Abs(NotesManager.instance.time - time) > offsetTime)
                {
                    result = InputResult.NONE;
                }
                else
                {
                    result = InputResult.PERFECT;
                    Debug.Log($"Note Result PERFECT: {time}, {id}");
                }
            }
            return result;
        }
    }


    float unitPerSecond = 2.0f;

    class NoteTrack
    {

        Queue<Note> notes = new Queue<Note>();
        public void Update()
        {
            if (notes.Count > 0)
            {
                var note = notes.Peek();
                var result = note.Update();

                if (result != InputResult.NONE)
                {
                    notes.Dequeue();
                }

                if (result == InputResult.PERFECT)
                {
                    GameObject.Destroy(note.noteObject);
                }
            }
        }

        public void AddNote(Note note)
        {
            notes.Enqueue(note);
        }

        public void OnInput(InputEvent ev)
        {
            if (notes.Count > 0)
            {
                var note = notes.Peek();
                note.OnInput(ev);
            }
        }

    }


    NoteTrack[] tracks = new NoteTrack[2];

    public static NotesManager instance;

    private void Awake()
    {
        startTime = -1;
        instance = this;
        tracks = new NoteTrack[2];
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i] = new NoteTrack();
        }
    }

    void Start()
    {
        Load();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            UpdatePosition();
            foreach (var t in tracks)
            {
                t.Update();
            }
        }
    }

    bool isPlaying = false;

    public float startTime { get; private set; }
    public float time => Time.realtimeSinceStartup - startTime;

    void StartGame()
    {
        isPlaying = true;
        startTime = Time.realtimeSinceStartup;
        GetComponent<PlayableDirector>().Play();
    }

    void Load()
    {
        if (gameLevel == null) { return; }
        if (trackRoot == null) { return; }
        if (clickNotePrefab == null) { return; }

        var tracks = gameLevel.GetRootTracks().Where( x => x is SignalTrack).ToArray();

        var leftTrack = tracks[0];
        var leftNotes = leftTrack.GetMarkers();

        foreach (var m in leftNotes)
        {
            MuseNoteMarker n = m as MuseNoteMarker;
            var note = GameObject.Instantiate(clickNotePrefab, this.leftTrack);
            note.transform.localPosition = Vector3.right * (float)n.time * unitPerSecond;
            this.tracks[0].AddNote(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
        }

        var rightTrack = tracks[1];
        var rightNotes = rightTrack.GetMarkers();

        foreach (var m in rightNotes)
        {
            MuseNoteMarker n = m as MuseNoteMarker;
            var note = GameObject.Instantiate(clickNotePrefab, this.rightTrack);
            note.transform.localPosition = Vector3.right * (float)n.time * unitPerSecond;
            this.tracks[1].AddNote(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
        }

    }

    Vector3 trackOffset = Vector3.zero;
    void UpdatePosition()
    {
        if (trackRoot != null)
        {
            var playedTime = Time.realtimeSinceStartup - startTime;

            //trackOffset.x = playedTime * unitPerSecond;
            trackRoot.transform.localPosition = -1 * Vector3.right * playedTime * unitPerSecond;
        }
    }

    public void OnRNodeClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            tracks[1].OnInput(InputEvent.PRESSED);
        }

        if (context.canceled)
        {
            tracks[1].OnInput(InputEvent.RELEASED);
        }
    }


    public void OnLNodeClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            tracks[0].OnInput(InputEvent.PRESSED);
        }

        if (context.canceled)
        {
            tracks[0].OnInput(InputEvent.RELEASED);
        }
    }
}
