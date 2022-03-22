using Client.Envir;
using Library;

namespace Client.Models.ClassSpecific.Assassin
{
    class AssassinSoundPlayer : IClassPlayer
    {
        public void PlayAttackSound(int WeaponShape)
        {
            if (WeaponShape >= 1200)
                DXSoundManager.Play(SoundIndex.ClawAttack);
            else if (WeaponShape >= 1100)
                DXSoundManager.Play(SoundIndex.GlaiveAttack);
            else
            {
                switch (WeaponShape)
                {
                    case 100:
                        DXSoundManager.Play(SoundIndex.WandSwing);
                        break;
                    case 9:
                    case 101:
                        DXSoundManager.Play(SoundIndex.WoodSwing);
                        break;
                    case 102:
                        DXSoundManager.Play(SoundIndex.AxeSwing);
                        break;
                    case 103:
                        DXSoundManager.Play(SoundIndex.DaggerSwing);
                        break;
                    case 104:
                        DXSoundManager.Play(SoundIndex.ShortSwordSwing);
                        break;
                    case 26:
                    case 105:
                        DXSoundManager.Play(SoundIndex.IronSwordSwing);
                        break;
                    default:
                        DXSoundManager.Play(SoundIndex.FistSwing);
                        break;
                }
            }
        }
    }
}
