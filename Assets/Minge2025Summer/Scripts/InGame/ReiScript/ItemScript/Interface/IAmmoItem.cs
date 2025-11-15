using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface
{
    public interface IAmmoItem : IReiItem
    {
        public AmmoType GetAmmoType { get; }
    }
}