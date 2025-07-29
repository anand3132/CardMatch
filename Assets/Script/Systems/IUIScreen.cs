using UnityEngine;

namespace CardMatch
{
    public interface IUIScreen
    {
        void Initialize();         
        void UpdateUI();          
        void Show();               
        void Hide();              
    }
}