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
    GameObject multiClickNotePrefab;

    [SerializeField]
    GameObject longClickNotePrefab;

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
        PENDING,
        FAILED,
    }

    class Note
    {
        public GameObject noteObject;
        public string id;
        public float time;
        public float offsetTime = 0.5f;

        protected InputResult result = InputResult.NONE;

        public virtual InputResult Update()
        {
            if (noteObject.transform.position.x < 0)
            {
                result = InputResult.FAILED;
                Debug.Log($"Note Result FAILED: {time}, {id}");
            }

            return result;
        }

        public virtual InputResult OnInput(InputEvent ev)
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

    class LongClickNote : Note
    {
        public float duration = 1;
        float pressTime = -1;
        float pressedDuration = 0;

        public override InputResult OnInput(InputEvent ev)
        {

            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                if (ev == InputEvent.PRESSED)
                {
                    if (Mathf.Abs(NotesManager.instance.time - time) > offsetTime
                        || Mathf.Abs(NotesManager.instance.time - (time + duration)) > offsetTime
                        || (NotesManager.instance.time > time && NotesManager.instance.time < (time + duration)))

                    {
                        result = InputResult.PENDING;
                        pressTime = NotesManager.instance.time;
                        //Debug.Log($"Note Result PERFECT: {time}, {id}");
                    }
                    else
                    {
                        result = InputResult.NONE;
                    }
                }
            }

            if (ev == InputEvent.RELEASED && result == InputResult.PENDING)
            {
                pressedDuration = NotesManager.instance.time - pressTime;
                result = InputResult.PERFECT;
            }

            return result;
        }

        public override InputResult Update()
        {
            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                if (noteObject.transform.position.x + duration * 2 < 0)
                {
                    if ((pressedDuration / duration) > 0.5)
                    {
                        result = InputResult.PERFECT;
                    }
                    else
                    {
                        result = InputResult.FAILED;
                        Debug.Log($"Note Result FAILED: {time}, {id}");
                    }
                }
            }
            return result;
        }
    }

    class MutiClickNote : Note
    {
        public int clickCount = 1;
        public float duration = 1;
        public int clickedCount = 0;

        public override InputResult OnInput(InputEvent ev)
        {
            if (ev == InputEvent.PRESSED)
            {
                if (result == InputResult.NONE)
                {
                    if (Mathf.Abs(NotesManager.instance.time - time) > offsetTime
                        || Mathf.Abs(NotesManager.instance.time - (time + duration)) > offsetTime
                        || (NotesManager.instance.time > time && NotesManager.instance.time < (time + duration)))
                    {
                        clickedCount++;
                        Debug.Log($"MutiClickNote performed:{clickedCount}, time:{NotesManager.instance.time}");
                        if (clickedCount >= clickCount)
                        {
                            result = InputResult.PERFECT;
                        }
                        else
                        {
                            result = InputResult.PENDING;
                        }

                    }
                    else
                    {
                        result = InputResult.NONE;
                    }
                }
                else if (result == InputResult.PENDING)
                {
                    if (Mathf.Abs(NotesManager.instance.time - time) > offsetTime
                        || Mathf.Abs(NotesManager.instance.time - (time + duration)) > offsetTime
                        || (NotesManager.instance.time > time && NotesManager.instance.time < (time + duration)))
                    {
                        clickedCount++;
                        Debug.Log($"MutiClickNote performed:{clickedCount}, time:{NotesManager.instance.time}");
                        if (clickedCount >= clickCount)
                        {
                            result = InputResult.PERFECT;
                        }
                        else
                        {
                            result = InputResult.PENDING;
                        }
                    }
                }

            }
            return result;


            //return base.OnInput(ev);
        }

        public override InputResult Update()
        {
            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                if (noteObject.transform.position.x + duration * 2 < 0)
                {
                    if (clickedCount < clickCount)
                    {
                        result = InputResult.FAILED;
                        Debug.Log($"Note Result FAILED: {time}, {id}");
                    }
                    else
                    {
                        result = InputResult.PERFECT;
                    }
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

                if (result != InputResult.NONE && result != InputResult.PENDING)
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


    Note CreateNoteInstance(object so, Transform track)
    {
        if (so is MuseNoteMarker)
        {
            MuseNoteMarker n = so as MuseNoteMarker;
            var note = GameObject.Instantiate(clickNotePrefab, track);
            note.transform.localPosition = Vector3.right * (float)n.time * unitPerSecond;

            return new Note() { noteObject = note, time = (float)n.time, id = n.guid };
        }
        else if (so is TimelineClip)
        {
            TimelineClip tc = so as TimelineClip;
            if (tc.asset is MuseMultiClickNote)
            {
                var n = tc.asset as MuseMultiClickNote;
                var note = GameObject.Instantiate(multiClickNotePrefab, track);
                note.transform.localPosition = Vector3.right * (float)tc.start * unitPerSecond;
                note.transform.localScale = new Vector3(1 * (float)tc.duration * unitPerSecond, 1, 1);

                return new MutiClickNote()
                {
                    noteObject = note,
                    time = (float)tc.start,
                    duration = (float)tc.duration,
                    clickCount = n.clickCount,
                    id = n.guid,
                };
            }
            else if (tc.asset is MuseLongClickNote)
            {
                var n = tc.asset as MuseLongClickNote;
                var note = GameObject.Instantiate(longClickNotePrefab, track);
                note.transform.localPosition = Vector3.right * (float)tc.start * unitPerSecond;
                note.transform.localScale = new Vector3(1 * (float)tc.duration * unitPerSecond, 1, 1);


                return new LongClickNote()
                {
                    noteObject = note,
                    time = (float)tc.start,
                    duration = (float)tc.duration,
                    id = n.guid,
                };
            }

        }

        return null;

    }

    void Load()
    {
        if (gameLevel == null) { return; }
        if (trackRoot == null) { return; }
        if (clickNotePrefab == null) { return; }

        var tracks = gameLevel.GetRootTracks().Where(x => x is MuseNoteTrackTrack).ToArray();

        var leftTrack = tracks[0];
        var leftNotes = leftTrack.GetMarkers();
        var leftNoteClips = leftTrack.GetClips();

        var tempNotesList = new List<Note>();

        foreach (var m in leftNotes)
        {
            MuseNoteMarker n = m as MuseNoteMarker;
            var note = GameObject.Instantiate(clickNotePrefab, this.leftTrack);
            note.transform.localPosition = Vector3.right * (float)n.time * unitPerSecond;
            // this.tracks[0].AddNote(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
            tempNotesList.Add(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
        }

        foreach (var m in leftNoteClips)
        {
            var note = CreateNoteInstance(m, this.leftTrack);
            tempNotesList.Add(note);
        }

        tempNotesList = tempNotesList.OrderBy(x => x.time).ToList();
        foreach (var m in tempNotesList)
        {
            this.tracks[0].AddNote(m);
        }


        var rightTrack = tracks[1];
        var rightNotes = rightTrack.GetMarkers();
        var rightNoteClips = rightTrack.GetClips();
        tempNotesList.Clear();

        foreach (var m in rightNotes)
        {
            MuseNoteMarker n = m as MuseNoteMarker;
            var note = GameObject.Instantiate(clickNotePrefab, this.rightTrack);
            note.transform.localPosition = Vector3.right * (float)n.time * unitPerSecond;
            // this.tracks[1].AddNote(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
            tempNotesList.Add(new Note() { noteObject = note, time = (float)n.time, id = n.guid });
        }

        foreach (var m in rightNoteClips)
        {
            var note = CreateNoteInstance(m, this.rightTrack);
            tempNotesList.Add(note);
        }

        tempNotesList = tempNotesList.OrderBy(x => x.time).ToList();
        foreach (var m in tempNotesList)
        {
            this.tracks[1].AddNote(m);
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
