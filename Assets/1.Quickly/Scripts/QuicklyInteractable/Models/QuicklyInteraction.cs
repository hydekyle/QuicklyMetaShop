using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Quickly
{
    public interface IQuicklyValueUpdatable
    {
        public async UniTask ValueUpdate(string value) { }
    }
}
