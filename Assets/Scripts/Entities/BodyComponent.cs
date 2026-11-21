using System;
using Entities.Locations;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Физический скафандр сущности в оперативной памяти (уровень L1).
    /// Хранит пространственные координаты, ориентацию, текущую локацию
    /// и ссылку на физическое тело в сцене Unity (уровень L2).
    /// </summary>
    [Serializable]
    public class BodyComponent
    {
        /// <summary>Идентификатор тела (совпадает с EntityId).</summary>
        public string BodyId { get; set; }

        /// <summary>Идентификатор текущей локации сущности (см. LocationIds).</summary>
        public string LocationId { get; set; } = LocationIds.Default;

        /// <summary>Локальная позиция сущности внутри локации.</summary>
        public Vector3 Position { get; set; }

        /// <summary>Локальная ориентация сущности.</summary>
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        /// <summary>Ссылка на активное физическое тело уровня L2 (коллайдеры/хитбоксы в сцене Unity).</summary>
        public IEntityL2 View { get; set; }

        /// <summary>Создано ли в сцене Unity активное физическое тело L2.</summary>
        public bool HasPhysicalBody => View != null;

        public BodyComponent() { }

        public BodyComponent(string bodyId, string locationId, Vector3 position, Quaternion rotation)
        {
            BodyId = bodyId;
            LocationId = string.IsNullOrEmpty(locationId) ? LocationIds.Default : locationId;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Атомарная установка позиции и вращения с передачей значений в физическое тело L2.
        /// </summary>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            View?.SetPosition(position, rotation);
        }
    }
}
