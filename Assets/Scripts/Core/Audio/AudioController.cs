using UnityEngine;

namespace Xonix.Audio
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;
        [SerializeField] private AudioSource source;
        [Header("Clips")]
        [SerializeField] private AudioClip damageClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip startClip;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayDamage() 
        {
            source.clip = damageClip;
            source.Play();
        }

        public void PlayWin() 
        {
            source.clip = winClip;
            source.Play();
        }

        public void PlayLose() 
        {
            source.clip = loseClip;
            source.Play();
        }

        public void PlayStart() 
        {
            source.clip = startClip;
            source.Play();
        }
    }
}
