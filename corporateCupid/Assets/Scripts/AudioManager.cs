using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] FMOD.Studio.EventInstance BackgroundAirconAudio;
    [SerializeField] FMOD.Studio.EventInstance CabinetCloseAudio;
    [SerializeField] FMOD.Studio.EventInstance CabinetOpenAudio;
    [SerializeField] FMOD.Studio.EventInstance CabinetPaperInAudio;
    [SerializeField] FMOD.Studio.EventInstance CabinetPaperOutAudio;
    [SerializeField] FMOD.Studio.EventInstance ClockTickAudio;
    [SerializeField] FMOD.Studio.EventInstance EnvelopeSubmitAudio;
    [SerializeField] FMOD.Studio.EventInstance EndOfDayBeepsAudio;
    [SerializeField] FMOD.Studio.EventInstance Keyboard1Audio;
    [SerializeField] FMOD.Studio.EventInstance Keyboard2Audio;
    [SerializeField] FMOD.Studio.EventInstance Keyboard3Audio;
    [SerializeField] FMOD.Studio.EventInstance Keyboard4Audio;
    [SerializeField] FMOD.Studio.EventInstance LtRFootstepsAudio;
    [SerializeField] FMOD.Studio.EventInstance RtLFootstepsAudio;
    [SerializeField] FMOD.Studio.EventInstance PlayerTapAudio;
    [SerializeField] FMOD.Studio.EventInstance ProfileEnvelopeAudio;
    [SerializeField] FMOD.Studio.EventInstance ProfilePinboardAudio;
    [SerializeField] FMOD.Studio.EventInstance PunchClockAudio;
    [SerializeField] FMOD.Studio.EventInstance PaperShredderAudio;
    [SerializeField] FMOD.Studio.EventInstance MainMenuPaperSelectAudio;
    [SerializeField] FMOD.Studio.EventInstance MainMenuClickAudio;
    [SerializeField] FMOD.Studio.EventInstance MusicAudio;
    [SerializeField] FMOD.Studio.EventInstance NotebookFlipAudio;
    public static AudioManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        MainMenuPaperSelectAudio = FMODUnity.RuntimeManager.CreateInstance("event:/MainMenuPaperSelect");
        MainMenuClickAudio = FMODUnity.RuntimeManager.CreateInstance("event:/MainMenuClick");
        MusicAudio = FMODUnity.RuntimeManager.CreateInstance("event:/Music");
        CabinetOpenAudio = FMODUnity.RuntimeManager.CreateInstance("event:/CabinetOpen");
        CabinetCloseAudio = FMODUnity.RuntimeManager.CreateInstance("event:/CabinetClose");
        CabinetPaperOutAudio = FMODUnity.RuntimeManager.CreateInstance("event:/CabinetPaperOut");
        CabinetPaperInAudio = FMODUnity.RuntimeManager.CreateInstance("event:/CabinetPaperIn");
        PaperShredderAudio = FMODUnity.RuntimeManager.CreateInstance("event:/PaperShredder");
        ClockTickAudio = FMODUnity.RuntimeManager.CreateInstance("event:/ClockTick");
        EnvelopeSubmitAudio = FMODUnity.RuntimeManager.CreateInstance("event:/EnvelopeSubmit");
        EndOfDayBeepsAudio = FMODUnity.RuntimeManager.CreateInstance("event:/EndOfDayBeeps");
        Keyboard1Audio = FMODUnity.RuntimeManager.CreateInstance("event:/Keyboard1");
        Keyboard2Audio = FMODUnity.RuntimeManager.CreateInstance("event:/Keyboard2");
        Keyboard3Audio = FMODUnity.RuntimeManager.CreateInstance("event:/Keyboard3");
        Keyboard4Audio = FMODUnity.RuntimeManager.CreateInstance("event:/Keyboard4");
        LtRFootstepsAudio = FMODUnity.RuntimeManager.CreateInstance("event:/LtRFootsteps");
        RtLFootstepsAudio = FMODUnity.RuntimeManager.CreateInstance("event:/RtLFootsteps");
        PlayerTapAudio = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerTap");
        ProfileEnvelopeAudio = FMODUnity.RuntimeManager.CreateInstance("event:/ProfileEnvelope");
        ProfilePinboardAudio = FMODUnity.RuntimeManager.CreateInstance("event:/ProfilePinboard");
        PunchClockAudio = FMODUnity.RuntimeManager.CreateInstance("event:/PunchClock");
        NotebookFlipAudio = FMODUnity.RuntimeManager.CreateInstance("event:/NotebookFlip");
        BackgroundAirconAudio = FMODUnity.RuntimeManager.CreateInstance("event:/BackgroundAircon");
    }

    public void Playclip(string path)
    {
        if (path.Equals("Click"))
        {
            MainMenuClickAudio.start();
        }
        if (path.Equals("Music"))
        {
            StartCoroutine(Music());
        }
        if (path.Equals("COpen"))
        {
            CabinetOpenAudio.start();
        }
        if (path.Equals("CClose"))
        {
            CabinetCloseAudio.start();
        }
        if (path.Equals("CPaperIN"))
        {
            CabinetPaperInAudio.start();
        }
        if (path.Equals("CPaperOUT"))
        {
            CabinetPaperOutAudio.start();
        }
        if (path.Equals("Shredder"))
        {
            PaperShredderAudio.start();
        }
        if (path.Equals("ClockTick"))
        {
            ClockTickAudio.start();
        }
        if (path.Equals("ESubmit"))
        {
            EnvelopeSubmitAudio.start();
        }
        if (path.Equals("EndBeeps"))
        {
            EndOfDayBeepsAudio.start();
        }
        if (path.Equals("1"))
        {
            Keyboard1Audio.start();
        }
        if (path.Equals("2"))
        {
            Keyboard2Audio.start();
        }
        if (path.Equals("3"))
        {
            Keyboard3Audio.start();
        }
        if (path.Equals("4"))
        {
            Keyboard4Audio.start();
        }
        if (path.Equals("5"))
        {
            LtRFootstepsAudio.start();
        }
        if (path.Equals("6"))
        {
            RtLFootstepsAudio.start();
        }
        if (path.Equals("PTap"))
        {
            PlayerTapAudio.start();
        }
        if (path.Equals("PEnvelope"))
        {
            ProfileEnvelopeAudio.start();
        }
        if (path.Equals("PProfile"))
        {
            ProfilePinboardAudio.start();
        }
        if (path.Equals("PClock"))
        {
            PunchClockAudio.start();
        }
        if (path.Equals("Notebook"))
        {
            NotebookFlipAudio.start();
        }
        if (path.Equals("Aircon"))
        {
            if (this != null)
            {
                StartCoroutine(AirC());
            }
        }

    }

    public IEnumerator AirC()
    {
        BackgroundAirconAudio.start();
        FMOD.Studio.PLAYBACK_STATE sTATE;
        BackgroundAirconAudio.getPlaybackState(out sTATE);
        while (sTATE != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            yield return null;
            BackgroundAirconAudio.getPlaybackState(out sTATE);
        }
        StartCoroutine(AirC());
    }


    
    public IEnumerator Music()
    {
        MusicAudio.start();
        FMOD.Studio.PLAYBACK_STATE sTATE;
        MusicAudio.getPlaybackState(out sTATE);
        while (sTATE != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            yield return null;
            MusicAudio.getPlaybackState(out sTATE);
        }
        StartCoroutine(Music());
    }

    public void AirCStop()
    {
        if (this != null)
        {
            StopAllCoroutines();
            BackgroundAirconAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    

    public void StopAudio()
    {
        StopAllCoroutines();
        MusicAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimatedSound()
    {
        MainMenuPaperSelectAudio.start();
    }


}
