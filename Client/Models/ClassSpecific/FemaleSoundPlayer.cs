using Client.Envir;
using Library;

namespace Client.Models.ClassSpecific
{
    class FemaleSoundPlayer : IGenderPlayer
    {
        public void PlayStruckSound()
        {
            DXSoundManager.Play(SoundIndex.FemaleStruck);
            DXSoundManager.Play(SoundIndex.GenericStruckPlayer);
        }

        public void PlayDieSound()
        {
            DXSoundManager.Play(SoundIndex.FemaleDie);
        }
    }
}
