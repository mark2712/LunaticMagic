using System;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Контракт физического тела сущности в сцене Unity (уровень L2).
    /// Отвечает за коллайдеры, хитбоксы, CharacterController или Rigidbody.
    /// </summary>
    public interface IEntityL2 : IDisposable
    {
        /// <summary>Идентификатор тела (совпадает с EntityId).</summary>
        string BodyId { get; }

        /// <summary>Признак кинематического состояния физики.</summary>
        bool IsKinematic { get; set; }

        /// <summary>Синхронизация координат сцены Unity с логическими координатами L1.</summary>
        void SetPosition(Vector3 position, Quaternion rotation);
    }

    /// <summary>
    /// Заглушка физического тела L2 для работы без фактического спавна префабов.
    /// </summary>
    public class EntityL2Stub : IEntityL2
    {
        public string BodyId { get; set; } = string.Empty;
        public bool IsKinematic { get; set; } = true;

        public void SetPosition(Vector3 position, Quaternion rotation) { }
        public void Dispose() { }
    }
}
