using UnityEngine;

public class test_btton : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    public void OnClick()
    {
        SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
    }
}
