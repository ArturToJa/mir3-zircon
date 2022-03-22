using System;
using Client.Envir;
using Library;

namespace Client.Models.ClassSpecific
{
    public interface IGenderPlayer
    {
        public void PlayStruckSound();
        public void PlayDieSound();
    }

    public interface IClassPlayer
    {
        public void PlayAttackSound(int WeaponShape);
    }

    public class SoundPlayer
    {
        IGenderPlayer genderPlayer;
        IClassPlayer classPlayer;

        public SoundPlayer(MirGender gender, MirClass mirClass)
        {
            if (gender == MirGender.Male)
            {
                genderPlayer = new MaleSoundPlayer();
            }
            else
            {
                genderPlayer = new FemaleSoundPlayer();
            }

            if(mirClass != MirClass.Assassin)
            {
                classPlayer = new CommonSoundPlayer();
            }
            else
            {
                classPlayer = new Assassin.AssassinSoundPlayer();
            }
        }

        public void PlayStruckSound()
        {
            genderPlayer.PlayStruckSound();
        }

        public void PlayDieSound()
        {
            genderPlayer.PlayDieSound();
        }

        public void PlayAttackSound(int WeaponShape)
        {
            classPlayer.PlayAttackSound(WeaponShape);
        }
    }
}
