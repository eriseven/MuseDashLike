using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

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

    [SerializeField]
    TextMeshProUGUI scoreLable;


    public enum InputEvent
    {
        PRESSED,
        RELEASED,
    }

    public enum InputResult
    {
        NONE,
        PERFECT,
        GOOD,
        SUCCESS,
        PENDING,
        FAILED,
    }


    int totalScore = 0;

    void OnPerfect(InputResult result)
    {
        int score = 0;
        scoreConfig.TryGetValue(result, out score);
        totalScore += score;

        Debug.Log($"Get Score:{score}, Total Score:{totalScore}");
        if (scoreLable != null)
        {
            scoreLable.text = totalScore.ToString();
        }
    }

    [Serializable]
    public class PerformScoreConfig : SerializableDictionaryBase<InputResult, int> { }

    public PerformScoreConfig scoreConfig;


    class Note
    {
        public Action<InputResult> OnPerfect = delegate { };
        public GameObject noteObject;
        public string id;
        public float time;

        public float perfectOffsetTime = 0.5f;
        public float goodOffsetTime = 0.7f;
        public float successOffsetTime = 1f;


        protected InputResult result = InputResult.NONE;

        public InputResult currentResult => result;

        protected void LogResult()
        {
            Debug.Log($"Note Result {result.ToString()}: {time}, {id}");
        }

        protected virtual InputResult CheckResult()
        {
            return InputResult.NONE;
        }

        public virtual InputResult Update()
        {
            var xPos = noteObject.transform.position.x;
            if (xPos + successOffsetTime < 0)
            {
                result = InputResult.FAILED;
                LogResult();
            }

            return result;
        }

        public virtual InputResult OnInput(InputEvent ev)
        {
            if (currentResult != InputResult.NONE)
            {
                return currentResult;
            }

            if (ev == InputEvent.PRESSED)
            {
                var currOffsetTime = Mathf.Abs(NotesManager.instance.time - time);
                if (currOffsetTime <= perfectOffsetTime)
                {
                    result = InputResult.PERFECT;
                }
                else if (currOffsetTime <= goodOffsetTime)
                {
                    result = InputResult.GOOD;
                }
                else if (currOffsetTime <= successOffsetTime)
                {
                    result = InputResult.SUCCESS;
                }

                if (result != InputResult.NONE && result != InputResult.FAILED && result != InputResult.PENDING)
                {
                    OnPerfect(result);
                    LogResult();
                }
            }
            return result;
        }
    }

    class DoubleClickNote : Note
    {
        public DoubleClickNote slaveNote;

        InputResult _pendingResult = InputResult.NONE;
        public InputResult pendingResult => _pendingResult;

        public void NotifyFinalResult(InputResult result)
        {
            this.result = result;
            if (result != InputResult.NONE && result != InputResult.FAILED && result != InputResult.PENDING)
            {
                OnPerfect(result);
                LogResult();
            }
        }

        public override InputResult OnInput(InputEvent ev)
        {
            if (_pendingResult != InputResult.NONE)
            {
                return _pendingResult;
            }

            if (ev == InputEvent.PRESSED)
            {
                var currOffsetTime = Mathf.Abs(NotesManager.instance.time - time);
                if (currOffsetTime <= perfectOffsetTime)
                {
                    _pendingResult = InputResult.PERFECT;
                }
                else if (currOffsetTime <= goodOffsetTime)
                {
                    _pendingResult = InputResult.GOOD;
                }
                else if (currOffsetTime <= successOffsetTime)
                {
                    _pendingResult = InputResult.SUCCESS;
                }

                //if (result != InputResult.NONE && result != InputResult.FAILED && result != InputResult.PENDING)
                //{
                //    OnPerfect(result);
                //    LogResult();
                //}
            }
            return result;
        }

        public override InputResult Update()
        {
            var xPos = noteObject.transform.position.x;
            if (xPos + successOffsetTime < 0)
            {
                result = InputResult.FAILED;
                _pendingResult = result;
                LogResult();
            }

            if (_pendingResult != InputResult.NONE && _pendingResult != InputResult.FAILED && _pendingResult != InputResult.PENDING)
            {
                if (slaveNote != null)
                {
                    var _slaveResult = slaveNote.pendingResult;
                    if (_slaveResult != InputResult.NONE && _slaveResult != InputResult.FAILED && _slaveResult != InputResult.PENDING)
                    {
                        result = (InputResult)Math.Max((int)_pendingResult, (int)_slaveResult);
                        LogResult();
                        slaveNote.NotifyFinalResult(result);
                    }
                }
            }

            return result;
        }
    }

    class LongClickNote : Note
    {
        public float duration = 1;
        public float perfectPercent = 0.9f;
        public float goodPercent = 0.8f;
        public float successPercent = 0.7f;

        float pressTime = -1;
        float pressedDuration = 0;

        public override InputResult OnInput(InputEvent ev)
        {

            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                if (ev == InputEvent.PRESSED)
                {
                    if (Mathf.Abs(NotesManager.instance.time - time) > perfectOffsetTime
                        || Mathf.Abs(NotesManager.instance.time - (time + duration)) > perfectOffsetTime
                        || (NotesManager.instance.time > time && NotesManager.instance.time < (time + duration)))

                    {
                        result = InputResult.PENDING;
                        OnPerfect(result);
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
                var currPerformPercent = pressedDuration / duration;
                if (currPerformPercent >= perfectPercent)
                {
                    result = InputResult.PERFECT;
                }
                else if (currPerformPercent >= goodPercent)
                {
                    result = InputResult.GOOD;
                }
                else if (currPerformPercent >= successPercent)
                {
                    result = InputResult.SUCCESS;
                }
                else
                {
                    result = InputResult.FAILED;
                }

                if (result != InputResult.FAILED)
                {
                    OnPerfect(result);
                }
            }

            return result;
        }

        public override InputResult Update()
        {
            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                //if (noteObject.transform.position.x + duration * 2 < 0)

                if (NotesManager.instance.time - (time + duration) > perfectOffsetTime)
                {
                    //pressedDuration = NotesManager.instance.time - pressTime;
                    //if ((pressedDuration / duration) > perfromPercent)
                    //{
                    //    result = InputResult.PERFECT;
                    //    OnPerfect(result);
                    //}
                    //else
                    {
                        result = InputResult.FAILED;
                        LogResult();
                    }
                }
            }
            return result;
        }
    }

    class MutiClickNote : Note
    {
        public int perfectClickCount = 1;
        public int goodClickCount = 1;
        public int successClickCount = 1;

        public float duration = 1;
        public int clickedCount = 0;

        public override InputResult OnInput(InputEvent ev)
        {
            if (ev == InputEvent.PRESSED)
            {
                if (result == InputResult.NONE || result == InputResult.PENDING)
                {
                    if (Mathf.Abs(NotesManager.instance.time - time) > perfectOffsetTime
                        || Mathf.Abs(NotesManager.instance.time - (time + duration)) > perfectOffsetTime
                        || (NotesManager.instance.time > time && NotesManager.instance.time < (time + duration)))
                    {
                        clickedCount++;
                        //Debug.Log($"MutiClickNote performed:{clickedCount}, time:{NotesManager.instance.time}");
                        if (clickedCount >= perfectClickCount)
                        {
                            result = InputResult.PERFECT;
                        }
                        else if (clickedCount >= goodClickCount)
                        {
                            result = InputResult.GOOD;
                        }
                        else if (clickedCount >= successClickCount)
                        {
                            result = InputResult.SUCCESS;
                        }
                        else
                        {
                            result = InputResult.PENDING;
                        }

                        OnPerfect(result);
                        if (result == InputResult.PENDING)
                        {
                            Debug.Log($"MutiClickNote performed:{clickedCount}, time:{NotesManager.instance.time}");
                        }
                        else
                        {
                            LogResult();
                        }
                    }
                    else
                    {
                        result = InputResult.NONE;
                    }
                }

            }
            return result;
        }

        public override InputResult Update()
        {
            if (result == InputResult.PENDING || result == InputResult.NONE)
            {
                //if (noteObject.transform.position.x + duration * 2 < 0)
                if (NotesManager.instance.time - (time + duration) > perfectOffsetTime)
                {
                    if (clickedCount >= perfectClickCount)
                    {
                        result = InputResult.PERFECT;
                    }
                    else if (clickedCount >= goodClickCount)
                    {
                        result = InputResult.GOOD;
                    }
                    else if (clickedCount >= successClickCount)
                    {
                        result = InputResult.SUCCESS;
                    }
                    else
                    {
                        result = InputResult.FAILED;
                    }
                }
            }
            return result;
        }
    }

    [SerializeField]
    float _unitPerSecond = 2.0f;

    public float unitPerSecond => _unitPerSecond;


    class NoteTrack
    {

        public Action<InputResult> OnPerfect;

        Queue<Note> notes = new Queue<Note>();
        public void Update()
        {
            if (notes.Count > 0)
            {
                var note = notes.Peek();
                var originResult = note.currentResult;
                var result = note.Update();

                if (result != InputResult.NONE && result != InputResult.PENDING)
                {
                    notes.Dequeue();

                    if (result != InputResult.FAILED)
                    {
                        GameObject.Destroy(note.noteObject);
                        if (result != originResult)
                        {
                            OnPerfect(result);
                        }
                    }

                }

                //if (result == InputResult.PERFECT)
                //{
                //    GameObject.Destroy(note.noteObject);
                //    if (result != originResult)
                //    {
                //        OnPerfect(result);
                //    }
                //}
            }
        }

        public void AddNote(Note note)
        {
            note.OnPerfect += OnPerfect;
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

        tracks[0].OnPerfect += OnLeftPerfect;
        tracks[1].OnPerfect += OnRightPerfect;
    }

    IEnumerator Start()
    {
        Load();
        if (startGame != null)
        {
            startGame.gameObject.SetActive(true);
        }
        yield return null;
        //StartGame();
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

    [SerializeField]
    Button startGame;

    [SerializeField]
    Button restartGame;

    [SerializeField]
    ParticleSystem leftPerfectFx;

    [SerializeField]
    ParticleSystem rightPerfectFx;

    void OnLeftPerfect(InputResult result)
    {
        leftPerfectFx.Play();
        OnPerfect(result);
    }

    void OnRightPerfect(InputResult result)
    {
        rightPerfectFx.Play();
        OnPerfect(result);
    }



    public void StartGame()
    {
        totalScore = 0;
        if (startGame != null)
        {
            startGame.gameObject.SetActive(false);
        }

        isPlaying = true;
        startTime = Time.realtimeSinceStartup;
        GetComponent<PlayableDirector>().Play();
    }

    public void OnGameEnd()
    {
        isPlaying = false;
        if (restartGame != null)
        {
            restartGame.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Test");
    }

    Note CreateNoteInstance(object so, Transform track)
    {
        if (so is MuseClickNote)
        {
            MuseClickNote n = so as MuseClickNote;
            var note = GameObject.Instantiate(clickNotePrefab, track);
            note.transform.localPosition = Vector3.right * (float)n.time * _unitPerSecond;

            return new Note()
            {
                noteObject = note,
                time = (float)n.time,
                id = n.guid,
                perfectOffsetTime = n.successOffsetTime,
                goodOffsetTime = n.goodOffsetTime,
                successOffsetTime = n.successOffsetTime,
            };
        }
        else if (so is MuseDoubleClickNote)
        {
            MuseDoubleClickNote n = so as MuseDoubleClickNote;
            var note = GameObject.Instantiate(clickNotePrefab, track);
            note.transform.localPosition = Vector3.right * (float)n.time * _unitPerSecond;

            var otherNote = GameObject.Instantiate(clickNotePrefab, track);
            otherNote.transform.localPosition = Vector3.right * (float)n.time * _unitPerSecond;

            var slave = new DoubleClickNote()
            {
                noteObject = otherNote,
                time = (float)n.time,
                id = $"slave-for{n.guid}",
                //id = n.guid,
                perfectOffsetTime = n.successOffsetTime,
                goodOffsetTime = n.goodOffsetTime,
                successOffsetTime = n.successOffsetTime,
            };


            return new DoubleClickNote()
            {
                noteObject = note,
                time = (float)n.time,
                id = n.guid,
                perfectOffsetTime = n.successOffsetTime,
                goodOffsetTime = n.goodOffsetTime,
                successOffsetTime = n.successOffsetTime,
                slaveNote = slave,
            };

        }
        else if (so is TimelineClip)
        {
            TimelineClip tc = so as TimelineClip;
            if (tc.asset is MuseMultiClickNote)
            {
                var n = tc.asset as MuseMultiClickNote;
                var note = GameObject.Instantiate(multiClickNotePrefab, track);
                note.transform.localPosition = Vector3.right * (float)tc.start * _unitPerSecond;
                note.transform.localScale = new Vector3(1 * (float)tc.duration * _unitPerSecond, 1, 1);

                return new MutiClickNote()
                {
                    noteObject = note,
                    time = (float)tc.start,
                    duration = (float)tc.duration,
                    perfectClickCount = n.perfectClickCount,
                    goodClickCount = n.goodClickCount,
                    successClickCount = n.successClickCount,
                    id = n.guid,
                };
            }
            else if (tc.asset is MuseLongClickNote)
            {
                var n = tc.asset as MuseLongClickNote;
                var note = GameObject.Instantiate(longClickNotePrefab, track);
                note.transform.localPosition = Vector3.right * (float)tc.start * _unitPerSecond;
                note.transform.localScale = new Vector3(1 * (float)tc.duration * _unitPerSecond, 1, 1);


                return new LongClickNote()
                {
                    noteObject = note,
                    time = (float)tc.start,
                    duration = (float)tc.duration,
                    perfectPercent = n.perfectPercent,
                    goodPercent = n.goodPercent,
                    successPercent = n.successPercent,
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
            var note = CreateNoteInstance(m, this.leftTrack);
            tempNotesList.Add(note);
        }

        foreach (var m in leftNoteClips)
        {
            var note = CreateNoteInstance(m, this.leftTrack);
            tempNotesList.Add(note);
        }

        tempNotesList = tempNotesList.OrderBy(x => x.time).ToList();
        foreach (var m in tempNotesList)
        {
            m.noteObject.transform.SetParent(this.leftTrack);
            m.noteObject.transform.localPosition = Vector3.right * (float)m.time * _unitPerSecond;

            this.tracks[0].AddNote(m);
        }


        var slaveNotes = tempNotesList.Where(x => x is DoubleClickNote).Select(x => ((DoubleClickNote)x).slaveNote).ToArray();

        var rightTrack = tracks[1];
        var rightNotes = rightTrack.GetMarkers();
        var rightNoteClips = rightTrack.GetClips();
        tempNotesList.Clear();

        foreach (var m in rightNotes)
        {
            var note = CreateNoteInstance(m, this.rightTrack);
            tempNotesList.Add(note);
        }

        foreach (var m in rightNoteClips)
        {
            var note = CreateNoteInstance(m, this.rightTrack);
            tempNotesList.Add(note);
        }

        foreach (var m in slaveNotes)
        {
            tempNotesList.Add(m);
        }

        tempNotesList = tempNotesList.OrderBy(x => x.time).ToList();
        foreach (var m in tempNotesList)
        {
            m.noteObject.transform.SetParent(this.rightTrack);
            m.noteObject.transform.localPosition = Vector3.right * (float)m.time * _unitPerSecond;
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
            trackRoot.transform.localPosition = -1 * Vector3.right * playedTime * _unitPerSecond;
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

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
