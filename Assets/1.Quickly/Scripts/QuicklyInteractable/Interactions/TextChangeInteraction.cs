using Cysharp.Threading.Tasks;
using MText;
using Sirenix.OdinInspector;
using TMPro;

namespace Quickly
{
    public class TextChangeInteraction : IQuicklyValueUpdatable
    {
        [ShowIf("@targetText == null")]
        public Modular3DText targetText3D;
        [ShowIf("@targetText3D == null")]
        public TMP_Text targetText;

        public async UniTaskVoid ValueUpdate(string value)
        {
            if (targetText3D != null)
            {
                targetText3D.Text = value;
            }

            if (targetText != null)
            {
                targetText.text = value;
            }
        }
    }
}
