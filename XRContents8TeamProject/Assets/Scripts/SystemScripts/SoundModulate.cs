using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundModulate : MonoBehaviour
{
    public AudioMixer mixer;

    public void AudioMasterController(float sliderVal)
    {
        mixer.SetFloat("Master", Mathf.Log10(sliderVal) * 20);
    }

    public void AudioBGMController(float sliderVal)
    {
        mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
    }

    public void AudioSFXController(float sliderVal)
    {
        mixer.SetFloat("SFX", Mathf.Log10(sliderVal) * 20);
    }

    // 버튼 하나로 소리 끄기
    // Audio입력받는 쪽(AudioListener)에서 볼륨을 줄이는 것
    //public void ToggleAudioVolum()
    //{  
    //    AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    //}
}
