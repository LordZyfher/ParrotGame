using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Parrot : MonoBehaviour
{
    private bool isPressed = false;
    private bool pressIsPLayback = false;

    private bool micConnected = false;
    private AudioSource AudioSource;
    private int minFreq, maxFreq = 0;
    [SerializeField] int MaxRecordingLength = 20;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length <= 0)
        {
            Debug.Log("No microphone");
            micConnected = false;
        }
        else
        {
            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                Debug.Log(Microphone.devices[i]);
            }
            micConnected = true;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }
        }
    }

    public void OnClick()
    {
        StartCoroutine(SelectingPressType());
    }

    private IEnumerator SelectingPressType()
    {
        isPressed = true;
        yield return new WaitForSeconds(0.25f);
        if (pressIsPLayback)
        {
            Playback();
        }
        else
        {
            StartCoroutine(Recording());
        }
        pressIsPLayback = false;
    }

    private void Playback()
    {
        if (AudioSource.clip != null) AudioSource.Play();
        Debug.Log("Playback");
    }

    public void IsNotPressed()
    {
        isPressed = false;
        pressIsPLayback = true;
    }

    private IEnumerator Recording()
    {
        if (micConnected)
        {
            AudioSource.clip = Microphone.Start(null, true, MaxRecordingLength, maxFreq);
            StartCoroutine(MaxRecordingTime());
        }

        while (isPressed)
        {
            yield return null;
        }

        if (micConnected)
        {
            Microphone.End(null);
            StopCoroutine(MaxRecordingTime());
        }
    }

    private IEnumerator MaxRecordingTime()
    {
        yield return new WaitForSeconds(MaxRecordingLength);
        isPressed = false;
        Debug.Log("Recording exceeds maximum duration");
    }
}
