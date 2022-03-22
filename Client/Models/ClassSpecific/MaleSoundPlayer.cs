using Client.Envir;
using Library;

namespace Client.Models.ClassSpecific
{
    class MaleSoundPlayer : IGenderPlayer
    {
        public void PlayStruckSound()
        {
            DXSoundManager.Play(SoundIndex.MaleStruck);
            DXSoundManager.Play(SoundIndex.GenericStruckPlayer);
        }

        public void PlayDieSound()
        {
            DXSoundManager.Play(SoundIndex.MaleDie);
        }
    }
}
